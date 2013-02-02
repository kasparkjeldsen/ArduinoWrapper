using System;
using System.Text;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Management;

public class WinCom
{
    /// <summary>
    /// Best effort to get the right port to connect to. 
    /// NO PROMISES
    /// Windows part.
    /// </summary>
    /// <returns></returns>
    public static string GetArduinoCOMPortWindows()
    {
        List<String> list = new List<String>();
        ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
        String ardo = String.Empty;
        foreach (ManagementObject mo2 in searcher2.Get())
        {
            string name = mo2["Name"].ToString();
            if (name.Contains("(COM"))
            {
                if (name.Contains("Arduino"))
                    ardo = name;
            }
        }
        ardo = ardo.Substring(ardo.IndexOf("COM"));
        ardo = ardo.Substring(0, ardo.Length - 1);
        return ardo;
    }
}