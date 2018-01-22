using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using static VillageBackend.Enums;

namespace TopDown.Items
{
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

    /// <summary>
    /// Base amount of experience gained when crafted
    /// </summary>
    public int ExperienceValue { get; set; }

    public readonly string Name;

    /// <summary>
    /// The cost of the item
    /// </summary>
    public VillageBackend.Models.Resources ResourceCost;

    /// <summary>
    /// The stat sof the item
    /// </summary>
    public ItemStats.ItemStats Stats { get; set; }

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
