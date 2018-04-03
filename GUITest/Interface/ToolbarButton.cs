using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITest.Interface
{
  public enum ToolbarButtonStates
  {
    Nothing,
    Hovering,
    Clicked,
  }

  public class ToolbarButton
  {
    public event EventHandler Click;

    public Color Color;

    public ToolbarButtonStates CurrentState;

    public readonly Vector2 Origin;

    public Vector2 Position;

    public ToolbarButtonStates PreviousState;

    public Rectangle Rectangle
    {
      get
      {
        var width = Texture.Width * Scale;
        var height = Texture.Height * Scale;

        return new Rectangle((int)(Position.X - (width / 2)), (int)(Position.Y - (height / 2)), (int)width, (int)height);
      }
    }

    public float Scale;

    public readonly Texture2D Texture;

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position, null, Color, 0f, Origin, Scale, SpriteEffects.None, 0.1f);
    }

    public void OnClick()
    {
      Click?.Invoke(this, new EventArgs());
    }

    public ToolbarButton(Texture2D texture)
    {
      Texture = texture;

      Color = Color.White;

      Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

      Scale = 1f;

      CurrentState = ToolbarButtonStates.Nothing;
    }

    public void UnloadContent()
    {
      Texture.Dispose();
    }
  }
}
