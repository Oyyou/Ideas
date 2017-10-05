using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDown.States;

namespace TopDown.Furnitures
{
  public class Bed : Furniture
  {
    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        var x = (int)Position.X;
        var y = (int)Position.Y;

        return new List<Rectangle>()
        {
          new Rectangle(x, y, Rectangle.Width, Rectangle.Height),
        };
      }
    }

    public Bed(Texture2D texture, GameScreen gameState) : base(texture, gameState)
    {
    }
  }
}
