﻿using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Engine;

namespace TopDown.FX
{
  public class RockParticle : Sprite
  {
    public RockParticle(Texture2D texture) : base(texture)
    {
      var xSpeed = (float)GameEngine.Random.NextDouble() * (2 - -2) + -2;
      var ySpeed = (float)GameEngine.Random.NextDouble() * (-1 - -2) + -2;

      Velocity = new Vector2(xSpeed, ySpeed);
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      Position += Velocity;

      Velocity.Y += 0.3f;
    }
  }
}
