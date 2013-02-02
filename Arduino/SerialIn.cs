using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KK.Arduino
{
    public class SerialIn
    {
        private Arduino arduino;
        public  String received;
        private String read;
        private Thread t;
        private String sack;
        private Object lockThis;
        private String mStart;
        private String mStop;
        private String ACK;
        public readonly String aCK;
        private String sackDivider;
        private String mReturn;
        private String tmpS;
        private int sackPos;
        public SerialIn(Arduino arduino)
        {
            this.mStart = "?";
            this.mStop = "*";
            this.ACK = "ACK";
            this.aCK = "aCK";
            this.sackDivider = "#";
            this.mReturn = "RETURN:";
            this.sackPos = 0;
            this.tmpS = String.Empty;
            this.sack = String.Empty;
            this.arduino = arduino;
            this.lockThis = new Object();
        }
        public void Listen()
        {
            String buffer = String.Empty;
            var d = DateTime.Now;
            while (true)
            {
                if (DateTime.Now.Subtract(d).TotalMilliseconds > 100) 
                    break;
                    buffer += arduino.read();
                    if (!String.IsNullOrEmpty(buffer))
                    {
                        if (buffer.Contains(mStop) && buffer.Contains(mStart))
                        {
                            String message = buffer.Substring(buffer.IndexOf(mStart)+1);
                            if (message.IndexOf(mStop) > 0)
                            {
                                message = message.Substring(0, message.IndexOf(mStop));
                                received = message;
                            }
                            if (message.Contains(ACK))
                            {
                                sackPos = sack.Length;
                                sack += message;
                                buffer = String.Empty;
                                break;
                            }
                            if (message.Contains(aCK)) buffer = String.Empty;
                            if (message.Contains("FAILED"))
                            {
                                buffer = String.Empty;
                                throw new ArduinoException(ArduinoException.commandFailed);
                            }
                            message = String.Empty;
                            break;
                        }
                    }
                
            }
        }
        /// <summary>
        /// Returns value of last read
        /// </summary>
        /// <returns></returns>
        public String Read()
        {
            return read;
        }
        /// <summary>
        /// Used together with fire from Command, when fire should block for ack.
        /// Checks input for acknowledgements from the Arduino.
        /// You really shouldn't use this, but use the Command class.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public Boolean ack(String s, int random)
        {
            Listen();
            String tmpsack = sack;
            if (!String.IsNullOrEmpty(tmpsack))
            {
                if (tmpsack.Contains(s) && !tmpS.Contains(sackDivider + random + sackDivider + s))
                {
                    if (tmpsack.Contains(mReturn))
                    {
                        read = tmpsack.Substring(tmpsack.IndexOf(mReturn)+mReturn.Length,1);
                    }
                    tmpS += sackDivider + random + sackDivider + s;
                    ResetAcks();
                    return true;
                }
                else return false;
            }
            else return false;
        }
        /// <summary>
        /// Used by command class to reset after waiting for acks.
        /// You really shouldn't use this, but use the Command class.
        /// </summary>
        public void ResetAcks()
        {
            sack = String.Empty;
            tmpS = String.Empty;
        }
    }
}
