using System;
using System.Device.Gpio;
using System.Device.I2c;

namespace ABElectronicsUK
{
    /// <summary>
    ///     Class for controlling the Servo Pi expansion board from AB Electronics UK
    ///     Based on the PCA9685 PWM controller IC from NXT.
    /// </summary>
    public class ServoPi : IDisposable
    {
        private readonly ABE_Helpers helper = new ABE_Helpers();

        // create an instance of the i2c bus and GPIO controller
        private I2cDevice i2cbus;

        // Define registers values from the datasheet
        private const byte MODE1 = 0x00;
        private const byte MODE2 = 0x01;
        private const byte SUBADR1 = 0x02;
        private const byte SUBADR2 = 0x03;
        private const byte SUBADR3 = 0x04;
        private const byte ALLCALLADR = 0x05;
        private const byte LED0_ON_L = 0x06;
        private const byte LED0_ON_H = 0x07;
        private const byte LED0_OFF_L = 0x08;
        private const byte LED0_OFF_H = 0x09;
        private const byte ALL_LED_ON_L = 0xFA;
        private const byte ALL_LED_ON_H = 0xFB;
        private const byte ALL_LED_OFF_L = 0xFC;
        private const byte ALL_LED_OFF_H = 0xFD;
        private const byte PRE_SCALE = 0xFE;

        // define mode bits
        private const byte MODE1_EXTCLK = 6; // use external clock
        private const byte MODE1_SLEEP = 4; // sleep mode
        private const byte MODE1_ALLCALL = 0; // all call address

        private const byte MODE2_INVRT = 4; // invert output
        private const byte MODE2_OCH = 3; // output type
        private const byte MODE2_OUTDRV = 2; // output type
        private const byte MODE2_OUTNE1 = 0; // output mode when not enabled

        // private variables
        private byte oepin = 7;

        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        System.Runtime.InteropServices.SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        ///     Create an instance of a Servo Pi bus.
        /// </summary>
        /// <param name="address">I2C address of Servo Pi bus</param>
        /// <param name="outputEnablePin">GPIO pin for Output Enable function.  The default GPIO pin 4 is not supported in .net Core so the OE pad will need to be connected to a different GPIO pin.</param>
        /// <example>ABElectronicsUK.ServoPi servo = new ABElectronicsUK.ServoPi();</example>
        public ServoPi(byte address = 0x40, byte outputEnablePin = 0)
        {
            Address = address;
            IsConnected = false;
            oepin = outputEnablePin;
        }


        /// <summary>
        ///     I2C address for the Servo Pi bus.
        /// </summary>
        /// <example>servopi.Address = 0x40;</example>
        public byte Address { get; set; }

        /// <summary>
        ///     Set the GPIO pin for the output enable function.
        ///     The default GPIO pin 4 is not supported in .net Core so the OE pad will need to be connected to a different GPIO pin.
        /// </summary>
        /// <example>servopi.OutputEnablePin = 17;</example>
        public byte OutputEnablePin { get { return oepin; } set { oepin = value; } }

        /// <summary>
        ///     Shows if there is a connection with the Servo Pi
        /// </summary>
        /// <example>if (servopi.IsConnected) { }</example>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     Open a connection with the Servo Pi
        /// </summary>
        /// <returns></returns>
        /// <example>servopi.Connect();</example>

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

                if (i2cbus != null)
                {
                    // Connection is established so set IsConnected to true

                    IsConnected = true;

                    helper.WriteI2CByte(i2cbus, MODE1, 0x00);

                    // Check to see if the output pin has been set and if so try to connect to the GPIO pin on the Raspberry Pi
                    if (oepin != 0)
                    {
                        GpioController gpio = new GpioController();
                        gpio.OpenPin(oepin, PinMode.Output);
                        gpio.Write(oepin, PinValue.High);
                        gpio.Dispose();
                    }

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
        ///     Event occurs when connection is made.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        ///     Set the output frequency of all PWM channels.
        ///     The output frequency is programmable from a typical 40Hz to 1000Hz.
        /// </summary>
        /// <param name="freq">Integer frequency value</param>
        /// <example>servopi.SetPWMFreqency(500);</example>
        public void SetPWMFreqency(int freq)
        {
            var scaleval = 25000000.0; // 25MHz
            scaleval /= 4096.0; // 12-bit
            scaleval /= freq;
            scaleval -= 1.0;
            var prescale = Math.Floor(scaleval + 0.5);
            var oldmode = helper.ReadI2CByte(i2cbus, MODE1);
            var newmode = (byte)((oldmode & 0x7F) | 0x10);
            helper.WriteI2CByte(i2cbus, MODE1, newmode);
            helper.WriteI2CByte(i2cbus, PRE_SCALE, (byte)Math.Floor(prescale));
            helper.WriteI2CByte(i2cbus, MODE1, oldmode);
            helper.WriteI2CByte(i2cbus, MODE1, (byte)(oldmode | 0x80));
        }

        /// <summary>
        ///     Set the PWM output on a single <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel">1 to 16</param>
        /// <param name="on">Value between 0 and 4096</param>
        /// <param name="off">Value between 0 and 4096</param>
        /// <example>servopi.SetPWM(1,512,1024);</example>
        public void SetPWM(byte channel, short on, short off)
        {
            if (channel < 1 || channel > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }
            
            if (on < 0 || on > 4095)
            {
                throw new ArgumentOutOfRangeException(nameof(on));
            }

            if (off < 0 || on > off)
            {
                throw new ArgumentOutOfRangeException(nameof(on));
            }

            SetPWMOnTime(channel, on);
            SetPWMOffTime(channel, off);
        }

        /// <summary>
        /// Set the output on time on a single channel
        /// </summary>
        /// <param name="channel">1 to 16</param>
        /// <param name="on">0 to 4095</param>
        public void SetPWMOnTime(byte channel, short on)
        {
            if (channel< 1 || channel> 16){
                throw new ArgumentOutOfRangeException(nameof(channel));
            }


            if (on < 0 || on > 4095){
                throw new ArgumentOutOfRangeException(nameof(on));
            }

            channel = (byte)(channel - 1);
            helper.WriteI2CByte(i2cbus, (byte)(LED0_ON_L + 4 * channel), (byte)(on & 0xFF));
            helper.WriteI2CByte(i2cbus, (byte)(LED0_ON_H + 4 * channel), (byte)(on >> 8));
        }

        /// <summary>
        /// Set the output off time on a single channel
        /// </summary>
        /// <param name="channel">1 to 16</param>
        /// <param name="off">0 to 4095</param>
        public void SetPWMOffTime(byte channel, short off)
        {
            if (channel < 1 || channel > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }


            if (off < 0 || off > 4095)
            {
                throw new ArgumentOutOfRangeException(nameof(off));
            }

            channel = (byte)(channel - 1);
            helper.WriteI2CByte(i2cbus, (byte)(LED0_OFF_L + 4 * channel), (byte)(off & 0xFF));
            helper.WriteI2CByte(i2cbus, (byte)(LED0_OFF_H + 4 * channel), (byte)(off >> 8));
        }

        /// <summary>
        ///     Set PWM output on all channels.
        /// </summary>
        /// <param name="on">Value between 0 and 4096</param>
        /// <param name="off">Value between 0 and 4096</param>
        /// <example>servopi.SetAllPWM(512,1024);</example>
        public void SetAllPWM(short on, short off)
        {
            helper.WriteI2CByte(i2cbus, ALL_LED_ON_L, (byte) (on & 0xFF));
            helper.WriteI2CByte(i2cbus, ALL_LED_ON_H, (byte) (on >> 8));
            helper.WriteI2CByte(i2cbus, ALL_LED_OFF_L, (byte) (off & 0xFF));
            helper.WriteI2CByte(i2cbus, ALL_LED_OFF_H, (byte) (off >> 8));
        }

        /// <summary>
        ///     Disable output via OE pin.  Only used when the OE jumper is joined.
        /// </summary>
        /// <example>servopi.OutputDisable();</example>
        public void OutputDisable()
        {
            if (oepin == 0)
            {
                throw new InvalidOperationException("Output Enable Pin was not set.");
            }
            else
            {
                GpioController gpio = new GpioController();
                gpio.OpenPin(oepin, PinMode.Output);
                gpio.Write(oepin, PinValue.High);
                gpio.Dispose();
            }
        }

        /// <summary>
        ///     Enable output via OE pin.  Only used when the OE jumper is joined.
        /// </summary>
        /// <example>servopi.OutputEnable();</example>
        public void OutputEnable()
        {
            if (oepin == 0)
            {
                throw new InvalidOperationException("Output Enable Pin was not set.");
            }
            else
            {
                GpioController gpio = new GpioController();
                gpio.OpenPin(oepin, PinMode.Output);
                gpio.Write(oepin, PinValue.Low);
                gpio.Dispose();
            }
        }

        /// <summary>
        /// The All Call Address allows all Servo Pi devices on the I2C bus to be controlled at the same time.
        /// </summary>
        /// <param name="address">0x03 to 0x77</param>
        public void SetAllCallAddress(byte address)
        {
            if (address > 0x03 && address <= 0x77)
            {
                var oldmode = helper.ReadI2CByte(i2cbus, MODE1);
                var newmode = (byte)(oldmode | (1 << MODE1_ALLCALL));
                helper.WriteI2CByte(i2cbus, MODE1, newmode);
                helper.WriteI2CByte(i2cbus, ALLCALLADR, (byte)(address << 1));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }
        }

        /// <summary>
        /// Enable the I2C address for the All Call function
        /// </summary>
        public void EnableAllCallAddress()
        {
            var oldmode = helper.ReadI2CByte(i2cbus, MODE1);
            var newmode = (byte)(oldmode | (1 << MODE1_ALLCALL));
            helper.WriteI2CByte(i2cbus, MODE1, newmode);
        }

        /// <summary>
        /// Disable the I2C address for the All Call function
        /// </summary>
        public void DisableAllCallAddress()
        {
            var oldmode = helper.ReadI2CByte(i2cbus, MODE1);
            var newmode = (byte)(oldmode & ~(1 << MODE1_ALLCALL));
            helper.WriteI2CByte(i2cbus, MODE1, newmode);
        }

        /// <summary>
        /// Sets or gets the sleep status for the Servo Pi.  
        /// Set true to put the device into a sleep state or false to wake the device.
        /// Get true = sleeping, false = awake.
        /// </summary>
        public bool Sleep
        {
            get
            {
                var oldmode = helper.ReadI2CByte(i2cbus, MODE1);
                return helper.CheckBit(oldmode, MODE1_SLEEP);
            }
            set
            {
                var oldmode = helper.ReadI2CByte(i2cbus, MODE1);
                var newmode = oldmode;
                if (value == true)
                {
                    newmode = (byte)(oldmode | (1 << MODE1_SLEEP));
                }
                else
                {
                    newmode = (byte)(oldmode & ~(1 << MODE1_SLEEP));
                }
                
                helper.WriteI2CByte(i2cbus, MODE1, newmode);
            }            
        }

        /// <summary>
        /// Invert the PWM output on all channels.  
        /// true = invert, false = normal.
        /// </summary>
        public bool InvertOutput
        {
            get
            {
                var oldmode = helper.ReadI2CByte(i2cbus, MODE2);
                return helper.CheckBit(oldmode, MODE2_INVRT);
            }
            set
            {
                var oldmode = helper.ReadI2CByte(i2cbus, MODE2);
                var newmode = oldmode;
                if (value == true)
                {
                    newmode = (byte)(oldmode | (1 << MODE2_INVRT));
                }
                else
                {
                    newmode = (byte)(oldmode & ~(1 << MODE2_INVRT));
                }

                helper.WriteI2CByte(i2cbus, MODE2, newmode);
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