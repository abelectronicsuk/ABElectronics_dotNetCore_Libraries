/*
ABElectronics Servo Pi | Set All Call Address

This demo shows how to set the i2c address for the all call functionality and enable it.
*/

using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ServoPi object with an I2C address of 0x40
        ServoPi servo = new ServoPi(0x40);

        static void Main(string[] args)
        {
            // Call ServoPi_Connect()
            Program demo = new Program();
            demo.ServoPi_Connect();
        }

        private void ServoPi_Connect()
        {
            // Connect to the Servo Pi and wait for the Connected event
            servo.Connected += ServoPi_Connected;
            servo.Connect();
        }


        private void ServoPi_Connected(object sender, EventArgs e)
        {
            // Set the all call address to 0x60
            servo.SetAllCallAddress(0x60);

            // Enable the All Call address
            servo.EnableAllCallAddress();
        }
    }
}
