using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend
{
  public class Enums
  {
    public enum ItemCategories
    {
      Weapon,
      Armour,
      Tool,
      Clothing,
      Jewellery,
      Medicine
    }

    public enum ItemMaterials
    {
      Wood,
      Stone,
      Iron,
      Copper,
      Silver,
      Gold,
    }

    public enum BuildingStates
    {
      Placing,
      Placed,
      Constructing,
      Built,
      Demolishing,
    }

    public enum BuildingTypes
    {
      Housing,
      Labour,
      Arts,
      Misc,
    }
  }
}
