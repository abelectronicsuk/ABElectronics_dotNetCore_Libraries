/*
AB Electronics UK Expander Pi | IO Interrupts Demo

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
            // Set all pins on the IO bus as inputs with internal pull-ups enabled.

            expander.IOSetPortPullups(0, 0xFF);
            expander.IOSetPortPullups(1, 0xFF);
            expander.IOSetPortDirection(0, 0xFF);
            expander.IOSetPortDirection(1, 0xFF);

            // Invert both ports so pins will show 1 when grounded
            expander.IOInvertPort(0, 0xFF);
            expander.IOInvertPort(1, 0xFF);

            // Set the interrupt polarity as active high and mirroring disabled, so
            // pins 1 to 8 trigger INT A and pins 9 to 16 trigger INT B
            expander.IOSetInterruptPolarity(1);
            expander.IOMirrorInterrupts(0);

            // Set the interrupts default value to 0x00 so the interrupt will trigger when any pin registers as true
            expander.IOSetInterruptDefaults(0, 0x00);
            expander.IOSetInterruptDefaults(1, 0x00);

            // Set the interrupt type as 1 for ports A and B so an interrupt is
            // fired when the pin matches the default value
            expander.IOSetInterruptType(0, 0xFF);
            expander.IOSetInterruptType(1, 0xFF);

            // Enable interrupts for all pins
            expander.IOSetInterruptOnPort(0, 0xFF);
            expander.IOSetInterruptOnPort(1, 0xFF);

            // Reset the interrupt status
            expander.IOResetInterrupts();

            Console.WriteLine("Waiting for interrupt...");

            while (true)
            {

                // read the interrupt status for each port.  
                // If the status is not 0 an interrupt has occurred on one of the pins 
                // so read the value from the interrupt capture.

                if (expander.IOReadInterruptStatus(0) != 0)
                {
                    Console.WriteLine("Port 0: " + expander.IOReadInterruptCapture(0));
                }
                if (expander.IOReadInterruptStatus(1) != 0)
                {
                    Console.WriteLine("Port 1: " + expander.IOReadInterruptCapture(1));
                }

                Thread.Sleep(2000); // sleep 2 seconds before checking again
            }
        }

    }
}
