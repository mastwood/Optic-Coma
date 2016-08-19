using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Serializer
{
    public class XMLManager<T>
    {
        public Type type;

        public T Load(string path)
        {
            T instance;
            using(TextReader reader = new StreamReader(path))
            {
                XmlSerializer s = new XmlSerializer(type);
                instance = (T)s.Deserialize(reader);
            }
            return instance;
        }
        public void Save(string path, object obj)
        {
            using(TextWriter w = new StreamWriter(path))
            {
                XmlSerializer s = new XmlSerializer(type);
                s.Serialize(w, obj);
            }
        }
    }
}
