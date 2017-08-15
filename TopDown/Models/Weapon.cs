using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDown.Models
{
  public class Weapon : ItemV2
  {
    public int AttackSpeed { get; set; }

    public int Damage { get; set; }

    public int Range { get; set; }
  }
}
