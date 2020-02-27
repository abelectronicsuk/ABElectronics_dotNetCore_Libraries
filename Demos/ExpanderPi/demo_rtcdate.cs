/*
ABElectronics Expander Pi | Date Demo

This demo shows how to set the current time to the RTC and read the date and time on the Expander Pi real-time clock at 1 second intervals
*/

using System;
using System.Timers;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ExpanderPi object
        ExpanderPi expander = new ExpanderPi();

        private Timer timer;

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
            // Set the RTC date to the current date
            expander.RTCDate = DateTime.Now;

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
            // Read the date from the RTC
            Console.WriteLine(expander.RTCDate.ToString("s"));
        }
    }
}
