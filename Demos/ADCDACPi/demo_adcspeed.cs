/*
AB Electronics UK ADC DAC Pi 2-Channel ADC, 2-Channel DAC | ADC Speed Demo

this demo tests the maximum sample speed for the ADC
*/


using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ADCDACPi object
        ADCDACPi adcdac = new ADCDACPi();

        static void Main(string[] args)
        {
            // Call AdcDac_Connect()
            Program demo = new Program();
            demo.AdcDac_Connect();
        }

        private void AdcDac_Connect()
        {
            // Connect to the ADC DAC Pi and wait for the Connected event
            adcdac.Connected += AdcDac_Connected;
            adcdac.Connect();
        }


        private void AdcDac_Connected(object sender, EventArgs e)
        {
            // set the reference voltage.  this should be set to the exact voltage
            // measured on the raspberry pi 3.3V rail.
            adcdac.ADCReferenceVoltage = 3.3;

            Int32 counter = 1;
            Int32 totalsamples = 100000;

            double[] readarray = new double[totalsamples];

            DateTime starttime = DateTime.Now;

            // Clear the console window
            Console.Clear();

            Console.WriteLine("Start: " + starttime.ToString("s"));

            while (counter < totalsamples)
            {
                // read the voltage from channel 1 and display it on the screen
                readarray[counter] = adcdac.ReadADCVoltage(1);

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
