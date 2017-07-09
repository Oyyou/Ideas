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
using TopDown.Buildings;

namespace TopDown.Controls.BuildMenu
{
  public class ItemMenu : Component
  {
    private Sprite _background;

    private List<ItemMenuOption> _items;

    private GameScreen _gameState;

    private Vector2 _position;

    public ItemMenuOption CurrentButton;

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameState.State != States.States.PlacingItems &&
        _gameState.State != States.States.ItemMenu)
        return;

      _background.Draw(gameTime, spriteBatch);

      foreach (var item in _items)
        item.Draw(gameTime, spriteBatch);
    }

    public ItemMenu(GameScreen gameState)
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

      var bed = new ItemMenuOption(buttonTexture, font)
      {
        Text = "Bed",
        Layer = _background.Layer + 0.01f,
        Furniture = new Furniture(content.Load<Texture2D>("Furniture/Bed"), _gameState)
        {
          State = FurnatureStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      bed.Click += Item_Click;

      var toilet = new ItemMenuOption(buttonTexture, font)
      {
        Text = "Toilet",
        Layer = _background.Layer + 0.01f,
        Furniture = new Furniture(content.Load<Texture2D>("Furniture/Toilet"), _gameState)
        {
          State = FurnatureStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      toilet.Click += Item_Click;

      var done = new ItemMenuOption(buttonTexture, font)
      {
        Text = "Done",
        Layer = _background.Layer + 0.01f,
      };

      done.Click += Done_Click;

      _items = new List<ItemMenuOption>()
      {
        bed,
        toilet,
        new ItemMenuOption(buttonTexture, font)
        {
          Text = "Bath",
          Layer = _background.Layer + 0.01f,
        },
        new ItemMenuOption(buttonTexture, font)
        {
          Text = "Fridge",
          Layer = _background.Layer + 0.01f,
        },
        done,
      };

      var y = _position.Y + 5;

      foreach (var item in _items)
      {
        item.LoadContent(content);

        item.Position = new Vector2(_position.X + 5, y);

        y += item.Rectangle.Height + 5;
      }
    }

    private void Item_Click(object sender, EventArgs e)
    {
      if (_gameState.State == States.States.PlacingItems)
        return;

      var itemOption = sender as ItemMenuOption;

      if (itemOption.Furniture == null)
        throw new Exception($"Furniture hasn't been set for '{itemOption.Text}' option.");

      itemOption.Furniture.Building = _gameState.SelectedBuilding;
      itemOption.Furniture.Layer = _gameState.SelectedBuilding.Layer + 0.01f;

      _gameState.SelectedBuilding.Components.Add(itemOption.Furniture);

      CurrentButton = itemOption;
      _gameState.State = States.States.PlacingItems;
    }

    private void Done_Click(object sender, EventArgs e)
    {
      _gameState.State = States.States.Playing;
      _gameState.SelectedBuilding.State = BuildingStates.Building;
      _gameState.SelectedBuilding = null;
    }

    public override void UnloadContent()
    {
      _background.UnloadContent();

      foreach (var item in _items)
        item.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameState.State != States.States.PlacingItems &&
        _gameState.State != States.States.ItemMenu)
        return;

      _background.Update(gameTime);

      foreach (var item in _items)
      {
        item.Update(gameTime);
      }
    }
  }
}
