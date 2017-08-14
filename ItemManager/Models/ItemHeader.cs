using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemManager.Models
{
  public class ItemHeader
  {
    public string Category { get; set; }

    public ObservableCollection<ItemHeader> Items { get; set; }

    public ItemHeader()
    {
      Items = new ObservableCollection<ItemHeader>();
    }
  }
}
