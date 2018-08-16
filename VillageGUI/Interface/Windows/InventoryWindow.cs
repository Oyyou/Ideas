using Engine.Interface.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageGUI.Interface.Windows
{
  public class InventoryWindow : Window
  {
    public override Rectangle WindowRectangle => Rectangle;

    public InventoryWindow(ContentManager content) : base(content)
    {
      Name = "Inventory";

      Texture = content.Load<Texture2D>("Interface/Window2x_1y");

      SetPositions();
    }

    public override void SetPositions()
    {
      Position = new Vector2((Game1.ScreenWidth / 2) - (WindowRectangle.Width / 2),
        Game1.ScreenHeight - Texture.Height - 100);
    }

    public override void UnloadContent()
    {
      Texture.Dispose();
    }

    public override void Update(GameTime gameTime)
    {
      _hasUpdated = true;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      if (!_hasUpdated)
        return;

      var original = graphics.GraphicsDevice.Viewport;

      DrawWindow(gameTime, spriteBatch);

      graphics.GraphicsDevice.Viewport = original;
    }

    protected void DrawWindow(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);

      spriteBatch.DrawString(_font, Name, new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

      spriteBatch.End();
    }
  }
}
