/*
AB Electronics UK Expander Pi | RTC clock output demo

This demo shows how to enable the clock square wave output on the RTC Pi real-time clock and set the frequency
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
            // Enable the square wave output
            expander.RTCOutput = true;

            // Set the frequency to 32.768KHz
            expander.RTCFrequency = 4;


            // Wait for ESC key press to exit the program
            Console.WriteLine("Clock enabled and freqency set to 32.768KHz");
        }
    }
}
