﻿using System;
using System.IO.Ports;
using System.Threading;
using KK.Arduino;
using System.Collections;

namespace Testing
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
           // ArduinoConsole();
            Arduino a = new Arduino(Arduino.getArduinoPort());
            a.Connect();
            Pin p = a.getPin(2);
            p.setIOType(Pin.DIGITAL);
            p.setIO(Pin.OUT);
            p.switchOn();
           /*
            Arduino arduino = new Arduino(Arduino.getArduinoPort());
            arduino.Connect();
            Pin pin2 = arduino.getPin(Pin.PIN2);
            pin2.setIOType(Pin.DIGITAL);
            pin2.setIO(Pin.OUT);
            pin2.switchOn();
            Pin pinA0 = arduino.getPin(Pin.PINA0);
            pinA0.setIOType(Pin.ANALOG);
            Console.WriteLine(pinA0.Read());
            */

            Console.ReadKey();
        }
        static void ArduinoConsole()
        {
            String com = Arduino.getArduinoPort();
            Arduino arduino = new Arduino(com);
            arduino.Connect();
            Console.WriteLine("Connected to Arduino on " + com);
            Command c = new Command(arduino);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Handshake succesfull");
            Pin redLed = arduino.getPin(Pin.PIN2);
            redLed.setIOType(Pin.DIGITAL);
            redLed.setIO(Pin.OUT);
            Console.WriteLine("pin 2 succesfully set as OUTPUT");
            int i = 0;
            Double ms = 0;
            Console.WriteLine("Testing delay...");
            while (i < 100)
            {
                var d = DateTime.Now;
                redLed.switchOn();
                ms += DateTime.Now.Subtract(d).TotalMilliseconds;
                redLed.switchOff();
                i++;
            }
            var mms = ms / 100;
            Console.WriteLine("Led 2 just blinked 100 times. Average command delay is: " + mms + "ms");
            Console.WriteLine("Total time was: " + ms + "ms");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Arduino Console Running!");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Write \"help\" to view commands. Write \"exit\" to exit");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                String s = Console.ReadLine();
                if (s == "help")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("To setup a digital pin write \"Sd(0-1)(0-13);\". First parm 0 for input, 1 for output. Second parms is pinnumber.");
                    Console.WriteLine("To setup a analog pin write \"Sa0(0-13);\". First parm is pinnumber.");
                    Console.WriteLine("To flip output on pin write \"d(0-1)(0-13);. First parameter is 0 for low, 1 for high. Second is pinnumber.\"");
                    Console.WriteLine("To get value of a connected digital pin write \"i(0);\" where parameter is pinnumber");
                    Console.WriteLine("To get value of a connected analog pin write \"A(0);\" where parameter is pinnumber");
                    Console.WriteLine("Type \"reset;\" to cancel all effects");
                }
                else if (s == "reset;")
                {
                    try
                    {
                        arduino.Reset();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Command ran succesfully");
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Command FAILED");
                        Console.WriteLine(e);
                    }
                }
                else if (s == "exit")
                {
                    Console.WriteLine("Disconnecting and closing");
                    break;
                }
                else
                {
                    if (s.Length > 1)
                    {
                        if (s.Length < 6)
                            for (int b = 0; b <= 6 - s.Length; b++)
                                s += "0";
                        if (s[0] != 'i' && s[0] !='A')
                        {
                            try
                            {
                                if (s[0] == 'S')
                                {
                                    String t = s.Substring(3, s.IndexOf(";") - 3);
                                    arduino.getPin(int.Parse(t)).setIOType(Pin.DIGITAL);
                                }
                                c.ConsoleFire(s);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Command ran succesfully");
                            }
                            catch (Exception)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Command FAILED");
                            }
                        }
                        else
                        {
                            try
                            {
                                Char C = s[0];
                                s = s.Substring(1, s.IndexOf(";") - 1);
                                Console.ForegroundColor = ConsoleColor.Green;
                                if(C == 'A')
                                    Console.WriteLine("Result of pin " + s + " = " + c.AnalogRead(int.Parse(s)));
                                else Console.WriteLine("Result of pin " + s + " = " + c.DigitalRead(int.Parse(s)));
                            }
                            catch (Exception)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Command FAILED");
                            }

                        }
                    }
                }
            }

            arduino.Disconnect();
            System.Environment.Exit(0);  

        }

    }
}
