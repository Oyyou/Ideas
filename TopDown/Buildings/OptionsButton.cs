using Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TopDown.States;

namespace TopDown.Buildings
{
  public class OptionsButton : Button
  {
    public OptionsButton(Texture2D texture, SpriteFont font) : base(texture, font)
    {
    }

    public override void Update(GameTime gameTime)
    {
      if (!IsEnabled)
        return;

      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

      IsHovering = false;
      IsClicked = false;

      if (GameScreen.Mouse.RectangleWithCamera.Intersects(Rectangle))
      {
        IsHovering = true;

        if (GameScreen.Mouse.LeftClicked)
        {
          IsClicked = true;
          OnClick();
        }
      }

      foreach (var component in Components)
        component.Update(gameTime);
    }
  }
}
