using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Optic_Coma
{
    public class TileSystem
    {
        int width, height;
        Vector2 location;
        public TileSystem()
        {
            width = 0;
            height = width;
            location = Vector2.Zero;
        }
        public void Draw(Texture2D texture, SpriteBatch spriteBatch)
        {
            width = texture.Width;
            height = texture.Height;
            for(int i = 0; i < ScreenManager.Instance.Dimensions.X; i++)
            {
                for (int j = 0; j < ScreenManager.Instance.Dimensions.Y; j++)
                {
                    if(i % width == 0 && j % height == 0)
                    {
                        location.X = i;
                        location.Y = j;
                        spriteBatch.Draw
                        (
                            texture,
                            location,
                            null,
                            Color.White,
                            0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            ScreenManager.Instance.TileLayer
                        );
                    }
                }
            }
        }
    }
}
