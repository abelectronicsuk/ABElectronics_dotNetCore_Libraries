/*
AB Electronics UK ADC DAC Pi 2-Channel ADC, 2-Channel DAC | DAC Write Demo

this demo will generate a 1.5V p-p square wave at 1Hz
*/

using System;
using System.Timers;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ADCDACPi object
        ADCDACPi adcdac = new ADCDACPi();

        private Timer timer;
        private bool logicstate = false;

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
            // Set the DAC gain to 1 giving a voltage range of 0 to 2.047V
            adcdac.DACGain = 2;

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
                adcdac.SetDACVoltage(1, 0); // Set voltage to 0V
            }
            else
            {
                adcdac.SetDACVoltage(1, 1.5); // Set voltage to 1.5V
            }
            logicstate = !logicstate; // Invert the logicstate variable
        }

    }
}
