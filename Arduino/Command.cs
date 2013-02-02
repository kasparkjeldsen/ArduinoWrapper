using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace KK.Arduino
{
    public class Command
    {
        private String commands;
        private String digitalOut, digitalIn,analogIn, setup, blank, handshake, msend;
        /// <summary>
        /// blocks the thread untill a sent command has been acknowledged
        /// default is true;
        /// use false at own risk!
        /// </summary>
        public Boolean blockForAck;
        private Arduino arduino;
        private SerialIn sIn;
        private String zero;
        public Command(Arduino arduino)
        {
            this.digitalOut = "d{0}{1};";
            this.digitalIn = "i{0};";
            this.analogIn = "A{0};";
            this.setup = "S{0}{1}{2};";
            this.blank = "000000";
            this.handshake = "[H00000]";
            this.zero = "0";
            this.msend = "[{0}]";
            this.blockForAck = true;
            this.arduino = arduino;
            this.commands = String.Empty;
            this.sIn = new SerialIn(this.arduino);
        }
        /// <summary>
        /// Reads value 0 or 1 from given port number.
        /// Advise: if the pin isn't connected to anything, the result will be completly random.
        /// Note: Always autofires due to return value needed.
        /// </summary>
        /// <param name="port">pin number</param>
        /// <returns></returns>

        internal Boolean Handshake()
        {
            arduino.Send(handshake); //send handshake.
            String s = String.Empty; 
            int u = 0; //try 3 times.
            while (true)
            {
                sIn.Listen(); //listen for acknowledge
                if (sIn.received != null) //if we recieved some data
                {
                    s = sIn.received;
                    if (s.Length > 0) break; //break out of loop and hope for the best
                }
                else Thread.Sleep(8);
                u++;
                if (u == 3) break;
            }
            if (s.Contains(sIn.aCK)) return true; //if its acknowledged, return true
            else return false;
        }
        /// <summary>
        /// Tries to read the value of the pin.
        /// Returns int 0 or 1.
        /// Left open, since non-destructive, but you should use the Pin class.
        /// </summary>
        /// <param name="port">pinnumber</param>
        /// <returns></returns>
        public int DigitalRead(int port)
        {
            if (port > -1 && port < 14) //if port is within range
            {
                String newDigitalIn = digitalIn;
                newDigitalIn = String.Format(newDigitalIn, port);
                if (newDigitalIn.Length < 6)
                {
                    for (int i = 0; i < 8 - newDigitalIn.Length; i++)
                        newDigitalIn += zero;
                }

                fire(newDigitalIn);
                return int.Parse(sIn.Read());
            }
            else throw new ArduinoException(ArduinoException.portNotInRange);
        }
        /// <summary>
        /// Tries to read analog input from port
        /// Returns value 0 to 1023
        /// </summary>
        /// <param name="port">pinnumber</param>
        /// <returns></returns>
        public int AnalogRead(int port)
        {
            if (port > -1 && port < 6) //if port is in range
            {
                String newAnaloglIn = analogIn;
                newAnaloglIn = String.Format(newAnaloglIn, port);
                if (newAnaloglIn.Length < 6)
                {
                    for (int i = 0; i < 8 - newAnaloglIn.Length; i++)
                        newAnaloglIn += zero;
                }

                fire(newAnaloglIn);
                return int.Parse(sIn.Read());
            }
            else throw new ArduinoException(ArduinoException.portNotInRange);
        }
        /// <summary>
        /// Sets a outgoing pin to either On or Off.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="onOff"></param>
        internal void DigitalOut(int port, Boolean onOff)
        {
            if (port > -1 && port < 14) //if ports in range
            {
                int of = -1;
                if (onOff) of = 1;
                else of = 0;
                String newDigitalOut = digitalOut;
                newDigitalOut = String.Format(newDigitalOut, of, port);
                if (newDigitalOut.Length < 6)
                    for (int i = 0; i < 7 - newDigitalOut.Length; i++)
                        newDigitalOut += zero;
                fire(newDigitalOut);
            }
            else new ArduinoException(ArduinoException.portNotInRange);
        }
        /// <summary>
        /// Used for setting in and outgoing pins
        /// Todo: incomming analog.
        /// </summary>
        /// <param name="type">digital/analog</param>
        /// <param name="p1">pin nummer</param>
        /// <param name="p2">on/off</param>
        internal void Setup(String type, int port, Boolean inOut)
        {
            if (type == Pin.DIGITAL || type == Pin.ANALOG)
            {
                if (port > -1 && port < 14)
                {
                    if (inOut == Pin.IN || inOut == Pin.OUT)
                    {
                        String newSetup = setup;
                        int of = -1;
                        if (inOut) of = 1;
                        else of = 0;
                        newSetup = String.Format(newSetup, type, of, port);
                        if (newSetup.Length < 6)
                        {
                            for (int i = 0; i < 6 - newSetup.Length; i++)
                                newSetup += zero;
                        }

                        fire(newSetup);
                    }
                }
                else throw new ArduinoException(ArduinoException.portNotInRange);
            }
            else throw new ArduinoException(ArduinoException.ioTypeException);
        }
        /// <summary>
        /// Sends command and aways ACK signal from device.
        /// </summary>
        /// <param name="cm">command</param>
        internal void fire(String cm)
        {
            arduino.Send(String.Format(msend, cm));
            if (blockForAck)
            {
                var d = DateTime.Now;
                while (true)
                {
                    if (DateTime.Now.Subtract(d).TotalMilliseconds > 200) throw new ArduinoException(ArduinoException.commandTimedOut);
                    String ss = cm.Substring(0,cm.IndexOf(";")+1);
                        bool ack = sIn.ack(ss, 3);
                        if (ack) break;
                }
            }
        }
        /// <summary>
        /// Public access method to the internal fire()
        /// TODO* secure command.
        /// </summary>
                                                                                    /// <param name="s"></param>
        public void ConsoleFire(string s)
        {
            fire(s);
        }
    }
}
