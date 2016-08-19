using System;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Optic_Coma
{
    class XmlManager<T>
    {
        public Type Type;

        public T Load(string path)
        {
            T instance;
            using (TextReader reader = new StreamReader(path))
            {
                XmlSerializer xml = new XmlSerializer(Type);
                instance = (T)xml.Deserialize(reader);
            }
            return instance;
        }
        public void Save(string path, object obj)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                XmlSerializer xml = new XmlSerializer(Type);
                xml.Serialize(writer, obj);
            }
        }
    }
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
