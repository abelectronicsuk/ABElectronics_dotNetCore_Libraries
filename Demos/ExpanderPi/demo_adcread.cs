/*
AB Electronics UK Expander Pi | ADC Read Demo

this demo reads the voltage from the 8 ADC inputs
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

            // Clear the console window
            Console.Clear();


            while (true)
            {
                // Read ADC channels 1 to 8 in single-ended mode and write them to the console
                Console.WriteLine("ADC Channel 1: " + expander.ADCReadVoltage(1, 1).ToString("N6"));
                Console.WriteLine("ADC Channel 2: " + expander.ADCReadVoltage(2, 1).ToString("N6"));
                Console.WriteLine("ADC Channel 3: " + expander.ADCReadVoltage(3, 1).ToString("N6"));
                Console.WriteLine("ADC Channel 4: " + expander.ADCReadVoltage(4, 1).ToString("N6"));
                Console.WriteLine("ADC Channel 5: " + expander.ADCReadVoltage(5, 1).ToString("N6"));
                Console.WriteLine("ADC Channel 6: " + expander.ADCReadVoltage(6, 1).ToString("N6"));
                Console.WriteLine("ADC Channel 7: " + expander.ADCReadVoltage(7, 1).ToString("N6"));
                Console.WriteLine("ADC Channel 8: " + expander.ADCReadVoltage(8, 1).ToString("N6"));

                // Move the cursor to the top of the console
                Console.SetCursorPosition(0, 0);
            }
        }

    }
}
