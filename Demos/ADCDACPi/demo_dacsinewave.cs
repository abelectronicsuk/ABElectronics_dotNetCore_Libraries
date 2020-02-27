/*
ABElectronics ADC-DAC Pi 2-Channel ADC, 2-Channel DAC 
DAC sine wave generator demo

this demo generates a sine wave on the DAC output
*/


using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of teh ADCDACPi object
        ADCDACPi adcdac = new ADCDACPi();

        static void Main(string[] args)
        {
            // Call AdcDac_Connect()
            Program demo = new Program();
            demo.AdcDac_Connect();
        }

        private void AdcDac_Connect()
        {
            // Connect to the ADCDAC Pi and wait for the Connected event
            adcdac.Connected += AdcDac_Connected;
            adcdac.Connect();
        }


        private void AdcDac_Connected(object sender, EventArgs e)
        {
            // Set the DAC gain to 2 giving a voltage range of 0 to 3.3V
            adcdac.DACGain = 2;

            // Define an array containing a sine wave

            int NumberOfPositions = 128;

            short[] SinewaveArray = new short[NumberOfPositions + 1];
            double xStep = 1.0 / NumberOfPositions;
            double[] xValues = new double[NumberOfPositions + 1];
            for (int i = 0; i < NumberOfPositions + 1; i++)
            {
                xValues[i] = i * xStep;
                double sinevalue = Math.Sin(xValues[i] * 2 * Math.PI);
                short dacvalue = (short)Math.Round((2048 * sinevalue) + 2048);
                if (dacvalue > 4095) dacvalue = 4095;
                SinewaveArray[i] = dacvalue;
            }

            Console.WriteLine("Outputting sine wave to DAC channel 1");

            // Loop through SinewaveArray and write each value to the DAC
            int x = 0;
            while (true)
            {
                adcdac.SetDACRaw(1, SinewaveArray[x]);
                x++;
                if (x >= (NumberOfPositions)) { x = 0; }
            }

        }

    }
}
