using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Buildings.Templates;
using TopDown.States;

namespace TopDown.Buildings.Labour
{
  public class Blacksmith : Building
  {
    public override BuildingStates BuildingState
    {
      get { return State; }
      set
      {
        if (State == value)
          return;

        State = value;

        var height = CurrentSprite.Rectangle.Height;
        var width = CurrentSprite.Rectangle.Width;

        switch (State)
        {
          case BuildingStates.Placing:
            CollisionRectangles = new List<Rectangle>();
            break;
          case BuildingStates.Placed:
          case BuildingStates.Building:
            CollisionRectangles = new List<Rectangle>()
            {
              new Rectangle((int)Position.X, (int)Position.Y, width, height),
            };
            break;
          case BuildingStates.Built_Out:

            var yDiff = _template.OutExtraHeight;
            var xDiff = _template.OutExtraWidth;

            var collisionRectangles = new List<Rectangle>()
            {
              new Rectangle((int)Position.X, (int)Position.Y, width - xDiff, 1), // Top
              new Rectangle((int)Position.X, (int)Position.Y, 1, 96), // Left
              new Rectangle((int)Position.X + (width - 1) - xDiff, (int)Position.Y, 1, height - yDiff), // Right
              new Rectangle((int)Position.X + 49, (int)Position.Y + 95, 48, 1), // bottom left
              new Rectangle((int)Position.X + 96, (int)Position.Y + 96, 1, 64), 
              new Rectangle((int)Position.X + 96, (int)Position.Y + height - yDiff - 1, 96, 1), // bottom right
            };

            collisionRectangles.AddRange(Components.SelectMany(c => c.CollisionRectangles).ToList());

            CollisionRectangles = collisionRectangles;

            break;
        }
      }
    }

    public Blacksmith(GameScreen gameState, BuildingTemplate template) : base(gameState, template)
    {
    }
  }
}
