using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.Models
{
  public class Squad
  {
    public string Name { get; set; }

    public Villager Villager_1 { get; set; }

    public Villager Villager_2 { get; set; }

    public Villager Villager_3 { get; set; }

    public Villager Villager_4 { get; set; }

    public List<Villager> Villagers
    {
      get
      {
        return new List<Villager>()
        {
          Villager_1,
          Villager_2,
          Villager_3,
          Villager_4,
        }.Where(c => c != null).ToList();
      }
    }
  }
}
