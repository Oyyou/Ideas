using Engine;
using Engine.States;
using VillageGUI.Interface.Buttons;
using VillageGUI.Interface.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Managers;
using VillageBackend.Models;

namespace VillageGUI.Interface
{
  public class Toolbar
  {
    private List<ToolbarButton> _buttons;

    private ContentManager _content;

    private KeyboardState _currentKeyboardState;

    private MouseState _currentMouseState;

    private KeyboardState _previousKeyboardState;

    private MouseState _previousMouseState;
    
    private State _state;

    public Toolbar(State game, ContentManager content)
    {
      _state = game;

      _content = content;

      _buttons = new List<ToolbarButton>()
      {
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Build"), Keys.B)
        {
          Click = Build_Click
        },
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map"), Keys.M),
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad"), Keys.S)
        {
          Click = Squad_Click,
        },
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Inventory"), Keys.I)
        {
          Click = Inventory_Click,
        },
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Crafting"), Keys.C)
        {
          Click = Crafting_Click,
        },
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Jobs"), Keys.J)
        {
          Click = Jobs_Click,
        },
      };

      SetButtonPositions();
    }

    private void Build_Click(Button obj)
    {
      _state.OpenWindow("Build");
    }

    private void Inventory_Click(Button obj)
    {
      _state.OpenWindow("Inventory");
    }

    public void OnScreenResize()
    {
      SetButtonPositions();
    }

    public void SetButtonPositions()
    {
      var screenWidth = Game1.ScreenWidth;
      var screenHeight = Game1.ScreenHeight;

      var spaceBetween = 10;

      var buttonWidth = _buttons.FirstOrDefault().Texture.Width;
      var buttonHeight = _buttons.FirstOrDefault().Texture.Height;

      var x = (screenWidth / 2) - (_buttons.Sum(c => buttonWidth + spaceBetween) / 2) + (buttonWidth / 2);
      var y = screenHeight - (buttonHeight);

      foreach (var button in _buttons)
      {
        button.Position = new Vector2(x, y);
        x += button.Texture.Width + spaceBetween;
      }
    }

    public void Update(GameTime gameTime)
    {
      _previousKeyboardState = _currentKeyboardState;
      _currentKeyboardState = Keyboard.GetState();
	    
      _previousMouseState = _currentMouseState;
      _currentMouseState = Mouse.GetState();

      var mouseRectangle = new Rectangle((int)_currentMouseState.Position.X, (int)_currentMouseState.Position.Y, 1, 1);

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

      foreach (var button in _buttons)
      {	      
        switch (button.CurrentState)
        {
          case ButtonStates.Nothing:

            if (mouseRectangle.Intersects(button.Rectangle))
              button.CurrentState = ButtonStates.Hovering;

            if (_previousKeyboardState != _currentKeyboardState &&
                _currentKeyboardState.IsKeyDown(button.OpenKey))
            {
              _state.CloseWindow();

              foreach (var b in _buttons)
                b.CurrentState = ButtonStates.Nothing;

              button.CurrentState = ButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ButtonStates.Hovering:

            if (!mouseRectangle.Intersects(button.Rectangle))
              button.CurrentState = ButtonStates.Nothing;

            if (clicked ||
                (_previousKeyboardState != _currentKeyboardState &&
                 _currentKeyboardState.IsKeyDown(button.OpenKey)))
            {
              _state.CloseWindow();

              foreach (var b in _buttons)
                b.CurrentState = ButtonStates.Nothing;

              button.CurrentState = ButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ButtonStates.Clicked:

            if (_previousKeyboardState != _currentKeyboardState &&
                 _currentKeyboardState.IsKeyDown(button.OpenKey))
            {
              _state.CloseWindow();

              foreach (var b in _buttons)
                b.CurrentState = ButtonStates.Nothing;

              break;
            }

            if (clicked)
            {
              Console.WriteLine("Clicked");

              if (!mouseRectangle.Intersects(GameEngine.ScreenRectangle))
              {
                Console.WriteLine("Not on screen");
                continue;
              }

              if (mouseRectangle.Intersects(button.Rectangle))
                Console.WriteLine("Is over button");

              if (mouseRectangle.Intersects(button.Rectangle) || // If we're clicking a button that is already clicked..
                 (!_buttons.Any(c => c.Rectangle.Intersects(mouseRectangle)) && // Or clicking something that isn't a button, or an open window
                  !mouseRectangle.Intersects(_state.WindowRectangle)))
              {
                Console.WriteLine("Closing Window");
                _state.CloseWindow();

                foreach (var b in _buttons)
                  b.CurrentState = ButtonStates.Nothing;

                // Set the clicked button to "Hover" because that's where the mouse'll be
                button.CurrentState = ButtonStates.Hovering;
              }
            }

            if (clicked && !_buttons.Any(c => c.Rectangle.Intersects(mouseRectangle)) && !_state.IsWindowOpen) // Check if we're clicking somewhere that isn't on any button
            {
              foreach (var b in _buttons)
                b.CurrentState = ButtonStates.Nothing;

              button.CurrentState = ButtonStates.Hovering;
            }

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }

    private void Crafting_Click(object sender)
    {
      _state.OpenWindow("Crafting");
    }

    private void Jobs_Click(object sender)
    {
      _state.OpenWindow("Jobs");
    }

    private void Squad_Click(object sender)
    {
      _state.OpenWindow("Squads");
    }

    public void UnloadContent()
    {
      foreach (var button in _buttons)
        button.UnloadContent();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      foreach (var button in _buttons)
      {
        switch (button.CurrentState)
        {
          case ButtonStates.Nothing:

            button.Color = Color.White;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Hovering:

            button.Color = Color.YellowGreen;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Clicked:

            button.Color = Color.YellowGreen;

            button.Scale = 1.2f;

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }

        button.Draw(gameTime, spriteBatch);
      }
    }
  }
}
