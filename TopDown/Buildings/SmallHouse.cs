using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Buildings.Templates;
using TopDown.States;

namespace TopDown.Buildings
{
  public class SmallHouse : Building
  {
    public SmallHouse(GameScreen gameState, BuildingTemplate template) : base(gameState, template)
    {
    }
  }
}
