/*
AB Electronics UK Expander Pi | ADC Speed Demo

this demo tests the maximum sample speed for the ADC
*/

using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ExpanderPi object
        ExpanderPi expander = new ExpanderPi();

        static void Main(string[] args)
        {
            // Call ExpanderPi_Connect()
            Program demo = new Program();
            demo.ExpanderPi_Connect();
        }

        private void ExpanderPi_Connect()
        {
            // Connect to the Expander Pi and wait for the Connected event
            expander.Connected += ExpanderPi_Connected;
            expander.Connect();
        }


        private void ExpanderPi_Connected(object sender, EventArgs e)
        {
            // set the reference voltage to the onboard voltage reference of 4.096V.
            expander.ADCReferenceVoltage = 4.096;

            Int32 counter = 1;
            Int32 totalsamples = 10000;

            double[] readarray = new double[totalsamples];

            DateTime starttime = DateTime.Now;

            // Clear the console window
            Console.Clear();

            Console.WriteLine("Start: " + starttime.ToString("s"));

            while (counter < totalsamples)
            {
                // read the voltage from channel 1 and display it on the screen
                readarray[counter] = expander.ADCReadVoltage(1, 1);

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
