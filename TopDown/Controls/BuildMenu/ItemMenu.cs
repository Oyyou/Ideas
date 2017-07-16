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

    private ContentManager _content;

    private List<ItemMenuOption> _items;

    private GameScreen _gameState;

    private Vector2 _position;

    public ItemMenuOption CurrentButton;

    private void Cancel_Click(object sender, EventArgs e)
    {
      _gameState.State = States.States.BuildMenu;

      if (_gameState.SelectedBuilding != null)
        _gameState.SelectedBuilding.IsRemoved = true;

      if (_gameState.SelectedPathBuilder != null)
        _gameState.SelectedPathBuilder.IsRemoved = true;

      this.Reset();
    }

    public override void CheckCollision(Component component)
    {

    }

    private void Done_Click(object sender, EventArgs e)
    {
      _gameState.State = States.States.Playing;

      if (_gameState.SelectedBuilding != null)
      {
        _gameState.SelectedBuilding.State = BuildingStates.Building;
        _gameState.SelectedBuilding = null;
      }
      
      if(_gameState.SelectedPathBuilder != null)
      {
        _gameState.SelectedPathBuilder.State = Builders.PathBuilderStates.Finished;
        _gameState.SelectedPathBuilder = null;
      }

      Reset();
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
      _content = content;

      _position = new Vector2(25, 25);

      _background = new Sprite(content.Load<Texture2D>("Controls/ItemMenu"))
      {
        Position = _position,
        Layer = 0.98f,
      };

      _background.LoadContent(content);

      _buttonTexture = content.Load<Texture2D>("Controls/BuildMenuMainOptionButton");

      _font = content.Load<SpriteFont>("Fonts/Font");


    }

    public void Reset()
    {
      foreach (var item in _items)
      {
        item.CanClick = true;
        item.CurrentState = ItemMenuOptionStates.Clickable;
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
      if (_gameState.State != States.States.PlacingItems &&
        _gameState.State != States.States.ItemMenu)
        return;

      _background.Update(gameTime);

      foreach (var item in _items)
      {
        if (item.PreviousState == ItemMenuOptionStates.Clicked &&
          item.CurrentState == ItemMenuOptionStates.Placed)
        {
          foreach (var i in _items)
          {
            if (i == item)
              continue;

            i.CanClick = true;
          }
        }
      }

      foreach (var item in _items)
      {
        item.Update(gameTime);
      }
    }

    private Texture2D _buttonTexture;

    private SpriteFont _font;

    public void Open(BuildMenuSubItem component)
    {
      _items = new List<ItemMenuOption>();

      _gameState.State = component.GameScreenSetValue;

      // I've added the 'ToList' to remove the reference from the original
      _items = component.Items.ToList();

      var done = new ItemMenuOption(_buttonTexture, _font)
      {
        Text = "Done",
      };

      done.Click += Done_Click;

      var cancel = new ItemMenuOption(_buttonTexture, _font)
      {
        Text = "Cancel",
      };

      cancel.Click += Cancel_Click;

      _items.Add(done);
      _items.Add(cancel);
            
      var y = _position.Y + 5;

      foreach (var item in _items)
      {
        item.LoadContent(_content);

        item.Layer = _background.Layer + 0.01f;

        item.Position = new Vector2(_position.X + 5, y);

        y += item.Rectangle.Height + 5;
      }
    }
  }
}
