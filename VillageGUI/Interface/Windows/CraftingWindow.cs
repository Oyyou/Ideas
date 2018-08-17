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
using Engine.Input;

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

        return new Vector2(0, MathHelper.Clamp(-Scrollbar._innerY, -1000, 2400));
      }
    }

    public IEnumerable<Control> Items;

    public void UnloadContent()
    {
      foreach (var item in Items)
        item.UnloadContent();
    }
  }

  public class CraftingWindow : Window
  {
    private GameManagers _gameManagers;

    #region Items
    private List<Weapon> _weapons;

    private List<Armour> _armours;
    #endregion

    #region Sections
    private List<WindowSection> _sections;

    private WindowSection _categorySection;

    private WindowSection _villagerSection;

    private WindowSection _itemSection;

    private WindowSection _queueSection;
    #endregion

    private const int _spaceBetween = 10;

    //private QueueWindow _queueWindow;

    private Texture2D _buttonTexture;
    private SpriteFont _buttonFont;

    /// <summary>
    /// The item to be created
    /// </summary>
    public ItemV2 Item { get; private set; }

    public override Rectangle WindowRectangle { get => Rectangle; }

    public CraftingWindow(ContentManager content, GraphicsDevice graphicsDevice, GameManagers gameManagers) : base(content)
    {
      _gameManagers = gameManagers;

      Name = "Crafting";

      var width = 750;
      var height = 360;

      Texture = new Texture2D(graphicsDevice, width, height);// content.Load<Texture2D>("Interface/Window2x_1y");

      var colours = new Color[width * height];

      var boarderWidth = 2;

      int index = 0;

      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          var colour = new Color(43, 43, 43, 200);

          if (x < boarderWidth || x > (width - 1) - boarderWidth ||
             y < boarderWidth || y > (height - 1) - boarderWidth)
            colour = new Color(0, 0, 0, 200);

          colours[index++] = colour;
        }
      }

      Texture.SetData(colours);

      _buttonTexture = content.Load<Texture2D>("Interface/Button");
      _buttonFont = content.Load<SpriteFont>("Fonts/Font");

      //_queueWindow = new QueueWindow(content, _gameManagers.ItemManager);

      _categorySection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = new List<Button>(),
      };

      _villagerSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = new List<Button>(),
      };

      _itemSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = new List<Button>(),
      };

      _queueSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = new List<Button>(),
      };

      _sections = new List<WindowSection>()
      {
        _categorySection,
        _villagerSection,
        _itemSection,
        _queueSection,
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
          Layer = this.Layer + 0.01f,
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

    public override void UnloadContent()
    {
      Texture.Dispose();

      foreach (var section in _sections)
        section.UnloadContent();
    }

    private void CategoryClicked(object sender)
    {
      var button = sender as Button;

      var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), button.Text);

      _villagerSection.Items = _gameManagers.VillagerManager.Villagers.Select(c =>
        new VillagerButton(_buttonTexture, _buttonFont)
        {
          Layer = this.Layer + 0.01f,
          Text = c.Name,
          Category = category,
          Click = VillagerClicked,
          Villager = c,
        }
      ).ToList();

      _itemSection.Items = new List<ItemButton>();
      _queueSection.Items = new List<ItemButton>();

      SetSectionPositions(_villagerSection);
    }

    private void VillagerClicked(Button button)
    {
      var villagerButton = button as VillagerButton;

      var villagerId = villagerButton.Villager.Id;

      _itemSection.Items = new List<Button>();

      var category = villagerButton.Category;

      switch (category)
      {
        case ItemCategories.Weapon:
          _itemSection.Items = _weapons
            .Where(c => _gameManagers.ItemManager.CanCraft(villagerButton.Villager, c))
            .Select(c =>
          {
            return GetItemButton(c);
          }).ToList();
          break;
        case ItemCategories.Armour:
          _itemSection.Items = _armours
            .Where(c => _gameManagers.ItemManager.CanCraft(villagerButton.Villager, c))
            .Select(c =>
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

      _villagerId = villagerId;

      _queueSection.Items = _gameManagers.ItemManager.QueuedItems
        .Where(c => c.CrafterId == villagerId)
        .Select(c => GetQueueButton(c)).ToList();

      SetSectionPositions(_itemSection);
      SetSectionPositions(_queueSection);
    }

    private ItemButton GetItemButton(ItemV2 item)
    {
      var button = new ItemButton(GetItemTexture(item), item)
      {
        Click = ItemClicked,
        Layer = this.Layer + 0.01f,
      };

      return button;
    }

    private ItemButton GetQueueButton(ItemV2 item)
    {
      var button = new ItemButton(GetItemTexture(item), item)
      {
        Click = QueueClicked,
        Layer = this.Layer + 0.01f,
      };

      return button;
    }

    private Texture2D GetItemTexture(ItemV2 item)
    {
      var fullPath = $"{Directory.GetCurrentDirectory()}\\Content\\Interface\\ItemIcons\\{item.Name}.xnb";

      string content = "Interface/NoImage";

      if (File.Exists(fullPath))
        content = "Interface/ItemIcons/" + item.Name;

      var texture = _content.Load<Texture2D>(content);
      return texture;
    }

    private int _villagerId;
    private void ItemClicked(object sender)
    {
      var button = sender as ItemButton;
      Console.WriteLine($"Item button '{button.Item.Name}' clicked");

      Item = button.Item;
      Item.CrafterId = _villagerId;

      if (Resources.CanAfford(_gameManagers.ItemManager.Resources, Item.ResourceCost))
        _gameManagers.ItemManager.AddToQueue(Item.Clone() as ItemV2);

      _queueSection.Items = _gameManagers.ItemManager.QueuedItems
        .Where(c => c.CrafterId == _villagerId)
        .Select(c => GetQueueButton(c)).ToList();

      SetSectionPositions(_queueSection);
    }

    private void QueueClicked(object sender)
    {
      var button = sender as ItemButton;

      Item = button.Item;

      _gameManagers.ItemManager.QueuedItems.Remove(Item);

      _queueSection.Items = _gameManagers.ItemManager.QueuedItems
        .Where(c => c.CrafterId == _villagerId)
        .Select(c => GetQueueButton(c)).ToList();

      SetSectionPositions(_queueSection);
    }

    public override void SetPositions()
    {
      var screenWidth = Game1.ScreenWidth;
      var screenHeight = Game1.ScreenHeight;

      Position = new Vector2((screenWidth / 2) - (WindowRectangle.Width / 2), screenHeight - Texture.Height - 100);

      var y = (int)Position.Y + 35;

      var height = Texture.Height - 35;

      _categorySection.Area = new Rectangle((int)Position.X, y, 190, height);
      _categorySection.Scrollbar.Position = new Vector2(_categorySection.Area.X + _categorySection.Area.Width - 20, y);

      _villagerSection.Area = new Rectangle(_categorySection.Area.Right, y, 190, height);
      _villagerSection.Scrollbar.Position = new Vector2(_villagerSection.Area.X + _villagerSection.Area.Width - 20, y);

      _itemSection.Area = new Rectangle(_villagerSection.Area.Right, y, 210, height);
      _itemSection.Scrollbar.Position = new Vector2(_itemSection.Area.X + _itemSection.Area.Width - 20, y);

      _queueSection.Area = new Rectangle(_itemSection.Area.Right, y, Texture.Width - (_categorySection.Area.Width + _villagerSection.Area.Width + _itemSection.Area.Width) - 10, height);
      _queueSection.Scrollbar.Position = new Vector2(_queueSection.Area.X + _queueSection.Area.Width - 20, y);

      foreach (var section in _sections)
        SetSectionPositions(section);
    }

    private void SetSectionPositions(WindowSection section)
    {
      if (section.Items == null)
        return;

      if (section.Items.Count() == 0)
        return;

      var buttonHeight = section.Items.FirstOrDefault().Rectangle.Height;
      var buttonWidth = section.Items.FirstOrDefault().Rectangle.Width;

      var x = _spaceBetween + (buttonWidth / 2);
      var y = (section.Area.Y + (buttonHeight / 2)) + 3;

      foreach (var button in section.Items)
      {
        button.Position = new Vector2(x, y);
        x += button.Rectangle.Width + _spaceBetween;

        if ((x + (button.Rectangle.Width / 2)) > (section.Area.Width))
        {
          x = _spaceBetween + (buttonWidth / 2);
          y += buttonHeight + _spaceBetween;
        }
      }
    }

    public override void Update(GameTime gameTime)
    {
      _hasUpdated = true;

      foreach (var section in _sections)
        section.Scrollbar.Update(gameTime);

      if (GameMouse.Rectangle.Intersects(this.WindowRectangle))
      {
        GameMouse.AddObject(this);
      }
      else
      {
        GameMouse.ClickableObjects.Remove(this);
      }

      UpdateCategories();

      UpdateVillagers();

      UpdateItems();

      UpdateQueue();
    }

    private void UpdateCategories()
    {
      var translation = _categorySection.Matrix.Translation;

      var mouseRectangle = GameMouse.Rectangle;

      var mouseRectangleWithCamera = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - (_categorySection.Area.X)) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (_categorySection.Area.Y)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (Button button in _categorySection.Items)
        button.Update(mouseRectangle, (List<Button>)_categorySection.Items, mouseRectangleWithCamera, windowRectangle);
    }

    private void UpdateVillagers()
    {
      var translation = _villagerSection.Matrix.Translation;

      var mouseRectangle = GameMouse.Rectangle;

      var mouseRectangleWithCamera = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - (_villagerSection.Area.X)) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (_villagerSection.Area.Y)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (VillagerButton button in _villagerSection.Items)
        button.Update(mouseRectangle, (List<VillagerButton>)_villagerSection.Items, mouseRectangleWithCamera, windowRectangle);
    }

    private void UpdateItems()
    {
      var translation = _itemSection.Matrix.Translation;

      var mouseRectangle = GameMouse.Rectangle;

      var mouseRectangleWithCamera_Items = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - (_itemSection.Area.X)) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (_itemSection.Area.Y)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (ItemButton button in _itemSection.Items)
        button.Update(mouseRectangle, (List<ItemButton>)_itemSection.Items, mouseRectangleWithCamera_Items, windowRectangle);
    }

    private void UpdateQueue()
    {
      var translation = _queueSection.Matrix.Translation;

      var mouseRectangle = GameMouse.Rectangle;

      var mouseRectangleWithCamera = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - (_queueSection.Area.X)) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (_queueSection.Area.Y)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (ItemButton button in _queueSection.Items)
        button.Update(mouseRectangle, (List<ItemButton>)_queueSection.Items, mouseRectangleWithCamera, windowRectangle);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      if (!_hasUpdated)
        return;

      var original = graphics.GraphicsDevice.Viewport;

      DrawWindow(gameTime, spriteBatch);

      foreach (var section in _sections)
      {
        graphics.GraphicsDevice.Viewport = new Viewport(section.Area);

        spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: section.Matrix);

        foreach (var button in section.Items)
          button.Draw(gameTime, spriteBatch);

        spriteBatch.End();
      }

      graphics.GraphicsDevice.Viewport = original;
    }

    protected void DrawWindow(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

      foreach (var section in _sections)
        section.Scrollbar.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, Name, new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);

      spriteBatch.End();
    }
  }
}
