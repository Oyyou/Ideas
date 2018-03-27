﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using static VillageBackend.Enums;
using Microsoft.Xna.Framework.Graphics;
using GUITest.Interface.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using VillageBackend.Models;
using System.IO;

namespace GUITest.Interface.Windows
{
  public class WindowSection
  {
    private Rectangle _area;

    public Rectangle Area
    {
      get { return _area; }
      set
      {
        _area = value;
        Scrollbar.ScrollArea = _area;
      }
    }

    public Matrix Matrix
    {
      get
      {
        return Matrix.CreateTranslation(Camera.X, Camera.Y, 0);
      }
    }

    public Scrollbar Scrollbar;

    public Vector2 Camera
    {
      get
      {

        return new Vector2(0, MathHelper.Clamp(-Scrollbar._innerY + 60, -1000, 2400));
      }
    }

    public IEnumerable<Button> Items;
  }

  public class CraftingWindow : Window
  {
    #region Items
    private List<Weapon> _weapons;

    private List<Armour> _armours;
    #endregion

    private WindowSection _categorySection;

    private WindowSection _itemSection;

    private const int _spaceBetween = 10;

    /// <summary>
    /// The item to be created
    /// </summary>
    public ItemV2 Item { get; private set; }

    public CraftingWindow(ContentManager content) : base(content)
    {
      Name = "Crafting";

      Texture = content.Load<Texture2D>("Interface/Window2x_1y");

      _categorySection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content),
        Items = new List<Button>(),
      };

      _itemSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content),
        Items = new List<Button>(),
      };

      var buttonTexture = content.Load<Texture2D>("Interface/Button");
      var buttonFont = content.Load<SpriteFont>("Fonts/Font");

      var categories = Enum.GetNames(typeof(ItemCategories)).ToList();

      _categorySection.Items = categories.Select(c =>
      {
        var button = new Button(buttonTexture, buttonFont)
        {
          Text = c,
        };

        button.Click += CategoryClicked;

        return button;

      }).ToList();

      LoadItems();

      SetPositions();
    }

    private void LoadItems()
    {
      _weapons = new List<Weapon>();
      _armours = new List<Armour>();

      // Is this okay?
      var files = Directory.GetFiles("../../../../../VillageBackend/Content/", "*.json");

      foreach (var file in files)
      {
        var fileName = Path.GetFileNameWithoutExtension(file);

        var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), fileName);

        switch (category)
        {
          case ItemCategories.Weapon:
            _weapons = JsonConvert.DeserializeObject<List<Weapon>>(File.ReadAllText(file),
              new JsonSerializerSettings() { Formatting = Formatting.Indented });
            break;
          case ItemCategories.Armour:
            _armours = JsonConvert.DeserializeObject<List<Armour>>(File.ReadAllText(file),
              new JsonSerializerSettings() { Formatting = Formatting.Indented });
            break;
          case ItemCategories.Tool:
          case ItemCategories.Clothing:
          case ItemCategories.Jewellery:
          case ItemCategories.Medicine:
          default:
            throw new Exception("Not implemented category: " + category.ToString());
        }
      }
    }

    private void CategoryClicked(object sender, EventArgs e)
    {
      var button = sender as Button;

      _itemSection.Items = new List<Button>();

      var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), button.Text);

      switch (category)
      {
        case ItemCategories.Weapon:
          _itemSection.Items = _weapons.Select(c =>
          {
            return GetItemButton(c);
          }).ToList();
          break;
        case ItemCategories.Armour:
          _itemSection.Items = _armours.Select(c =>
          {
            return GetItemButton(c);
          }).ToList();
          break;
        case ItemCategories.Tool:

          break;
        case ItemCategories.Clothing:

          break;
        case ItemCategories.Jewellery:

          break;
        case ItemCategories.Medicine:

          break;
        default:
          throw new Exception("Unknown category: " + category);
      }

      SetItemsPositions();
    }

    private ItemButton GetItemButton(ItemV2 item)
    {
      var fullPath = $"{Directory.GetCurrentDirectory()}\\Content\\Interface\\ItemIcons\\{item.Name}.xnb";

      string content = "Interface/NoImage";

      if (File.Exists(fullPath))
        content = "Interface/ItemIcons/" + item.Name;

      var button = new ItemButton(_content.Load<Texture2D>(content), item);
      button.Click += ItemClicked;

      return button;
    }

    private void ItemClicked(object sender, EventArgs e)
    {
      var button = sender as ItemButton;

      Item = button.Item;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      if (!_hasUpdated)
        return;

      var original = graphics.GraphicsDevice.Viewport;

      DrawWindow(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport((int)Position.X + 190, (int)Position.Y + 35, Texture.Width - 170, Texture.Height - 35);

      DrawButtons(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport((int)Position.X, (int)Position.Y + 35, 190, Texture.Height - 35);

      DrawCategories(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = original;
    }

    protected void DrawButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _itemSection.Matrix);

      foreach (var button in _itemSection.Items)
      {
        switch (button.CurrentState)
        {
          case ToolbarButtonStates.Nothing:

            button.Color = Color.White;

            button.Scale = 1.0f;

            break;
          case ToolbarButtonStates.Hovering:

            button.Color = Color.YellowGreen;

            button.Scale = 1.0f;

            break;
          case ToolbarButtonStates.Clicked:

            // This will be removed

            button.Color = Color.YellowGreen;

            button.Scale = 1.2f;

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }

        button.Draw(gameTime, spriteBatch);
      }

      spriteBatch.End();
    }

    protected void DrawCategories(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _categorySection.Matrix);

      foreach (var button in _categorySection.Items)
      {
        switch (button.CurrentState)
        {
          case ToolbarButtonStates.Nothing:

            button.Color = Color.White;

            button.Scale = 1.0f;

            break;
          case ToolbarButtonStates.Hovering:

            button.Color = Color.YellowGreen;

            button.Scale = 1.0f;

            break;
          case ToolbarButtonStates.Clicked:

            // This will be removed

            button.Color = Color.YellowGreen;

            button.Scale = 1.05f;

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }

        button.Draw(gameTime, spriteBatch);
      }

      spriteBatch.End();
    }

    protected void DrawWindow(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

      _itemSection.Scrollbar.Draw(gameTime, spriteBatch);

      _categorySection.Scrollbar.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, Name, new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);

      spriteBatch.End();
    }

    public override void SetPositions()
    {
      var screenWidth = Game1.ScreenWidth;
      var screenHeight = Game1.ScreenHeight;

      Position = new Vector2((Game1.ScreenWidth / 2) - (Texture.Width / 2), screenHeight - Texture.Height - 100);

      _categorySection.Area = new Rectangle((int)Position.X, (int)Position.Y + 35, 190, Texture.Height - 35);
      _categorySection.Scrollbar.Position = new Vector2((Position.X + 170), Position.Y + 35);

      _itemSection.Area = new Rectangle((int)Position.X + 190, (int)Position.Y + 35, Texture.Width - 170, Texture.Height - 35);
      _itemSection.Scrollbar.Position = new Vector2((Position.X + Texture.Width) - 20 - 10, Position.Y + 35);

      var x = _spaceBetween + (_categorySection.Items.FirstOrDefault().Rectangle.Width / 2);
      var y = Position.Y + 3;

      var categories = Enum.GetNames(typeof(ItemCategories)).ToList();

      foreach(var item in _categorySection.Items)
      {
        item.Position = new Vector2(x, y);
        y += item.Rectangle.Height + 5;
      }

      SetItemsPositions();
    }

    private void SetItemsPositions()
    {
      if (_itemSection.Items.Count() > 0)
      {
        var buttonHeight = _itemSection.Items.FirstOrDefault().Texture.Height;
        var buttonWidth = _itemSection.Items.FirstOrDefault().Texture.Width;

        var x = _spaceBetween + (buttonWidth / 2);

        var y = 30;

        foreach (var button in _itemSection.Items)
        {
          button.Position = new Vector2(x, y);
          x += button.Texture.Width + _spaceBetween;

          if ((x + (button.Rectangle.Width / 2)) > (_itemSection.Area.X))
          {
            x = _spaceBetween + (buttonWidth / 2);
            y += buttonHeight + _spaceBetween;
          }
        }
      }
    }

    public override void Update(GameTime gameTime)
    {
      _hasUpdated = true;

      _itemSection.Scrollbar.Update(gameTime);
      _categorySection.Scrollbar.Update(gameTime);

      _previousMouseState = _currentMouseState;
      _currentMouseState = Mouse.GetState();

      UpdateItems();

      UpdateCategories();
    }

    private void UpdateItems()
    {
      var translation = _itemSection.Matrix.Translation;

      var mouseRectangle = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);

      var mouseRectangleWithCamera_Items = new Rectangle(
        (int)((_currentMouseState.X - (Position.X + 190)) - translation.X),
        (int)((_currentMouseState.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (var button in _itemSection.Items)
      {
        switch (button.CurrentState)
        {
          case ToolbarButtonStates.Nothing:

            if (mouseRectangleWithCamera_Items.Intersects(button.Rectangle) && mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ToolbarButtonStates.Hovering;

            break;
          case ToolbarButtonStates.Hovering:

            if (!mouseRectangleWithCamera_Items.Intersects(button.Rectangle) || !mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ToolbarButtonStates.Nothing;

            if (clicked)
            {
              foreach (var b in _itemSection.Items)
                b.CurrentState = ToolbarButtonStates.Nothing;

              button.CurrentState = ToolbarButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ToolbarButtonStates.Clicked:

            // Close the window, and start to place the building!

            //var mouseRectangle = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);
            //var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            if (clicked && !_itemSection.Items.Any(c => c != button && c.Rectangle.Intersects(mouseRectangleWithCamera_Items))) // Check if we're clicking somewhere that isn't on any button
            {
              foreach (var b in _itemSection.Items)
                b.CurrentState = ToolbarButtonStates.Nothing;

              button.CurrentState = ToolbarButtonStates.Hovering;
            }

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }

    private void UpdateCategories()
    {
      var translation = _categorySection.Matrix.Translation;

      var mouseRectangle = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);

      var mouseRectangleWithCamera_Categories = new Rectangle(
        (int)((_currentMouseState.X - Position.X) - translation.X),
        (int)((_currentMouseState.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (var button in _categorySection.Items)
      {
        switch (button.CurrentState)
        {
          case ToolbarButtonStates.Nothing:

            if (mouseRectangleWithCamera_Categories.Intersects(button.Rectangle) && mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ToolbarButtonStates.Hovering;

            break;
          case ToolbarButtonStates.Hovering:

            if (!mouseRectangleWithCamera_Categories.Intersects(button.Rectangle) || !mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ToolbarButtonStates.Nothing;

            if (clicked)
            {
              foreach (var b in _categorySection.Items)
                b.CurrentState = ToolbarButtonStates.Nothing;

              button.CurrentState = ToolbarButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ToolbarButtonStates.Clicked:

            // Close the window, and start to place the building!

            //var mouseRectangle = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);
            //var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            if (clicked && (mouseRectangleWithCamera_Categories.Intersects(windowRectangle)) && !_categorySection.Items.Any(c => c != button && c.Rectangle.Intersects(mouseRectangleWithCamera_Categories))) // Check if we're clicking somewhere that isn't on any button
            {
              foreach (var b in _categorySection.Items)
                b.CurrentState = ToolbarButtonStates.Nothing;

              button.CurrentState = ToolbarButtonStates.Hovering;
            }

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }
  }
}