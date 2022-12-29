using System;
using System.Device.I2c;

namespace ABElectronicsUK
{
    /// <summary>
    ///     Class for controlling the ADC Pi and ADC Pi Plus expansion boards from AB Electronics UK
    /// </summary>
    public class ADCPi : IDisposable
    {
        // create a byte array and fill it with initial values to define the size

        private Byte[] __adcreading = {0, 0, 0, 0};
        private byte bitrate = 18; // current bit rate

        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        System.Runtime.InteropServices.SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);
        // internal variables

        private byte config1 = 0x9C; // PGAx1, 18 bit, continuous conversion, channel 1
        private byte config2 = 0x9C; // PGAx1, 18 bit, continuous-shot conversion, channel 1
        private byte conversionmode = 1; // Conversion Mode
        private byte currentchannel1; // channel variable for ADC 1
        private byte currentchannel2; // channel variable for ADC 2
        private readonly ABE_Helpers helper = new ABE_Helpers();

        private I2cDevice i2cbus1; // i2c bus for ADC chip 1
        private I2cDevice i2cbus2; // i2c bus for ADC chip 2
        private double lsb = 0.0000078125; // default LSB value for 18 bit
        private double pga = 0.5; // current PGA setting
        private byte gain = 1; // gain property value
        private bool signbit;

        /// <summary>
        ///     Create an instance of an ADC Pi bus.
        /// </summary>
        /// <param name="i2caddress1">I2C address for the U1 (channels 1 - 4)</param>
        /// <param name="i2caddress2">I2C address for the U2 (channels 5 - 8)</param>
        public ADCPi(byte i2caddress1 = 0x68, byte i2caddress2 = 0x69)
        {
            Address1 = i2caddress1;
            Address2 = i2caddress2;
            IsConnected = false;
        }

        /// <summary>
        ///     I2C address for the U1 (channels 1 - 4).
        /// </summary>
        public byte Address1 { get; set; }

        /// <summary>
        ///     I2C address for the U2 (channels 5 - 8).
        /// </summary>
        public byte Address2 { get; set; }

        /// <summary>
        ///     Shows if there is a connection with the ADC Pi.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     Open a connection with the ADC Pi.
        /// </summary>
        public void Connect()
        {
            if (IsConnected)
            {
                return; // Already connected
            }

            // Initialize both I2C busses
            try
            {
                var i2cbus1ConnectionSettings = new I2cConnectionSettings(1, Address1);
                i2cbus1 = I2cDevice.Create(i2cbus1ConnectionSettings);

                var i2cbus2ConnectionSettings = new I2cConnectionSettings(1, Address2);
                i2cbus2 = I2cDevice.Create(i2cbus2ConnectionSettings);

                

                 // check if the i2c busses are not null
                if (i2cbus1 != null && i2cbus2 != null)
                {
                    
                    // set the initial bit rate and trigger a Connected event handler
                    IsConnected = true;
                    this.BitRate = 18;
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
        ///     Private method for updating the configuration to the selected <paramref name="channel" />.
        /// </summary>
        /// <param name="channel">ADC channel, 1 - 8</param>
        private void SetChannel(byte channel)
        {
            // Check to see if the selected channel is already the current channel.
            // If not then update the appropriate config to the new channel settings.

            if (channel < 5 && channel != currentchannel1)
            {
                switch (channel)
                {
                    case 1:
                        config1 = helper.UpdateByte(config1, 5, false);
                        config1 = helper.UpdateByte(config1, 6, false);
                        currentchannel1 = 1;
                        break;
                    case 2:
                        config1 = helper.UpdateByte(config1, 5, true);
                        config1 = helper.UpdateByte(config1, 6, false);
                        currentchannel1 = 2;
                        break;
                    case 3:
                        config1 = helper.UpdateByte(config1, 5, false);
                        config1 = helper.UpdateByte(config1, 6, true);
                        currentchannel1 = 3;
                        break;
                    case 4:
                        config1 = helper.UpdateByte(config1, 5, true);
                        config1 = helper.UpdateByte(config1, 6, true);
                        currentchannel1 = 4;
                        break;
                }
            }
            else if (channel >= 5 && channel <= 8 && channel != currentchannel2)
            {
                switch (channel)
                {
                    case 5:
                        config2 = helper.UpdateByte(config2, 5, false);
                        config2 = helper.UpdateByte(config2, 6, false);
                        currentchannel2 = 5;
                        break;
                    case 6:
                        config2 = helper.UpdateByte(config2, 5, true);
                        config2 = helper.UpdateByte(config2, 6, false);
                        currentchannel2 = 6;
                        break;
                    case 7:
                        config2 = helper.UpdateByte(config2, 5, false);
                        config2 = helper.UpdateByte(config2, 6, true);
                        currentchannel2 = 7;
                        break;
                    case 8:
                        config2 = helper.UpdateByte(config2, 5, true);
                        config2 = helper.UpdateByte(config2, 6, true);
                        currentchannel2 = 8;
                        break;
                }
            }
        }

        /// <summary>
        ///     Returns the voltage from the selected ADC <paramref name="channel" />.
        /// </summary>
        /// <param name="channel">1 to 8</param>
        /// <returns>Read voltage</returns>
        public double ReadVoltage(byte channel)
        {
            var raw = ReadRaw(channel); // get the raw value
            if (signbit) // check to see if the sign bit is present, if it is then the voltage is negative and can be ignored.
            {
                return 0.0; // returned a negative voltage so return 0
            }
            var voltage = raw * (lsb / pga) * 2.471; // calculate the voltage and return it
            return voltage;
        }

        /// <summary>
        ///     Reads the raw value from the selected ADC <paramref name="channel" />.
        /// </summary>
        /// <param name="channel">1 to 8</param>
        /// <returns>raw integer value from ADC buffer</returns>
        public int ReadRaw(byte channel)
        {
            CheckConnected();

            // variables for storing the raw bytes from the ADC
            byte h = 0;
            byte l = 0;
            byte m = 0;
            byte s = 0;
            byte config = 0;

            var t = 0;
            signbit = false;

            // create a new instance of the I2C device
            I2cDevice bus;

            SetChannel(channel);

            // get the configuration and i2c bus for the selected channel
            if (channel < 5)
            {
                config = config1;
                bus = i2cbus1;
            }
            else if (channel >= 5 && channel <= 8)
            {
                config = config2;
                bus = i2cbus2;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            // if the conversion mode is set to one-shot update the ready bit to 1
            if (conversionmode == 0)
            {
                config = helper.UpdateByte(config, 7, true);
                helper.WriteI2CByte(bus, config, 0x00);
                config = helper.UpdateByte(config, 7, false);
            }

            // keep reading the ADC data until the conversion result is ready
            var timeout = 15000; // number of reads before a timeout occurs
            var x = 0;
            do
            {
                if (bitrate == 18)
                {
                    __adcreading = helper.ReadI2CBlockData(bus, config, 4);
                    h = __adcreading[0];
                    m = __adcreading[1];
                    l = __adcreading[2];
                    s = __adcreading[3];
                }
                else
                {
                    __adcreading = helper.ReadI2CBlockData(bus, config, 3);
                    h = __adcreading[0];
                    m = __adcreading[1];
                    s = __adcreading[2];
                }

                // check bit 7 of s to see if the conversion result is ready
                if (!helper.CheckBit(s, 7))
                {
                    break;
                }

                if (x > timeout)
                {
                    // timeout occurred
                    throw new TimeoutException();
                }

                x++;
            } while (true);

            // extract the returned bytes and combine in the correct order
            switch (bitrate)
            {
                case 18:

                    t = ((h & 3) << 16) | (m << 8) | l;
                    signbit = helper.CheckIntBit(t, 17);
                    if (signbit)
                    {
                        t = helper.UpdateInt(t, 17, false);
                    }
                    break;
                case 16:
                    t = (h << 8) | m;
                    signbit = helper.CheckIntBit(t, 15);
                    if (signbit)
                    {
                        t = helper.UpdateInt(t, 15, false);
                    }
                    break;
                case 14:

                    t = ((h & 63) << 8) | m;
                    signbit = helper.CheckIntBit(t, 13);
                    if (signbit)
                    {
                        t = helper.UpdateInt(t, 13, false);
                    }
                    break;
                case 12:
                    t = ((h & 15) << 8) | m;
                    signbit = helper.CheckIntBit(t, 11);
                    if (signbit)
                    {
                        t = helper.UpdateInt(t, 11, false);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(channel));
            }

            return t;
        }

        /// <summary>
        ///     Gets or sets the PGA (Programmable Gain Amplifier) gain. Set to 1, 2, 4 or 8
        /// </summary>
        public byte PGA
        {
            get { return gain; }
            set
            {
                switch (value)
                {
                    case 1:
                        config1 = helper.UpdateByte(config1, 0, false);
                        config1 = helper.UpdateByte(config1, 1, false);
                        config2 = helper.UpdateByte(config2, 0, false);
                        config2 = helper.UpdateByte(config2, 1, false);
                        pga = 0.5;
                        gain = 1;
                        break;
                    case 2:
                        config1 = helper.UpdateByte(config1, 0, true);
                        config1 = helper.UpdateByte(config1, 1, false);
                        config2 = helper.UpdateByte(config2, 0, true);
                        config2 = helper.UpdateByte(config2, 1, false);
                        pga = 1;
                        gain = 2;
                        break;
                    case 4:
                        config1 = helper.UpdateByte(config1, 0, false);
                        config1 = helper.UpdateByte(config1, 1, true);
                        config2 = helper.UpdateByte(config2, 0, false);
                        config2 = helper.UpdateByte(config2, 1, true);
                        pga = 2;
                        gain = 4;
                        break;
                    case 8:
                        config1 = helper.UpdateByte(config1, 0, true);
                        config1 = helper.UpdateByte(config1, 1, true);
                        config2 = helper.UpdateByte(config2, 0, true);
                        config2 = helper.UpdateByte(config2, 1, true);
                        pga = 4;
                        gain = 8;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }

            }
        }

        /// <summary>
        ///     Gets or sets the sample resolution bit rate.
        ///     12 = 12 bit(240SPS max),
        ///     14 = 14 bit(60SPS max),
        ///     16 = 16 bit(15SPS max),
        ///     18 = 18 bit(3.75SPS max)
        /// </summary>

        public byte BitRate
        {
            get { return bitrate; }
            set
            {
                switch (value)
                {
                    case 12:
                        config1 = helper.UpdateByte(config1, 2, false);
                        config1 = helper.UpdateByte(config1, 3, false);
                        config2 = helper.UpdateByte(config2, 2, false);
                        config2 = helper.UpdateByte(config2, 3, false);
                        bitrate = 12;
                        lsb = 0.0005;
                        break;
                    case 14:
                        config1 = helper.UpdateByte(config1, 2, true);
                        config1 = helper.UpdateByte(config1, 3, false);
                        config2 = helper.UpdateByte(config2, 2, true);
                        config2 = helper.UpdateByte(config2, 3, false);
                        bitrate = 14;
                        lsb = 0.000125;
                        break;
                    case 16:
                        config1 = helper.UpdateByte(config1, 2, false);
                        config1 = helper.UpdateByte(config1, 3, true);
                        config2 = helper.UpdateByte(config2, 2, false);
                        config2 = helper.UpdateByte(config2, 3, true);
                        bitrate = 16;
                        lsb = 0.00003125;
                        break;
                    case 18:
                        config1 = helper.UpdateByte(config1, 2, true);
                        config1 = helper.UpdateByte(config1, 3, true);
                        config2 = helper.UpdateByte(config2, 2, true);
                        config2 = helper.UpdateByte(config2, 3, true);
                        bitrate = 18;
                        lsb = 0.0000078125;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the conversion mode for the ADC.
        ///     0 = One shot conversion mode, 1 = Continuous conversion mode
        /// </summary>
        private byte ConversionMode
        {
            get { return conversionmode; }
            set
            {
                if (value == 1)
                {
                    config1 = helper.UpdateByte(config1, 4, true);
                    config2 = helper.UpdateByte(config2, 4, true);
                    conversionmode = 1;
                }
                else if (value == 0)
                {
                    config1 = helper.UpdateByte(config1, 4, false);
                    config2 = helper.UpdateByte(config2, 4, false);
                    conversionmode = 0;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
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
                i2cbus1?.Dispose();
                i2cbus1 = null;

                i2cbus2?.Dispose();
                i2cbus2 = null;

                IsConnected = false;
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }
}