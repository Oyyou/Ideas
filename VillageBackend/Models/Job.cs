using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.Models
{
  public class Job
  {
    public string Name { get; set; }

    public int? VillagerId { get; set; }

    public int BuildingId { get; set; }
  }
}
