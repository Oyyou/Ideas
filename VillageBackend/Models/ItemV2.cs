using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VillageBackend.Enums;

namespace VillageBackend.Models
{
  public abstract class ItemV2 : ICloneable
  {
    public ItemCategories Category { get; set; }
    
    /// <summary>
    /// The id of the villager who crafted the item
    /// </summary>
    public int CrafterId { get; set; }
    
    /// <summary>
    /// The amount of time spent crafting the item
    /// </summary>
    public float CraftingTime { get; set; }

    /// <summary>
    /// How long it takes to craft the item
    /// </summary>
    public float CraftTime { get; set; }

    public bool ShowCraftingProgress { get; set; }

    /// <summary>
    /// Base amount of experience gained when crafted
    /// </summary>
    public float ExperienceValue { get; set; }

    public ItemMaterials Material { get; set; }

    public string Name { get; set; }
    
    /// <summary>
    /// The resources used to craft the item
    /// </summary>
    public Resources ResourceCost { get; set; }

    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}
