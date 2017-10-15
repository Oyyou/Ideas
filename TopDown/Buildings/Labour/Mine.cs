using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Microsoft.Xna.Framework;
using TopDown.States;
using Engine;
using TopDown.Buildings;
using Microsoft.Xna.Framework.Graphics;
using TopDown.Sprites;
using Microsoft.Xna.Framework.Content;

namespace TopDown.Buildings.Labour
{
  public class Mine : Building
  {
    private const float _maxWorkTimer = 4f;

    private float _workTimer;

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
              new Rectangle(x + 32, y + 32, width - 64, 1), // Top
              new Rectangle(x + 32, y + 32, 1, height - 64), // Left
              new Rectangle(x + (width - 1) - 32, y + 32, 1, height - 64), // Right
              new Rectangle(x + 32, y + (height - 1) - 32, 32, 1), // bottom left
              new Rectangle(x + 96, y + (height - 1) - 32, 32, 1), // bottom right
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
        return new List<Wall>();
      }
    }

    protected override bool CanPlace()
    {
      var canPlace = base.CanPlace();

      if (!canPlace)
        return false;

      // Check to see if the mine is on minerals
      var validMineralPositions = new List<Vector2>()
      {
        new Vector2(Position.X + 32, Position.Y + 32),
        new Vector2(Position.X + 64, Position.Y + 32),
        new Vector2(Position.X + 32, Position.Y + 64),
        new Vector2(Position.X + 64, Position.Y + 64),
      };



      return true;
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _buttons = new List<OptionsButton>()
      {
        _demolishButton,
        _fireButton,
        _hireButton,
      };

      foreach (var button in _buttons)
        button.LoadContent(content);
    }

    public Mine(GameScreen gameState, Texture2D textureInside, Texture2D textureOutsideTop, Texture2D textureOutsideBottom) : base(gameState, textureInside, textureOutsideTop, textureOutsideBottom)
    {
      Name = "Mine";
    }

    protected override void SetDoorLocations()
    {
      DoorLocations = new List<DoorLocation>();

      for (int y = -32; y <= Rectangle.Height; y += 32)
      {
        for (int x = -32; x <= Rectangle.Width; x += 32)
        {
          var position = new Vector2((Rectangle.X + x), (Rectangle.Y + y));

          if (x == -32 || y == -32 ||
            x == Rectangle.Width || y == Rectangle.Height)
          {
            if ((x == -32 && y == -32) ||
              (x == -32 && y == Rectangle.Height) ||
              (x == Rectangle.Width && y == -32) ||
              (x == Rectangle.Width && y == Rectangle.Height))
            {
              continue;
            }

            DoorLocations.Add(new DoorLocation()
            {
              Position = position,
              IsValid = false,
            });
          }
        }
      }
    }

    public override void Work(NPC npc, GameTime gameTime)
    {
      var position = new Vector2(Rectangle.X + 64, Rectangle.Y + 64);

      if (npc.Position != position)
      {
        npc.WalkTo(position);
        if (position == npc.Position)
        {
          // Resets the timer when they finally get to work
          _workTimer = 0f;
        }
      }
      else
      {
        npc.IsVisible = false; // Since they'll by "in the mine", they need to invisible.

        _workTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * GameScreen.GameSpeed;

        // _maxWorkTimer will be multiplied by the skill rating of the npc
        if (_workTimer > _maxWorkTimer)
        {
          _gameScreen.Resources.Stone++;
          _workTimer = 0f;
        }
      }
    }
  }
}