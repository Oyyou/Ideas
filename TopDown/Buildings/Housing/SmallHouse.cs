using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Furnitures;
using TopDown.States;

namespace TopDown.Buildings.Housing
{
  public class SmallHouse : Building
  {
    private Texture2D _bedTexture;

    public const int MaxResidents = 2;

    public int ResidentCount { get; set; }

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
              new Rectangle(x, y, 1, height), // Left
              new Rectangle(x + (width - 1), y, 1, height), // Right
              new Rectangle(x, y + height - 1, 32, 1), // bottom left
              new Rectangle(x + 64, y + height - 1, width - 64, 1), // bottom right
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

    public SmallHouse(GameScreen gameState, Texture2D textureInside, Texture2D textureOutsideTop, Texture2D textureOutsideBottom) : base(gameState, textureInside, textureOutsideTop, textureOutsideBottom)
    {
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _bedTexture = content.Load<Texture2D>("Furniture/Bed");
    }

    protected override void SetDoorLocations()
    {
      DoorLocations = new List<DoorLocation>()
      {
        new DoorLocation()
        {
          Position = new Vector2(_spriteInside.Rectangle.X + 32, _spriteInside.Rectangle.Bottom),
          IsValid = false,
        },
      };
    }

    public override void OnBuilt()
    {
      Components.Add(new Bed(_bedTexture, _gameScreen)
      {
        State = PlacableObjectStates.Placed,
        Position = Position + new Vector2(64, 32),
      });
    }
  }
}
