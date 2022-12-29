/*
AB Electronics UK RTC Pi | RTC memory demo

This demo shows how to write to and read from the RTC SRAM memory
*/

using System;
using System.Text;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the RTCPi object
        RTCPi rtc = new RTCPi();

        static void Main(string[] args)
        {
            // Call Rtc_Connect()
            Program demo = new Program();
            demo.Rtc_Connect();
        }

        private void Rtc_Connect()
        {
            // Connect to the RTC and wait for the Connected event
            rtc.Connected += Rtc_Connected;
            rtc.Connect();
        }


        private void Rtc_Connected(object sender, EventArgs e)
        {
            // Create an array of bytes to write to the SRAM
            byte[] writebuffer = new byte[] { (byte)'I', (byte)'t', (byte)' ', (byte)'W', (byte)'o', (byte)'r', (byte)'k', (byte)'s', (byte)'!' };

            // Write the array to memory at address 0x08
            rtc.WriteMemory(0x08, writebuffer);

            // Read the same bytes from memory at address 0x08 into a byte array
            byte[] readbuffer = rtc.ReadMemory(0x08, (byte)writebuffer.Length);

            // Output the contents of the read buffer to the console
            Console.WriteLine(Encoding.Default.GetString(readbuffer));
        }

    }
}
