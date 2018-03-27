using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Models
using static VillageBackend.Enums;

namespace VillageBackend.Managers
{
  public class ItemManager
  {
    /// <summary>
    /// All items that have been crafted
    /// </summary>
    public List<ItemV2> Items { get; private set; }
    
    /// <summary>
    /// The items added to the queue to be crafed by the assigned villager
    /// </summary>
    public List<ItemV2> QueuedItems { get; private set; }    
  }
}
