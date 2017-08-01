using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace TopDown.Items
{
  public class Item : Sprite
  {
    public float CraftTime { get; private set; }

    public string Name { get; private set; }

    public Models.Resources ResourceCost;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="craftTime">How long it'll take to craft (not including worker modifier)</param>
    /// <param name="name">The name of the craftable item</param>
    public Item(Texture2D texture, float craftTime, string name) : base(texture)
    {
      CraftTime = craftTime;

      Name = name;
    }
  }
}
