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
using TopDown.States;
using Microsoft.Xna.Framework.Input;
using Engine.Controls;

namespace TopDown.Controls.BuildMenu
{
  public class ItemMenu : Component
  {
    private Sprite _background;

    private List<Button> _items;

    private GameState _gameState;

    private Vector2 _position;

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameState.State != States.States.PlacingItems)
        return;

      _background.Draw(gameTime, spriteBatch);

      foreach (var item in _items)
        item.Draw(gameTime, spriteBatch);
    }

    public ItemMenu(GameState gameState)
    {
      _gameState = gameState;
    }

    public override void LoadContent(ContentManager content)
    {
      _position = new Vector2(25, 25);

      _background = new Sprite(content.Load<Texture2D>("Controls/ItemMenu"))
      {
        Position = _position,
        Layer = 0.98f,
      };

      _background.LoadContent(content);

      var buttonTexture = content.Load<Texture2D>("Controls/BuildMenuMainOptionButton");

      var font = content.Load<SpriteFont>("Fonts/Font");

      _items = new List<Button>()
      {
        new Button(buttonTexture, font)
        {
          Text = "Bed",
          Layer = _background.Layer + 0.01f,
        },
        new Button(buttonTexture, font)
        {
          Text = "Toilet",
          Layer = _background.Layer + 0.01f,
        },
        new Button(buttonTexture, font)
        {
          Text = "Bath",
          Layer = _background.Layer + 0.01f,
        },
        new Button(buttonTexture, font)
        {
          Text = "Fridge",
          Layer = _background.Layer + 0.01f,
        },
      };

      var y = _position.Y + 5;

      foreach (var item in _items)
      {
        item.LoadContent(content);

        item.Position = new Vector2(_position.X + 5, y);

        y += item.Rectangle.Height + 5;
      }
    }

    public override void UnloadContent()
    {
      _background.UnloadContent();

      foreach (var item in _items)
        item.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameState.State != States.States.PlacingItems)
        return;

      _background.Update(gameTime);

      foreach (var item in _items)
        item.Update(gameTime);
    }
  }
}
