using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VillageBackend.Models;
using VillageGUI.Interface.Buttons;

namespace VillageGUI.Interface.Combat
{
  public class HeroPanel : Control
  {
    private Texture2D _texture;

    private Texture2D _defaultAvatar;

    private SpriteFont _font;

    private Squad _squad;

    private List<HeroButton> _heroButtons = new List<HeroButton>();

    private KeyboardState _previousKeyboardState;

    private KeyboardState _currentKeyboardState;

    public override Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
      }
    }

    public int SelectedHeroIndex { get; set; } = -1;

    public override Vector2 Position { get; set; }

    public float Layer { get; set; }

    public HeroPanel(Squad squad)
    {
      _squad = squad;
    }

    public void LoadContent(ContentManager content)
    {
      _texture = content.Load<Texture2D>("Interface/Combat/HeroPanel");

      _defaultAvatar = content.Load<Texture2D>("Avatars/Default");

      _font = content.Load<SpriteFont>("Fonts/Calibri_12pt");

      for (int i = 0; i < _squad.Villagers.Count; i++)
      {
        _heroButtons.Add(new HeroButton(_defaultAvatar, (Keys)Enum.Parse(typeof(Keys), "D" + (i + 1)))
        {
          Layer = this.Layer + 0.01f,
          Click = HeroButtonClicked,
          SelectedHeroIndex = i,
          Villager = _squad.Villagers[i],
        });
      }

      SetPositions();
    }

    public void SetPositions()
    {
      Position = new Vector2(10, GameEngine.ScreenHeight - 10 - Rectangle.Height);

      for (int i = 0; i < _squad.Villagers.Count; i++)
      {
        _heroButtons[i].Position = Position + new Vector2(4 + ((_defaultAvatar.Width + 2) * i), 20) + _heroButtons[i].Origin;
      }
    }

    private void HeroButtonClicked(Button button)
    {
      var heroButton = button as HeroButton;

      SelectedHeroIndex = heroButton.SelectedHeroIndex;
    }

    public override void UnloadContent()
    {
      _texture.Dispose();

      foreach (var button in _heroButtons)
        button.UnloadContent();
    }

    public void Update(GameTime gameTime)
    {
      _previousKeyboardState = _currentKeyboardState;
      _currentKeyboardState = Keyboard.GetState();

      var clicked = GameMouse.Clicked;

      foreach (var button in _heroButtons)
      {
        switch (button.CurrentState)
        {
          case ButtonStates.Nothing:

            if (GameMouse.Rectangle.Intersects(button.Rectangle))
            {
              GameMouse.AddObject(button);
              if (GameMouse.ValidObject == button)
              {
                button.CurrentState = ButtonStates.Hovering;
              }
              else
              {
                GameMouse.ClickableObjects.Remove(button);
              }
            }

            if (_previousKeyboardState != _currentKeyboardState &&
                _currentKeyboardState.IsKeyDown(button.OpenKey))
            {

              foreach (var b in _heroButtons)
                b.CurrentState = ButtonStates.Nothing;

              button.CurrentState = ButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ButtonStates.Hovering:

            if (!GameMouse.Rectangle.Intersects(button.Rectangle) || GameMouse.ValidObject != button)
            {
              GameMouse.ClickableObjects.Remove(button);
              button.CurrentState = ButtonStates.Nothing;
              break;
            }

            if (clicked ||
                (_previousKeyboardState != _currentKeyboardState &&
                 _currentKeyboardState.IsKeyDown(button.OpenKey)))
            {

              foreach (var b in _heroButtons)
              {
                GameMouse.ClickableObjects.Remove(b);
                b.CurrentState = ButtonStates.Nothing;
              }

              button.CurrentState = ButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ButtonStates.Clicked:

            if (GameMouse.Rectangle.Intersects(button.Rectangle))
            {
              GameMouse.AddObject(button);
            }
            else
            {
              GameMouse.ClickableObjects.Remove(button);
            }

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);

      spriteBatch.DrawString(_font, _squad.Name, Position + new Vector2(4, 4), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

      spriteBatch.DrawString(_font, "Abilities", Position + new Vector2(300, 20), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

      foreach (var button in _heroButtons)
      {
        button.Draw(gameTime, spriteBatch);
        spriteBatch.DrawString(_font, button.Villager.Turns.ToString(), button.Position, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, button.Layer + 0.01f);
      }
    }
  }
}
