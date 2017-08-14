using ItemManager.Commands;
using ItemManager.Models;
using ItemManager.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static BusinessLogic.Items.Item;

namespace ItemManager.ViewModels
{
  public class ViewModel : ObservableObject
  {
    private string _category;

    private string _imagePath;

    private ObservableCollection<ItemHeader> _itemHeaders;

    private string _name;

    public ICommand BrowseImages
    {
      get;
      private set;
    }

    public string Category
    {
      get { return _category; }
      set
      {
        _category = value;
        OnPropertyChanged("Category");
      }
    }

    public ItemCategories[] Categories
    {
      get
      {
        return Enum.GetValues(typeof(ItemCategories)) as ItemCategories[];
      }
    }

    public string ImagePath
    {
      get { return _imagePath; }
      set
      {
        _imagePath = value;
        OnPropertyChanged("ImagePath");
      }
    }

    public ObservableCollection<ItemHeader> ItemHeaders
    {
      get { return _itemHeaders; }
      set
      {
        _itemHeaders = value;
        OnPropertyChanged("ItemHeaders");
      }
    }

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        OnPropertyChanged("Name");
      }
    }

    public ViewModel()
    {
      BrowseImages = new BrowseImages(this);

      ItemHeaders = new ObservableCollection<ItemHeader>()
      {
        new ItemHeader()
        {
          Category="Test 1",
          Items = new ObservableCollection<ItemHeader>()
          {
            new ItemHeader()
            {
              Category= "Test 1.1"
            },
            new ItemHeader()
            {
              Category= "Test 1.2"
            },
          },
        },
        new ItemHeader()
        {
          Category="Test 2",
          Items = new ObservableCollection<ItemHeader>()
          {
            new ItemHeader()
            {
              Category= "Test 2.1"
            },
          }
        }
      };
    }
  }
}
