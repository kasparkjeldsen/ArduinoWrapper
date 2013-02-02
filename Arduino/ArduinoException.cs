using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KK.Arduino
{
    [Serializable]
    class ArduinoException : System.Exception
    {
        public static String ioTypeException = "SetIOType needs to be called and set before other operations can be made on the pin";
        public static String handshakeFailed = "Handshake with arduino unit has failed!";
        public static String toManyCommands = "Too many commands without a fire(). Maximum commands are 8.";
        public static String portAlreadyOpen = "The port to the Arduino appears to be open already!";
        public static String commandTimedOut = "The sent command timed out.";
        public static String commandFailed = "The command has failed.";
        public ArduinoException()
            : base()
        {

        }
        public ArduinoException(string message)
            : base(message)
        {

        }
        public ArduinoException(string message, System.Exception inner)
            : base(message, inner)
        {

        }
        protected ArduinoException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) 
        { 
        }

    }
}
