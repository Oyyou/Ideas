using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VillageBackend.Models;

namespace VillageGUI.Interface.Buttons
{
  public class QueueButton : Button
  {
    public ItemV2 Item { get; private set; }

    private Texture2D _progressTexture;

    public QueueButton(Texture2D texture, Texture2D progressTexture, ItemV2 item) : base(texture)
    {
      _progressTexture = progressTexture;

      Item = item;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      switch (this.CurrentState)
      {
        case ButtonStates.Nothing:

          this.Color = Color.White;

          this.Scale = 1.0f;

          break;
        case ButtonStates.Hovering:

          this.Color = Color.YellowGreen;

          this.Scale = 1.0f;

          break;
        case ButtonStates.Clicked:

          DrawClicked();

          break;
        default:
          throw new Exception("Unknown ToolbarButtonState: " + this.CurrentState.ToString());
      }

      spriteBatch.Draw(Texture, Position, null, Color, 0f, Origin, Scale, SpriteEffects.None, Layer);
      spriteBatch.Draw(_progressTexture, Position, new Rectangle(0, 0, Texture.Width, (int)(Texture.Height * (Item.CraftingTime / Item.CraftTime))), Color, 0f, Origin, Scale, SpriteEffects.None, Layer + 0.01f);

      DrawText(spriteBatch);
    }
  }
}
