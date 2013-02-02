using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace KK.Arduino
{
    /// <summary>
    /// Class closest to the arduino.
    /// Handles communication.
    /// </summary>
    public class SerialIn
    {
        #region vars
        private Arduino arduino;
        private String read;
        private Thread t;
        private String sack;
        private Object lockThis;
        private String mStart;
        private String mStop;
        private String ACK;
        private String sackDivider;
        private String mReturn;
        private String tmpS;
        private int sackPos;
        /// <summary>
        /// String containing data recieved
        /// </summary>
        public String received;
        /// <summary>
        /// String used to handshake.
        /// </summary>
        public readonly String aCK;
        #endregion
        /// <summary>
        /// Internal class, used to handle the direct communication between computer and arduino.
        /// Not supposed to be used by other classes than Command
        /// </summary>
        /// <param name="arduino"></param>
        internal SerialIn(Arduino arduino)
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
        /// <summary>
        /// Run to update Receive and sack string
        /// </summary>
        internal void Listen()
        {
            String buffer = String.Empty;
            var d = DateTime.Now; //break loop after 100 milliseconds
            while (true)
            {
                if (DateTime.Now.Subtract(d).TotalMilliseconds > 100) //check time.
                    break;
                    buffer += arduino.read(); //read
                    if (!String.IsNullOrEmpty(buffer)) //if we got data
                    {
                        if (buffer.Contains(mStop) && buffer.Contains(mStart)) //if it contains start and end char
                        {
                            String message = buffer.Substring(buffer.IndexOf(mStart)+1); //get message
                            if (message.IndexOf(mStop) > 0) //make sure the start and end chars are in the right order
                            {
                                message = message.Substring(0, message.IndexOf(mStop));
                                received = message; //save message.
                            }
                            if (message.Contains(ACK)) //if command acknowledged
                            {
                                sackPos = sack.Length;
                                sack += message;
                                buffer = String.Empty;
                                break;
                            }
                            if (message.Contains(aCK)) buffer = String.Empty; //if handshake acknowledged
                            if (message.Contains("FAILED")) //if command failed.
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
        /// Currently not in use.
        /// </summary>
        /// <returns></returns>
        internal String Read()
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
        internal Boolean ack(String s, int random)
        {
            Listen();
            String tmpsack = sack; //get tmp string.
            if (!String.IsNullOrEmpty(tmpsack)) //if its not empty
            {
                if (tmpsack.Contains(s) && !tmpS.Contains(sackDivider + random + sackDivider + s)) //if it contains the command we are blocking for
                {
                    if (tmpsack.Contains(mReturn)) //if its a return value
                    {
                        read = tmpsack.Substring(tmpsack.IndexOf(mReturn)+mReturn.Length);
                        read = read.Replace(";", "");
                    }
                    tmpS += sackDivider + random + sackDivider + s;
                    ResetAcks();
                    return true; //unblock
                }
                else return false;
            }
            else return false;
        }
        /// <summary>
        /// Used by command class to reset after waiting for acks.
        /// You really shouldn't use this, but use the Command class.
        /// </summary>
        internal void ResetAcks()
        {
            sack = String.Empty;
            tmpS = String.Empty;
        }
    }
}
