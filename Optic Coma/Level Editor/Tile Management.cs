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
    public class Tile
    {
        public Point Location;
        public Image Texture;
        public bool ShowGridLines;
        public Tile(Point l, Image i)
        {
            Location = l;
            Texture = i;
        }
        public void SetLocation(Point l)
        {
            Location = l;
        }
        public void SetTexture(Image i)
        {
            Texture = i;
        }
        public void Draw(Graphics g)
        {
            g.DrawImage(Texture, Location);
            if (ShowGridLines)
            {
                GraphicsUnit f = GraphicsUnit.Pixel;
                PointF[] points = {
                    new PointF(Texture.GetBounds(ref f).Top, Texture.GetBounds(ref f).Left),
                    new PointF(Texture.GetBounds(ref f).Top, Texture.GetBounds(ref f).Right),
                    new PointF(Texture.GetBounds(ref f).Bottom, Texture.GetBounds(ref f).Right),
                    new PointF(Texture.GetBounds(ref f).Bottom, Texture.GetBounds(ref f).Left)
                    };
                g.DrawLines(Pens.Black, points);
            }
        }
    }

    public class TileGrid
    {
        public List<Tile> Tiles;
        Bitmap ComposedImage;
        public TileGrid(List<Tile> t)
        {
            Tiles = t;
        }
        public void Composite()
        {
            int Width = 0, Height = 0;
            foreach (Tile t in Tiles)
            {
                if (t.Location.X > Width) Width = t.Location.X;
                if (t.Location.Y > Height) Height = t.Location.Y;
            }
            Bitmap b = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics canvas = Graphics.FromImage(b))
            {
                canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                foreach (Tile j in Tiles)
                {
                    canvas.DrawImage(j.Texture, j.Location);
                }
            }
            ComposedImage = b;
        }
        public Image GetComposedImage()
        {
            Composite();
            return ComposedImage;
        }
        public void LevelEditorDraw(Graphics g, Point PanOffset)
        {
            g.DrawImage(GetComposedImage(), PanOffset);
        }
    }
}
