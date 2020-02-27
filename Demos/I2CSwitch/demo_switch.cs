/*
ABElectronics I2CSwitch | Channel select demo

This demo shows how to set I2C output channel on the I2C switch.
*/

using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ADCPi object with I2C addresses 0x68 and 0x69
        I2CSwitch i2cswitch = new I2CSwitch();

        static void Main(string[] args)
        {
            // Call Adc_Connect()
            Program demo = new Program();
            demo.I2CSwitch_Connect();
        }

        private void I2CSwitch_Connect()
        {
            // Connect to the I2C Switch and wait for the Connected event
            i2cswitch.Connected += I2CSwitch_Connected;
            i2cswitch.Connect();
        }


        private void I2CSwitch_Connected(object sender, EventArgs e)
        {
            // Reset the I2C Switch
            i2cswitch.Reset();

            // Switch to channel 2
            i2cswitch.SwitchChannel(2);

            Console.WriteLine("I2C Switch changed to channel 2");
        }


    }
}
