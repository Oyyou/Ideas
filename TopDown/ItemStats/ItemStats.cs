using System.Collections.Generic;

namespace TopDown.ItemStats
{
  public abstract class ItemStats
  {
    public abstract Dictionary<string, int> GetContent();
  }
}