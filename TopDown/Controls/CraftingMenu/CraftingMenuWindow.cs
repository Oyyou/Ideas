using Engine.Controls;
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

namespace TopDown.Controls.CraftingMenu
{
  public class CraftingMenuWindow : MenuWindow
  {
    private Texture2D _mainButtonTexture;

    private Texture2D _subButtonTexture;

    private List<CraftingMenuSubButton> _subButtons;

    public Queue<Item> CraftingItems;

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.CraftingMenu)
        return;

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _subButtons)
        component.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, "Crafting", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
    }

    public CraftingMenuWindow(GameScreen gameScreen) : base(gameScreen)
    {
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _mainButtonTexture = content.Load<Texture2D>("Controls/BuildMenuMainOptionButton"); _subButtonTexture = content.Load<Texture2D>("Controls/BuildMenuSubOptionButton");

      _subButtonTexture = content.Load<Texture2D>("Controls/BuildMenuSubOptionButton");

      var weaponsButton = new Button(_mainButtonTexture, _font)
      {
        Text = "Weapons",
        Position = new Vector2(Position.X + 11, Position.Y + 56),
        Layer = 0.99f,
      };

      weaponsButton.Click += WeaponsButton_Click;

      var armourButton = new Button(_mainButtonTexture, _font)
      {
        Text = "Armour",
        Position = new Vector2(weaponsButton.Position.X, weaponsButton.Rectangle.Bottom + 5),
        Layer = 0.99f,
      };

      Components.Add(weaponsButton);
      Components.Add(armourButton);

      foreach (var component in Components)
        component.LoadContent(content);

      _subButtons = new List<CraftingMenuSubButton>();

      CraftingItems = new Queue<Item>();
    }

    private void WeaponsButton_Click(object sender, EventArgs e)
    {
      var woodenSwordButton = new CraftingMenuSubButton(_subButtonTexture, _font,
        new Item(_content.Load<Texture2D>("Items/WoodenSword"), 3, "Wooden Sword", ItemCategories.Weapons)
        {
          ResourceCost = new Models.Resources()
          {
            Wood = 2,
          }
        })
      {
        Layer = 0.99f,
      };

      woodenSwordButton.Click += Item_Click;

      _subButtons = new List<CraftingMenuSubButton>()
      {
        woodenSwordButton,
      };

      foreach (var component in _subButtons)
        component.LoadContent(_content);
    }

    private void Item_Click(object sender, EventArgs e)
    {
      var button = sender as CraftingMenuSubButton;

      CraftingItems.Enqueue((Item)button.CraftingItem.Clone());
    }

    public override void UnloadContent()
    {
      foreach (var component in Components)
        component.UnloadContent();

      Components?.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameScreen.State != States.GameStates.CraftingMenu)
        return;

      foreach (var component in Components)
        component.Update(gameTime);

      var x = Position.X + 196;
      var xIncrement = 0;

      var y = Position.Y + 56;

      foreach (var component in _subButtons)
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
