using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITest.Interface
{
  public class Window
  {
    private List<ToolbarButton> _buttons;

    private SpriteFont _font;

    public string Name { get; protected set; }

    public Vector2 Position { get; protected set; }

    public readonly Texture2D Texture;

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

      spriteBatch.DrawString(_font, "Window", new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);

      foreach (var button in _buttons)
        button.Draw(gameTime, spriteBatch);
    }

    public void OnScreenResize()
    {
      SetPosition();

      SetButtonPositions();
    }

    public void SetButtonPositions()
    {
      var screenWidth = Game1.ScreenWidth;

      var spaceBetween = 10;

      var buttonHeight = _buttons.FirstOrDefault().Texture.Height;

      var buttonWidth = _buttons.FirstOrDefault().Texture.Width;

      var x = Position.X + 10 + (buttonWidth / 2);

      var y = Position.Y  + 60;

      foreach (var button in _buttons)
      {
        button.Position = new Vector2(x, y);
        x += button.Texture.Width + spaceBetween;

        if (x >= Position.X + (Texture.Width))
        {
          x = Position.X + 10 + (buttonWidth / 2);
          y += buttonHeight + spaceBetween;
        }
      }
    }

    public Window(ContentManager content)
    {
      _font = content.Load<SpriteFont>("Fonts/Font");

      Texture = content.Load<Texture2D>("Interface/Window");

      SetPosition();

      _buttons = new List<ToolbarButton>()
      {
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
      };

      SetButtonPositions();
    }

    private void SetPosition()
    {
      Position = new Vector2(0, 0);// Game1.ScreenWidth / 2, (Texture.Height / 2) + 20);
    }
  }
}
