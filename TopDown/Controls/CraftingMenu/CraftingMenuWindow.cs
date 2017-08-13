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
using TopDown.ItemStats;
using TopDown.Models;
using TopDown.Sprites;
using TopDown.States;

namespace TopDown.Controls.CraftingMenu
{
  public class CraftingMenuWindow : MenuWindow
  {
    /// <summary>
    /// For the NPCs
    /// </summary>
    public ComboBox ComboBox;

    private List<Button> _mainButtons;

    private Texture2D _mainButtonTexture;

    private string _professionRequired;

    private Sprite _queueSprite;

    private List<CraftingMenuSubButton> _subButtons;

    private Texture2D _subButtonTexture;

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.CraftingMenu)
        return;

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _mainButtons)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _subButtons)
        component.Draw(gameTime, spriteBatch);

      if (ComboBox.SelectedItem != null)
      {
        var npc = ComboBox.SelectedItem.Content as NPC;

        var x = _queueSprite.Position.X + 11;
        var y = _queueSprite.Position.Y + 11;

        var list = npc.CraftingItems.ToList();
        var extraCount = 0;
        for (int i = 0; i < list.Count; i++)
        {
          var item = list[i];

          item.Position = new Vector2(x, y);
          item.Layer = _queueSprite.Layer + 0.001f;

          var increment = item.Rectangle.Width + 4;

          if (x > _queueSprite.Rectangle.Right - increment - item.Rectangle.Width)
            extraCount++;

          if (extraCount == 0)
          {
            item.Draw(gameTime, spriteBatch);

            x += increment;
          }
          else if(i == list.Count - 1) // if it's the last, let the player know how many other items are being crafted
          {
            var text = "+" + extraCount;

            var newX = x + 20 - (_font.MeasureString(text).X / 2);
            var newY = y + 20 - (_font.MeasureString(text).Y / 2);

            spriteBatch.DrawString(_font, text , new Vector2(newX, newY), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
          }
        }
      }

      spriteBatch.DrawString(_font, "Crafting", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);

      if (!string.IsNullOrEmpty(_professionRequired) && ComboBox.IsVisible)
      {
        var newText = _professionRequired.EndsWith(":") ? _professionRequired : _professionRequired + ":";

        var x = ComboBox.Rectangle.X - 5 - _font.MeasureString(newText).X;
        var y = (ComboBox.Rectangle.Y + (ComboBox.Rectangle.Height / 2)) - (_font.MeasureString(newText).Y / 2);

        spriteBatch.DrawString(_font, newText, new Vector2(x, y), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
      }
    }

    public CraftingMenuWindow(GameScreen gameScreen) : base(gameScreen)
    {
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _queueSprite = new Sprite(content.Load<Texture2D>("Controls/CraftingQueue"))
      {
        Position = new Vector2(_windowSprite.Rectangle.X, _windowSprite.Rectangle.Bottom + 5),
      };

      _mainButtonTexture = content.Load<Texture2D>("Controls/BuildMenuMainOptionButton"); _subButtonTexture = content.Load<Texture2D>("Controls/BuildMenuSubOptionButton");

      _subButtonTexture = content.Load<Texture2D>("Controls/CraftingMenu/ItemOption");

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

      armourButton.Click += ArmourButton_Click;

      var medicineButton = new Button(_mainButtonTexture, _font)
      {
        Text = "Medicine",
        Position = new Vector2(armourButton.Position.X, armourButton.Rectangle.Bottom + 5),
        Layer = 0.99f,
      };

      medicineButton.Click += MedicineButton_Click;

      ComboBox = new ComboBox()
      {
        DefaultText = "Select NPC",
        EmptyText = "No NPCs of profession",
      };

      _mainButtons = new List<Button>()
      {
        weaponsButton,
        armourButton,
        medicineButton,
      };

      Components.Add(ComboBox);
      Components.Add(_queueSprite);

      foreach (var component in Components)
        component.LoadContent(content);

      foreach (var component in _mainButtons)
        component.LoadContent(content);

      ComboBox.Position = new Vector2(_closeButton.Position.X - ComboBox.Width - 5, _closeButton.Position.Y);
      ComboBox.Layer = _closeButton.Layer;

      _subButtons = new List<CraftingMenuSubButton>();
    }

    private void MedicineButton_Click(object sender, EventArgs e)
    {
      var category = ItemCategories.Medicine;

      _subButtons = new List<CraftingMenuSubButton>();

      _professionRequired = "Doctor";
    }

    private void ArmourButton_Click(object sender, EventArgs e)
    {
      var category = ItemCategories.Armour;

      var woodenChestPieceButton = new CraftingMenuSubButton(_subButtonTexture, _font,
        new Item(_content.Load<Texture2D>("Items/WoodenChestPiece"), 3, "Wooden Chest Piece", category)
        {
          ExperienceValue = 3,
          ResourceCost = new Models.Resources()
          {
            Wood = 5,
          },
          Stats = new ArmourStats()
          {
            ArmourGained = 2,
          }
        })
      {
        Layer = 0.99f,
      };

      woodenChestPieceButton.Click += Item_Click;

      _subButtons = new List<CraftingMenuSubButton>()
      {
        woodenChestPieceButton,
      };

      foreach (var component in _subButtons)
        component.LoadContent(_content);

      _professionRequired = "Blacksmith";
    }

    private void WeaponsButton_Click(object sender, EventArgs e)
    {
      var category = ItemCategories.Weapons;

      var woodenSwordButton = new CraftingMenuSubButton(_subButtonTexture, _font,
        new Item(_content.Load<Texture2D>("Items/WoodenSword"), 3, "Wooden Sword", category)
        {
          ExperienceValue = 2,
          ResourceCost = new Models.Resources()
          {
            Wood = 2,
          },
          Stats = new WeaponStats()
          {
            AttackSpeed = 1,
            Damage = 3,
          },
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

      _professionRequired = "Blacksmith";
    }

    private void Item_Click(object sender, EventArgs e)
    {
      if (ComboBox.SelectedItem == null)
      {
        GameScreen.MessageBox.Show("Select an NPC");
        return;
      }

      var button = sender as CraftingMenuSubButton;

      var npc = ComboBox.SelectedItem.Content as NPC;

      npc.CraftingItems.Add((Item)button.CraftingItem.Clone());
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

      ComboBox.IsEnabled = false;
      ComboBox.IsVisible = false;

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

        if(component.IsSelected)
        {
          ComboBox.IsEnabled = true;
          ComboBox.IsVisible = true;
        }

        component.Update(gameTime);
      }

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

      if (ComboBox.SelectedItem != null)
      {
        var npc = ComboBox.SelectedItem.Content as NPC;

        foreach (var item in npc.CraftingItems.ToList())
        {
          item.Update(gameTime);

          if (GameScreen.Mouse.Rectangle.Intersects(item.Rectangle))
          {
            if (GameScreen.Mouse.LeftClicked)
            {
              npc.CraftingItems.Remove(item);

              if (npc.CraftingItem == item)
              {
                if (npc.CraftingItems.Count > 0)
                  npc.CraftingItem = npc.CraftingItems.First();
              }
            }
          }
        }
      }
    }
  }
}
