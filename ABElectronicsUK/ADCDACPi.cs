using System;
using System.Device.Spi;

namespace ABElectronicsUK
{
    /// <summary>
    ///     Class for accessing the ADCDAC Pi from AB Electronics UK.
    /// </summary>
    public class ADCDACPi : IDisposable
    {
        private const int SPI_BUS_ID = 0;
        private const Int32 ADC_CHIP_SELECT_LINE = 0; // ADC on SPI channel select CE0
        private const Int32 DAC_CHIP_SELECT_LINE = 1; // ADC on SPI channel select CE1
        private byte dacgain = 1;

        private SpiDevice adc;
        private double adcref = 3.3;

        private SpiDevice dac;

        /// <summary>
        ///     Event triggers when a connection is established.
        /// </summary>
        public bool IsConnected { get; private set; }

        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        System.Runtime.InteropServices.SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        ///     Open a connection to the ADCDAC Pi.
        /// </summary>
        public void Connect()
        {
            if (IsConnected)
            {
                return; // Already connected
            }

            try
            {
                // Create SPI initialization settings for the ADC
                var adcSettings = new SpiConnectionSettings(SPI_BUS_ID, ADC_CHIP_SELECT_LINE)
                    {
                        ClockFrequency = 10000000, // SPI clock frequency of 10MHz
                        Mode = SpiMode.Mode0
                    };

                adc = SpiDevice.Create(adcSettings); // Create an ADC connection with our bus controller and SPI settings

                // Create SPI initialization settings for the DAC
                var dacSettings =
                    new SpiConnectionSettings(SPI_BUS_ID, DAC_CHIP_SELECT_LINE)
                    {
                        ClockFrequency = 2000000,  // SPI clock frequency of 20MHz
                        Mode = SpiMode.Mode0
                    };

                dac = SpiDevice.Create(dacSettings); // Create an ADC connection with our bus controller and SPI settings

                IsConnected = true; // connection established, set IsConnected to true.

                // Fire the Connected event handler
                Connected?.Invoke(this, EventArgs.Empty);
            }
            /* If initialization fails, display the exception and stop running */
            catch (Exception ex)
            {
                IsConnected = false;
                throw new Exception("SPI Initialization Failed", ex);
            }
        }

        /// <summary>
        ///     Event occurs when connection is made.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        ///     Read the voltage from the selected <paramref name="channel" /> on the ADC.
        /// </summary>
        /// <param name="channel">1 or 2</param>
        /// <returns>voltage</returns>
        public double ReadADCVoltage(byte channel)
        {
            if (channel < 1 || channel > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            var raw = ReadADCRaw(channel);
            var voltage = adcref / 4096 * raw; // convert the raw value into a voltage based on the reference voltage.
            return voltage;
        }


        /// <summary>
        ///     Read the raw value from the selected <paramref name="channel" /> on the ADC.
        /// </summary>
        /// <param name="channel">1 or 2</param>
        /// <returns>Integer</returns>
        public int ReadADCRaw(byte channel)
        {
            if (channel < 1 || channel > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            CheckConnected();

            var writeArray = new byte[] { 0x01, (byte) ((1 + channel) << 6), 0x00}; // create the write bytes based on the input channel

            var readBuffer = new byte[3]; // this holds the output data

            adc.TransferFullDuplex(writeArray, readBuffer); // transfer the adc data

            var ret = (short) (((readBuffer[1] & 0x0F) << 8) + readBuffer[2]); // combine the two bytes into a single 16bit integer

            return ret;
        }

        /// <summary>
        ///     Gets or sets the reference voltage for the analogue to digital converter.
        ///     The ADC uses the raspberry pi 3.3V power as a reference so using this property to set the reference to match the exact output 
        ///     voltage from the 3.3V regulator will increase the accuracy of the ADC readings.
        /// </summary>
        public double ADCReferenceVoltage
        {
            get { return adcref; }   // get method
            set {
                if (value < 0.0 || value > 7.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Reference voltage must be between 0.0V and 7.0V.");
                }

                adcref = value;
            }  
        }

        /// <summary>
        ///     Set the <paramref name="voltage" /> for the selected channel on the DAC.
        /// </summary>
        /// <param name="channel">1 or 2</param>
        /// <param name="voltage">When DAC Gain = 1 the voltage can be between 0 and 2.047V. When DAC Gain = 2 the voltage can be between 0 and 3.3V</param>
        public void SetDACVoltage(byte channel, double voltage)
        {
            // Check for valid channel and voltage variables
            if (channel < 1 || channel > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            if (dacgain == 1 && voltage < 0.0 || voltage > 2.048)
            {
                throw new ArgumentOutOfRangeException(nameof(voltage));
            }

            if (dacgain == 2 && voltage < 0.0 || voltage > 3.3)
            {
                throw new ArgumentOutOfRangeException(nameof(voltage));
            }
            
            var rawval = Convert.ToInt16(voltage / 2.048 * 4096); // convert the voltage into a raw value
            SetDACRaw(channel, rawval);

        }

        /// <summary>
        ///     Set the raw <paramref name="value" /> from the selected <paramref name="channel" /> on the DAC.
        /// </summary>
        /// <param name="channel">1 or 2</param>
        /// <param name="value">Value between 0 and 4095</param>
        public void SetDACRaw(byte channel, short value)
        {
            CheckConnected();

            if (channel < 1 || channel > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            // split the raw value into two bytes and send it to the DAC.
            var lowByte = (byte) (value & 0xff);
            byte highByte = 0;
            if (dacgain == 1)
            {
                highByte = (byte)(((value >> 8) & 0xff) | ((channel - 1) << 7) | (0x1 << 5) | (1 << 4));
            }
            else
            {
                highByte = (byte)(((value >> 8) & 0xff) | ((channel - 1) << 7) | (1 << 4));
            }
            var writeBuffer = new [] { highByte, lowByte};
            dac.Write(writeBuffer);
        }

        /// <summary>
        ///    Gets or Sets the gain for the DAC.
        ///    When gain = 1 the voltage range will be 0V to 2.047V
        ///    When gain = 2 the voltage range will be 0V to VCC which is typically 3.3V on a Raspberry Pi.
        /// </summary>
        /// <param name="gain">1 or 2</param>
        public byte DACGain
        {
            get { return dacgain; }   // get method
            set {
                if (value < 1 || value > 2)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                dacgain = value;
            }  // set method
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
                adc?.Dispose();
                adc = null;

                dac?.Dispose();
                dac = null;

                IsConnected = false;
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }
}