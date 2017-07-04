using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Sprites;
using Engine.Controls;
using TopDown.States;
using Microsoft.Xna.Framework.Input;

namespace TopDown.Controls
{
  public class Toolbar : Component
  {
    private List<Button> _icons;

    private GameState _gameState;

    private Sprite _toolbarSprite;

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      _toolbarSprite.Draw(gameTime, spriteBatch);

      foreach (var icon in _icons)
        icon.Draw(gameTime, spriteBatch);
    }

    private void FistButton_Click(object sender, EventArgs e)
    {
      GameState.Mouse.MouseState = MouseStates.Building;
    }

    public override void LoadContent(ContentManager content)
    {
      _toolbarSprite = new Sprite(content.Load<Texture2D>("Controls/Toolbar"));
      _toolbarSprite.Position = new Vector2((GameEngine.ScreenWidth / 2) - (_toolbarSprite.Rectangle.Width / 2), GameEngine.ScreenHeight - _toolbarSprite.Rectangle.Height - 20);

      var fistButton = new Button(content.Load<Texture2D>("Controls/Icons/Fist"));
      fistButton.Click += FistButton_Click;

      var pickaxeButton = new Button(content.Load<Texture2D>("Controls/Icons/Pickaxe"));
      pickaxeButton.Click += PickaxeButton_Click;

      _icons = new List<Button>()
      {
        fistButton,
        pickaxeButton,
      };

      var x = _toolbarSprite.Position.X + 1;

      foreach (var icon in _icons)
      {
        icon.Position = new Vector2(x, _toolbarSprite.Position.Y + 1);

        x += 60;
      }
    }

    private void PickaxeButton_Click(object sender, EventArgs e)
    {
      GameState.Mouse.MouseState = MouseStates.Mining;
    }

    public Toolbar(GameState gameState)
    {
      _gameState = gameState;
    }

    public override void UnloadContent()
    {
      _toolbarSprite.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if (Keyboard.GetState().IsKeyDown(Keys.D1))
        GameState.Mouse.MouseState = MouseStates.Building;
      else if (Keyboard.GetState().IsKeyDown(Keys.D2))
        GameState.Mouse.MouseState = MouseStates.Mining;

      _toolbarSprite.Update(gameTime);

      foreach (var icon in _icons)
        icon.Update(gameTime);
    }
  }
}
