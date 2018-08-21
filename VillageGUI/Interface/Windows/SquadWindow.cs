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
using VillageGUI.Interface.Panels;

namespace VillageGUI.Interface.Windows
{
  public class SquadWindow : Window
  {
    private GraphicsDevice _graphicsDevice;

    private List<WindowSection> _sections;

    private SquadManager _squadManager;

    private WindowSection _squadSection;

    public override Rectangle WindowRectangle => Rectangle;

    public SquadWindow(ContentManager content, GraphicsDevice graphicsDevice, SquadManager squadManager) : base(content)
    {
      _graphicsDevice = graphicsDevice;

      _sections = new List<WindowSection>();

      _squadManager = squadManager;

      Name = "Squads";

      var width = GameEngine.ScreenWidth - 20;
      var height = GameEngine.ScreenHeight - 20 - 100;

      Texture = new Texture2D(graphicsDevice, width, height);

      var outerTexture = new Texture2D(graphicsDevice, 20, height - 35 - 10); // 35 is space at top, 10 is space at bottom
      var innerTexture = new Texture2D(graphicsDevice, 14, 1);

      Helpers.SetTexture(Texture, new Color(43, 43, 43, 200), new Color(0, 0, 0, 200));
      Helpers.SetTexture(outerTexture, new Color(43, 43, 43), new Color(0, 0, 0));
      Helpers.SetTexture(innerTexture, new Color(69, 69, 69), new Color(0, 0, 0), 0);

      _squadSection = new WindowSection()
      {
        Scrollbar = new Scrollbar(outerTexture, innerTexture)
        {
          Layer = this.Layer + 0.01f,
        },
        Items = new List<SquadPanel>(),
      };

      _sections = new List<WindowSection>()
      {
        _squadSection,
      };
    }

    public override void SetPositions()
    {
      var screenWidth = Game1.ScreenWidth;
      var screenHeight = Game1.ScreenHeight;

      Position = new Vector2((screenWidth / 2) - (WindowRectangle.Width / 2), 10);

      var y = (int)Position.Y + 35;

      var height = Texture.Height - 35 - 10;

      _squadSection.Area = new Rectangle((int)Position.X, y, Texture.Width - 20, height);
      _squadSection.Scrollbar.Position = new Vector2(_squadSection.Area.X + _squadSection.Area.Width - 20, y);


      var t = _squadManager.Squads.Select(c => new SquadPanel(_content, _graphicsDevice, c, _squadSection)).ToList();
      t.Add(new SquadPanel(_content, _graphicsDevice, null, _squadSection));

      _squadSection.Items = t;

      SetSectionPositions(_squadSection);
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
