using System;
using System.Device.I2c;
using System.Device.Gpio;
using System.Threading;

namespace ABElectronicsUK
{
    /// <summary>
    ///     Class for controlling the I2C Switch expansion board from AB Electronics UK
    ///     Based on the PCA9546A I2C Switch from NXP.
    /// </summary>
    public class I2CSwitch : IDisposable
    {

        private I2cDevice i2cbus;
        private ABE_Helpers helper = new ABE_Helpers();

        private byte resetpin = 13; // default reset pin address

        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        System.Runtime.InteropServices.SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        ///     Create an instance of an I2CSwitch bus.
        /// </summary>
        /// <param name="i2caddress">I2C Address of I2C Switch bus</param>
        public I2CSwitch(byte i2caddress = 0x70)
        {
            Address = i2caddress;
            resetpin = 13;
            IsConnected = false;
        }

        /// <summary>
        ///     I2C address for the I2C Switch bus
        /// </summary>
        public byte Address { get { return resetpin; } set { resetpin = value; } }

        /// <summary>
        ///     GPIO pin for resetting the I2C Switch
        /// </summary>
        public byte ResetPin { get; set; }

        /// <summary>
        ///     Shows if there is a connection with the I2C Switch
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     Open a connection with the IO Pi.
        /// </summary>

        public void Connect()
        {
            if (IsConnected)
            {
                return; // Already connected
            }

            // Initialize the I2C bus
            try
            {
                var i2cbusConnectionSettings = new I2cConnectionSettings(1, Address);
                i2cbus = I2cDevice.Create(i2cbusConnectionSettings);

                // Check to see if the output pin has been set and if so try to connect to the GPIO pin on the Raspberry Pi and set the pin high
                if (ResetPin != 0)
                {
                    GpioController gpio = new GpioController();
                    gpio.OpenPin(resetpin, PinMode.Output);
                    gpio.Write(resetpin, PinValue.High);
                    gpio.Dispose();
                }

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
        ///     Enable the specified I2C channel and disable other channels.
        /// </summary>
        /// <param name="channel">1 to 4</param>
        public void SwitchChannel(byte channel)
        {
            CheckConnected();

            if (channel >= 1 && channel <= 4)
            {
                byte buffer = 0;
                channel = (byte)(channel - 1);
                buffer = helper.UpdateByte(buffer, channel, true);
                helper.WriteI2CSingleByte(i2cbus, buffer);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }
        }

        /// <summary>
        ///     Set the state of an individual I2C channel.
        /// </summary>
        /// <param name="channel">1 to 4</param>
        /// <param name="state">true or false</param>
        public void SetChannelState(byte channel, bool state)
        {
            CheckConnected();

            if (channel >= 1 && channel <= 4)
            {
                byte buffer = helper.ReadI2CSingleByte(i2cbus);
                channel = (byte)(channel - 1);
                if (state)
                {
                    buffer = helper.UpdateByte(buffer, channel, true);
                }
                else
                {
                    buffer = helper.UpdateByte(buffer, channel, false);
                }                
                helper.WriteI2CSingleByte(i2cbus, buffer);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }
        }

        /// <summary>
        ///     Get the state of an individual I2C channel.
        /// </summary>
        /// <param name="channel">1 to 4</param>
        public bool GetChannelState(byte channel)
        {
            CheckConnected();

            if (channel >= 1 && channel <= 4)
            {
                byte buffer = helper.ReadI2CSingleByte(i2cbus);
                channel = (byte)(channel - 1);
                return helper.CheckBit(buffer, channel);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }
        }

        /// <summary>
        ///     Reset the I2C switch.
        /// </summary>
        public void Reset()
        {
            GpioController gpio = new GpioController();
            gpio.OpenPin(ResetPin, PinMode.Output);
            gpio.Write(ResetPin, PinValue.Low);
            Thread.Sleep(1); // wait 1ms before setting the pin high again
            gpio.Write(ResetPin, PinValue.High);
            Thread.Sleep(1); // wait another 1ms for the reset to complete
            gpio.Dispose();
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
