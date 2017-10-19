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
  public class Toolbar
  {
    private List<ToolbarButton> _buttons;

    private MouseState _currentMouseState;

    private MouseState _previousMouseState;

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
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

            button.Color = Color.YellowGreen;

            button.Scale = 1.2f;

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }

        button.Draw(gameTime, spriteBatch);
      }
    }

    public void OnScreenResize()
    {
      SetButtonPositions();
    }

    public void SetButtonPositions()
    {
      var screenWidth = 800; // Todo: this

      var spaceBetween = 10;

      var buttonWidth = _buttons.FirstOrDefault().Texture.Width;

      var x = (screenWidth / 2) - (_buttons.Sum(c => buttonWidth + spaceBetween) / 2) + (buttonWidth/ 2);

      foreach (var button in _buttons)
      {
        button.Position = new Vector2(x, 420);
        x += button.Texture.Width + spaceBetween;
      }
    }

    public void Update(GameTime gameTime)
    {
      _previousMouseState = _currentMouseState;
      _currentMouseState = Mouse.GetState();

      var mouseRectangle = new Rectangle((int)_currentMouseState.Position.X, (int)_currentMouseState.Position.Y, 1, 1);

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

      foreach (var button in _buttons)
      {
        switch (button.CurrentState)
        {
          case ToolbarButtonStates.Nothing:

            if (mouseRectangle.Intersects(button.Rectangle))
              button.CurrentState = ToolbarButtonStates.Hovering;

            break;
          case ToolbarButtonStates.Hovering:

            if (!mouseRectangle.Intersects(button.Rectangle))
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

            if (clicked && !_buttons.Any(c => c != button && c.Rectangle.Intersects(mouseRectangle))) // Check if we're clicking somewhere that isn't on any button
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

    public Toolbar(ContentManager content)
    {
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
  }
}
