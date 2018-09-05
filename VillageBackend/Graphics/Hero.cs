﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VillageBackend.Models;

namespace VillageBackend.Graphics
{
  public class Hero : Sprite
  {
    /// <summary>
    /// Set this to "WalkingPath" when we first set it
    /// </summary>
    private List<Vector2> _originalWalkingPath = null;

    public List<Vector2> WalkingPath { get; private set; } = new List<Vector2>();

    private Vector2 _velocity;

    //public int Turns { get; set; } = 2;

    public Villager Villager { get; set; }

    public bool HasFinishedWalking { get; private set; }

    public override Rectangle GridRectangle
    {
      get
      {
        return Rectangle;
      }
    }

    public Hero(Texture2D texture) : base(texture)
    {
    }

    public void SetPath(List<Point> points)
    {
      HasFinishedWalking = false;

      if (WalkingPath.Count > 0)
        return;

      WalkingPath = points.Select(c => new Vector2(c.X * 32, c.Y * 32)).ToList();
    }

    public override void Update(GameTime gameTime)
    {
      Walk();

      Position += _velocity;
    }

    private void Walk()
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
        var distance = _originalWalkingPath.Count;
        var usedTurns = (int)Math.Ceiling((float)distance / (float)Villager.Speed);

        Console.WriteLine(usedTurns);

        Villager.Turns -= usedTurns;
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