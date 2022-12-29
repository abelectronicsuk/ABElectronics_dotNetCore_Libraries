using System;
using System.Device.I2c;

namespace ABElectronicsUK
{
    /// <summary>
    ///     Class for controlling the RTC Pi and RTC Pi Plus expansion boards from AB Electronics UK
    ///     Based on the DS1307 real-time clock from Maxim.
    /// </summary>
    public class RTCPi : IDisposable
    {
        // Register addresses for the DS1307 IC
        private const byte SECONDS = 0x00;
        private const byte MINUTES = 0x01;
        private const byte HOURS = 0x02;
        private const byte DAYOFWEEK = 0x03;
        private const byte DAY = 0x04;
        private const byte MONTH = 0x05;
        private const byte YEAR = 0x06;
        private const byte CONTROL = 0x07;

        // the DS1307 does not store the current century so that has to be added on manually.
        private int century = 2000;

        // initial configuration - square wave and output disabled, frequency set to 32.768KHz.
        private byte config = 0x03;
        private readonly ABE_Helpers helper = new ABE_Helpers();

        private I2cDevice i2cbus; // create an instance of the i2c bus.

        // variables
        private readonly byte rtcAddress = 0x68; // I2C address

        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        System.Runtime.InteropServices.SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        ///     Create an instance of an RTC Pi bus.
        /// </summary>
        public RTCPi()
        {
            IsConnected = false;
        }

        /// <summary>
        ///     Shows if there is a connection with the RTC Pi.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     The DS1307 does not store the current century so that has to be set manually.
        /// </summary>
        public int Century { get { return century; } set { century = value; } }

        /// <summary>
        ///     Open a connection with the RTC Pi.
        /// </summary>
        /// <returns></returns>

        public void Connect()
        {
            if (IsConnected)
            {
                return; // Already connected
            }

            // Initialize the I2C bus
            try
            {
                var i2cbusConnectionSettings = new I2cConnectionSettings(1, rtcAddress);
                i2cbus = I2cDevice.Create(i2cbusConnectionSettings);

                if (i2cbus != null)
                {
                    // i2c bus is connected so set IsConnected to true and fire the Connected event handler
                    IsConnected = true;

                    // Fire the Connected event handler
                    Connected?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                throw new Exception("I2C Initialization Failed", ex);
            }
        }

        /// <summary>
        ///     Event occurs when a connection is made.
        /// </summary>
        public event EventHandler Connected;


        /// <summary>
        ///     Converts BCD to integer format.
        /// </summary>
        /// <param name="x">BCD formatted byte</param>
        /// <returns></returns>
        private int BCDtoInt(byte x)
        {
            return x - 6 * (x >> 4);
        }

        /// <summary>
        ///     Converts byte to BCD format.
        /// </summary>
        /// <param name="val">value to convert</param>
        /// <returns>Converted byte</returns>
        private byte BytetoBCD(int val)
        {
            return (byte)(val / 10 * 16 + val % 10);
        }

        /// <summary>
        ///     Set or get the date and time from the RTC.
        /// </summary>
        /// <param name="date">DateTime</param>
        /// <returns>DateTime</returns>
        public DateTime Date
        {
            get
            {
                CheckConnected();

                // Read the date from the RTC registers and decode it
                var DateArray = helper.ReadI2CBlockData(i2cbus, 0, 7);
                var year = BCDtoInt(DateArray[6]) + century;
                var month = BCDtoInt(DateArray[5]);
                var day = BCDtoInt(DateArray[4]);
                var hours = BCDtoInt(DateArray[2]);
                var minutes = BCDtoInt(DateArray[1]);
                var seconds = BCDtoInt(DateArray[0]);

                try
                {
                    var date = new DateTime(year, month, day, hours, minutes, seconds);
                    return date;
                }
                catch
                {
                    var date = new DateTime(1990, 01, 01, 01, 01, 01);
                    return date;
                }
            }

            set
            {
                CheckConnected();
                DateTime date = value;
                // Write the date to the RTC registers
                helper.WriteI2CByte(i2cbus, SECONDS, BytetoBCD(date.Second));
                helper.WriteI2CByte(i2cbus, MINUTES, BytetoBCD(date.Minute));
                helper.WriteI2CByte(i2cbus, HOURS, BytetoBCD(date.Hour));
                helper.WriteI2CByte(i2cbus, DAYOFWEEK, BytetoBCD((int)date.DayOfWeek));
                helper.WriteI2CByte(i2cbus, DAY, BytetoBCD(date.Day));
                helper.WriteI2CByte(i2cbus, MONTH, BytetoBCD(date.Month));
                helper.WriteI2CByte(i2cbus, YEAR, BytetoBCD(date.Year - century));
            }
        }

        /// <summary>
        /// Enable or disable the clock output pin.
        /// Set output as true or false.  Gets output state as true or false.
        /// </summary>
        public bool Output
        {
            get
            {
                CheckConnected();
                return helper.CheckBit(helper.ReadI2CByte(i2cbus, CONTROL), CONTROL);
            }
            set
            {
                CheckConnected();
                if (value == true)
                {
                    config = helper.UpdateByte(config, 7, true);
                    config = helper.UpdateByte(config, 4, true);
                    helper.WriteI2CByte(i2cbus, CONTROL, config);
                }
                else
                {
                    config = helper.UpdateByte(config, 7, false);
                    config = helper.UpdateByte(config, 4, false);
                    helper.WriteI2CByte(i2cbus, CONTROL, config);
                }
            }
        }

        /// <summary>
        ///     Get or set the frequency of the output pin square wave.
        ///     Options are: 1 = 1Hz, 2 = 4.096KHz, 3 = 8.192KHz, 4 = 32.768KHz
        /// </summary>
        /// 
        public byte Frequency
        {
            get
            {
                CheckConnected();
                // get the control register from the RTC
                config = helper.ReadI2CByte(i2cbus, CONTROL);

                // extract bits rs0 and rs1
                bool rs0 = helper.CheckBit(config, 0);
                bool rs1 = helper.CheckBit(config, 1);

                // get the frequency from bits rs0 and rs1
                if (!rs0 && !rs1) return 1; // 1Hz
                else if (rs0 && !rs1) return 2; // 4.096KHz
                else if (!rs0 && rs1) return 3; // 8.192KHz
                else if (rs0 && rs1) return 4; // 32.768KHz
                else return 0;
            }
            set
            {
                CheckConnected();
                byte frequency = value;

                // update bits rs0 and rs1 in the config
                switch (frequency)
                {
                    case 1:
                        config = helper.UpdateByte(config, 0, false);
                        config = helper.UpdateByte(config, 1, false);
                        break;
                    case 2:
                        config = helper.UpdateByte(config, 0, true);
                        config = helper.UpdateByte(config, 1, false);
                        break;
                    case 3:
                        config = helper.UpdateByte(config, 0, false);
                        config = helper.UpdateByte(config, 1, true);
                        break;
                    case 4:
                        config = helper.UpdateByte(config, 0, true);
                        config = helper.UpdateByte(config, 1, true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }

                // write to control register
                helper.WriteI2CByte(i2cbus, CONTROL, config);
            }

        }

        /// <summary>
        /// Write to the memory on the DS1307.  Memory range is 0x08 to 0x3F.  
        /// </summary>
        /// <param name="address">First memory address where the values with be written.</param>
        /// <param name="values">Byte array containing values to be written to memory.  Array length can not exceed the memory space available.</param>
        public void WriteMemory(byte address, byte[] values)
        {
            // Check if the address is within the available memory range
            if (address >= 0x08 && address <= 0x3F)
            {
                // Check if the address + value length is within the bounds of the memory
                if (address + values.Length <= 0x3F)
                {
                    // Write values to memory
                    helper.WriteI2CBlock(i2cbus, address, values);
                }
                else
                {
                    throw new OverflowException("Array length out of bounds");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }
        }

        /// <summary>
        /// Read from the memory on the DS1307.  Memory range is 0x08 to 0x3F.  
        /// </summary>
        /// <param name="address">First memory address where the values with be read.</param>
        /// <param name="length">Number of bytes to read from memory</param>
        /// <returns>Byte array containing read memory values.</returns>
        public byte[] ReadMemory(byte address, byte length)
        {
            // Check if the address is within the available memory range
            if (address >= 0x08 && address <= 0x3F)
            {
                // Check if the address + value length is within the bounds of the memory
                if (address + length <= 0x3F)
                {
                    // Read values from memory
                    return helper.ReadI2CBlockData(i2cbus, address, length);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(length));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }
        }

        private void CheckConnected()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not connected. You must call .Connect() first.");
            }
        }


        /// <summary>
        ///     Dispose of the resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                i2cbus?.Dispose();
                i2cbus = null;

                IsConnected = false;
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }
}