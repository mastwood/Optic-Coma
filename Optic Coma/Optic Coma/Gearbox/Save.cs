using System;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Optic_Coma
{
    
    public class LogWriter
    {
        public string[] lines;
        public static string path = "";
        public DateTime dateTime = DateTime.Now;

        public static void Write(string message, string stackTrace, string errorCode)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("ERROR LOG {dateTime} \r \r");

                    sw.Write(message);
                    sw.Write(stackTrace);
                }
            }
        }
    }
}
