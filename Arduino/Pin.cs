using System;
using System.Collections.Generic;
using System.Text;

namespace KK.Arduino
{
    /// <summary>
    /// Constains the Pin class and a bunch of helping Statics
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
        public static readonly int PINA0 = 0;
        public static readonly int PINA1 = 1;
        public static readonly int PINA2 = 2;
        public static readonly int PINA3 = 3;
        public static readonly int PINA4 = 4;
        public static readonly int PINA5 = 5;
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
        /// <summary>
        /// Read-only. You'll break the program changin this one.
        /// </summary>
        public readonly String name;
        /// <summary>
        /// Constructor. Held internal.
        /// Use Arduino().getPin(pinnumber)
        /// </summary>
        /// <param name="pinNumber"></param>
        /// <param name="arduino"></param>
        /// <param name="command"></param>
        internal Pin(int pinNumber,Arduino arduino, Command command)
        {
            this.arduino = arduino;
            this.pinNumber = pinNumber;
            this.command = command;
            typeDefined = false;
            this.name = "PIN" + pinNumber;
        }
        /// <summary>
        /// Sets weather it is a digital or analog pin.
        /// You won't be able to use any output methods on analog.
        /// (See Pin.Digital / Pin.Analog statics
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Boolean setIOType(String type)
        {
            if (type != Pin.DIGITAL && type != Pin.ANALOG) throw new ArduinoException(ArduinoException.ioTypeException);
            this.type = type;
            typeDefined = true;
            return typeDefined;
        }
        /// <summary>
        /// Set weather pin should be output or input.
        /// Always input if analog
        /// (See Pin.OUT / Pin.IN
        /// *TODO, set handle for analog - force - input
        /// </summary>
        /// <param name="io"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Assumes digital Pin. Sets pin to send signal.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Assumes digital pin. Turns off signal if any.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Just a helper method for quick testing. "Blinks" a Pin.
        /// I.E. Turns it on and off as fast as possible.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Returns pinvalue
        /// *TODO analog return
        /// </summary>
        /// <returns></returns>
        public int Read()
        {
            if (!typeDefined) throw new ArduinoException(ArduinoException.ioTypeException);
            else
            {
                try
                {
                    if (type == Pin.DIGITAL)
                        return command.DigitalRead(pinNumber);
                    else return command.AnalogRead(pinNumber);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }
    }
}
