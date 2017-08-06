using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace TopDown.Items
{
  public enum ItemCategories
  {
    Weapons,
    Armour,
    Tool,
    Clothing,
    Jewellery
  }

  public class Item : Sprite
  {
    public ItemCategories Category { get; set; }

    /// <summary>
    /// The amount of time spent crafting the item
    /// </summary>
    public float CraftingTime { get; set; }

    /// <summary>
    /// How long it takes to craft the item
    /// </summary>
    public readonly float CraftTime;

    public readonly string Name;

    /// <summary>
    /// The cost of the item
    /// </summary>
    public Models.Resources ResourceCost;

    /// <param name="craftTime">How long it'll take to craft (not including worker modifier) in seconds</param>
    /// <param name="name">The name of the craftable item</param>
    public Item(Texture2D texture, float craftTime, string name, ItemCategories category) : base(texture)
    {
      CraftTime = craftTime;

      Name = name;

      Category = category;
    }
  }
}
