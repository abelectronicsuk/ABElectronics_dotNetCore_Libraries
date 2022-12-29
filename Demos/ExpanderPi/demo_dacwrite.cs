/*
AB Electronics UK Expander Pi | DAC Write Demo

this demo will generate a 1.5V p-p square wave at 1Hz
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
        private bool logicstate = false;

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
            // Create a timer with a 500ms time interval
            timer = new System.Timers.Timer
            {
                Interval = 500
            };

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
            if (logicstate)
            {
                expander.DACSetVoltage(1, 0, 1); // Set voltage to 0V with the gain set to 1
            }
            else
            {
                expander.DACSetVoltage(1, 1.5, 1); // Set voltage to 1.5V with the gain set to 1
            }
            logicstate = !logicstate; // Invert the logicstate variable
        }

    }
}
