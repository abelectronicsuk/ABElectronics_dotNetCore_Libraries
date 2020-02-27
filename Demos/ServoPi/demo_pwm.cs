/*
ABElectronics Servo Pi | PWM output demo

This demo shows how to set a 1KHz output frequency and change the pulse width between the minimum and maximum values
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
            // Set PWM frequency to 1 Khz
            servo.SetPWMFreqency(1000);

            // Increase and decrease the PWM duty cycle in increments of 5
            while (true)
            {
                for (short i = 0; i <= 4095; i += 5)
                {
                    servo.SetPWM(1, 0, i);
                }
                for (short i = 4095; i >= 0; i -= 5)
                {
                    servo.SetPWM(1, 0, i);
                }
            }
        }
    }
}
