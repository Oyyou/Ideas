using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Sprites;
using TopDown.States;

namespace TopDown.Buildings.Labour
{
  public class Blacksmith : Building
  {
    protected override int _outsideExtraHeight => 64;

    protected override int _outsideExtraWidth => 40;

    public override BuildingStates State
    {
      get { return _state; }
      set
      {
        if (_state == value)
          return;

        _state = value;

        switch (_state)
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
              new Rectangle(x, y, 1, 96), // Left
              new Rectangle(x + (width - 1), y, 1, height), // Right
              new Rectangle(x + 49, y + 95, 48, 1), // bottom left
              new Rectangle(x + 96, y + 96, 1, 64),
              new Rectangle(x + 96, y + height - 1, 96, 1), // bottom right
            };

            collisionRectangles.AddRange(Components.SelectMany(c => c.CollisionRectangles).ToList());

            CollisionRectangles = collisionRectangles;

            break;
        }
      }
    }

    public Blacksmith(GameScreen gameState, Texture2D textureInside, Texture2D textureOutside) : base(gameState, textureInside, textureOutside)
    {
      Name = "Blacksmith";
    }

    protected override void SetDoorLocations()
    {
      _doorLocations = new List<DoorLocation>()
      {
        new DoorLocation()
        {
          Position = new Vector2(_spriteInside.Rectangle.X - 32, _spriteInside.Rectangle.Y + 96),
          IsValid = false,
        },
        new DoorLocation()
        {
          Position = new Vector2(_spriteInside.Rectangle.X - 32, _spriteInside.Rectangle.Y + 128),
          IsValid = false,
        },
        new DoorLocation()
        {
          Position = new Vector2(_spriteInside.Rectangle.X , _spriteInside.Rectangle.Bottom),
          IsValid = false,
        },
        new DoorLocation()
        {
          Position = new Vector2(_spriteInside.Rectangle.X + 32, _spriteInside.Rectangle.Bottom),
          IsValid = false,
        },
        new DoorLocation()
        {
          Position = new Vector2(_spriteInside.Rectangle.X + 64, _spriteInside.Rectangle.Bottom),
          IsValid = false,
        },
      };
    }

    public override void Work(object sender, EventArgs e)
    {
      var npc = sender as NPC;

      // TODO: Walk to work!

      // Stand by anvil
    }
  }
}
