/*
AB Electronics UK ADC Differential Pi 8-Channel ADC speed test demo

Initialise the ADC device using the default addresses and test the
samples per second at the selected bit rate
*/


using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ADCDifferentialPi object with I2C addresses 0x68 and 0x69
        ADCDifferentialPi adc = new ADCDifferentialPi(0x68, 0x69);

        static void Main(string[] args)
        {
            // Call Adc_Connect()
            Program demo = new Program();
            demo.Adc_Connect();
        }

        private void Adc_Connect()
        {
            // Connect to the ADC Differential Pi and wait for the Connected event
            adc.Connected += Adc_Connected;
            adc.Connect();
        }


        private void Adc_Connected(object sender, EventArgs e)
        {
            // Set the ADC bit rate to 16 bit
            adc.BitRate = 16;

            Int32 counter = 1;
            Int32 totalsamples = 1000;

            double[] readarray = new double[totalsamples];

            DateTime starttime = DateTime.Now;

            // Clear the console window
            Console.Clear();

            Console.WriteLine("Start: " + starttime.ToString("s"));

            while (counter < totalsamples)
            {
                // read the voltage from channel 1 and display it on the screen
                readarray[counter] = adc.ReadVoltage(1);

                counter += 1;
            }

            DateTime endtime = DateTime.Now;

            Console.WriteLine("End: " + endtime.ToString("s"));

            double totalseconds = (endtime - starttime).TotalSeconds;

            double samplespersecond = totalsamples / totalseconds;

            Console.WriteLine(samplespersecond.ToString("N2") + " samples per second");

        }

    }
}
