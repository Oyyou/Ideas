using Engine;
using Engine.Controls;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.States;

namespace TopDown.Controls.Toolbars
{
  public abstract class Toolbar : Component
  {
    protected List<Button> _icons;

    protected GameScreen _gameScreen;

    protected Sprite _toolbarSprite;

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != GameStates.Playing)
        return;

      _toolbarSprite.Draw(gameTime, spriteBatch);

      var x = _toolbarSprite.Position.X;

      foreach (var icon in _icons)
      {
        icon.Position = new Vector2(x, _toolbarSprite.Position.Y);

        x += icon.Rectangle.Width;

        icon.Draw(gameTime, spriteBatch);
      }
    }

    protected void InitializeIcons(ContentManager content)
    {
      foreach (var icon in _icons)
        icon.LoadContent(content);
    }

    public override void LoadContent(ContentManager content)
    {

    }

    public Toolbar(GameScreen gameScreen)
    {
      _gameScreen = gameScreen;
    }

    public override void UnloadContent()
    {
      _toolbarSprite.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameScreen.State != GameStates.Playing)
        return;

      if (GameScreen.Keyboard.IsKeyPressed(Keys.D1))
        GameScreen.Mouse.MouseState = MouseStates.Building;
      else if (GameScreen.Keyboard.IsKeyPressed(Keys.D2))
        GameScreen.Mouse.MouseState = MouseStates.Mining;

      _toolbarSprite.Update(gameTime);

      foreach (var icon in _icons)
        icon.Update(gameTime);
    }
  }
}
