﻿using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TopDown.Sprites.Fences
{
  public class HorizontalFence : Sprite
  {
    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        return new List<Rectangle>()
        {
          new Rectangle(Rectangle.X, Rectangle.Y + 11, Rectangle.Width, 10),
        };
      }
    }

    public HorizontalFence(Texture2D texture) : base(texture)
    {
    }
  }
}
