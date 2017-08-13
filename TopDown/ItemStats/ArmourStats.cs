using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDown.ItemStats
{
  public class ArmourStats : ItemStats
  {
    public int ArmourGained { get; set; }

    public override Dictionary<string, int> GetContent()
    {
      return new Dictionary<string, int>()
      {
        { "Armour", ArmourGained },
      };
    }
  }
}
