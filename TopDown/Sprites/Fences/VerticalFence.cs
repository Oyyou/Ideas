using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TopDown.Sprites.Fences
{
  public class VerticalFence : Sprite
  {
    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        return new List<Rectangle>()
        {
          new Rectangle(Rectangle.X + 11, Rectangle.Y, 10, Rectangle.Height),
        };
      }
    }

    public VerticalFence(Texture2D texture) : base(texture)
    {
    }
  }
}
