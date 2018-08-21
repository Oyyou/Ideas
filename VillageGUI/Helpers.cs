using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageGUI
{
  public static class Helpers
  {
    public static void SetTexture(Texture2D texture, Color backgroundColour, Color borderColour, int borderWidth = 2)
    {
      var colours = new Color[texture.Width * texture.Height];

      int index = 0;

      for (int y = 0; y < texture.Height; y++)
      {
        for (int x = 0; x < texture.Width; x++)
        {
          var colour = backgroundColour;

          if (x < borderWidth || x > (texture.Width - 1) - borderWidth ||
             y < borderWidth || y > (texture.Height - 1) - borderWidth)
            colour = borderColour;

          colours[index++] = colour;
        }
      }

      texture.SetData(colours);
    }
  }
}
