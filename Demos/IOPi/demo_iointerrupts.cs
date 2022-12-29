/*
AB Electronics UK IO Pi | - IO Interrupts Demo

This example shows how to use the interrupt methods on the IO port.
Both ports will be set as inputs with pull-ups enabled and the 
pins inverted so they will show as on when connected to ground
The interrupts will be enabled and set so that 
ports 0 and 1 will trigger INT A and B respectively.
Using the read_interrupt_capture or read_port methods will
reset the interrupts.
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
            // Set all pins on the IO bus as inputs with internal pull-ups enabled.

            iobus.SetPortPullups(0, 0xFF);
            iobus.SetPortPullups(1, 0xFF);
            iobus.SetPortDirection(0, 0xFF);
            iobus.SetPortDirection(1, 0xFF);

            // Invert both ports so pins will show 1 when grounded
            iobus.InvertPort(0, 0xFF);
            iobus.InvertPort(1, 0xFF);

            // Set the interrupt polarity as active high and mirroring disabled, so
            // pins 1 to 8 trigger INT A and pins 9 to 16 trigger INT B
            iobus.SetInterruptPolarity(1);
            iobus.MirrorInterrupts(0);

            // Set the interrupts default value to 0x00 so the interrupt will trigger when any pin registers as true
            iobus.SetInterruptDefaults(0, 0x00);
            iobus.SetInterruptDefaults(1, 0x00);

            // Set the interrupt type as 1 for ports A and B so an interrupt is
            // fired when the pin matches the default value
            iobus.SetInterruptType(0, 0xFF);
            iobus.SetInterruptType(1, 0xFF);

            // Enable interrupts for all pins
            iobus.SetInterruptOnPort(0, 0xFF);
            iobus.SetInterruptOnPort(1, 0xFF);

            // Reset the interrupt status
            iobus.ResetInterrupts();

            Console.WriteLine("Waiting for interrupt...");

            while (true)
            {

                // read the interrupt status for each port.  
                // If the status is not 0 then an interrupt has occurred on one of the pins 
                // so read the value from the interrupt capture.

                if (iobus.ReadInterruptStatus(0) != 0)
                {
                    Console.WriteLine("Port 0: " + iobus.ReadInterruptCapture(0));
                }
                if (iobus.ReadInterruptStatus(1) != 0)
                {
                    Console.WriteLine("Port 1: " + iobus.ReadInterruptCapture(1));
                }

                Thread.Sleep(2000); // sleep 2 seconds before checking again
            }
        }
    }
}
