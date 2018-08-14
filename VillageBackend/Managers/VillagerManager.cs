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
    private GameManagers _gameManagers;

    public ObservableCollection<Villager> Villagers { get; private set; }

    public VillagerManager(GameManagers gameManagers)
    {
      _gameManagers = gameManagers;

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

    public void AssignJob(Villager villager, Job job)
    {
      foreach (var v in Villagers)
      {
        if (v.JobId == null)
          continue;

        if (v.JobId == job.Id)
          v.JobId = null;
      }

      villager.JobId = job.Id;

      Console.WriteLine("Villagers");
      foreach (var v in Villagers)
      {
        Console.WriteLine($"{v.Id} {v.Name}: {v.JobId}");
      }
      Console.WriteLine();
    }

    public bool HasJob(Villager villager)
    {
      return villager.JobId != null;
    }
  }
}
