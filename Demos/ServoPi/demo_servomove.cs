/*
AB Electronics UK Servo Pi | PWM servo move demo

This demo shows how to set the limits of movement on a servo
and then move between those positions
*/

using System;
using System.Threading;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ServoPi object with an I2C address of 0x40
        ServoPi servo = new ServoPi(0x40);

        // define the servo minimum, centre and maximum limits
        private const short servoMin = 250;  // Minimum pulse length out of 4096
        private const short servoMed = 400;  // Medium pulse length out of 4096
        private const short servoMax = 500;  // Maximum pulse length out of 4096

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
            // Set PWM frequency to 60Hz
            servo.SetPWMFreqency(60);

            // Move the servo to three different positions
            while (true)
            {
                servo.SetPWM(1, 0, servoMin);
                Thread.Sleep(500); // sleep for 500ms
                servo.SetPWM(1, 0, servoMed);
                Thread.Sleep(500); // sleep for 500ms
                servo.SetPWM(1, 0, servoMax);
                Thread.Sleep(500); // sleep for 500ms
            }
        }
    }
}
