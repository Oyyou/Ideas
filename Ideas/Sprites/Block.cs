using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ideas.Sprites
{
  public class Block : Sprite
  {
    private static string[] _blockTops = new string[]
    {
      "BlockTop_01",
      "BlockTop_02",
    };

    public static int Size = 64;

    public Block(Texture2D texture) : base(texture)
    {
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      var blockTopStandard = new Sprite(content.Load<Texture2D>("Sprites/Blocks/BlockTop_00"))
      {
        Position = this.Position,
        Layer = 0.02f
      };
      

      var blockTopRandom = new Sprite(content.Load<Texture2D>("Sprites/Blocks/" + _blockTops[Game1.Random.Next(0, _blockTops.Length)]))
      {
        Position = this.Position,
        Layer = 0.03f,
      };

      Components.Add(blockTopStandard);
      Components.Add(blockTopRandom);
    }
  }
}
