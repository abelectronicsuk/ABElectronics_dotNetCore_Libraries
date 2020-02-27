/*
ABElectronics Expander Pi | DAC Speed Demo

this demo will output a 3.3V square wave, testing the maximum speed of the DAC
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
            while (true)
            {
                expander.DACSetRaw(1, 4095, 2);  // set the voltage on channel 1 to 3.3V
                expander.DACSetRaw(1, 0, 2);  // set the voltage on channel 1 to 0V
            }
        }

    }
}
