using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OpticComa_Types
{
    [Serializable]
    public class EnemySpawnerProperties
    {
        List<EnemyProperties> EnemyConfigs;
    }
    /// <summary>
    /// Contains the properties of an enemy that will be loaded or saved alongside a level
    /// </summary>
    [Serializable]
    public class EnemyProperties
    {
        public EnemyType Mode;
        public Texture2D Texture;
    }
    /// <summary>
    /// Contains properties of some NPC that can be loaded and saved alongside a level
    /// </summary>
    [Serializable]
    public class NPCProperties
    {
        public NPCMode InitMode;
        public Vector2 InitLocation;
        public Texture2D Texture;
    }
    [Serializable]
    public class LevelSerializable
    {
        [XmlAttribute("EnemySpawnerPropertiesList")]
        public List<EnemySpawnerProperties> EnemySpawners;
        [XmlAttribute("ForegroundTexturePath")]
        public string Foreground;
        [XmlAttribute("MidgroundTexturePath")]
        public string Midground;
        [XmlAttribute("BackgroundTexturePath")]
        public string Background;
        public List<Vector2> PointLightLocations;
        public List<TriHitBox> TriHitBoxes;
        public List<RectHitBox> RectHitBoxes;
        public LevelSerializable()
        {

        }
    }

    public interface IHitBox
    {
        bool Contains(Vector2 point);
        bool Contains(Rectangle rect);
        void Update(Vector2 offset);
    }
    public class RectHitBox : IHitBox
    {
        public Vector2 Location;
        public Rectangle Size;

        [XmlIgnore]
        private Rectangle bounds;

        public RectHitBox()
        {
            bounds = new Rectangle((int)Location.X, (int)Location.Y, Size.Width, Size.Height);
        }
        public bool Contains(Rectangle r)
        {
            bounds = new Rectangle((int)Location.X, (int)Location.Y, Size.Width, Size.Height);
            Vector2 UR, UL, BR, BL;

            UR = new Vector2(r.Top, r.Right);
            UL = new Vector2(r.Top, r.Left);
            BR = new Vector2(r.Bottom, r.Right);
            BL = new Vector2(r.Bottom, r.Left);

            if (bounds.Contains(UR) || bounds.Contains(UL) || bounds.Contains(BR) || bounds.Contains(BL))
                return true;

            return false;
        }
        public bool Contains(Vector2 point)
        {
            bounds = new Rectangle((int)Location.X, (int)Location.Y, Size.Width, Size.Height);
            if (bounds.Contains(point)) return true;
            return false;
        }
        public void Update(Vector2 offset)
        {

        }
    }
    public class TriHitBox : IHitBox
    {
        [XmlIgnore]
        private double area;

        /// THE ARRAY IS ARRANGED LIKE THIS
        ///            0
        ///           / \
        ///          /   \
        ///         /     \
        ///        2-------1
        public Vector2[] Points;
        public Vector2 Location;

        public TriHitBox(Vector2[] v)
        {
            Points = v;
            area = 0.5 * (-Points[1].Y * Points[2].X + Points[0].Y * (Points[2].X - Points[1].X) +
                Points[0].X * (Points[1].Y - Points[2].Y) + Points[1].X * Points[2].Y); //magic code
        }
        public void Update(Vector2 offset)
        {
            for (int i = 0; i < 4; i++)
            {
                Points[i].X += offset.X;
                Points[i].Y += offset.Y;
            }
        }
        public bool Contains(Rectangle r)
        {
            double s, t;
            Vector2 topright = new Vector2(r.Right, r.Top);
            Vector2 topleft = new Vector2(r.Left, r.Top);
            Vector2 botright = new Vector2(r.Right, r.Bottom);
            Vector2 botleft = new Vector2(r.Left, r.Bottom);

            s = 1 / (2 * area) * (Points[0].Y * Points[2].X - Points[0].X * Points[2].Y +
                (Points[2].Y - Points[0].Y) * topright.X + (Points[0].X - Points[2].X) * topright.Y);
            t = 1 / (2 * area) * (Points[0].X * Points[1].Y - Points[0].Y * Points[1].X +
                (Points[0].Y - Points[1].Y) * topright.X + (Points[1].X - Points[0].X) * topright.Y);

            if (s > 0 && t > 0 && 1 - s - t > 0) return true;

            s = 1 / (2 * area) * (Points[0].Y * Points[2].X - Points[0].X * Points[2].Y +
                (Points[2].Y - Points[0].Y) * botright.X + (Points[0].X - Points[2].X) * botright.Y);
            t = 1 / (2 * area) * (Points[0].X * Points[1].Y - Points[0].Y * Points[1].X +
                (Points[0].Y - Points[1].Y) * botright.X + (Points[1].X - Points[0].X) * botright.Y);

            if (s > 0 && t > 0 && 1 - s - t > 0) return true;

            s = 1 / (2 * area) * (Points[0].Y * Points[2].X - Points[0].X * Points[2].Y +
                (Points[2].Y - Points[0].Y) * topleft.X + (Points[0].X - Points[2].X) * topleft.Y);
            t = 1 / (2 * area) * (Points[0].X * Points[1].Y - Points[0].Y * Points[1].X +
                (Points[0].Y - Points[1].Y) * topleft.X + (Points[1].X - Points[0].X) * topleft.Y);

            if (s > 0 && t > 0 && 1 - s - t > 0) return true;

            s = 1 / (2 * area) * (Points[0].Y * Points[2].X - Points[0].X * Points[2].Y +
                (Points[2].Y - Points[0].Y) * botleft.X + (Points[0].X - Points[2].X) * botleft.Y);
            t = 1 / (2 * area) * (Points[0].X * Points[1].Y - Points[0].Y * Points[1].X +
                (Points[0].Y - Points[1].Y) * botleft.X + (Points[1].X - Points[0].X) * botleft.Y);

            if (s > 0 && t > 0 && 1 - s - t > 0) return true;

            return false;
        }

        public bool Contains(Vector2 p)
        {
            double s, t;

            //magic code formula from better geometers than I
            s = 1 / (2 * area) * (Points[0].Y * Points[2].X - Points[0].X * Points[2].Y +
                (Points[2].Y - Points[0].Y) * p.X + (Points[0].X - Points[2].X) * p.Y);
            t = 1 / (2 * area) * (Points[0].X * Points[1].Y - Points[0].Y * Points[1].X +
                (Points[0].Y - Points[1].Y) * p.X + (Points[1].X - Points[0].X) * p.Y);

            if (s > 0 && t > 0 && 1 - s - t > 0) return true;

            return false;
        }
    }

}
