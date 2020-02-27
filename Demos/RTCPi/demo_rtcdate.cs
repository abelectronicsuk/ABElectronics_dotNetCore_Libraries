/*
ABElectronics RTC Pi |  Date Demo

This demo shows how to set the current time to the RTC and read the date and time on the RTC Pi real-time clock at 1 second intervals
*/

using System;
using System.Timers;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the RTCPi object
        RTCPi rtc = new RTCPi();

        private Timer timer;

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
            // Set the RTC date to the current date
            rtc.Date = DateTime.Now;

            // Create a timer that will run once a second.
            timer = new System.Timers.Timer();
            timer.Interval = 1000;

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            // Wait for ESC key press to exit the program
            Console.WriteLine("Press ESC to stop");
            do
            {
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine(rtc.Date.ToString("s"));
        }
    }
}
