using Engine;
using Engine.States;
using GUITest.Interface.Windows;
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

namespace GUITest.Interface
{
  public class Toolbar
  {
    private List<ToolbarButton> _buttons;

    private ContentManager _content;

    private KeyboardState _currentKeyboardState;

    private MouseState _currentMouseState;

    private ItemManager _itemManager;

    private KeyboardState _previousKeyboardState;

    private MouseState _previousMouseState;
    
    private State _state;

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
          case ToolbarButtonStates.Nothing:

            if (mouseRectangle.Intersects(button.Rectangle))
              button.CurrentState = ToolbarButtonStates.Hovering;

            if (_previousKeyboardState != _currentKeyboardState &&
                _currentKeyboardState.IsKeyDown(button.OpenKey))
            {
              _state.CloseWindow();

              foreach (var b in _buttons)
                b.CurrentState = ToolbarButtonStates.Nothing;

              button.CurrentState = ToolbarButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ToolbarButtonStates.Hovering:

            if (!mouseRectangle.Intersects(button.Rectangle))
              button.CurrentState = ToolbarButtonStates.Nothing;

            if (clicked ||
                (_previousKeyboardState != _currentKeyboardState &&
                 _currentKeyboardState.IsKeyDown(button.OpenKey)))
            {
              _state.CloseWindow();

              foreach (var b in _buttons)
                b.CurrentState = ToolbarButtonStates.Nothing;

              button.CurrentState = ToolbarButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ToolbarButtonStates.Clicked:

            if (_previousKeyboardState != _currentKeyboardState &&
                 _currentKeyboardState.IsKeyDown(button.OpenKey))
            {
              _state.CloseWindow();

              foreach (var b in _buttons)
                b.CurrentState = ToolbarButtonStates.Nothing;

              break;
            }

            if (clicked)
            {
              Console.WriteLine("Clicked");

              if (!mouseRectangle.Intersects(GameEngine.ScreenRectangle))
              {
                Console.WriteLine("Not in screen");
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
                  b.CurrentState = ToolbarButtonStates.Nothing;

                // Set the clicked button to "Hover" because that's where the mouse'll be
                button.CurrentState = ToolbarButtonStates.Hovering;
              }
            }

            if (clicked && !_buttons.Any(c => c.Rectangle.Intersects(mouseRectangle)) && !_state.IsWindowOpen) // Check if we're clicking somewhere that isn't on any button
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

    public Toolbar(State game, ItemManager itemManager, ContentManager content)
    {	    
      _state = game;
			
	    _itemManager = itemManager;
			
      _content = content;

      var squad = new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad"), Keys.S);
      squad.Click += Squad_Click;

      var crafting = new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Crafting"), Keys.C);
      crafting.Click += Crafting_Click;

      _buttons = new List<ToolbarButton>()
      {
        new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map"), Keys.M),
        squad,
        crafting,
        //new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Map")),
        //new ToolbarButton(content.Load<Texture2D>("Interface/ToolbarIcons/Squad")),
      };

      SetButtonPositions();
    }

    private void Crafting_Click(object sender, EventArgs e)
    {
      _state.OpenWindow(new CraftingWindow(_content, _itemManager));
    }

    private void Squad_Click(object sender, EventArgs e)
    {
      //_game.OpenWindow(new Window(_content));
    }

    public void UnloadContent()
    {
      foreach (var button in _buttons)
        button.UnloadContent();
    }
  }
}
