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
  public class Bar : Furniture
  {
    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        var x = (int)Position.X;
        var y = (int)Position.Y;

        return new List<Rectangle>()
        {
          new Rectangle(x, y, 128, 32),
          new Rectangle(x, y + 64, 192, 32),
          new Rectangle(x + 160, y, 32, 64),
        };
      }
    }

    public Bar(Texture2D texture, GameScreen gameState) : base(texture, gameState)
    {
    }
  }
}
