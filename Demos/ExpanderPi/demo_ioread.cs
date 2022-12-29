/*
AB Electronics UK Expander Pi | Digital I/O Read Demo

This example reads from all 16 pins on the Expander Pi digital IO port.
The internal pull-up resistors are enabled so each pin will read
as 1 unless the pin is connected to ground.
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
            // We will read the inputs 1 to 16 from the I/O bus so set port 0 and
            // port 1 as inputs and enable the internal pull-up resistors
            expander.IOSetPortDirection(0, 0xFF);
            expander.IOSetPortPullups(0, 0xFF);

            expander.IOSetPortDirection(1, 0xFF);
            expander.IOSetPortPullups(1, 0xFF);

            // clear the console
            Console.Clear();

            while (true)
            {
                // read the pins 1 to 16 and print the results
                Console.WriteLine("Bus 1                   Bus 2");
                Console.WriteLine("Pin 1:  " + FormatIO(expander.IOReadPin(1)));
                Console.WriteLine("Pin 2:  " + FormatIO(expander.IOReadPin(2)));
                Console.WriteLine("Pin 3:  " + FormatIO(expander.IOReadPin(3)));
                Console.WriteLine("Pin 4:  " + FormatIO(expander.IOReadPin(4)));
                Console.WriteLine("Pin 5:  " + FormatIO(expander.IOReadPin(5)));
                Console.WriteLine("Pin 6:  " + FormatIO(expander.IOReadPin(6)));
                Console.WriteLine("Pin 7:  " + FormatIO(expander.IOReadPin(7)));
                Console.WriteLine("Pin 8:  " + FormatIO(expander.IOReadPin(8)));
                Console.WriteLine("Pin 9:  " + FormatIO(expander.IOReadPin(9)));
                Console.WriteLine("Pin 10: " + FormatIO(expander.IOReadPin(10)));
                Console.WriteLine("Pin 11: " + FormatIO(expander.IOReadPin(11)));
                Console.WriteLine("Pin 12: " + FormatIO(expander.IOReadPin(12)));
                Console.WriteLine("Pin 13: " + FormatIO(expander.IOReadPin(13)));
                Console.WriteLine("Pin 14: " + FormatIO(expander.IOReadPin(14)));
                Console.WriteLine("Pin 15: " + FormatIO(expander.IOReadPin(15)));
                Console.WriteLine("Pin 16: " + FormatIO(expander.IOReadPin(16)));

                // Move the cursor to the top of the console
                Console.SetCursorPosition(0, 0);
            }
        }

        private string FormatIO(bool value)
        {
            return value ? "On " : "Off";
        }

    }
}
