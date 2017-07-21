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

    private GameScreen _gameState;

    private Vector2 _position;

    public ItemMenuButton CurrentButton;

    private void Cancel_Click(object sender, EventArgs e)
    {
      _gameState.State = States.GameStates.BuildMenu;

      if (_gameState.SelectedBuilding != null)
      {
        _gameState.SelectedBuilding.IsRemoved = true;
        _gameState.SelectedBuilding = null;
      }

      if (_gameState.SelectedPathBuilder != null)
      {
        _gameState.SelectedPathBuilder.IsRemoved = true;
        _gameState.SelectedPathBuilder = null;
      }

      this.Reset();
    }

    public override void CheckCollision(Component component)
    {

    }

    private void Done_Click(object sender, EventArgs e)
    {
      _gameState.State = States.GameStates.Playing;

      if (_gameState.SelectedBuilding != null)
      {
        _gameState.SelectedBuilding.State = BuildingStates.Building;
        _gameState.SelectedBuilding = null;
      }
      
      if(_gameState.SelectedPathBuilder != null)
      {
        _gameState.SelectedPathBuilder.Paths.Last().IsRemoved = true;

        foreach (var component in _gameState.SelectedPathBuilder.Paths)
        {
          _gameState.AddComponent(component);
        }

        _gameState.SelectedPathBuilder.IsRemoved = true;
        _gameState.SelectedPathBuilder = null;
      }

      _gameState.PathFinder.UpdateMap(_gameState.PathComponents.Select(c => c.Position).ToList());

      Reset();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameState.State != States.GameStates.PlacingItems &&
        _gameState.State != States.GameStates.ItemMenu)
        return;

      _background.Draw(gameTime, spriteBatch);

      foreach (var item in Components)
        item.Draw(gameTime, spriteBatch);
    }

    public ItemMenu(GameScreen gameState)
    {
      _gameState = gameState;
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

    public void Reset()
    {
      foreach (var component in Components)
      {
        ((ItemMenuButton)component).CanClick = true;
        ((ItemMenuButton)component).CurrentState = ItemMenuButtonStates.Clickable;
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
      if (_gameState.State != States.GameStates.PlacingItems &&
        _gameState.State != States.GameStates.ItemMenu)
        return;

      _background.Update(gameTime);

      foreach (var component in Components)
      {
        if (((ItemMenuButton)component).PreviousState == ItemMenuButtonStates.Clicked &&
          ((ItemMenuButton)component).CurrentState == ItemMenuButtonStates.Placed)
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

      _gameState.State = component.GameScreenSetValue;

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
