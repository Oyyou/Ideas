using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.States;

namespace TopDown.Buildings.Housing
{
  public class SmallHouse : Building
  {
    protected override int _outsideExtraHeight => 128;

    protected override int _outsideExtraWidth => 40;

    public override BuildingStates BuildingState
    {
      get { return State; }
      set
      {
        if (State == value)
          return;

        State = value;

        switch (State)
        {
          case BuildingStates.Placing:
            CollisionRectangles = new List<Rectangle>();
            break;
          case BuildingStates.Placed:
          case BuildingStates.Building:
            CollisionRectangles = new List<Rectangle>()
            {
              _spriteInside.Rectangle,
            };
            break;
          case BuildingStates.Built_Out:

            var height = _spriteInside.Rectangle.Height;
            var width = _spriteInside.Rectangle.Width;

            var x = (int)_spriteInside.Position.X;
            var y = (int)_spriteInside.Position.Y;

            var collisionRectangles = new List<Rectangle>()
            {
              new Rectangle(x, y, width, 1), // Top
              new Rectangle(x, y, 1, height), // Left
              new Rectangle(x + (width - 1), y, 1, height), // Right
              new Rectangle(x, y + height - 1, 16, 1), // bottom left
              new Rectangle(x + 64, y + height - 1, width - 64, 1), // bottom right
            };

            collisionRectangles.AddRange(Components.SelectMany(c => c.CollisionRectangles).ToList());

            CollisionRectangles = collisionRectangles;

            break;
        }
      }
    }

    public SmallHouse(GameScreen gameState, Texture2D textureInside, Texture2D textureOutside) : base(gameState, textureInside, textureOutside)
    {
    }
  }
}
