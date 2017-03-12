using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.InteropServices;

namespace Level_Editor
{
    public class ErrorHandler
    {
        public ErrorHandler()
        {

        }
        public static void AppendLog(Exception ex)
        {
            using (StreamWriter f = new StreamWriter(new FileStream("log.txt", FileMode.Append)))
            {
                f.WriteLineAsync(DateTime.Now + "\n" + ex.StackTrace + "\n");
            }
        }
        public static void ClearLog()
        {
            using (StreamWriter f = new StreamWriter(new FileStream("log.txt", FileMode.Create)))
            {
                f.WriteAsync("");
            }
        }
    }
}
