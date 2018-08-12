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

namespace VillageGUI.Interface.Windows
{
  public class JobsWindow : Window
  {
    private Texture2D _buttonTexture;

    private SpriteFont _buttonFont;

    private JobManager _jobManager;

    private VillagerManager _villagerManager;

    public override Rectangle WindowRectangle => Rectangle;

    #region Sections
    private WindowSection _jobsSection;

    private WindowSection _villagerSection;

    #endregion

    public JobsWindow(ContentManager content, JobManager jobManager, VillagerManager villagerManager) 
      : base(content)
    {
      _jobManager = jobManager;

      _villagerManager = villagerManager;

      Name = "Jobs";

      Texture = content.Load<Texture2D>("Interface/Window2x_1y");

      _buttonTexture = content.Load<Texture2D>("Interface/Button");
      _buttonFont = content.Load<SpriteFont>("Fonts/Font");

      _jobManager.Jobs.CollectionChanged += Jobs_CollectionChanged;
      _villagerManager.Villagers.CollectionChanged += Villagers_CollectionChanged; ;

      _jobsSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content),
        Items = _jobManager.Jobs.Select(c =>
          {
            var button = new Button(_buttonTexture, _buttonFont)
            {
              Text = c.Name,
              Click = JobClicked
            };

            return button;

          }).ToList(),
      };

      _villagerSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(content),
        Items = _villagerManager.Villagers.Select(c =>
        {
          return new Button(_buttonTexture, _buttonFont)
          {
            Text = c.Name,
            Click = JobClicked
          };
        }).ToList(),
      };

      SetPositions();
    }

    private void Villagers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      _villagerSection.Items = _villagerManager.Villagers.Select(c =>
      {
        return new Button(_buttonTexture, _buttonFont)
        {
          Text = c.Name,
          Click = JobClicked
        };
      }).ToList();

      SetPositions();
    }

    private void Jobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      _jobsSection.Items = _jobManager.Jobs.Select(c =>
        {
          var button = new Button(_buttonTexture, _buttonFont)
          {
            Text = c.Name,
            Click = JobClicked
          };

          return button;

        }).ToList();

      SetPositions();
    }

    private void JobClicked(object obj)
    {

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
        var x = 10 + (_jobsSection.Items.FirstOrDefault().Rectangle.Width / 2);
        var y = Position.Y + 3;

        foreach (var item in _jobsSection.Items)
        {
          item.Position = new Vector2(x, y);
          y += item.Rectangle.Height + 5;
        }
      }

      _villagerSection.Area = new Rectangle((int)Position.X + 190, (int)Position.Y + 35, Texture.Width - 170, Texture.Height - 35);
      _villagerSection.Scrollbar.Position = new Vector2((Position.X + Texture.Width) - 20 - 10, Position.Y + 35);
      
      if (_villagerSection.Items.Count() > 0)
      {
        var buttonHeight = _villagerSection.Items.FirstOrDefault().Texture.Height;
        var buttonWidth = _villagerSection.Items.FirstOrDefault().Texture.Width;

        var x = 10 + (buttonWidth / 2);

        var y = Position.Y + 3;

        foreach (var button in _villagerSection.Items)
        {
          button.Position = new Vector2(x, y);
          x += button.Texture.Width + 10;

          if ((x + (button.Rectangle.Width / 2)) > (_villagerSection.Area.X))
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

    public override void Update(GameTime gameTime)
    {
      _hasUpdated = true;

      _jobsSection.Scrollbar.Update(gameTime);
      _villagerSection.Scrollbar.Update(gameTime);

      _previousMouseState = _currentMouseState;
      _currentMouseState = Mouse.GetState();

      UpdateJobButtons();
      UpdateVillagerButtons();
    }

    private void UpdateJobButtons()
    {
      var translation = _jobsSection.Matrix.Translation;

      var mouseRectangle = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);

      var mouseRectangleWithCamera_Categories = new Rectangle(
        (int)((_currentMouseState.X - Position.X) - translation.X),
        (int)((_currentMouseState.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (var button in _jobsSection.Items)
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
              foreach (var b in _jobsSection.Items)
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

    private void UpdateVillagerButtons()
    {
      var translation = _villagerSection.Matrix.Translation;

      var mouseRectangle = new Rectangle(_currentMouseState.Position.X, _currentMouseState.Position.Y, 1, 1);

      var mouseRectangleWithCamera_Items = new Rectangle(
        (int)((_currentMouseState.X - (Position.X + 190)) - translation.X),
        (int)((_currentMouseState.Y - (Position.Y + 35)) - translation.Y),
        1,
        1
      );

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

      var windowRectangle = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

      foreach (var button in _villagerSection.Items)
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
              foreach (var b in _villagerSection.Items)
                b.CurrentState = ButtonStates.Nothing;

              button.OnClick();
            }

            break;

          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      if (!_hasUpdated)
        return;

      var original = graphics.GraphicsDevice.Viewport;

      DrawWindow(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport((int)Position.X, (int)Position.Y + 35, 190, Texture.Height - 35);

      DrawJobButtons(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = new Viewport((int)Position.X + 190, (int)Position.Y + 35, Texture.Width - 170, Texture.Height - 35);

      DrawVillagerButtons(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = original;
    }

    protected void DrawWindow(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
      
      _jobsSection.Scrollbar.Draw(gameTime, spriteBatch);

      _villagerSection.Scrollbar.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, Name, new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);

      spriteBatch.End();
    }

    protected void DrawJobButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _jobsSection.Matrix);

      foreach (var button in _jobsSection.Items)
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

    protected void DrawVillagerButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: _villagerSection.Matrix);

      foreach (var button in _villagerSection.Items)
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
  }
}
