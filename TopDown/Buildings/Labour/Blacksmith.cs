using Engine.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Sprites;
using TopDown.States;
using VillageBackend.Models;

namespace TopDown.Buildings.Labour
{
  public class Blacksmith : Building
  {
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
          case BuildingStates.Constructing:
            CollisionRectangles = new List<Rectangle>()
            {
              _spriteInside.Rectangle,
            };
            break;
          case BuildingStates.Built:

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

            if (Components != null)
              collisionRectangles.AddRange(Components.SelectMany(c => c.CollisionRectangles).ToList());

            CollisionRectangles = collisionRectangles;

            break;
        }
      }
    }

    protected override List<Wall> Walls
    {
      get
      {
        return new List<Wall>()
        {
          new Wall()
          {
            Direction = Wall.Directions.Down,
            Position = new Vector2(1, 2),
          },
          new Wall()
          {
            Direction = Wall.Directions.Down,
            Position = new Vector2(2, 2),
          },
          new Wall()
          {
            Direction = Wall.Directions.Left,
            Position = new Vector2(3, 3),
          },
          new Wall()
          {
            Direction = Wall.Directions.Left,
            Position = new Vector2(3, 4),
          },
          new Wall()
          {
            Direction = Wall.Directions.Up,
            Position = new Vector2(1, 3),
          },
          new Wall()
          {
            Direction = Wall.Directions.Up,
            Position = new Vector2(2, 3),
          },
          new Wall()
          {
            Direction = Wall.Directions.Right,
            Position = new Vector2(2, 3),
          },
          new Wall()
          {
            Direction = Wall.Directions.Right,
            Position = new Vector2(2, 4),
          },
        };
      }
    }

    public Blacksmith(GameScreen gameState, Texture2D textureInside, Texture2D textureOutsideTop, Texture2D textureOutsideBottom) : base(gameState, textureInside, textureOutsideTop, textureOutsideBottom)
    {
      Name = "Blacksmith";

      Job = new Job()
      {
        Name = Name,
      };
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _buttons = new List<OptionsButton>()
      {
        _demolishButton,
        _inspectButton,
        _fireButton,
        _hireButton,
      };

      foreach (var button in _buttons)
        button.LoadContent(content);
    }

    protected override void SetDoorLocations()
    {
      DoorLocations = new List<DoorLocation>()
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

    public override void Work(NPC npc, GameTime gameTime)
    {
      var anvil = Components.FirstOrDefault();

      var t = new Vector2(Position.X + 64, Position.Y + 64);
      var t2 = new Vector2(Rectangle.X + 64, Rectangle.Y + 64);

      var workPosition = anvil != null ? anvil.Position - new Vector2(32, 0) : new Vector2(Rectangle.X + 64, Rectangle.Y + 64);

      if (npc.Position != workPosition)
      {
        npc.WalkTo(workPosition);
      }
      else
      {
        npc.Villager.IsAtWork = true;
      }
    }
  }
}
