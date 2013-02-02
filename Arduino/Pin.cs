using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KK.Arduino
{
    /// <summary>
    /// Bunch of static vars usefull for using the Arduino and command class.
    /// </summary>
    public class Pin
    {
        #region statics
        public static readonly String DIGITAL = "d";
        public static readonly String ANALOG = "a";
        public static readonly int PIN0 = 0;
        public static readonly int PIN1 = 1;
        public static readonly int PIN2 = 2;
        public static readonly int PIN3 = 3;
        public static readonly int PIN4 = 4;
        public static readonly int PIN5 = 5;
        public static readonly int PIN6 = 6;
        public static readonly int PIN7 = 7;
        public static readonly int PIN8 = 8;
        public static readonly int PIN9 = 9;
        public static readonly int PIN10 = 10;
        public static readonly int PIN11 = 11;
        public static readonly int PIN12 = 12;
        public static readonly int PIN13 = 13;
        public static readonly bool IN = false;
        public static readonly bool OUT = true;
        public static readonly bool OFF = false;
        public static readonly bool ON = true;
        #endregion
        private int pinNumber;
        private Arduino arduino;
        private Command command;
        private bool typeDefined;
        private String type;
        public readonly String name;
        public Pin(int pinNumber,Arduino arduino, Command command)
        {
            this.arduino = arduino;
            this.pinNumber = pinNumber;
            this.command = command;
            typeDefined = false;
            this.name = "PIN" + pinNumber;
        }
        public Boolean setIOType(String type)
        {
            this.type = type;
            typeDefined = true;
            return typeDefined;
        }
        public Boolean setIO(bool io)
        {
            if (!typeDefined) throw new ArduinoException(ArduinoException.ioTypeException);
            else
            {
                try
                {
                    command.Setup(this.type, pinNumber, io);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public Boolean switchOn()
        {
            if (typeDefined)
            {
                try
                {
                    command.DigitalOut(pinNumber, Pin.ON);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else throw new ArduinoException(ArduinoException.ioTypeException);
        }
        public Boolean switchOff()
        {
            if (typeDefined)
            {
                try
                {
                    command.DigitalOut(pinNumber, Pin.OFF);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else throw new ArduinoException(ArduinoException.ioTypeException);
        }
        public Boolean blink()
        {
            if (!typeDefined) throw new ArduinoException(ArduinoException.ioTypeException);
            else
            {
                try
                {
                    command.DigitalOut(pinNumber, Pin.ON);
                    command.DigitalOut(pinNumber, Pin.OFF);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public int Read()
        {
            if (!typeDefined) throw new ArduinoException(ArduinoException.ioTypeException);
            else
            {
                try
                {
                    return command.DigitalRead(pinNumber);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }
    }
}
