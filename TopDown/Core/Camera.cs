using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDown.Core
{
  public class Camera
  {
    public Vector2 Position { get; private set; }

    public Matrix Transform { get; private set; }

    public void Follow(Vector2 target)
    {
      Position = target;

      Transform = Matrix.CreateTranslation(-Position.X + (GameEngine.ScreenWidth / 2), -Position.Y + (GameEngine.ScreenHeight / 2), 0);
    }
  }
}
