AB Electronics .Net Core Libraries
=====

.Net Core Library to use with Raspberry Pi expansion boards from https://www.abelectronics.co.uk

The AB Electronics .Net Core contains the following classes:

# ADCDACPi
This class contains methods for use with the ADC DAC Pi and ADC DAC Pi Zero from  https://www.abelectronics.co.uk/p/74/adc-dac-pi-zero-raspberry-pi-adc-and-dac-expansion-board

## Methods:

```
ReadADCVoltage(byte channel)
```
Read the voltage from the selected channel on the ADC  
**Parameters:** channel - 1 or 2  
**Returns:** number as double between 0 and 3.3

```
ReadADCRaw(byte channel)
```
Read the raw value from the selected channel on the ADC  
**Parameters:** channel - 1 or 2  
**Returns:** Integer

```
SetDACVoltage(byte channel, double voltage)
```
Set the voltage for the selected channel on the DAC  
**Parameters:** channel - 1 or 2, the voltage can be between 0 and 2.047 volts  
**Returns:** null 

```
SetDACRaw(byte channel, int value)
```
Set the raw value from the selected channel on the DAC  
**Parameters:** channel - 1 or 2, value int between 0 and 4095  
**Returns:** null 

## Properties:

```
double ADCReferenceVoltage {get; set;}
```
Gets or sets the reference voltage for the analogue to digital converter.  
The ADC uses the raspberry pi 3.3V power as a reference so using this property to set the reference to match the exact output voltage from the 3.3V regulator will increase the accuracy of the ADC readings.

```
byte DACGain {get; set;}
```
Gets or Sets the gain for the DAC.  
When gain = 1 the voltage range will be 0V to 2.047V  
When gain = 2 the voltage range will be 0V to VCC which is typically 3.3V on a Raspberry Pi.

## Usage

To use the ADCDACPi library in your code you must first import the library DLL:
```
using ABElectronicsUK;
```
Next, you must initialise the ADCDACPi class:
```
ABElectronicsUK.ADCDACPi adcdac = new ABElectronicsUK.ADCDACPi();
```
Next, we need to connect to the device and wait for the connection
```
adcdac.Connect();

while (!adcdac.IsConnected){}
```
Set the reference voltage.
```
ADCReferenceVoltage = 3.3;
```
Read the voltage from channel 2
```
double value = adcdac.ReadADCVoltage(2);
```

Set the DAC voltage on channel 2 to 1.5 volts
```
adcdac.SetDACVoltage(2, 1.5);
```

# ADCPi 
This class contains methods for use with the  ADC Pi, ADC Pi Plus and ADC Pi Zero from  https://www.abelectronics.co.uk/p/69/adc-pi-raspberry-pi-analogue-to-digital-converter  

## Methods:
```
Connect() 
```
Open a connection with the ADC Pi.  
**Parameters:** none  
**Returns:** null  

```
IsConnected() 
```
Shows if there is a connection with the ADC Pi.  
**Parameters:** none  
**Returns:** boolean  
```
Dispose() 
```
Dispose of the active I2C device  
**Parameters:** none  
**Returns:** null  
```
ReadVoltage(byte channel)
```
Read the voltage from the selected channel  
**Parameters:** channel as a byte - 1 to 8  
**Returns:** number as double between 0 and 5.0  

```
ReadRaw(byte channel)
```
Read the raw int value from the selected channel  
**Parameters:** channel as a byte - 1 to 8  
**Returns:** raw integer value from ADC buffer  

## Parameters:

```
byte PGA { get; set; }
```
Gets or sets the PGA (Programmable Gain Amplifier) gain. Set to 1, 2, 4 or 8  
**Parameters:** gain as byte -  1, 2, 4, 8  
**Returns:** byte - 1, 2, 4 or 8  

```
byte BitRate { get; set; }
```
Gets or sets the sample resolution bit rate.  
12 = 12 bit (240SPS max)  
14 = 14 bit (60SPS max)  
16 = 16 bit (15SPS max)  
18 = 18 bit (3.75SPS max)  
**Returns:** 12, 14, 16, 18   


```
byte ConversionMode { get; set; }
```
Gets or sets the conversion mode for the ADC.  
0 = One shot conversion mode, 1 = Continuous conversion mode    
**Returns:** 0 or 1

## Usage

To use the ADC Pi library in your code you must first import the library DLL:
```
using ABElectronicsUK;
```
Next, you must initialise the adc class:
```
ABElectronicsUK.ADCPi adc = new ADCPi(0x68, 0x69);
```
The arguments are the two I2C addresses of the ADC chips. The values shown are the default addresses of the ADC board.

Next, we need to connect to the device and wait for the connection before setting the bit rate and gain. The sample rate can be 12, 14, 16 or 18
```
adc.Connect();

while (!adc.IsConnected){}
adc.BitRate = 18;
adc.PGA = 1;
```

You can now read the voltage from channel 1 with:
```
double readvalue = 0;

readvalue = adc.ReadVoltage(1);
```


# ADCDifferentialPi
This class contains methods for use with the ADC Differential Pi from https://www.abelectronics.co.uk/p/65/adc-differential-pi-raspberry-pi-analogue-to-digital-converter

## Methods:
```
Connect() 
```
Open a connection with the ADC Differential Pi.  
**Parameters:** none  
**Returns:** null  

```
IsConnected() 
```
Shows if there is a connection with the ADC Differential Pi.  
**Parameters:** none  
**Returns:** boolean  

```
Dispose() 
```
Dispose of the active I2C device  
**Parameters:** none  
**Returns:** null  

```
ReadVoltage(byte channel)
```
Read the voltage from the selected channel  
**Parameters:** channel as a byte - 1 to 8  
**Returns:** number as double between -2.048 and 2.048  

```
ReadRaw(byte channel)
```
Read the raw int value from the selected channel  
**Parameters:** channel as a byte - 1 to 8  
**Returns:** raw integer value from ADC buffer  

## Parameters:

```
byte PGA { get; set; }
```
Gets or sets the PGA (Programmable Gain Amplifier) gain. Set to 1, 2, 4 or 8  
**Parameters:** gain as byte -  1, 2, 4, 8  
**Returns:** byte - 1, 2, 4 or 8  

```
byte BitRate { get; set; }
```
Gets or sets the sample resolution bit rate.  
12 = 12 bit (240SPS max)  
14 = 14 bit (60SPS max)  
16 = 16 bit (15SPS max)  
18 = 18 bit (3.75SPS max)  
**Returns:** 12, 14, 16, 18   


```
byte ConversionMode { get; set; }
```
Gets or sets the conversion mode for the ADC.  
0 = One shot conversion mode, 1 = Continuous conversion mode    
**Returns:** 0 or 1

### Usage

To use the ADC Differential Pi library in your code you must first import the library DLL:
```
using ABElectronicsUK;
```
Next, you must initialise the adc class:
```
ABElectronicsUK.ADCDifferentialPi adc = new ADCDifferentialPi(0x68, 0x69);
```
The arguments are the two I2C addresses of the ADC chips. The values shown are the default addresses of the ADC Differential Pi board.

Next, we need to connect to the device and wait for the connection before setting the bit rate and gain. The sample rate can be 12, 14, 16 or 18
```
adc.Connect();

while (!adc.IsConnected){}
adc.BitRate = 18;
adc.PGA = 1;
```

You can now read the voltage from channel 1 with:
```
double readvalue = 0;

readvalue = adc.ReadVoltage(1);
```

# ExpanderPi
This class contains methods to use with the Expander Pi from
https://www.abelectronics.co.uk/p/50/expander-pi

## Methods
```
Connect() 
```
Connect to the Expander Pi  
**Parameters:** none  
**Returns:** null  

```
IsConnected() 
```
Check if the device is connected  
**Parameters:** none  
**Returns:** boolean  

```
Dispose() 
```
Dispose of the active device  
**Parameters:** none  
**Returns:** null  

## ADC Methods

The ADC supports single-ended or differential modes.

In single-ended mode, each channel acts as an individual single-ended input.
  
In differential mode, the inputs are split into four differential pairs.  
Setting channel to 1 will make IN1 = IN+ and IN2 = IN-  
Setting channel to 2 will make IN1 = IN- and IN2 = IN+  
Setting channel to 3 will make IN3 = IN+ and IN4 = IN-  
Setting channel to 4 will make IN3 = IN- and IN4 = IN+  
Setting channel to 5 will make IN5 = IN+ and IN6 = IN-  
Setting channel to 6 will make IN5 = IN- and IN6 = IN+  
Setting channel to 7 will make IN7 = IN+ and IN8 = IN-  
Setting channel to 8 will make IN7 = IN- and IN8 = IN+  

```
ADCReadVoltage(byte channel, byte mode) 
```
Read the voltage from the selected channel on the ADC.  
**Parameters:** channel (1 to 8)  
**Parameters:** mode (1 = Single-Ended Input, 2 = Differential Input)  
**Returns:** (double) voltage  

```
ADCReadRaw(byte channel, byte mode)
```
Read the raw value from the selected channel on the ADC.  
**Parameters:** channel (1 to 8)  
**Parameters:** mode (1 = Single-Ended Input, 2 = Differential Input)  
**Returns:** voltage  

## ADC Parameters

```
double ADCReferenceVoltage { get; set; }
```
Gets or sets the reference voltage for the analogue to digital converter.  
The Expander Pi contains an onboard 4.096V voltage reference.  
If you want to use an external reference between 0V and 5V, disconnect the jumper J1 and connect your reference voltage to the Vref pin.  


## DAC Methods

```
DACSetVoltage(byte channel, double voltage, byte gain)
```
Set the voltage for the selected channel on the DAC.  
**Parameters:** channel (1 or 2)  
**Parameters:** voltage (Voltage will be between 0 and 2.047V when the gain is 1, 0 and 4.096V when the gain is 2)  
**Parameters:** gain (1 or 2)  
**Returns:** null  

```
DACSetRaw(byte channel, short value, byte gain)
```
Set the voltage for the selected channel on the DAC.  
The voltage will be between 0 and 2.047V when the gain is 1, 0 and 4.096V when the gain is 2  
**Parameters:** channel (1 or 2)  
**Parameters:** value (0 to 4095)  
**Parameters:** gain (1 or 2)  
**Returns:** null

## IO Methods

**Note:** Microchip recommends that pin 8 (GPA7) and pin 16 (GPB7) are used as outputs only.  This change was made for revision D MCP23017 chips manufactured after June 2020. See the [MCP23017 datasheet](https://www.abelectronics.co.uk/docs/pdf/microchip-mcp23017.pdf) for more information.

```
IOSetPinDirection(byte pin, bool direction)  
```
Sets the IO direction for an individual pin  
**Parameters:** pin - 1 to 16, direction - true = input, false = output  
**Returns:** null

```
IOSetPortDirection(byte port, byte direction)
```
Sets the IO direction for the specified IO port  
**Parameters:** port - 0 = pins 1 to 8, port 1 = pins 9 to 16, direction - true = input, false = output  
**Returns:** null

```
IOSetPinPullup(byte pin, bool value)
```
Set the internal 100K pull-up resistors for the selected IO pin  
**Parameters:** pin - 1 to 16, value: true = Enabled, false = Disabled  
**Returns:** null
```
IOSetPortPullups(byte port, byte value)
```
Set the internal 100K pull-up resistors for the selected IO port  
**Parameters:** 0 = pins 1 to 8, 1 = pins 9 to 16, value: true = Enabled, false = Disabled   
**Returns:** null

```
IOWritePin(byte pin, bool value)
```
Write to an individual pin 1 - 16  
**Parameters:** pin - 1 to 16, value - true = Enabled, false = Disabled  
**Returns:** null
```
IOWritePort(byte port, byte value)
```
Write to all pins on the selected port  
**Parameters:** port - 0 = pins 1 to 8, port 1 = pins 9 to 16, value - number between 0 and 255 or 0x00 and 0xFF  
**Returns:** null
```
IOReadPin(byte pin)
```
Read the value of an individual pin 1 - 16   
**Parameters:** pin: 1 to 16  
**Returns:** false = logic level low, true = logic level high
```
IOReadPort(byte port)
```
Read all pins on the selected port  
**Parameters:** port - 0 = pins 1 to 8, port 1 = pins 9 to 16  
**Returns:** number between 0 and 255 or 0x00 and 0xFF
```
IOInvertPort(byte port, byte polarity)
```
Invert the polarity of the pins on a selected port  
**Parameters:** port - 0 = pins 1 to 8, port 1 = pins 9 to 16, polarity - 0 = same logic state of the input pin, 1 = inverted logic state of the input pin  
**Returns:** null

```
IOInvertPin(byte pin, bool polarity)
```
Invert the polarity of the selected pin  
**Parameters:** pin - 1 to 16, polarity - false = same logic state of the input pin, true = inverted logic state of the input pin
**Returns:** null
```
IOMirrorInterrupts(byte value)
```
Mirror Interrupts  
**Parameters:** value - 1 = The INT pins are internally connected, 0 = The INT pins are not connected. INTA is associated with PortA and INTB is associated with PortB  
**Returns:** null

```
IOSetInterruptPolarity(byte value)
```
This sets the polarity of the INT output pins  
**Parameters:** 1 = Active - high. 0 = Active - low.  
**Returns:** null  

```
IOSetInterruptType(byte port, byte value)
```
Sets the type of interrupt for each pin on the selected port  
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16, value: number between 0 and 255 or 0x00 and 0xFF.  1 = interrupt is fired when the pin matches the default value, 0 = the interrupt is fired on state change  
**Returns:** null
```
IOSetInterruptDefaults(byte port, byte value)
```
These bits set the compare value for pins configured for interrupt-on-change on the selected port.  
If the associated pin level is the opposite of the register bit, an interrupt occurs.    
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16, value: compare value  
**Returns:** null
```
IOSetInterruptOnPort(byte port, byte value)
```
Enable interrupts for the pins on the selected port  
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16, value: number between 0 and 255 or 0x00 and 0xFF  
**Returns:** null

```
IOSetInterruptOnPin(byte pin, bool value)
```
Enable interrupts for the selected pin  
**Parameters:** pin - 1 to 16, value - true = interrupt enabled, false = interrupt disabled  
**Returns:** null

```
IOReadInterruptStatus(byte port)
```
Enable interrupts for the selected pin  
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16  
**Returns:** status

```
IOReadInterruptCapture(byte port)
```
Read the value from the selected port at the time of the last interrupt trigger  
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16  
**Returns:** status
```
IOResetInterrupts()
```
Set the interrupts A and B to 0  
**Parameters:** null  
**Returns:** null

## RTC Methods

```
RTCReadMemory(byte address, byte length)
```
Write to the SRAM memory on the DS1307.  Memory range is 0x08 to 0x3F.  
**Parameters:** address - First memory address where the values with be written.  
length - Number of bytes to read from memory.  
**Returns:** byte[] - Byte array containing read memory values.  

```
RTCWriteMemory(byte address, byte[] values)
```
Write to the SRAM memory on the DS1307.  Memory range is 0x08 to 0x3F.  
**Parameters:** address - First memory address where the values with be written.
values - Byte array containing values to be written to memory.  Array length can not exceed the memory space available.
**Returns:** null


## RTC Parameters

```
DateTime RTCDate { get; set; }
```
Set or get the date and time from the RTC.   
Takes a DateTime variable.  
**Returns:** DateTime

```
byte RTCFrequency { get; set; }
```
Get or set the frequency of the output pin square wave.  
Options are: 1 = 1Hz, 2 = 4.096KHz, 3 = 8.192KHz, 4 = 32.768KHz  

```
bool RTCOutput { get; set; }
```
Enable or disable the square wave output on the SQW pin.  
Set output as true or false.  Gets output state as true or false.


## Usage:

To use the Expander Pi library in your code you must first import the library DLL:
```
using ABElectronicsUK;
```
Next, you must initialise the ExpanderPi class:
```
ABElectronicsUK.ExpanderPi expi = new ABElectronicsUK.ExpanderPi();
```

Next, we need to connect to the device and wait for the connection before setting the digital IO ports to be inputs
```
expi.Connect();

while (!expi.IsConnected){}
expi.IOSetPortDirection(0, 0xFF);
expi.IOSetPortDirection(1, 0xFF);
```

You can now read the input status from pin 1 on the digital IO bus with:
```
bool value = expi.IOReadPin(1);
```

# I2CSwitch
This class contains methods for use with the I2C Switch from  
https://www.abelectronics.co.uk/p/84/i2c-switch  

## Methods:
```
Connect() 
```
Connect to the I2C device  
**Parameters:** none  
**Returns:** null  

```
IsConnected() 
```
Check if the device is connected  
**Parameters:** none  
**Returns:** boolean  

```
Dispose() 
```
Dispose of the active I2C device  
**Parameters:** none  
**Returns:** null  

```
SwitchChannel(byte channel)
```
Enable the specified I2C channel and disable other channels.  
**Parameters:** channel - 1 to 4  
**Returns:** null  

```
SetChannelState(byte channel, bool state)
```
Set the state of an individual I2C channel.  
**Parameters:** channel - 1 to 4, state - true or false  
**Returns:** null  

```
GetChannelState(byte channel)
```
Get the state of an individual I2C channel.  
**Parameters:** channel - 1 to 4  
**Returns:** state - true or false  

```
Reset()
```
Reset the I2C switch.  
**Returns:** null  

## Parameters

``` 
byte Address { get; set;}
```
Gets or sets the I2C address for the I2C Switch bus

```
byte ResetPin { get; set; }
```
GPIO pin for resetting the I2C Switch  


# IOPi
This class contains methods for use with the IO Pi, IO Pi Plus and IO Pi Zero from  
https://www.abelectronics.co.uk/p/54/io-pi-plus   
https://www.abelectronics.co.uk/p/71/io-pi-zero  

**Note:** Microchip recommends that pin 8 (GPA7) and pin 16 (GPB7) are used as outputs only.  This change was made for revision D MCP23017 chips manufactured after June 2020. See the [MCP23017 datasheet](https://www.abelectronics.co.uk/docs/pdf/microchip-mcp23017.pdf) for more information.

## Methods:
```
Connect() 
```
Connect to the I2C device  
**Parameters:** none  
**Returns:** null  

```
IsConnected() 
```
Check if the device is connected  
**Parameters:** none  
**Returns:** boolean  

```
Dispose() 
```
Dispose of the active I2C device  
**Parameters:** none  
**Returns:** null  

```
SetPinDirection(byte pin, bool direction)
```
Sets the IO direction for an individual pin  
**Parameters:** pin - 1 to 16, direction - true = input, false = output  
**Returns:** null  

```
SetPortDirection(byte port, byte direction)
```
Sets the IO direction for the specified IO port  
**Parameters:** port - 0 = pins 1 to 8, port 1 = pins 9 to 16, direction - true = input, false = output  
**Returns:** null  

```
SetPinPullup(byte pin, bool value)
```
Set the internal 100K pull-up resistors for the selected IO pin  
**Parameters:** pin - 1 to 16, value: true = Enabled, false = Disabled  
**Returns:** null  
```
SetPortPullups(byte port, byte value)
```
Set the internal 100K pull-up resistors for the selected IO port  
**Parameters:** 0 = pins 1 to 8, 1 = pins 9 to 16, value: true = Enabled, false = Disabled  
**Returns:** null  

```
WritePin(byte pin, bool value)
```
Write to an individual pin 1 - 16  
**Parameters:** pin - 1 to 16, value - true = Enabled, false = Disabled  
**Returns:** null  
```
WritePort(byte port, byte value)
```
Write to all pins on the selected port  
**Parameters:** port - 0 = pins 1 to 8, port 1 = pins 9 to 16, value -  number between 0 and 255 or 0x00 and 0xFF  
**Returns:** null
```
ReadPin(byte pin)
```
Read the value of an individual pin 1 - 16   
**Parameters:** pin: 1 to 16  
**Returns:** false = logic level low, true = logic level high
```
ReadPort(byte port)
```
Read all pins on the selected port  
**Parameters:** port - 0 = pins 1 to 8, port 1 = pins 9 to 16  
**Returns:** number between 0 and 255 or 0x00 and 0xFF
```
InvertPort(byte port, byte polarity)
```
Invert the polarity of the pins on a selected port  
**Parameters:** port - 0 = pins 1 to 8, port 1 = pins 9 to 16, polarity - 0 = same logic state of the input pin, 1 = inverted logic state of the input pin  
**Returns:** null

```
InvertPin(byte pin, bool polarity)
```
Invert the polarity of the selected pin  
**Parameters:** pin - 1 to 16, polarity - false = same logic state of the input pin, true = inverted logic state of the input pin
**Returns:** null
```
MirrorInterrupts(byte value)
```
Mirror Interrupts  
**Parameters:** value - 1 = The INT pins are internally connected, 0 = The INT pins are not connected. INTA is associated with PortA and INTB is associated with PortB  
**Returns:** null

```
SetInterruptPolarity(byte value)
```
This sets the polarity of the INT output pins  
**Parameters:** 1 = Active - high. 0 = Active - low.  
**Returns:** null  

```
SetInterruptType(byte port, byte value)
```
Sets the type of interrupt for each pin on the selected port  
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16, value: number between 0 and 255 or 0x00 and 0xFF. 1 = interrupt is fired when the pin matches the default value, 0 = the interrupt is fired on state change  
**Returns:** null
```
SetInterruptDefaults(byte port, byte value)
```
These bits set the compare value for pins configured for interrupt-on-change on the selected port.  
If the associated pin level is the opposite of the register bit, an interrupt occurs.    
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16, value: compare value  
**Returns:** null
```
SetInterruptOnPort(byte port, byte value)
```
Enable interrupts for the pins on the selected port  
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16, value: number between 0 and 255 or 0x00 and 0xFF  
**Returns:** null

```
SetInterruptOnPin(byte pin, bool value)
```
Enable interrupts for the selected pin  
**Parameters:** pin - 1 to 16, value - true = interrupt enabled, false = interrupt disabled  
**Returns:** null

```
ReadInterruptStatus(byte port)
```
Enable interrupts for the selected pin  
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16  
**Returns:** status

```
ReadInterruptCapture(byte port)
```
Read the value from the selected port at the time of the last interrupt trigger  
**Parameters:** port 0 = pins 1 to 8, port 1 = pins 9 to 16  
**Returns:** status
```
ResetInterrupts()
```
Set the interrupts A and B to 0  
**Parameters:** null  
**Returns:** null
## Usage

To use the IO Pi library in your code you must first import the library DLL:
```
using ABElectronicsUK;
```
Next, you must initialise the io class:
```
ABElectronicsUK.IOPi bus1 = new ABElectronicsUK.IOPi(0x20);
```
The argument is the I2C addresses of the IO chip. The value shown is the default addresses of the IO board which are 0x20 and 0x21.

Next, we need to connect to the device and wait for the connection before setting ports to be inputs
```
bus1.Connect();

while (!bus1.IsConnected){}
bus1.SetPortDirection(0, 0xFF);
bus1.SetPortDirection(1, 0xFF);
```

You can now read the input status from channel 1 with:
```
bool value = bus1.ReadPin(1);
```

# RTCPi
This class contains methods for use with the RTC Pi, RTC Pi Plus and RTC Pi Zero from https://www.abelectronics.co.uk/p/70/rtc-pi 

## Methods:
```
Connect() 
```
Connect to the I2C device  
**Parameters:** none  
**Returns:** null  

```
IsConnected() 
```
Check if the device is connected  
**Parameters:** none  
**Returns:** boolean  

```
Dispose() 
```
Dispose of the active I2C device  
**Parameters:** none  
**Returns:** null  

```
ReadMemory(byte address, byte length)
```
Read from the memory on the DS1307.  Memory range is 0x08 to 0x3F.  
**Parameters:** address - First memory address where the values with be read.  
length - Number of bytes to read from memory.  
**Returns:** byte[] - Byte array containing read memory values.  

```
WriteMemory(byte address, byte[] values)
```
Write to the SRAM memory on the DS1307.  Memory range is 0x08 to 0x3F.  
**Parameters:** address - First memory address where the values with be written.
values - Byte array containing values to be written to memory.  Array length can not exceed the memory space available.
**Returns:** null


## Parameters

```
DateTime Date { get; set; }
```
Set or get the date and time from the RTC.   
Takes a DateTime variable.  
**Returns:** DateTime

```
byte Frequency { get; set; }
```
Get or set the frequency of the output pin square wave.  
Options are: 1 = 1Hz, 2 = 4.096KHz, 3 = 8.192KHz, 4 = 32.768KHz  

```
bool Output { get; set; }
```
Enable or disable the square wave output on the SQW pin.  
Set output as true or false.  Gets output state as true or false.  


## Usage

To use the RTC Pi library in your code you must first import the library DLL:
```
using ABElectronicsUK;
```
Next, you must initialise the rtc class:
```
ABElectronicsUK.RTCPi rtc = new ABElectronicsUK.RTCPi();
```
Next, we need to connect to the device and wait for the connection
```
rtc.Connect();

while (!rtc.IsConnected)
{
}
```
You can set the date and time from the RTC chip to be the 25th of December 2015 at 6 AM with:
```
DateTime newdate = new DateTime(2015, 12, 25, 06, 00, 00);
rtc.Date = newdate;
```

You can read the date and time from the RTC chip with:
```
DateTime value = rtc.Date;
```


# ServoPi
This class contains methods for use with the Servo PWM Pi from  
https://www.abelectronics.co.uk/p/72/servo-pwm-pi

## Methods:
```
Connect() 
```
Connect to the I2C device  
**Parameters:** none  
**Returns:** null  

```
IsConnected() 
```
Check if the device is connected  
**Parameters:** none  
**Returns:** boolean  

```
Dispose() 
```
Dispose of the active I2C device  
**Parameters:** none  
**Returns:** null  

```
SetPWMFreqency(freq) 
```
Set the output frequency of all PWM channels.  
The output frequency is programmable from a typical 40Hz to 1000Hz.  
**Parameters:** freq - Integer frequency value  
**Returns:** null  

```
SetPWM(byte channel, short on, short off) 
```
Set the PWM output on a single channel  
**Parameters:** channel - 1 to 16, on - period 0 to 4095, off - period 0 to 4095   
**Returns:** null  

```
SetPWMOnTime(byte channel, short on)
```
Set the output on time on a single channel  
**Parameters:** channel - 1 to 16, on - period 0 to 4095  
**Returns:** null  

```
SetPWMOnTime(byte channel, short off)
```
Set the output off time on a single channel  
**Parameters:** channel - 1 to 16, off - period 0 to 4095  
**Returns:** null  

```
SetAllPwm( on, off) 
```
Set the output on all channels  
**Parameters:** on - period 0 to 4095, off - period 0 to 4095  
**Returns:** null  

```
OutputEnable()
```
Enable the output via the OE pin. Only used when the OE jumper is joined.  
**Parameters:** null  
**Returns:** null  

```
OutputDisable()
```
Disable the output via the OE pin. Only used when the OE jumper is joined.  
**Parameters:** null  
**Returns:** null  

```
SetAllCallAddress(byte address)
```
The All Call Address allows all Servo Pi devices on the I2C bus to be controlled at the same time.  
**Parameters:** address - 0x03 to 0x77  
**Returns:** null  

```
EnableAllCallAddress()
```
Enable the I2C address for the All Call function.  
**Returns:** null  

```
DisableAllCallAddress()
```
Disable the I2C address for the All Call function.  
**Returns:** null  

## Parameters

```
byte Address { get; set; }
```
I2C address for the Servo Pi bus.  

```
bool InvertOutput { get; set; }
```
Invert the PWM output on all channels.  
true = invert, false = normal.  

```
byte OutputEnablePin { get; set; }
```
Set the GPIO pin for the output-enable function.  
The default GPIO pin 4 is not supported in .net Core so the OE pad will need to be connected to a different GPIO pin.  

```
bool Sleep { get; set; }
```
Sets or gets the sleep status for the Servo Pi.   
Set true to put the device into a sleep state or false to wake the device.  
Get true = sleeping, false = awake.  


## Usage

To use the ServoPi Pi library in your code you must first import the library DLL:
```
using ABElectronicsUK;
```
Next, you must initialise the ServoPi class:
```
ABElectronicsUK.ServoPi servo = new ABElectronicsUK.ServoPi(0x40);
```
The argument is the I2C addresses of the Servo Pi chip.

Next, we need to connect to the device and wait for the connection
```
servo.Connect();

while (!servo.IsConnected){}
```
Set the PWM frequency to 60 Hz and enable the output
```
servo.SetPWMFreqency(60);                       
```
**Optional**  
You can set the enable pin to use the output enable functions and enable or disable the output. 
The default GPIO pin 4 is not supported in Windows 10 IOT and so the OE pad will need to be connected to a different GPIO pin to use this functionality.

```
servo.OutputEnablePin = 17; // set to GPIO pin 17
servo.OutputEnable();
```
Move the servo to a position and exit the application.
```
servo.SetPWM(1, 0, 300);
``` 
