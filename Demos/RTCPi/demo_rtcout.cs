/*
AB Electronics UK RTC Pi | RTC clock output demo

This demo shows how to enable the clock square wave output on the RTC Pi real-time clock and set the frequency
*/

using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the RTCPi object
        RTCPi rtc = new RTCPi();

        static void Main(string[] args)
        {
            // Call Rtc_Connect()
            Program demo = new Program();
            demo.Rtc_Connect();
        }

        private void Rtc_Connect()
        {
            // Connect to the RTC and wait for the Connected event
            rtc.Connected += Rtc_Connected;
            rtc.Connect();
        }


        private void Rtc_Connected(object sender, EventArgs e)
        {
            // Enable the square wave output
            rtc.Output = true;

            // Set the frequency to 32.768KHz
            rtc.Frequency = 4;


            // Wait for ESC key press to exit the program
            Console.WriteLine("Clock enabled and freqency set to 32.768KHz");
        }

    }
}
