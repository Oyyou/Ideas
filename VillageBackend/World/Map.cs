using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.World
{
  public class Map
  {
    public int Width { get; set; }

    public int Height { get; set; }

    public int TileWidth { get; set; }

    public int TileHeight { get; set; }

    /// <summary>
    /// These are non-collidable objects
    /// </summary>
    public List<Rectangle> MapObjects { get; private set; } = new List<Rectangle>();

    public void AddObject(Rectangle rectangle)
    {
      if (MapObjects.Any(c => c.Intersects(rectangle)))
        throw new Exception("Object already exists in position: " + rectangle);

      MapObjects.Add(rectangle);
    }

    public char[,] GetMap()
    {
      var map = new char[Height, Width];

      for (int y = 0; y < Height; y++)
      {
        for (int x = 0; x < Width; x++)
        {
          map[y, x] = '0';

          var rectangle = new Rectangle(x, y, 1, 1);

          if (MapObjects.Any(c => c.Intersects(rectangle)))
          {
            map[y, x] = '1';
          }
        }
      }

      return map;
    }

    public void Write(List<Point> path = null)
    {
      if (path == null)
        path = new List<Point>();

      for (int y = 0; y <= Height; y++)
      {
        for (int x = 0; x <= Width; x++)
        {
          if (path.Any(c => c == new Point(x, y)))
            Console.Write("P");
          else if (MapObjects.Any(c => c.Intersects(new Rectangle(x, y, 1, 1))))
            Console.Write("#");
          else
            Console.Write("0");
        }

        Console.WriteLine();
      }

      Console.WriteLine();
    }
  }
}
