using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VillageBackend.Graphics
{
  public class Enemy : Person
  {
    public List<Vector2> PatrolPaths;

    public Enemy(Dictionary<string, Animation> animations) : base(animations)
    {

    }

    public override void Update(GameTime gameTime)
    {
      Walk();

      Position += _velocity;

      SetAnimation();

      _animationManager.Update(gameTime);
    }

    protected override void Walk()
    {
      if (WalkingPath == null)
        return;

      if (WalkingPath.Count == 0)
        return;

      if (_originalWalkingPath == null)
        _originalWalkingPath = new List<Vector2>(WalkingPath);

      if (WalkingPath.FirstOrDefault() == Position)
        WalkingPath.RemoveAt(0);

      if (WalkingPath.Count == 0)
      {
        HasFinishedWalking = true;
        _originalWalkingPath = null;
      }

      var targetPosition = WalkingPath.Count > 0 ? WalkingPath.FirstOrDefault() : Position;

      var speed = 1f;

      _velocity = new Vector2();

      if (Position.X < targetPosition.X)
        _velocity.X = speed;
      else if (Position.X > targetPosition.X)
        _velocity.X = -speed;
      else if (Position.Y < targetPosition.Y)
        _velocity.Y = speed;
      else if (Position.Y > targetPosition.Y)
        _velocity.Y = -speed;
    }
  }
}
