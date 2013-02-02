using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Static helping tool
        /// Makes a best effort to deliver a connectable COM-String usable to connect to the Arduino
        /// Checks if running on windows or linux and act accordingly
        /// </summary>
        /// <returns></returns>
        public static String getArduinoPort()
        {
            int p = (int) Environment.OSVersion.Platform;
            if ((p == 4) || (p == 128)) //linux
            {
                enviroment = "linux";
                String[] coms = SerialPort.GetPortNames();
                foreach (String s in coms) 
                {
                    if (s.Contains("ACM")) return s;
                }
                    return null;
            }
            else //windows
            {
                enviroment = "windows";
                String com = WinCom.GetArduinoCOMPortWindows();
                return com;
            }
        }
        /// <summary>
        /// After a call to getArduinoPort this string contains "windows" or "linux"
        /// </summary>
        public static String enviroment;

        private SerialPort port;
        private ArrayList pins;
        private Command cm;
        private String pinName;
        /// <summary>
        /// Returns true if the device is open and usable.
        /// </summary>
        public Boolean isOpen;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="com">COM-adress</param>
        public Arduino(String com)
        {
            this.pinName = "PIN";
            this.port = new SerialPort(com, 115200);
            this.isOpen = port.IsOpen;
            this.pins = new ArrayList();
        }
        /// <summary>
        /// Disconnects from the device in a safe maner.
        /// </summary>
        public void Disconnect()
        {
            if (port.IsOpen)
                port.Close();
        }
        /// <summary>
        /// Returns a Pin object for ease of manipulation of the pins on the device
        /// Remember to set IO Type afterwards
        /// </summary>
        /// <param name="pinNumber">pinnumber</param>
        /// <returns></returns>
        public Pin getPin(int pinNumber)
        {
            foreach (Pin p in pins) if (p.name.Equals(pinName + pinNumber)) return p;
            Pin pin = new Pin(pinNumber, this, cm);
            pins.Add(pin);
            return pin;
        }
        /// <summary>
        /// Resets the device;
        /// (Note) Does not reset your pin-setup. It only stops all output from the device.
        /// </summary>
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
        internal void Send(String s)
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
        internal String read()
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
