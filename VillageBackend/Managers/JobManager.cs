using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Models;

namespace VillageBackend.Managers
{
  public class JobManager
  {
    public ObservableCollection<Job> Jobs { get; private set; }

    public JobManager()
    {
      Jobs = new ObservableCollection<Job>();
    }

    public void Add(Job job)
    {
      if (job == null)
        return;

      Jobs.Add(job);
    }

    public void Remove(Job job)
    {
      if (Jobs.Contains(job))
        Jobs.Remove(job);
    }
  }
}
