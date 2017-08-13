using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDown.ItemStats
{
  public class WeaponStats : ItemStats
  {
    public int AttackSpeed { get; set; }

    public int Damage { get; set; }

    public override Dictionary<string, int> GetContent()
    {
      return new Dictionary<string, int>()
      {
        { "Damage", Damage },
        { "Attack Speed", AttackSpeed },
      };
    }
  }
}
