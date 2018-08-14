using Engine.Interface.Windows;
using VillageGUI.Interface.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Managers;
using Engine.Input;
using VillageGUI.Interface.Panels;

namespace VillageGUI.Interface.Windows
{
  public class JobsWindow : Window
  {
    private Texture2D _buttonTexture;

    private Texture2D _villagerInfoTexture;

    private SpriteFont _buttonFont;

    private GameManagers _gameManagers;

    public override Rectangle WindowRectangle => Rectangle;

    #region Sections
    private WindowSection _jobsSection;

    private WindowSection _villagerSection;

    #endregion

    public JobsWindow(ContentManager content, GameManagers gameManager)
      : base(content)
    {
      _gameManagers = gameManager;

      Name = "Jobs";

      Texture = content.Load<Texture2D>("Interface/Window2x_1y");

      _buttonTexture = content.Load<Texture2D>("Interface/Button");
      _buttonFont = content.Load<SpriteFont>("Fonts/Font");

      _villagerInfoTexture = content.Load<Texture2D>("Interface/VillagerInfo");

      _gameManagers.JobManager.Jobs.CollectionChanged += Jobs_CollectionChanged;
      _gameManagers.VillagerManager.Villagers.CollectionChanged += Villagers_CollectionChanged; ;

      _jobsSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = _gameManagers.JobManager.Jobs.Select(c =>
          {
            var button = new JobButton(_buttonTexture, _buttonFont)
            {
              Text = c.Name,
              Click = JobClicked,
              Layer = this.Layer + 0.01f,
              Job = c,
            };

            return button;

          }).ToList(),
      };

      _villagerSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = _gameManagers.VillagerManager.Villagers.Select(c =>
        {
          return new VillagerInformationPanel(_content, _gameManagers)
          {
            //Text = c.Name,
            //Click = JobClicked,
            Layer = this.Layer + 0.01f,
            Villager = c,
          };
        }).ToList(),
      };

      SetPositions();
    }

    private void Jobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      _jobsSection.Items = _gameManagers.JobManager.Jobs.Select(c =>
      {
        return new JobButton(_buttonTexture, _buttonFont)
        {
          Text = c.Name,
          Click = JobClicked,
          Layer = this.Layer + 0.01f,
          Job = c,
        };
      }).ToList();

      SetPositions();
    }

    private void Villagers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      _villagerSection.Items = _gameManagers.VillagerManager.Villagers.Select(c =>
      {
        return new VillagerInformationPanel(_content, _gameManagers)
        {
          Layer = this.Layer + 0.01f,
          Villager = c,
        };
      }).ToList();

      SetPositions();
    }

    private void JobClicked(object obj)
    {
      var jobButton = obj as JobButton;

      _gameManagers.JobManager.SelectedJob = jobButton.Job;
    }

    public override void SetPositions()
    {
      Position = new Vector2((Game1.ScreenWidth / 2) - (WindowRectangle.Width / 2),
        Game1.ScreenHeight - Texture.Height - 100);

      _jobsSection.Area = new Rectangle((int)Position.X, (int)Position.Y + 35, 190, Texture.Height - 35);
      _jobsSection.Scrollbar.Position = new Vector2((Position.X + 170), Position.Y + 35);

      var jobItem = _jobsSection.Items.FirstOrDefault();

      if (jobItem != null)
      {
        var buttonHeight = jobItem.Rectangle.Height;
        var buttonWidth = jobItem.Rectangle.Width;

        var x = 10 + (buttonWidth / 2);
        var y = (_jobsSection.Area.Y + (buttonHeight / 2)) + 3;

        foreach (var item in _jobsSection.Items)
        {
          item.Position = new Vector2(x, y);
          y += item.Rectangle.Height + 5;
        }
      }

      _villagerSection.Area = new Rectangle((int)Position.X + 190, (int)Position.Y + 35, Texture.Width - 170, Texture.Height - 35);
      _villagerSection.Scrollbar.Position = new Vector2((Position.X + Texture.Width) - 20 - 10, Position.Y + 35);

      var villagerItem = _villagerSection.Items.FirstOrDefault();

      if (villagerItem != null)
      {
        var buttonHeight = villagerItem.Rectangle.Height;
        var buttonWidth = villagerItem.Rectangle.Width;

        var x = 10 + (buttonWidth / 2);
        var y = (_villagerSection.Area.Y + (buttonHeight / 2)) + 3;

        foreach (var button in _villagerSection.Items)
        {
          button.Position = new Vector2(x, y);
          x += button.Rectangle.Width + 10;

          if ((x + (button.Rectangle.Width / 2)) > (_villagerSection.Area.Width))
          {
            x = 10 + (buttonWidth / 2);
            y += buttonHeight + 10;
          }
        }
      }
    }

    public override void UnloadContent()
    {
      Texture.Dispose();

      _jobsSection.UnloadContent();
      _villagerSection.UnloadContent();
    }

    #region Update

    public override void Update(GameTime gameTime)
    {
      _hasUpdated = true;

      _jobsSection.Scrollbar.Update(gameTime);
      _villagerSection.Scrollbar.Update(gameTime);

      if (GameMouse.Rectangle.Intersects(this.WindowRectangle))
      {
        GameMouse.AddObject(this);
      }
      else
      {
        GameMouse.ClickableObjects.Remove(this);
      }

      UpdateJobButtons();
      UpdateVillagerButtons();
    }

    private void UpdateJobButtons()
    {
      var translation = _jobsSection.Matrix.Translation;

      var mouseRectangleWithCamera_Jobs = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - Position.X) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (JobButton button in _jobsSection.Items)
      {
        button.Update(GameMouse.Rectangle, (List<JobButton>)_jobsSection.Items, mouseRectangleWithCamera_Jobs, windowRectangle);
      }
    }

    private void UpdateVillagerButtons()
    {
      var translation = _villagerSection.Matrix.Translation;

      var mouseRectangleWithCamera_Items = new Rectangle(
        (int)((GameMouse.CurrentMouse.X - (Position.X + 190)) - translation.X),
        (int)((GameMouse.CurrentMouse.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (VillagerInformationPanel panel in _villagerSection.Items)
      {
        panel.Update(GameMouse.Rectangle, mouseRectangleWithCamera_Items, windowRectangle);
      }
    }

    #endregion

    #region Draw

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      if (!_hasUpdated)
        return;

      var original = graphics.GraphicsDevice.Viewport;

      DrawWindow(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport(_jobsSection.Area);

      DrawJobButtons(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport(_villagerSection.Area);

      DrawVillagerButtons(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = original;
    }

    protected void DrawWindow(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);

      _jobsSection.Scrollbar.Draw(gameTime, spriteBatch);

      _villagerSection.Scrollbar.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, Name, new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

      spriteBatch.End();
    }

    protected void DrawJobButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _jobsSection.Matrix);

      foreach (var button in _jobsSection.Items)
        button.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }

    protected void DrawVillagerButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _villagerSection.Matrix);

      foreach (var button in _villagerSection.Items)
        button.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }

    #endregion
  }
}
