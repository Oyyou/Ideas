using ItemManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemManager.Models
{
  public class Settings : ObservableObject
  {
    public string WorkingDirectory { get; set; }
  }
}
