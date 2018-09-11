using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Microsoft.Xna.Framework;

namespace VillageBackend.Graphics
{
  public abstract class Person : Sprite
  {
    protected Vector2 _velocity;

    protected List<Vector2> _originalWalkingPath = null;

    public List<Vector2> WalkingPath { get; protected set; } = new List<Vector2>();

    public bool HasFinishedWalking { get; protected set; }

    public override Rectangle GridRectangle
    {
      get
      {
        return Rectangle;
      }
    }

    public Person(Dictionary<string, Animation> animations) : base(animations)
    {
    }

    public void SetPath(List<Point> points)
    {
      HasFinishedWalking = false;

      if (WalkingPath.Count > 0)
        return;

      WalkingPath = points.Select(c => new Vector2(c.X * 32, c.Y * 32)).ToList();
    }

    protected abstract void Walk();

    public override void Update(GameTime gameTime)
    {
      Walk();

      Position += _velocity;

      SetAnimation();

      _animationManager.Update(gameTime);
    }

    protected void SetAnimation()
    {
      if (_velocity.X > 0)
      {
        _animationManager.Play(_animations["WalkRight"]);
      }
      else if (_velocity.X < 0)
      {
        _animationManager.Play(_animations["WalkLeft"]);
      }
      else if (_velocity.Y > 0)
      {
        _animationManager.Play(_animations["WalkDown"]);
      }
      else if (_velocity.Y < 0)
      {
        _animationManager.Play(_animations["WalkUp"]);
      }
      else
      {
        _animationManager.Stop();
      }
    }
  }
}
