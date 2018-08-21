using Engine;
using Engine.Input;
using Engine.Interface.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Managers;
using VillageGUI.Interface.Buttons;
using static VillageBackend.Enums;

namespace VillageGUI.Interface.Windows
{
  public class BuildWindow : Window
  {
    private GameManagers _gameManagers;

    #region Sections
    private List<WindowSection> _sections;

    private WindowSection _categorySection;

    private WindowSection _buildingSection;
    #endregion

    private Texture2D _buttonTexture;

    private SpriteFont _buttonFont;

    public override Rectangle WindowRectangle => Rectangle;

    public BuildWindow(ContentManager content, GraphicsDevice graphicsDevice, GameManagers gameManagers) : base(content)
    {
      _gameManagers = gameManagers;

      Name = "Build";

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

      _categorySection = new WindowSection()
      {
        Scrollbar = new Scrollbar(outerTexture, innerTexture)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = new List<Button>(),
      };

      _buildingSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(outerTexture, innerTexture)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = new List<Button>(),
      };

      _sections = new List<WindowSection>()
      {
        _categorySection,
        _buildingSection,
      };

      var categories = Enum.GetNames(typeof(BuildingTypes)).ToList();

      _categorySection.Items = categories.Select(c =>
      {
        return new Button(_buttonTexture, _buttonFont)
        {
          Text = c,
          Click = CategoryClicked,
          Layer = this.Layer + 0.01f,
        };
      }).ToList();
    }

    private void CategoryClicked(object sender)
    {
      var button = sender as Button;

      var category = (BuildingTypes)Enum.Parse(typeof(BuildingTypes), button.Text);

      //_buildingSection.Items = _gameManagers.VillagerManager.Villagers.Select(c =>
      //  new Button(_buttonTexture, _buttonFont)
      //  {
      //    Layer = this.Layer + 0.01f,
      //    Text = c.Name,
      //    //Category = category,
      //    Click = BuildingClicked,
      //    //Villager = c,
      //  }
      //).ToList();

      SetSectionPositions(_buildingSection);
    }

    private void BuildingClicked(Button obj)
    {

    }

    public override void SetPositions()
    {
      var screenWidth = Game1.ScreenWidth;
      var screenHeight = Game1.ScreenHeight;

      Position = new Vector2((screenWidth / 2) - (WindowRectangle.Width / 2), 10);

      var y = (int)Position.Y + 35;

      var height = Texture.Height - 35 - 10;

      _categorySection.Area = new Rectangle((int)Position.X, y, 190, height);
      _categorySection.Scrollbar.Position = new Vector2(_categorySection.Area.X + _categorySection.Area.Width - 20, y);

      var remainingWidth = Texture.Width - (_categorySection.Area.Width) - 10;

      _buildingSection.Area = new Rectangle(_categorySection.Area.Right, y, remainingWidth, height);
      _buildingSection.Scrollbar.Position = new Vector2(_buildingSection.Area.X + _buildingSection.Area.Width - 20, y);


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

      var x = 10 + (buttonWidth / 2);
      var y = (section.Area.Y + (buttonHeight / 2)) + 3;

      foreach (var button in section.Items)
      {
        button.Position = new Vector2(x, y);
        x += button.Rectangle.Width + 10;

        if ((x + (button.Rectangle.Width / 2)) > (section.Area.Width) - 30)
        {
          x = 10 + (buttonWidth / 2);
          y += buttonHeight + 10;
        }
      }
    }

    public override void UnloadContent()
    {
      Texture.Dispose();

      foreach (var section in _sections)
        section.UnloadContent();
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

      UpdateBuildings();
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

    private void UpdateBuildings()
    {
      var translation = _buildingSection.Matrix.Translation;

      var mouseRectangle = GameMouse.Rectangle;

      var mouseRectangleWithCamera = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - (_buildingSection.Area.X)) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (_buildingSection.Area.Y)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (Button button in _buildingSection.Items)
        button.Update(mouseRectangle, (List<Button>)_buildingSection.Items, mouseRectangleWithCamera, windowRectangle);
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
