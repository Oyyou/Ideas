using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace TopDown.Sprites
{
  public class Path : Sprite
  {
    public Path(Texture2D texture) : base(texture)
    {
      IsCollidable = false;
    }
  }
}
