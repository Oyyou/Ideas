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

      var resources = CraftingItem.ResourceCost;

      spriteBatch.DrawString(_font, "Food: " + resources.Food, new Vector2(Rectangle.X + 5, Rectangle.Y + 40), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      spriteBatch.DrawString(_font, "Wood: " + resources.Wood, new Vector2(Rectangle.X + (Rectangle.Width / 2) + 5, Rectangle.Y + 40), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      spriteBatch.DrawString(_font, "Stone: " + resources.Stone, new Vector2(Rectangle.X + 5, Rectangle.Y + 65), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      spriteBatch.DrawString(_font, "Gold: " + resources.Gold, new Vector2(Rectangle.X + (Rectangle.Width / 2) + 5, Rectangle.Y + 65), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
    }
  }
}
