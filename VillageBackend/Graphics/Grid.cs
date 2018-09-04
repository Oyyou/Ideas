using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.World;

namespace VillageBackend.Graphics
{
  public class Grid
  {
    private List<Sprite> _tiles;

    public bool IsVisible { get; set; }

    public Grid(GraphicsDevice graphicsDevice, Map map)
    {
      var texture = new Texture2D(graphicsDevice, 32, 32);

      var colours = new Color[texture.Width * texture.Height];

      int index = 0;
      for(int y = 0; y < texture.Width; y++)
      {
        for(int x = 0; x < texture.Height; x++)
        {
          var colour = new Color(0, 0, 0, 0);

          if (y == 0 || y == texture.Width - 1 ||
            x == 0 || x == texture.Height - 1)
            colour = new Color(0, 0, 0, 255);

          colours[index] = colour;

          index++;
        }
      }

      texture.SetData(colours);

      _tiles = new List<Sprite>();

      for (int y = 0; y < map.Height; y++)
      {
        for (int x = 0; x < map.Width; x++)
        {
          var position = new Vector2(x * map.TileWidth, y * map.TileHeight);

          _tiles.Add(new Sprite(texture)
          {
            IsFixedLayer = true,
            Layer = 0.25f,
            Position = position,
          });
        }
      }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!IsVisible)
        return;

      foreach (var sprite in _tiles)
        sprite.Draw(gameTime, spriteBatch);
    }
  }
}
