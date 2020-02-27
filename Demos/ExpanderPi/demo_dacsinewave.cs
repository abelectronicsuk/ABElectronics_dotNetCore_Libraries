/*
ABElectronics Expander Pi | DAC sine wave generator demo

this demo generates a sine wave on the DAC output
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
            // Connect to the ExpanderPi Pi and wait for the Connected event
            expander.Connected += ExpanderPi_Connected;
            expander.Connect();
        }


        private void ExpanderPi_Connected(object sender, EventArgs e)
        {
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
                expander.DACSetRaw(1, SinewaveArray[x], 2);
                x++;
                if (x >= (NumberOfPositions)) { x = 0; }
            }
        }

    }
}
