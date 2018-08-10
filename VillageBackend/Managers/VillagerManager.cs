using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Models;

namespace VillageBackend.Managers
{
  public class VillagerManager
  {
    public ObservableCollection<Villager> Villagers { get; private set; }

    public VillagerManager()
    {
      Villagers = new ObservableCollection<Villager>();
    }

    public void Add(Villager villager)
    {
      Villagers.Add(villager);
    }

    public void Remove(Villager villager)
    {
      if (Villagers.Contains(villager))
        Villagers.Remove(villager);
    }
  }
}
