using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Microsoft.Xna.Framework;
using TopDown.States;

namespace TopDown.Sprites
{
  public class NPC : Sprite
  {
    private GameScreen _gameScreen;

    private List<Vector2> _walkingPath;

    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        return new List<Rectangle>()
        {
          new Rectangle(Rectangle.X + 4, Rectangle.Bottom - 24, 24, 24),
        };
      }
    }

    public override Vector2 Position { get => base.Position + new Vector2(0, 24); set => base.Position = value - new Vector2(0, 24); }

    public NPC(Dictionary<string, Animation> animations, GameScreen gameScreen) : base(animations)
    {
      _gameScreen = gameScreen;

      _walkingPath = new List<Vector2>();
    }

    protected override void SetAnimation()
    {
      if (Velocity.X > 0)
      {
        _animationManager.Play(_animations["WalkRight"]);
      }
      else if (Velocity.X < 0)
      {
        _animationManager.Play(_animations["WalkLeft"]);
      }
      else if (Velocity.Y > 0)
      {
        _animationManager.Play(_animations["WalkDown"]);
      }
      else if (Velocity.Y < 0)
      {
        _animationManager.Play(_animations["WalkUp"]);
      }
      else
      {
        _animationManager.Stop();
      }
    }

    public override void Update(GameTime gameTime)
    {
      if (_walkingPath.Count > 0)
      {
        if (_walkingPath[0] == Position)
          _walkingPath.RemoveAt(0);
      }

      if (_walkingPath.Count == 0)
      {
        Random random = new Random();

        var value = random.Next(0, _gameScreen.PathComponents.Count);

        _walkingPath = _gameScreen.PathFinder.FindPath(Position, _gameScreen.PathComponents[value].Position);
        _gameScreen.PathFinder.WriteMap();
      }

      var targetPosition = _walkingPath.Count > 0 ? _walkingPath.FirstOrDefault() : Position;

      if (Position.X < targetPosition.X)
        Velocity.X = 1;
      else if (Position.X > targetPosition.X)
        Velocity.X = -1;
      else if (Position.Y < targetPosition.Y)
        Velocity.Y = 1;
      else if (Position.Y > targetPosition.Y)
        Velocity.Y = -1;

      _animationManager.Update(gameTime);
    }
  }
}
