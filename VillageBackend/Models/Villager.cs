using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.Models
{
  public class Villager
  {
    private static int _ids;

    public static string[] Names = new string[]
    {
      "Bob",
      "Jimmy",
      "Fred",
      "Tim",
      "John",
    };

    public readonly int Id;

    public readonly string Name;

    public int? JobId { get; set; }

    public float Experience { get; set; }

    public bool IsAtWork { get; set; }

    public readonly int MaxTurns = 2; 

    public int Turns { get; set; } = 2;

    public Villager()
    {
      _ids++;

      Id = _ids;

      Name = Names[new Random().Next(0, Names.Length)];
    }
  }
}
