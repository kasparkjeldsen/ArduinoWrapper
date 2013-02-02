using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace KK.Arduino
{
    public class Command
    {
        private String commands;
        private String digitalOut, digitalIn, setup, blank, handshake, msend;
        /// <summary>
        /// auto sends commands if limit is exceeded.
        /// </summary>
        public Boolean autoFire;
        /// <summary>
        /// blocks the thread untill a sent command has been acknowledged
        /// default is true;
        /// use false at own risk!
        /// </summary>
        public Boolean blockForAck;
        private Boolean alwaysFire;
        private Arduino arduino;
        private SerialIn sIn;
        private String zero;
        public Command(Arduino arduino)
        {
            this.digitalOut = "d{0}{1};";
            this.digitalIn = "i{0};";
            this.setup = "S{0}{1}{2};";
            this.blank = "000000";
            this.handshake = "[H00000]";
            this.zero = "0";
            this.msend = "[{0}]";
            this.autoFire = false;
            this.blockForAck = true;
            this.alwaysFire = true;
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

        public Boolean Handshake()
        {
            arduino.Send(handshake);
            String s = String.Empty;
            int u = 0;
            while (true)
            {
                sIn.Listen();
                if (sIn.received != null)
                {
                    s = sIn.received;
                    if (s.Length > 0) break;
                }
                else Thread.Sleep(8);
                u++;
                if (u == 3) break;
            }
            if (s.Contains(sIn.aCK)) return true;
            else
            {
                return false;
            }
        }
        public int DigitalRead(int port)
        {
            if (port > -1 && port < 14)
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
            return -1;
        }
        /// <summary>
        /// Sets a outgoing pin to either On or Off.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="onOff"></param>
        public void DigitalOut(int port, Boolean onOff)
        {
            if (port > -1 && port < 14)
            {
                int of = -1;
                if (onOff) of = 1;
                else of = 0;
                String newDigitalOut = digitalOut;
                newDigitalOut = String.Format(newDigitalOut, of, port);
                if (newDigitalOut.Length < 6)
                {
                    for (int i = 0; i < 7 - newDigitalOut.Length; i++)
                        newDigitalOut += zero;
                }

                fire(newDigitalOut);
            }
        }
        /// <summary>
        /// Used for setting in and outgoing pins
        /// Todo: incomming analog.
        /// </summary>
        /// <param name="type">digital/analog</param>
        /// <param name="p1">pin nummer</param>
        /// <param name="p2">on/off</param>
        public void Setup(String type, int port, Boolean inOut)
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
            }
        }
        private void addCommand(String s)
        {
                commands = s;
        }
        public void fire(String cm)
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
    }
}
