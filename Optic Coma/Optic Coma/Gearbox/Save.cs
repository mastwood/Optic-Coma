﻿using System;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Threading;
using System.ComponentModel;


namespace OpticComa_Main
{
    /// <summary>
    /// Contains data to save upon game exit
    /// </summary>
    [Serializable]
    public struct SaveData
    {
        [XmlElement("Location_X")]
        public static float LocationX { get; set; }

        [XmlElement("Location_Y")]
        public static float LocationY { get; set; }

        [XmlElement("RecentSavePoint")]
        public static float RecentSavePoint { get; set; }
    }
    /// <summary>
    /// Serializes save files
    /// </summary>
    public class SaveFileSerializer
    {
        public SaveFileSerializer()
        {

        }
        public static void Save(XmlSerializer xml, float[] i)
        {
            try
            {
                using (TextWriter f = new StreamWriter(@"Content\save1.xml"))
                {
                    xml.Serialize(f, i);
                }
            }
            catch(Exception ex)
            {
            }
        }
        public static float[] Load(XmlSerializer xml)
        {
            var i = new float[3];

            try
            {
                using(var f = new FileStream(@"Content\save1.xml", FileMode.Open))
                    i = (float[])xml.Deserialize(f);

                return i;
            }
            catch (FileNotFoundException ex)
            {
                return null;
            }
        }
        
    }
    /// <summary>
    /// Writes error reports
    /// </summary>
    public class LogWriter
    {
        public string[] Lines { get; set; }
        public static string Path = "";
        public DateTime Tracker { get; set; }

        public LogWriter()
        {
            Tracker = DateTime.Now;
        }
        public static void Write(string message, string stackTrace, string errorCode)
        {
            if (!File.Exists(Path))
            {
                // Create a file to write to.
                using (var sw = File.CreateText(Path))
                {
                    sw.WriteLine("ERROR LOG {Tracker} \r \r");

                    sw.Write(message);
                    sw.Write(stackTrace);
                }
            }
        }
    }
}
