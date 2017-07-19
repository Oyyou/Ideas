using Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TopDown.Controls.ItemMenu;

namespace TopDown.Controls.BuildMenu
{
  public class BuildMenuSubButton : Button
  {
    public States.GameStates GameScreenSetValue { get; set; }

    public List<ItemMenuButton> Items { get; set; }

    public Models.Resources ResourceCost;

    public BuildMenuSubButton(Texture2D texture, SpriteFont font)
      : base(texture, font)
    {
      Items = new List<ItemMenuButton>();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Rectangle, null, _colour, 0, new Vector2(0, 0), SpriteEffects.None, Layer);

      if (!string.IsNullOrEmpty(Text) && _font != null)
      {
        float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
        float y = Rectangle.Y + 5;

        spriteBatch.DrawString(_font, Text, new Vector2(x, y), Color.Red, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);

        spriteBatch.DrawString(_font, "Food: " + ResourceCost.Food, new Vector2(Rectangle.X + 5, Rectangle.Y + 40), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
        spriteBatch.DrawString(_font, "Wood: " + ResourceCost.Wood, new Vector2(Rectangle.X + (Rectangle.Width / 2) + 5, Rectangle.Y + 40), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
        spriteBatch.DrawString(_font, "Stone: " + ResourceCost.Stone, new Vector2(Rectangle.X + 5, Rectangle.Y + 65), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
        spriteBatch.DrawString(_font, "Gold: " + ResourceCost.Gold, new Vector2(Rectangle.X + (Rectangle.Width / 2) + 5, Rectangle.Y + 65), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      }

    }
  }
}
