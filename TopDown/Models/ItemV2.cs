using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Items;

namespace TopDown.Models
{
  public abstract class ItemV2
  {
    public ItemCategories Category { get; set; }

    /// <summary>
    /// The amount of time spent crafting the item
    /// </summary>
    public float CraftingTime { get; set; }

    /// <summary>
    /// How long it takes to craft the item
    /// </summary>
    public float CraftTime { get; set; }

    /// <summary>
    /// Base amount of experience gained when crafted
    /// </summary>
    public int ExperienceValue { get; set; }

    public ItemMaterials Material { get; set; }

    public string Name { get; set; }
  }
}
