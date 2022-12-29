/*
AB Electronics UK Expander Pi | Digital I/O Write Demo

This example uses the IOWritePin and IOWritePort methods to switch the pins
on and off on the I/O bus.
*/

using System;
using System.Threading;
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
            // We will write to the pins 9 to 16 so set port 1 to be outputs
            expander.IOSetPortDirection(1, 0x00);

            // turn off the pins
            expander.IOWritePort(1, 0x00);

            // and disable the internal pull-up resistors
            expander.IOSetPortPullups(1, 0x00);

            while (true)
            {
                // count to 255 and display the value on pins 9 to 16 in binary format
                for (int i = 0; i < 256; i++)
                {
                    Thread.Sleep(50); // wait 50ms

                    expander.IOWritePort(1, (byte)i);
                }

                // turn off all of the pins on bank 1
                expander.IOWritePort(1, 0x00);

                // now turn on all of the LEDs in turn by writing to one pin at a time
                expander.IOWritePin(9, true);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(10, true);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(11, true);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(12, true);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(13, true);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(14, true);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(15, true);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(16, true);

                // and turn off all of the LEDs in turn by writing to one pin at a time
                expander.IOWritePin(9, false);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(10, false);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(11, false);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(12, false);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(13, false);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(14, false);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(15, false);

                Thread.Sleep(100); // wait 100ms

                expander.IOWritePin(16, false);

            }
        }
    }
}
