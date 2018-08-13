using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageGUI.Interface
{
  public abstract class Control
  {
    public abstract Rectangle Rectangle { get; }

    public abstract Vector2 Position { get; set; }

    public abstract void UnloadContent();

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
  }
}
