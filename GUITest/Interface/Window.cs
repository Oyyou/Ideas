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

    private Matrix _cameraMatrix;

    private MouseState _currentMouseState;

    private SpriteFont _font;

    private bool _hasUpdated;

    private MouseState _previousMouseState;

    private Scrollbar _scrollbar;

    public Vector2 CameraPosition { get; protected set; }

    public string Name { get; protected set; }

    public Vector2 Position { get; protected set; }

    public readonly Texture2D Texture;

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      if (!_hasUpdated)
        return;

      var original = graphics.GraphicsDevice.Viewport;

      DrawWindow(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport((int)Position.X, (int)Position.Y + 35, Texture.Width, Texture.Height - 35);

      DrawButtons(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = original;
    }

    private void DrawButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _cameraMatrix);

      foreach (var button in _buttons)
      {
        switch (button.CurrentState)
        {
          case ToolbarButtonStates.Nothing:

            button.Color = Color.White;

            button.Scale = 1.0f;

            break;
          case ToolbarButtonStates.Hovering:

            button.Color = Color.YellowGreen;

            button.Scale = 1.0f;

            break;
          case ToolbarButtonStates.Clicked:

            // This will be removed

            button.Color = Color.YellowGreen;

            button.Scale = 1.2f;

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }

        button.Draw(gameTime, spriteBatch);
      }

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
      var screenWidth = Game1.ScreenWidth;
      var screenHeigh = Game1.ScreenHeight;

      var spaceBetween = 10;

      var buttonHeight = _buttons.FirstOrDefault().Texture.Height;
      var buttonWidth = _buttons.FirstOrDefault().Texture.Width;

      Position = new Vector2((Game1.ScreenWidth / 2) - (Texture.Width / 2), screenHeigh - Texture.Height - (buttonHeight * 2));

      CameraPosition = new Vector2(0, Position.Y);

      _scrollbar.Position = new Vector2((Position.X + Texture.Width) - 20 - 10, Position.Y + 35);

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
      _hasUpdated = true;

      CameraPosition = new Vector2(CameraPosition.X, MathHelper.Clamp(-_scrollbar._innerY + 60, -1000, 2400));

      _scrollbar.Update(gameTime);
      _previousMouseState = _currentMouseState;
      _currentMouseState = Mouse.GetState();

      _cameraMatrix = Matrix.CreateTranslation(CameraPosition.X, CameraPosition.Y, 0);

      var translation = _cameraMatrix.Translation;

      var mouseRectangle = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);

      var mouseRectangleWithCamera = new Rectangle(
        (int)((_currentMouseState.X - Position.X) - translation.X),
        (int)((_currentMouseState.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (var button in _buttons)
      {
        switch (button.CurrentState)
        {
          case ToolbarButtonStates.Nothing:

            if (mouseRectangleWithCamera.Intersects(button.Rectangle) && mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ToolbarButtonStates.Hovering;

            break;
          case ToolbarButtonStates.Hovering:

            if (!mouseRectangleWithCamera.Intersects(button.Rectangle) || !mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ToolbarButtonStates.Nothing;

            if (clicked)
            {
              foreach (var b in _buttons)
                b.CurrentState = ToolbarButtonStates.Nothing;

              button.CurrentState = ToolbarButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ToolbarButtonStates.Clicked:

            // Close the window, and start to place the building!

            //var mouseRectangle = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);
            //var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            if (clicked && (mouseRectangleWithCamera.Intersects(windowRectangle)) && !_buttons.Any(c => c != button && c.Rectangle.Intersects(mouseRectangleWithCamera))) // Check if we're clicking somewhere that isn't on any button
            {
              foreach (var b in _buttons)
                b.CurrentState = ToolbarButtonStates.Nothing;

              button.CurrentState = ToolbarButtonStates.Hovering;
            }

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }

    public Window(ContentManager content)
    {
      _font = content.Load<SpriteFont>("Fonts/Font");

      _hasUpdated = false;

      Texture = content.Load<Texture2D>("Interface/Window");

      _scrollbar = new Scrollbar(content);

      _buttons = new List<ToolbarButton>()
      {
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
      };

      SetPositions();
    }
  }
}
