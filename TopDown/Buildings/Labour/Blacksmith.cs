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

    public Blacksmith(GameScreen gameState, Texture2D textureInside, Texture2D textureOutside) : base(gameState, textureInside, textureOutside)
    {
      Name = "Blacksmith";
    }

    private void CraftItem(NPC npc, GameTime gameTime)
    {
      npc.CraftingItem.CraftingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (npc.CraftingItem.CraftingTime >= npc.CraftingItem.CraftTime)
      {
        npc.Skills.Blacksmith.Experience += npc.CraftingItem.ExperienceValue;

        _gameScreen.InventoryItems.Add(npc.CraftingItem);
        npc.CraftingItems.Remove(npc.CraftingItem);
        npc.CraftingItem = null;
      }
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      var inspectButton = new Engine.Controls.Button(content.Load<Texture2D>("Controls/Button"), content.Load<SpriteFont>("Fonts/Font"))
      {
        Text = "Inspect",
      };

      var demolishButton = new Engine.Controls.Button(content.Load<Texture2D>("Controls/Button"), content.Load<SpriteFont>("Fonts/Font"))
      {
        Text = "Demolish",
      };

      _buttons = new List<Engine.Controls.Button>()
      {
        demolishButton,
        inspectButton ,
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
        if (npc.CraftingItem != null)
        {
          CraftItem(npc, gameTime);
        }
        else
        {
          if (npc.CraftingItems.Count > 0)
          {
            // Assign the first item in the queue to the NPC
            npc.CraftingItem = npc.CraftingItems.First();
          }

          if (npc.CraftingItem != null)
          {
            CraftItem(npc, gameTime);
          }
        }
      }
    }
  }
}
