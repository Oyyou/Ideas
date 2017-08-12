using Engine.Controls;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Items;
using TopDown.States;

namespace TopDown.Controls.InventoryMenu
{
  public class InventoryMenuWindow : MenuWindow
  {
    private List<Item> _items;

    private List<Button> _mainButtons;

    private Texture2D _mainButtonTexture;

    private Texture2D _subButtonTexture;

    private void CategoryButton_Click(object sender, EventArgs e)
    {
      var button = sender as Button;
      var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), button.Text);

      _items = _gameScreen.InventoryItems.Where(c => c.Category == category).ToList();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.InventoryMenu)
        return;

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _mainButtons)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _items)
      {
        component.Layer = 0.99f;
        component.Draw(gameTime, spriteBatch);
      }

      spriteBatch.DrawString(_font, "Inventory", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
    }

    public InventoryMenuWindow(GameScreen gameScreen) : base(gameScreen)
    {
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _items = new List<Item>();

      _mainButtonTexture = content.Load<Texture2D>("Controls/BuildMenuMainOptionButton"); _subButtonTexture = content.Load<Texture2D>("Controls/BuildMenuSubOptionButton");

      _subButtonTexture = content.Load<Texture2D>("Controls/BuildMenuSubOptionButton");

      // add inventory button components here
      var categories = Enum.GetValues(typeof(ItemCategories));

      var x = Position.X + 11;
      var y = Position.Y + 56;
      var yIncrement = _mainButtonTexture.Height + 5;

      _mainButtons = new List<Button>();

      foreach (var category in categories)
      {
        var categoryButton = new Button(_mainButtonTexture, _font)
        {
          Text = category.ToString(),
          Position = new Vector2(x, y),
          Layer = 0.99f,
        };

        categoryButton.Click += CategoryButton_Click;

        y += yIncrement;

        _mainButtons.Add(categoryButton);
      }

      foreach (var component in _mainButtons)
        component.LoadContent(content);

      foreach (var component in Components)
        component.LoadContent(content);
    }

    protected override void OnClose()
    {
      base.OnClose();

      _items = new List<Item>();
    }

    public override void UnloadContent()
    {
      foreach (var component in Components)
        component.UnloadContent();

      Components?.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameScreen.State != States.GameStates.InventoryMenu)
      {
        foreach (var component in _mainButtons)
        {
          component.IsSelected = false;
        }

        _items = new List<Item>();

        return;
      }

      foreach (var component in Components)
        component.Update(gameTime);

      foreach (var component in _mainButtons)
      {
        // Sets the colour of the selected button
        if (component.IsClicked)
        {
          foreach (var c in _mainButtons)
          {
            c.IsSelected = false;
          }

          component.IsSelected = true;
        }

        component.Update(gameTime);
      }

      var x = Position.X + 196;
      var xIncrement = 0;

      var y = Position.Y + 56;

      foreach (var component in _items)
      {
        component.Position = new Vector2(x + xIncrement, y);

        xIncrement += component.Rectangle.Width + 5;

        if (x + xIncrement > 708)
        {
          y += component.Rectangle.Height + 5;
          xIncrement = 0;
        }

        component.Update(gameTime);
      }
    }
  }
}
