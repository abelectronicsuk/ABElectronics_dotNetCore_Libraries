/*
AB Electronics UK Expander Pi | RTC memory demo

This demo shows how write to and read from the RTC SRAM memory
*/

using System;
using System.Text;
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
            // Create an array of bytes to write to the SRAM
            byte[] writebuffer = new byte[] { (byte)'I', (byte)'t', (byte)' ', (byte)'W', (byte)'o', (byte)'r', (byte)'k', (byte)'s', (byte)'!' };

            // Write the array to memory at address 0x08
            expander.RTCWriteMemory(0x08, writebuffer);

            // Read the same bytes from memory at address 0x08 into a byte array
            byte[] readbuffer = expander.RTCReadMemory(0x08, (byte)writebuffer.Length);

            // Output the contents of the read buffer to the console
            Console.WriteLine(Encoding.Default.GetString(readbuffer));
        }
    }
}
