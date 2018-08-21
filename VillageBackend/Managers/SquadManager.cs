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
  }
}
