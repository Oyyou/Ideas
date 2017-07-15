using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Buildings.Templates;
using TopDown.States;

namespace TopDown.Buildings
{
  public class Path : Building
  {
    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        return new List<Rectangle>();
      }
    }

    public override Sprite CurrentSprite
    {
      get
      {
        switch (BuildingState)
        {
          case BuildingStates.Placing:
          case BuildingStates.Building:
          case BuildingStates.Placed:
          case BuildingStates.Built_In:
          case BuildingStates.Built_Out:
            return _builtSprite;

          default:
            throw new Exception("Unknown state: " + BuildingState);
        }
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      CurrentSprite.Draw(gameTime, spriteBatch);
    }

    public Path(GameScreen gameState, BuildingTemplate template) : base(gameState, template)
    {
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _builtSprite = new Sprite(content.Load<Texture2D>("Sprites/Paths/StonePath"))
      {
        Layer = DefaultLayer,
      };

      _placedSprite = null;
    }

    public override void Update(GameTime gameTime)
    {
      switch (BuildingState)
      {
        case BuildingStates.Placing:

          Position = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

          bool canPlace = true;

          CurrentSprite.Color = Color.White;

          foreach (var component in _gameState.CollidableComponents)
          {
            if (component.CollisionRectangles.Any(c => c.Intersects(this.Rectangle)))
            {
              canPlace = false;
              CurrentSprite.Color = Color.Red;
              break;
            }
          }

          if (GameScreen.Mouse.LeftClicked && canPlace)
          {
            BuildingState = BuildingStates.Built_In;
            _gameState.State = States.States.Playing;
          }

          break;
        case BuildingStates.Placed:
          throw new Exception("Shouldn't be able to 'placed' persé");

        case BuildingStates.Building:
          throw new Exception("Shouldn't be able to 'build'");


        case BuildingStates.Built_In:
        case BuildingStates.Built_Out:
          
          CurrentSprite.Layer = Building.DefaultLayer;

          break;

        default:
          break;
      }
    }
  }
}
