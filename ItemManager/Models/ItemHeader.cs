using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Models;

namespace ItemManager.Models
{
  public class ItemHeader
  {
    public string Category { get; set; }

    public IEnumerable<ItemV2> Items { get; set; }

    public ItemHeader()
    {
    }
  }
}
