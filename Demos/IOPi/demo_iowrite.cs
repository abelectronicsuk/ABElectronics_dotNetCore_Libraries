/*
ABElectronics IO Pi | Digital I/O Write Demo

This example uses the WritePin and WritePort methods to switch the pins
on and off on the I/O bus.
*/

using System;
using System.Threading;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create an instance of the IOPi object with an I2C address of 0x20
        IOPi iobus = new IOPi(0x20);

        static void Main(string[] args)
        {
            // Call IO_Connect()
            Program demo = new Program();
            demo.IO_Connect();
        }

        private void IO_Connect()
        {
            // Connect to the IO Pi bus 1 and wait for the Connected event
            iobus.Connected += IO_Connected;
            iobus.Connect();
        }

        private void IO_Connected(object sender, EventArgs e)
        {
            // We will write to the pins 9 to 16 so set port 1 to be outputs
            iobus.SetPortDirection(1, 0x00);

            // turn off the pins
            iobus.WritePort(1, 0x00);

            // and disable the internal pull-up resistors
            iobus.SetPortPullups(1, 0x00);

            while (true)
            {
                // count to 255 and display the value on pins 9 to 16 in binary format
                for (int i = 0; i < 256; i++)
                {
                    Thread.Sleep(50); // wait 50ms

                    iobus.WritePort(1, (byte)i);
                }

                // turn off all of the pins on bank 1
                iobus.WritePort(1, 0x00);

                // now turn on all of the leds in turn by writing to one pin at a time
                iobus.WritePin(9, true);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(10, true);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(11, true);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(12, true);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(13, true);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(14, true);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(15, true);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(16, true);

                // and turn off all of the leds in turn by writing to one pin at a time
                iobus.WritePin(9, false);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(10, false);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(11, false);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(12, false);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(13, false);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(14, false);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(15, false);

                Thread.Sleep(100); // wait 100ms

                iobus.WritePin(16, false);

            }
        }
    }
}
