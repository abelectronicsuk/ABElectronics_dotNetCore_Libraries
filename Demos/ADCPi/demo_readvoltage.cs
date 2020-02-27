/*
ABElectronics ADC Pi 8-Channel ADC demo

Initialise the ADC device using the default addresses and sample rate,
change this value if you have changed the address selection jumpers

Sample rate can be 12,14, 16 or 18
*/


using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ADCPi object with I2C addresses 0x68 and 0x69
        ADCPi adc = new ADCPi(0x68, 0x69);

        static void Main(string[] args)
        {
            // Call Adc_Connect()
            Program demo = new Program();
            demo.Adc_Connect();
        }

        private void Adc_Connect()
        {
            // Connect to the ADC Pi and wait for the Connected event
            adc.Connected += Adc_Connected;
            adc.Connect();
        }


        private void Adc_Connected(object sender, EventArgs e)
        {
            // Set the ADC bit rate to 16 bit
            adc.BitRate = 16;

            // Clear the console window

            Console.Clear();

            while (true)
            {
                // read from adc channels and print to screen
                Console.WriteLine("Channel 1: " + adc.ReadVoltage(1).ToString("N3"));
                Console.WriteLine("Channel 2: " + adc.ReadVoltage(2).ToString("N3"));
                Console.WriteLine("Channel 3: " + adc.ReadVoltage(3).ToString("N3"));
                Console.WriteLine("Channel 4: " + adc.ReadVoltage(4).ToString("N3"));
                Console.WriteLine("Channel 5: " + adc.ReadVoltage(5).ToString("N3"));
                Console.WriteLine("Channel 6: " + adc.ReadVoltage(6).ToString("N3"));
                Console.WriteLine("Channel 7: " + adc.ReadVoltage(7).ToString("N3"));
                Console.WriteLine("Channel 8: " + adc.ReadVoltage(8).ToString("N3"));
                Console.SetCursorPosition(0, 0);
            }

        }


    }
}
