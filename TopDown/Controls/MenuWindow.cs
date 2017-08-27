using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDown.States;
using Engine.Sprites;
using Engine.Controls;

namespace TopDown.Controls
{
  public class MenuWindow : Component
  {
    protected Button _closeButton;

    protected ContentManager _content;

    protected SpriteFont _font;

    protected Vector2 _fontPosition;

    protected GameScreen _gameScreen;

    protected Sprite _windowSprite;

    public override void CheckCollision(Component component)
    {

    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      OnClose();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      throw new NotImplementedException();
    }

    public override void LoadContent(ContentManager content)
    {
      _content = content;

      var backgroundTexture = content.Load<Texture2D>("Controls/BuildMenu");

      _windowSprite = new Sprite(backgroundTexture)
      {
        Layer = 0.98f,
        Position = Position,
      };

      _font = content.Load<SpriteFont>("Fonts/Font");

      var closeButtonTexture = content.Load<Texture2D>("Controls/Close");

      _closeButton = new Button(closeButtonTexture)
      {
        Layer = 0.99f
      };

      _closeButton.Click += CloseButton_Click;

      Components = new List<Component>()
      {
        _windowSprite,
        _closeButton,
      };

      foreach (var component in Components)
        component.LoadContent(content);
    }

    public MenuWindow(GameScreen gameScreen)
    {
      _gameScreen = gameScreen;
    }

    protected virtual void OnClose()
    {
      _gameScreen.State = States.GameStates.Playing;
    }

    public override void UnloadContent()
    {
      throw new NotImplementedException();
    }

    public override void Update(GameTime gameTime)
    {
      _windowSprite.Position = new Vector2(
   (GameEngine.ScreenWidth / 2) - (_windowSprite.Rectangle.Width / 2),
   25f);

      _fontPosition = new Vector2(
        _windowSprite.Position.X + 5,
        _windowSprite.Position.Y + 5);

      _closeButton.Position = new Vector2(
          _windowSprite.Rectangle.Right - _closeButton.Rectangle.Width - 5,
          _windowSprite.Rectangle.Top + 5);
    }
  }
}
