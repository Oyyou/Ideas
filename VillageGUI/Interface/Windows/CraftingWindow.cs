using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using static VillageBackend.Enums;
using Microsoft.Xna.Framework.Graphics;
using VillageGUI.Interface.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using VillageBackend.Models;
using VillageBackend.Managers;
using System.IO;
using Engine.Interface.Windows;

namespace VillageGUI.Interface.Windows
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

    public void UnloadContent()
    {
      foreach (var item in Items)
        item.UnloadContent();
    }
  }

  public class CraftingWindow : Window
  {
    private ItemManager _itemManager;

    #region Items
    private List<Weapon> _weapons;

    private List<Armour> _armours;
    #endregion

    #region Sections
    private WindowSection _categorySection;

    private WindowSection _itemSection;
    #endregion

    private const int _spaceBetween = 10;

    private QueueWindow _queueWindow;

    /// <summary>
    /// The item to be created
    /// </summary>
    public ItemV2 Item { get; private set; }

    public override Rectangle WindowRectangle { get => new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width + 10 + _queueWindow.Rectangle.Width, Rectangle.Height); }

    public CraftingWindow(ContentManager content, ItemManager itemManager) : base(content)
    {
      _itemManager = itemManager;

      Name = "Crafting";

      Texture = content.Load<Texture2D>("Interface/Window2x_1y");

      _queueWindow = new QueueWindow(content, itemManager);

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
        return new Button(buttonTexture, buttonFont)
        {
          Text = c,
          Click = CategoryClicked,
        };


      }).ToList();

      LoadItems();

      SetPositions();
    }

    private void LoadItems()
    {
      _weapons = new List<Weapon>();
      _armours = new List<Armour>();

      var files = Directory.GetFiles("Content/Items", "*.json");

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

    private void CategoryClicked(object sender)
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

      var button = new ItemButton(_content.Load<Texture2D>(content), item)
      {
        Click = ItemClicked,
      };

      return button;
    }

    private void ItemClicked(object sender)
    {
      var button = sender as ItemButton;
      Console.WriteLine($"Item button '{button.Item.Name}' clicked");

      Item = button.Item;

      if (Resources.CanAfford(_itemManager.Resources, button.Item.ResourceCost))
        _itemManager.AddToQueue(button.Item);
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

      _queueWindow.Draw(gameTime, spriteBatch, graphics);
    }

    protected void DrawButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _itemSection.Matrix);

      foreach (var button in _itemSection.Items)
      {
        switch (button.CurrentState)
        {
          case ButtonStates.Nothing:

            button.Color = Color.White;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Hovering:

            button.Color = Color.YellowGreen;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Clicked:

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
          case ButtonStates.Nothing:

            button.Color = Color.White;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Hovering:

            button.Color = Color.YellowGreen;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Clicked:

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

      Position = new Vector2((screenWidth / 2) - (WindowRectangle.Width / 2), screenHeight - Texture.Height - 100);

      _queueWindow.Position = new Vector2(Position.X + this.Rectangle.Width + 10, Position.Y);
      _queueWindow.SetPositions();

      _categorySection.Area = new Rectangle((int)Position.X, (int)Position.Y + 35, 190, Texture.Height - 35);
      _categorySection.Scrollbar.Position = new Vector2((Position.X + 170), Position.Y + 35);

      _itemSection.Area = new Rectangle((int)Position.X + 190, (int)Position.Y + 35, Texture.Width - 170, Texture.Height - 35);
      _itemSection.Scrollbar.Position = new Vector2((Position.X + Texture.Width) - 20 - 10, Position.Y + 35);

      var x = _spaceBetween + (_categorySection.Items.FirstOrDefault().Rectangle.Width / 2);
      var y = Position.Y + 3;

      var categories = Enum.GetNames(typeof(ItemCategories)).ToList();

      foreach (var item in _categorySection.Items)
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

        var y = Position.Y + 3;

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

      _queueWindow.Update(gameTime);
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
          case ButtonStates.Nothing:

            if (mouseRectangleWithCamera_Items.Intersects(button.Rectangle) && mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ButtonStates.Hovering;

            break;
          case ButtonStates.Hovering:

            if (!mouseRectangleWithCamera_Items.Intersects(button.Rectangle) || !mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ButtonStates.Nothing;

            if (clicked)
            {
              foreach (var b in _itemSection.Items)
                b.CurrentState = ButtonStates.Nothing;

              button.OnClick();
            }

            break;

          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }

    public override void UnloadContent()
    {
      Texture.Dispose();

      _categorySection.UnloadContent();

      _itemSection.UnloadContent();

      _queueWindow.UnloadContent();
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
          case ButtonStates.Nothing:

            if (mouseRectangleWithCamera_Categories.Intersects(button.Rectangle) && mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ButtonStates.Hovering;

            break;
          case ButtonStates.Hovering:

            if (!mouseRectangleWithCamera_Categories.Intersects(button.Rectangle) || !mouseRectangle.Intersects(windowRectangle))
              button.CurrentState = ButtonStates.Nothing;

            if (clicked)
            {
              foreach (var b in _categorySection.Items)
                b.CurrentState = ButtonStates.Nothing;

              button.CurrentState = ButtonStates.Clicked;

              button.OnClick();
            }

            break;
          case ButtonStates.Clicked:

            //if (clicked && (mouseRectangleWithCamera_Categories.Intersects(windowRectangle)) && !_categorySection.Items.Any(c => c != button && c.Rectangle.Intersects(mouseRectangleWithCamera_Categories))) // Check if we're clicking somewhere that isn't on any button
            //{
            //  foreach (var b in _categorySection.Items)
            //    b.CurrentState = ToolbarButtonStates.Nothing;

            //  button.CurrentState = ToolbarButtonStates.Hovering;
            //}

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }
  }
}
