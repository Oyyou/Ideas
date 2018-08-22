using Engine;
using Engine.Input;
using Engine.Interface.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Managers;
using VillageBackend.Models;
using VillageGUI.Interface.Buttons;
using static VillageBackend.Enums;

namespace VillageGUI.Interface.Windows
{
  public class InventoryWindow : Window
  {
    public override Rectangle WindowRectangle => Rectangle;

    private Texture2D _buttonTexture;

    private SpriteFont _buttonFont;

    private ItemManager _itemManager;

    #region Section

    private WindowSection _leftSection;

    private WindowSection _rightSection;

    #endregion

    public InventoryWindow(ContentManager content, GraphicsDevice graphicsDevice, ItemManager itemManager) : base(content)
    {
      _itemManager = itemManager;

      Name = "Inventory";

      var width = GameEngine.ScreenWidth - 20;
      var height = GameEngine.ScreenHeight - 20 - 100;

      Texture = new Texture2D(graphicsDevice, width, height);// content.Load<Texture2D>("Interface/Window2x_1y");

      var outerTexture = new Texture2D(graphicsDevice, 20, height - 35 - 10); // 35 is space at top, 10 is space at bottom
      var innerTexture = new Texture2D(graphicsDevice, 14, 1);

      Helpers.SetTexture(Texture, new Color(43, 43, 43, 200), new Color(0, 0, 0, 200));
      Helpers.SetTexture(outerTexture, new Color(43, 43, 43), new Color(0, 0, 0));
      Helpers.SetTexture(innerTexture, new Color(69, 69, 69), new Color(0, 0, 0), 0);

      _buttonTexture = content.Load<Texture2D>("Interface/Button");
      _buttonFont = content.Load<SpriteFont>("Fonts/Font");

      var categories = Enum.GetNames(typeof(ItemCategories)).ToList();

      _leftSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(outerTexture, innerTexture)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = categories.Select(c =>
        {
          return new Button(_buttonTexture, _buttonFont)
          {
            Text = c,
            Click = CategoryClicked,
          };
        }).ToList(),
      };

      _rightSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(outerTexture, innerTexture)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = new List<Button>(),
      };

      SetPositions();
    }

    private void CategoryClicked(Button button)
    {
      var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), button.Text);

      _rightSection.Items = _itemManager.Items
        .Where(c => c.Category == category)
        .Select(c => GetItemButton(c)).ToList();

      SetSectionPositions(_rightSection);
    }

    private ItemButton GetItemButton(ItemV2 item)
    {
      var fullPath = $"{Directory.GetCurrentDirectory()}\\Content\\Interface\\ItemIcons\\{item.Name}.xnb";

      string content = "Interface/NoImage";

      if (File.Exists(fullPath))
        content = "Interface/ItemIcons/" + item.Name;

      var button = new ItemButton(_content.Load<Texture2D>(content), item)
      {
        //Click = ItemClicked,
        Layer = this.Layer + 0.01f,
      };

      return button;
    }

    public override void SetPositions()
    {
      var screenWidth = Game1.ScreenWidth;
      var screenHeight = Game1.ScreenHeight;

      Position = new Vector2((screenWidth / 2) - (WindowRectangle.Width / 2), 10);

      var sectionY = (int)Position.Y + 35;

      var height = Texture.Height - 35 - 10;

      _leftSection.Area = new Rectangle((int)Position.X, sectionY, 190, height);
      _leftSection.Scrollbar.Position = new Vector2(_leftSection.Area.X + _leftSection.Area.Width - 20, sectionY);

      _rightSection.Area = new Rectangle(_leftSection.Area.Right, sectionY, Texture.Width - _leftSection.Area.Width - 10, height);
      _rightSection.Scrollbar.Position = new Vector2(_rightSection.Area.X + _rightSection.Area.Width - 20, sectionY);

      var x = 10 + (_leftSection.Items.FirstOrDefault().Rectangle.Width / 2);
      var y = (_leftSection.Area.Y + (_leftSection.Items.FirstOrDefault().Rectangle.Height / 2)) + 3;

      foreach (var item in _leftSection.Items)
      {
        item.Position = new Vector2(x, y);
        y += item.Rectangle.Height + 5;
      }

      SetSectionPositions(_leftSection);
      SetSectionPositions(_rightSection);
    }

    private void SetSectionPositions(WindowSection section)
    {
      if (section.Items == null)
        return;

      if (section.Items.Count() == 0)
        return;

      var spaceBetween = 10;

      var buttonHeight = section.Items.FirstOrDefault().Rectangle.Height;
      var buttonWidth = section.Items.FirstOrDefault().Rectangle.Width;

      var x = spaceBetween + (buttonWidth / 2);
      var y = (section.Area.Y + (buttonHeight / 2)) + 3;

      foreach (var button in section.Items)
      {
        button.Position = new Vector2(x, y);
        x += button.Rectangle.Width + spaceBetween;

        if ((x + (button.Rectangle.Width / 2)) > (section.Area.Right) - 30)
        {
          x = spaceBetween + (buttonWidth / 2);
          y += buttonHeight + spaceBetween;
        }
      }
    }

    public override void UnloadContent()
    {
      Texture.Dispose();
    }

    public override void Update(GameTime gameTime)
    {
      _hasUpdated = true;

      _leftSection.Scrollbar.Update(gameTime);
      _rightSection.Scrollbar.Update(gameTime);

      UpdateLeftSection();

      UpdateRightSection();
    }

    private void UpdateLeftSection()
    {
      var translation = _leftSection.Matrix.Translation;

      var mouseRectangle = GameMouse.Rectangle;

      var mouseRectangleWithCamera_Categories = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - Position.X) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (Button button in _leftSection.Items)
        button.Update(mouseRectangle, (List<Button>)_leftSection.Items, mouseRectangleWithCamera_Categories, windowRectangle);
    }

    private void UpdateRightSection()
    {
      var translation = _rightSection.Matrix.Translation;

      var mouseRectangle = GameMouse.Rectangle;

      var mouseRectangleWithCamera_Items = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - (Position.X + 190)) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (ItemButton button in _rightSection.Items)
        button.Update(mouseRectangle, (List<ItemButton>)_rightSection.Items, mouseRectangleWithCamera_Items, windowRectangle);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      if (!_hasUpdated)
        return;

      var original = graphics.GraphicsDevice.Viewport;

      DrawWindow(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport((int)Position.X, (int)Position.Y + 35, 190, Texture.Height - 35);

      DrawLeftSection(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport((int)Position.X + 190, (int)Position.Y + 35, Texture.Width - 170, Texture.Height - 35);

      DrawRightSection(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = original;
    }

    protected void DrawWindow(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);

      _leftSection.Scrollbar.Draw(gameTime, spriteBatch);

      _rightSection.Scrollbar.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, Name, new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

      spriteBatch.End();
    }

    protected void DrawLeftSection(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _leftSection.Matrix);

      foreach (var button in _leftSection.Items)
        button.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }

    protected void DrawRightSection(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _rightSection.Matrix);

      foreach (var button in _rightSection.Items)
        button.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }
  }
}
