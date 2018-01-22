using ItemManager.Commands;
using ItemManager.Models;
using ItemManager.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using VillageBackend.Models;
using static VillageBackend.Enums;

namespace ItemManager.ViewModels
{
  public class ViewModel : ObservableObject
  {
    #region Fields

    private string _category;

    private string _craftTime;

    private string _experienceValue;

    private string _imagePath;

    private ObservableCollection<ItemHeader> _itemHeaders;

    private string _material;

    private string _name;

    private Settings _settings;

    private string _status;

    #endregion

    #region Properties

    public ICommand AddItem
    {
      get;
      private set;
    }

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

    public ObservableCollection<DictionaryEntry> Categories
    {
      get
      {
        var categories = new ObservableCollection<DictionaryEntry>();
        foreach (var item in (Enum.GetValues(typeof(ItemCategories)) as ItemCategories[]).ToList())
        {
          categories.Add(new DictionaryEntry(item, item));
        }

        return categories;
      }
    }

    public string CraftTime
    {
      get { return _craftTime; }
      set
      {
        _craftTime = value;
        OnPropertyChanged("CraftTime");
      }
    }

    public ICommand EditItem
    {
      get;
      private set;
    }

    public string ExperienceValue
    {
      get { return _experienceValue; }
      set
      {
        _experienceValue = value;
        OnPropertyChanged("ExperienceValue");
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

    public Dictionary<ItemCategories, List<Tuple<string, string>>> ImagesToCopy { get; set; }

    public ObservableCollection<ItemHeader> ItemHeaders
    {
      get { return _itemHeaders; }
      set
      {
        _itemHeaders = value;
        OnPropertyChanged("ItemHeaders");
      }
    }

    public string Material
    {
      get { return _material; }
      set
      {
        _material = value;
        OnPropertyChanged("Material");
      }
    }

    public ObservableCollection<DictionaryEntry> Materials
    {
      get
      {
        var materials = new ObservableCollection<DictionaryEntry>();
        foreach (var item in (Enum.GetValues(typeof(ItemMaterials)) as ItemMaterials[]).ToList())
        {
          materials.Add(new DictionaryEntry(item, item));
        }

        return materials;
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

    public ICommand SaveItems
    {
      get;
      private set;
    }
    
    public ICommand SetWorkingDirectory
    {
      get;
      private set;
    }

    public string Status
    {
      get { return _status; }
      set
      {
        _status = value;
        OnPropertyChanged("Status");
      }
    }

    public string WorkingDirectory
    {
      get { return _settings.WorkingDirectory; }
      set
      {
        _settings.WorkingDirectory = value;
        OnPropertyChanged("WorkingDirectory");
      }
    }

    #endregion
    
    public void LoadJsonContent()
    {
      if (string.IsNullOrEmpty(WorkingDirectory))
        return;

      ItemHeaders = new ObservableCollection<ItemHeader>();

      var itemFiles = new Dictionary<string, IEnumerable<ItemV2>>();

      foreach (var file in Directory.GetFiles(WorkingDirectory, "*.json"))
      {
        var fileName = Path.GetFileNameWithoutExtension(file);

        var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), fileName);

        IEnumerable<ItemV2> items = null;

        switch (category)
        {
          case ItemCategories.Weapon:
            items = JsonConvert.DeserializeObject<List<Weapon>>(File.ReadAllText(file),
              new JsonSerializerSettings() { Formatting = Formatting.Indented });
            break;
          case ItemCategories.Armour:
            items = JsonConvert.DeserializeObject<List<Armour>>(File.ReadAllText(file),
              new JsonSerializerSettings() { Formatting = Formatting.Indented });
            break;
          case ItemCategories.Tool:
          case ItemCategories.Clothing:
          case ItemCategories.Jewellery:
          case ItemCategories.Medicine:
          default:
            MessageBox.Show("Not implemented category: " + category.ToString());
            break;
        }

        ItemHeaders.Add(new ItemHeader()
        {
          Category = category.ToString(),
          Items = items,
        });
      }
    }

    private void LoadSettings()
    {
      var serializer = new XmlSerializer(typeof(Settings));

      if (File.Exists("settings.xml"))
      {
        using (var stream = new FileStream("settings.xml", FileMode.Open))
        {
          _settings = (Settings)serializer.Deserialize(stream);
        }
      }
    }

    public void OpenItem(ItemV2 item)
    {
      Name = item.Name;
      Category = item.Category.ToString();
      Material = item.Material.ToString();
      ExperienceValue = item.ExperienceValue.ToString();
      CraftTime = item.CraftTime.ToString();
      ImagePath = $"{WorkingDirectory}\\{Category}\\{Name}.png";
    }

    public void SaveSettings()
    {
      var serializer = new XmlSerializer(typeof(Settings));

      using (var stream = new FileStream("settings.xml", FileMode.Create))
      {
        serializer.Serialize(stream, _settings);
      }
    }

    public ViewModel()
    {
      _settings = new Settings();

      ImagesToCopy = new Dictionary<ItemCategories, List<Tuple<string, string>>>();

      LoadSettings();

      LoadJsonContent();

      AddItem = new AddItem(this);

      BrowseImages = new BrowseImages(this);

      EditItem = new EditItem(this);

      SaveItems = new SaveItems(this);

      SetWorkingDirectory = new SetWorkingDirectory(this);
    }
  }
}
