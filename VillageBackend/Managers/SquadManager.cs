using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Models;

namespace VillageBackend.Managers
{
  public class SquadManager
  {
    private GameManagers _gameManagers;

    public List<Squad> Squads { get; set; }

    public SquadManager(GameManagers gameManagers)
    {
      _gameManagers = gameManagers;

      Squads = new List<Squad>();
    }

    public void Add(string squadName, Villager villager1, Villager villager2 = null, Villager villager3 = null, Villager villager4 = null)
    {
      if (Exists(squadName))
        return;

      Squads.Add(new Squad()
      {
        Name = squadName,
        Villager_1 = villager1,
        Villager_2 = villager2,
        Villager_3 = villager3,
        Villager_4 = villager4,
      });
    }

    public bool Exists(string squadName)
    {
      return Squads.Any(c => c.Name.Equals(squadName, StringComparison.CurrentCultureIgnoreCase));
    }
  }
}
