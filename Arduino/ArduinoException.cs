using System;


namespace KK.Arduino
{
    /// <summary>
    /// Just a class to give some nice exceptions for when you fuck up.
    /// </summary>
    [Serializable]
    class ArduinoException : System.Exception
    {
        public static String ioTypeException = "SetIOType needs to be called and set before other operations can be made on the pin";
        public static String handshakeFailed = "Handshake with arduino unit has failed! Try pressing the Reset button and run again.";
        public static String toManyCommands = "Too many commands without a fire(). Maximum commands are 8.";
        public static String portAlreadyOpen = "The port to the Arduino appears to be open already!";
        public static String commandTimedOut = "The sent command timed out.";
        public static String commandFailed = "The command has failed.";
        public static String portNotInRange = "The specified port is not within range of the Arduino";
        public static String nonExisitingIOType = "The IO type specified is not supported.";
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
