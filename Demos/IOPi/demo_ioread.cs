/*
ABElectronics IO Pi | Digital I/O Read Demo

This example reads from all 16 pins on both buses on the IO Pi Plus.
The internal pull-up resistors are enabled so each pin will read
as 1 unless the pin is connected to ground.
*/

using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create two instances of the IOPi object with  I2C addresses of 0x20 and 0x21
        IOPi iobus1 = new IOPi(0x20);
        IOPi iobus2 = new IOPi(0x21);

        static void Main(string[] args)
        {
            // Call IO_Connect()
            Program demo = new Program();
            demo.IO_Connect();
        }

        private void IO_Connect()
        {
            // Connect to the IO Pi bus 1 and wait for the Connected event
            iobus1.Connected += IO1_Connected;
            iobus1.Connect();
        }

        private void IO1_Connected(object sender, EventArgs e)
        {
            // Now connect to the IO Pi bus 2 and wait for the Connected event
            iobus2.Connected += IO2_Connected;
            iobus2.Connect();
        }

        private void IO2_Connected(object sender, EventArgs e)
        {
            // We will read the inputs 1 to 16 from the I/O bus so set port 0 and
            // port 1 to be inputs and enable the internal pull-up resistors
            iobus1.SetPortDirection(0, 0xFF);
            iobus1.SetPortPullups(0, 0xFF);

            iobus1.SetPortDirection(1, 0xFF);
            iobus1.SetPortPullups(1, 0xFF);

            // Repeat the steps above for the second bus
            iobus2.SetPortDirection(0, 0xFF);
            iobus2.SetPortPullups(0, 0xFF);

            iobus2.SetPortDirection(1, 0xFF);
            iobus2.SetPortPullups(1, 0xFF);

            // clear the console
            Console.Clear();

            while (true)
            {
                // read the pins 1 to 16 on both buses and print the results
                Console.WriteLine("Bus 1                   Bus 2");
                Console.WriteLine("Pin 1:  " + FormatIO(iobus1.ReadPin(1)) + "           Pin 1:  " + FormatIO(iobus2.ReadPin(1)));
                Console.WriteLine("Pin 2:  " + FormatIO(iobus1.ReadPin(2)) + "           Pin 2:  " + FormatIO(iobus2.ReadPin(2)));
                Console.WriteLine("Pin 3:  " + FormatIO(iobus1.ReadPin(3)) + "           Pin 3:  " + FormatIO(iobus2.ReadPin(3)));
                Console.WriteLine("Pin 4:  " + FormatIO(iobus1.ReadPin(4)) + "           Pin 4:  " + FormatIO(iobus2.ReadPin(4)));
                Console.WriteLine("Pin 5:  " + FormatIO(iobus1.ReadPin(5)) + "           Pin 5:  " + FormatIO(iobus2.ReadPin(5)));
                Console.WriteLine("Pin 6:  " + FormatIO(iobus1.ReadPin(6)) + "           Pin 6:  " + FormatIO(iobus2.ReadPin(6)));
                Console.WriteLine("Pin 7:  " + FormatIO(iobus1.ReadPin(7)) + "           Pin 7:  " + FormatIO(iobus2.ReadPin(7)));
                Console.WriteLine("Pin 8:  " + FormatIO(iobus1.ReadPin(8)) + "           Pin 8:  " + FormatIO(iobus2.ReadPin(8)));
                Console.WriteLine("Pin 9:  " + FormatIO(iobus1.ReadPin(9)) + "           Pin 9:  " + FormatIO(iobus2.ReadPin(9)));
                Console.WriteLine("Pin 10: " + FormatIO(iobus1.ReadPin(10)) + "           Pin 10: " + FormatIO(iobus2.ReadPin(10)));
                Console.WriteLine("Pin 11: " + FormatIO(iobus1.ReadPin(11)) + "           Pin 11: " + FormatIO(iobus2.ReadPin(11)));
                Console.WriteLine("Pin 12: " + FormatIO(iobus1.ReadPin(12)) + "           Pin 12: " + FormatIO(iobus2.ReadPin(12)));
                Console.WriteLine("Pin 13: " + FormatIO(iobus1.ReadPin(13)) + "           Pin 13: " + FormatIO(iobus2.ReadPin(13)));
                Console.WriteLine("Pin 14: " + FormatIO(iobus1.ReadPin(14)) + "           Pin 14: " + FormatIO(iobus2.ReadPin(14)));
                Console.WriteLine("Pin 15: " + FormatIO(iobus1.ReadPin(15)) + "           Pin 15: " + FormatIO(iobus2.ReadPin(15)));
                Console.WriteLine("Pin 16: " + FormatIO(iobus1.ReadPin(16)) + "           Pin 16: " + FormatIO(iobus2.ReadPin(16)));

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
