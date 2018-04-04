using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.Models
{
  public class Resources
  {
    public int Food { get; set; }

    public int Gold { get; set; }

    public int Stone { get; set; }

    public int Wood { get; set; }

    public Dictionary<string, int> GetContent()
    {
      return new Dictionary<string, int>()
      {
        { "Food", Food },
        { "Gold", Gold },
        { "Stone", Stone },
        { "Wood", Wood },
      }.ToDictionary(c => c.Key, v => v.Value);
    }
    
    public static bool CanAfford(Resources stock, Resources itemCost)
    {
      return stock.GetContent().All(c => c.Value >= itemCost.GetContent()[c.Key]);
    }
  }
}
