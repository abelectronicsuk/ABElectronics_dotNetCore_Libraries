/*
AB Electronics UK ADC DAC Pi 2-Channel ADC, 2-Channel DAC | ADC Read Demo

this demo reads the voltage from channel 1 on the ADC inputs
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

            // Clear the console window
            Console.Clear();


            while (true)
            {
                // Read ADC channels 1 and 2 and write them to the console
                Console.Write("ADC Channel 1: " + adcdac.ReadADCVoltage(1).ToString("N6"));

                // Move the cursor to the top of the console
                Console.SetCursorPosition(0, 0);
            }
        }

    }
}
