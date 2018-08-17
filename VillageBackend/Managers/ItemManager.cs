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
    private GameManagers _gameManagers;

    /// <summary>
    /// All items that have been crafted
    /// </summary>
    public List<ItemV2> Items { get; set; }

    /// <summary>
    /// The resources from the actual game
    /// </summary>
    public readonly Resources Resources;

    /// <summary>
    /// The items added to the queue to be crafed by the assigned villager
    /// </summary>
    public ObservableCollection<ItemV2> QueuedItems { get; private set; }
		
	  public ItemManager(GameManagers gameManagers, Resources resources)
		{
      _gameManagers = gameManagers;

      Resources = resources;

      QueuedItems = new ObservableCollection<ItemV2>();
		}
    
    public void AddToQueue(ItemV2 item)
    {
      if(!Resources.CanAfford(Resources, item.ResourceCost))
        throw new Exception("Check to see if the item is affordable before adding to q");
      
			QueuedItems.Add(item);
    }

    /// <summary>
    /// Can the villager craft this item
    /// </summary>
    /// <param name="villager">The selected villager</param>
    /// <param name="item">The item required for crafting</param>
    public bool CanCraft(Villager villager, ItemV2 item)
    {
      return true;
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
