using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace OpticComa_Main
{
    //All of these may be unnecessary now due to the contentimporter
    
    using SysPoint = System.Drawing.Point;
    using SysImage = System.Drawing.Image;
    using SysColor = System.Drawing.Color;
    public class WinFormsConverters
    {
        public static Microsoft.Xna.Framework.Vector2 ConvertToVector2(System.Drawing.Point p)
        {
            return new Microsoft.Xna.Framework.Vector2(p.X, p.Y);
        }
        public static Microsoft.Xna.Framework.Vector2 ConvertToVector2(System.Drawing.PointF p)
        {
            return new Microsoft.Xna.Framework.Vector2(p.X, p.Y);
        }
        public static Microsoft.Xna.Framework.Vector2 ConvertToVector2(System.Drawing.Size p)
        {
            return new Microsoft.Xna.Framework.Vector2(p.Width, p.Height);
        }
        public static Microsoft.Xna.Framework.Rectangle ConvertToRect(System.Drawing.Rectangle p)
        {
            return new Microsoft.Xna.Framework.Rectangle(p.Location.X, p.Location.Y, p.Width, p.Height);
        }
        public static Microsoft.Xna.Framework.Rectangle ConvertToRect(System.Drawing.RectangleF p)
        {
            return new Microsoft.Xna.Framework.Rectangle((int)p.Location.X, (int)p.Location.Y, (int)p.Width, (int)p.Height);
        }
        public static Microsoft.Xna.Framework.Vector4 ConvertToVector4(System.Drawing.RectangleF p)
        {
            return new Microsoft.Xna.Framework.Vector4(p.Location.X, p.Location.Y, p.Width, p.Height);
        }
        public static Texture2D ConvertToTexture2D(Bitmap texture)
        {
            Microsoft.Xna.Framework.Color[] pixels = new Microsoft.Xna.Framework.Color[texture.Width * texture.Height];
            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    SysColor c = texture.GetPixel(x, y);
                    pixels[(y * texture.Width) + x] = new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);
                }
            }

            Texture2D myTex = new Texture2D(
              Foundation.GlobalGraphicsDeviceManager.GraphicsDevice,
              texture.Width,
              texture.Height);

            myTex.SetData(pixels);
            return myTex;
        }
    }    
}
