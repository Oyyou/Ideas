using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

    private Scrollbar _scrollbar;

    public Vector2 CameraPosition { get; protected set; }

    public string Name { get; protected set; }

    public Vector2 Position { get; protected set; }

    public readonly Texture2D Texture;

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      var original = graphics.GraphicsDevice.Viewport;

      DrawWindow(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport((int)Position.X, (int)Position.Y + 35, Texture.Width, Texture.Height);

      DrawButtons(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = original;
    }

    private void DrawButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: Matrix.CreateTranslation(CameraPosition.X, CameraPosition.Y, 0));

      foreach (var button in _buttons)
        button.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }

    private void DrawWindow(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

      _scrollbar.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, "Window", new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);

      spriteBatch.End();
    }

    public void OnScreenResize()
    {
      SetPositions();
    }

    public void SetPositions()
    {
      CameraPosition = new Vector2(0, CameraPosition.Y);

      Position = new Vector2((Game1.ScreenWidth / 2) - (Texture.Width / 2), 20);

      _scrollbar.Position = new Vector2((Position.X + Texture.Width) - 20 - 10, Position.Y + 35);

      var screenWidth = Game1.ScreenWidth;

      var spaceBetween = 10;

      var buttonHeight = _buttons.FirstOrDefault().Texture.Height;

      var buttonWidth = _buttons.FirstOrDefault().Texture.Width;

      var x = CameraPosition.X + 10 + (buttonWidth / 2);

      var y = CameraPosition.Y + 25;

      foreach (var button in _buttons)
      {
        button.Position = new Vector2(x, y);
        x += button.Texture.Width + spaceBetween;

        if (x >= (CameraPosition.X + (Texture.Width) - 40))
        {
          x = CameraPosition.X + 10 + (buttonWidth / 2);
          y += buttonHeight + spaceBetween;
        }
      }
    }

    public void Update(GameTime gameTime)
    {
      CameraPosition = new Vector2(CameraPosition.X, MathHelper.Clamp(-_scrollbar._innerY + 60, -1000, 2400));

      var height = (int)(_buttons.Last().Position.Y - _buttons.FirstOrDefault().Position.Y);

      _scrollbar.Update(gameTime, height);
    }

    public Window(ContentManager content)
    {
      _font = content.Load<SpriteFont>("Fonts/Font");

      Texture = content.Load<Texture2D>("Interface/Window");

      _scrollbar = new Scrollbar(content);

      _buttons = new List<ToolbarButton>()
      {
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
      };

      SetPositions();
    }
  }
}
