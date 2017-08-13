using Engine.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Items;

namespace TopDown.Controls.CraftingMenu
{
  public class CraftingMenuSubButton : Button
  {
    public Item CraftingItem { get; private set; }

    public CraftingMenuSubButton(Texture2D texture, SpriteFont font, Item craftingItem)
      : base(texture, font)
    {
      CraftingItem = craftingItem;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Rectangle, null, _colour, 0, new Vector2(0, 0), SpriteEffects.None, Layer);

      var text = CraftingItem.Name;

      float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(text).X / 2);
      float y = Rectangle.Y + 5;

      spriteBatch.DrawString(_font, text, new Vector2(x, y), Color.Red, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);

      var resources = CraftingItem.ResourceCost.GetContent();

      var x1 = Rectangle.X + 5;
      var y1 = Rectangle.Y + 30;

      spriteBatch.DrawString(_font, "Resources:", new Vector2(x1, y1), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);

      foreach (var resource in resources)
      {
        spriteBatch.DrawString(_font, resource.Key + ": " + resource.Value, new Vector2(x1, y1 += 25), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      }

      var stats = CraftingItem.Stats.GetContent();

      var x2 = Rectangle.X + (Rectangle.Width / 2) + 5;
      var y2 = Rectangle.Y + 30;

      spriteBatch.DrawString(_font, "Stats:", new Vector2(x2, y2), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);

      foreach (var stat in stats)
      {
        spriteBatch.DrawString(_font, stat.Key + ": " + stat.Value, new Vector2(x2, y2 += 25), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      }
    }
  }
}
