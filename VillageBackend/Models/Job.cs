using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.Models
{
  public class Job
  {
    private static int _ids;

    public string Name { get; set; }

    public readonly int Id;

    public Job()
    {
      _ids++;

      Id = _ids;
    }
  }
}
