using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.Managers
{
  public class GameManagers
  {
    public ItemManager ItemManager { get; set; }

    public JobManager JobManager { get; set; }

    public SquadManager SquadManager { get; set; }

    public VillagerManager VillagerManager { get; set; }
  }
}
