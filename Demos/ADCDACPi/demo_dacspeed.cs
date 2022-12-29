/*
AB Electronics UK ADC DAC Pi 2-Channel ADC, 2-Channel DAC | DAC Speed Demo

this demo will output a 3.3V square wave, testing the maximum speed of the DAC
*/


using System;
using ABElectronicsUK;

namespace Tester
{
    class Program
    {
        // Create a new instance of the ADCDACPi object
        ADCDACPi adcdac = new ADCDACPi();

        static void Main(string[] args)
        {
            // Call AdcDac_Connect()
            Program demo = new Program();
            demo.AdcDac_Connect();
        }

        private void AdcDac_Connect()
        {
            // Connect to the ADC DAC Pi and wait for the Connected event
            adcdac.Connected += AdcDac_Connected;
            adcdac.Connect();
        }


        private void AdcDac_Connected(object sender, EventArgs e)
        {
            // Set the DAC gain to 2 giving a voltage range of 0 to 3.3V
            adcdac.DACGain = 2;

            while (true)
            {
                adcdac.SetDACRaw(1, 4095);  // set the voltage on channel 1 to 3.3V
                adcdac.SetDACRaw(1, 0);  // set the voltage on channel 1 to 0V
            }
        }

    }
}
