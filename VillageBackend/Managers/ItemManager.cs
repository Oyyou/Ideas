using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Models;
using static VillageBackend.Enums;

namespace VillageBackend.Managers
{
  public class ItemManager
  {
    /// <summary>
    /// All items that have been crafted
    /// </summary>
    public List<ItemV2> Items { get; private set; }

    /// <summary>
    /// The resources from the actual game
    /// </summary>
    public readonly Resources Resources;

    /// <summary>
    /// The items added to the queue to be crafed by the assigned villager
    /// </summary>
    public ObservableCollection<ItemV2> QueuedItems { get; private set; }
		
	  public ItemManager(Resources resources)
		{
			Resources = resources;

      QueuedItems = new ObservableCollection<ItemV2>();
		}
    
    public void AddToQueue(ItemV2 item)
    {
      if(!Resources.CanAfford(Resources, item.ResourceCost))
        throw new Exception("Check to see if the item is affordable before adding to q");
      
			QueuedItems.Add(item);
    }
		
		public void Update(GameTime gameTime)
		{
			foreach (var items in QueuedItems.GroupBy(c => c.CrafterId))
			{
				var firstItem = items.FirstOrDefault();
				
				if (firstItem != null)
				{
					firstItem.CraftingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
					
					if (firstItem.CraftingTime >= firstItem.CraftTime)
					{
						Items.Add(firstItem);
						QueuedItems.Remove(firstItem);
					}
				}
			}
		}
  }
}
