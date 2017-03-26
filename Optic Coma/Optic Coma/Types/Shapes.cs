using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace OpticComa_Main
{
    

    public class Triangle
    {
        readonly Vector2[] V;
        Vector2 topright, topleft, botright, botleft;
        double area;
        double s, t;

        public Triangle(Vector2[] v) //Triangle points must be input in the order top left -> bottom right -> bottom left (CLOCKWISE)
        {
            V = v;
            area = 0.5 * (-V[1].Y * V[2].X + V[0].Y * (V[2].X - V[1].X) + V[0].X * (V[1].Y - V[2].Y) + V[1].X * V[2].Y); //magic code
        }
        
        public bool ContainsCornersOf(Rectangle r)
        {
            topright = new Vector2(r.Right, r.Top);
            topleft = new Vector2(r.Left, r.Top);
            botright = new Vector2(r.Right, r.Bottom);
            botleft = new Vector2(r.Left, r.Bottom);

            s = 1 / (2 * area) * (V[0].Y * V[2].X - V[0].X * V[2].Y + (V[2].Y - V[0].Y) * topright.X + (V[0].X - V[2].X) * topright.Y);
            t = 1 / (2 * area) * (V[0].X * V[1].Y - V[0].Y * V[1].X + (V[0].Y - V[1].Y) * topright.X + (V[1].X - V[0].X) * topright.Y);

            if (s > 0 && t > 0 && 1-s-t > 0) return true;

            s = 1 / (2 * area) * (V[0].Y * V[2].X - V[0].X * V[2].Y + (V[2].Y - V[0].Y) * botright.X + (V[0].X - V[2].X) * botright.Y);
            t = 1 / (2 * area) * (V[0].X * V[1].Y - V[0].Y * V[1].X + (V[0].Y - V[1].Y) * botright.X + (V[1].X - V[0].X) * botright.Y);

            if (s > 0 && t > 0 && 1 - s - t > 0) return true;

            s = 1 / (2 * area) * (V[0].Y * V[2].X - V[0].X * V[2].Y + (V[2].Y - V[0].Y) * topleft.X + (V[0].X - V[2].X) * topleft.Y);
            t = 1 / (2 * area) * (V[0].X * V[1].Y - V[0].Y * V[1].X + (V[0].Y - V[1].Y) * topleft.X + (V[1].X - V[0].X) * topleft.Y);

            if (s > 0 && t > 0 && 1 - s - t > 0) return true;

            s = 1 / (2 * area) * (V[0].Y * V[2].X - V[0].X * V[2].Y + (V[2].Y - V[0].Y) * botleft.X + (V[0].X - V[2].X) * botleft.Y);
            t = 1 / (2 * area) * (V[0].X * V[1].Y - V[0].Y * V[1].X + (V[0].Y - V[1].Y) * botleft.X + (V[1].X - V[0].X) * botleft.Y);

            if (s > 0 && t > 0 && 1 - s - t > 0) return true;

            return false;
        }
        
    }
}
