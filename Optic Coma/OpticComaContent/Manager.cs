using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Xml.Serialization;
using System.IO;
using OpticComa_Types;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpticComa_Content
{
    [ContentImporter("lvl", DefaultProcessor = "LevelFileProcessor",
    DisplayName = "Level File Importer - OpticComa_Content")]
    public class LevelImporter : ContentImporter<LevelSerializable>
    {
        public override LevelSerializable Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing XML file: {0}", filename);

            using (var streamReader = new StreamReader(filename))
            {
                var deserializer = new XmlSerializer(typeof(LevelSerializable));
                return (LevelSerializable)deserializer.Deserialize(streamReader);
            }
        }
    }
    [ContentTypeWriter]
    public class LevelContentWriter : ContentTypeWriter<LevelSerializable>
    {
        protected override void Write(ContentWriter output, LevelSerializable value)
        {
            output.WriteObject<LevelSerializable>(value);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(LevelSerializable).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "OpticComa_Content.LevelContentReader, OpticComa_Content";
        }

    }
}
