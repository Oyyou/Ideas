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

    public Villager()
    {
      _ids++;

      Id = _ids;

      Name = Names[new Random().Next(0, Names.Length)];
    }
  }
}
