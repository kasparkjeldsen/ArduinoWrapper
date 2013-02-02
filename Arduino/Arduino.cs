using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Collections;
namespace KK.Arduino
{
    /// <summary>
    /// C# Wrapper for an Arduino Uno
    /// Written by Kaspar Kjeldsen.
    /// </summary>
    public class Arduino
    {
        public static String getArduinoPort()
        {
            int p = (int) Environment.OSVersion.Platform;
            if ((p == 4) || (p == 128)) //linux
            {
                Console.WriteLine("LINUX detected");
                String[] coms = SerialPort.GetPortNames();
                foreach (String s in coms) 
                {
                    if (s.Contains("ACM")) return s;
                }
                    return null;
            }
            else //windows
            {
                Console.WriteLine("WINDOWS detected");
                String com = WinCom.GetArduinoCOMPortWindows();
                return com;
            }
        }

        private SerialPort port;
        private ArrayList pins;
        private Command cm;
        private String pinName;
        public Boolean isOpen;
        /// <summary>
        /// Construtor. Tries to find a COM port it can use.
        /// </summary>
        public Arduino(String com)
        {
            this.pinName = "PIN";
            this.port = new SerialPort(com, 115200);
            this.isOpen = port.IsOpen;
            this.pins = new ArrayList();
        }
        public void Disconnect()
        {
            if (port.IsOpen)
                port.Close();
        }
        public Pin getPin(int pinNumber)
        {
            foreach (Pin p in pins) if (p.name.Equals(pinName + pinNumber)) return p;
            Pin pin = new Pin(pinNumber, this, cm);
            pins.Add(pin);
            return pin;
        }
        public void Reset()
        {
            foreach (Pin p in pins) p.switchOff();
        }
        /// <summary>
        /// Connects computer with Arduino
        /// </summary>
        public void Connect()
        {
            if (!port.IsOpen)
            {
                port.Open();
                cm = new Command(this);
            }
            else throw new ArduinoException(ArduinoException.portAlreadyOpen);
            if (!cm.Handshake())
            {
                port.Close();
                port.Open();
                if (!cm.Handshake()) throw new ArduinoException(ArduinoException.handshakeFailed);
            }
        }
        /// <summary>
        /// Send String to Arduino
        /// </summary>
        /// <param name="s"></param>
        public void Send(String s)
        {
            try
            {
                port.Write(s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// Read String from arduino.
        /// </summary>
        /// <returns></returns>
        public String read()
        {
            try
            {
                String ret = port.ReadExisting();
                return ret;
            }
            catch (Exception)
            {   //no error handling, we expect a lot of mis-treading.
                return null;
            }
        }
    }
}
