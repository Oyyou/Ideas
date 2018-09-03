using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TiledReader
{
  internal static class Program
  {
    internal static void Main(string[] args)
    {
      var map = TiledMap.Load("TileMaps", "Level_001.tmx");

      var textures = map.Tileset.Select(c => Path.GetFileNameWithoutExtension(c.Image.Source));

      foreach (var layer in map.Layer)
      {
        var lines = layer.Data.Split('\n').Where(c => !string.IsNullOrEmpty(c)).ToList();

        foreach (var line in lines)
        {
          var values = line.Split(',').Where(c => !string.IsNullOrEmpty(c)).ToList();

          foreach (var value in values)
          {

          }
        }
      }
    }
  }
}
