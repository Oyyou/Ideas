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
using TopDown.Controls.BuildMenu;

namespace TopDown.Controls.ItemMenu
{
  public class ItemMenu : Component
  {
    private Sprite _background;

    private Texture2D _buttonTexture;

    private ContentManager _content;

    private SpriteFont _font;

    private GameScreen _gameScreen;

    private Vector2 _position;

    public ItemMenuButton CurrentButton;

    private void Cancel_Click(object sender, EventArgs e)
    {
      _gameScreen.State = States.GameStates.BuildMenu;

      if (_gameScreen.SelectedBuilding != null)
      {
        _gameScreen.SelectedBuilding.IsRemoved = true;
        _gameScreen.SelectedBuilding = null;
      }

      if (_gameScreen.SelectedPathBuilder != null)
      {
        _gameScreen.SelectedPathBuilder.IsRemoved = true;
        _gameScreen.SelectedPathBuilder = null;
      }

      this.FullReset();
    }

    public override void CheckCollision(Component component)
    {

    }

    private void Done_Click(object sender, EventArgs e)
    {
      // Items that need to be placed before finishing the building
      var requiredItems = Components.Cast<ItemMenuButton>().Where(c => c.IsRequired && c.CurrentState != ItemMenuButtonStates.Placed);

      if (requiredItems.Count() > 0)
      {
        var button = sender as ItemMenuButton;
        button.CurrentState = ItemMenuButtonStates.Clickable;

        GameScreen.MessageBox.Show("Still need to add: " + string.Join(", ", requiredItems.Select(c => c.Text).ToArray()));
        return;
      }

      _gameScreen.State = States.GameStates.Playing;

      if (_gameScreen.SelectedBuilding != null)
      {
        _gameScreen.SelectedBuilding.State = BuildingStates.Constructing;
        _gameScreen.SelectedBuilding = null;
      }

      if (_gameScreen.SelectedPathBuilder != null)
      {
        _gameScreen.SelectedPathBuilder.Paths.Last().IsRemoved = true;

        foreach (var component in _gameScreen.SelectedPathBuilder.Paths)
        {
          _gameScreen.AddComponent(component);
        }

        _gameScreen.SelectedPathBuilder.IsRemoved = true;
        _gameScreen.SelectedPathBuilder = null;

        _gameScreen.UpdateMap();
      }

      FullReset();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.PlacingItems &&
        _gameScreen.State != States.GameStates.ItemMenu)
        return;

      _background.Draw(gameTime, spriteBatch);

      foreach (var item in Components)
        item.Draw(gameTime, spriteBatch);
    }

    public ItemMenu(GameScreen gameState)
    {
      _gameScreen = gameState;
    }

    public override void LoadContent(ContentManager content)
    {
      Components = new List<Component>();

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

    public void FullReset()
    {
      foreach (var component in Components)
      {
        ((ItemMenuButton)component).CanClick = true;
        ((ItemMenuButton)component).CurrentState = ItemMenuButtonStates.Clickable;
        ((ItemMenuButton)component).Amount = ((ItemMenuButton)component).StartAmount.Value;
      }
    }

    public override void UnloadContent()
    {
      _background.UnloadContent();

      foreach (var item in Components)
        item.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameScreen.State != States.GameStates.PlacingItems &&
        _gameScreen.State != States.GameStates.ItemMenu)
        return;

      _background.Update(gameTime);

      foreach (var component in Components)
      {
        if ((((ItemMenuButton)component).PreviousState == ItemMenuButtonStates.Clicked &&
          ((ItemMenuButton)component).CurrentState == ItemMenuButtonStates.Placed) ||
          ((ItemMenuButton)component).Amount > 0)
        {
          foreach (var c in Components)
          {
            if (c == component)
              continue;

            ((ItemMenuButton)c).CanClick = true;
          }
        }
      }

      foreach (var item in Components)
      {
        item.Update(gameTime);
      }
    }

    public void Open(BuildMenuSubButton component)
    {
      Components = new List<Component>();

      _gameScreen.State = component.GameScreenSetValue;

      Components = component.Items.Select(c => c as Component).ToList();

      var done = new ItemMenuButton(_buttonTexture, _font)
      {
        Text = "Done",
      };

      done.Click += Done_Click;

      var cancel = new ItemMenuButton(_buttonTexture, _font)
      {
        Text = "Cancel",
      };

      cancel.Click += Cancel_Click;

      Components.Add(done);
      Components.Add(cancel);

      var y = _position.Y + 5;

      foreach (var item in Components)
      {
        item.LoadContent(_content);

        item.Layer = _background.Layer + 0.01f;

        item.Position = new Vector2(_position.X + 5, y);

        y += item.Rectangle.Height + 5;
      }
    }
  }
}
