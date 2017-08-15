using ItemManager.Models;
using ItemManager.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using TopDown.Items;
using TopDown.Models;

namespace ItemManager.Commands
{
  public class SetWorkingDirectory : ICommand
  {
    private ViewModel _viewModel;

    public event EventHandler CanExecuteChanged;

    public SetWorkingDirectory(ViewModel viewModel)
    {
      _viewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      var dialog = new FolderBrowserDialog()
      {

      };
      var result = dialog.ShowDialog();

      if (result == DialogResult.OK)
      {
        _viewModel.ItemHeaders = new ObservableCollection<ItemHeader>();

        var directory = dialog.SelectedPath;

        _viewModel.WorkingDirectory = directory;

        var itemFiles = new Dictionary<string, IEnumerable<ItemV2>>();

        foreach (var file in Directory.GetFiles(directory, "*.json"))
        {
          var fileName = Path.GetFileNameWithoutExtension(file);

          var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), fileName);

          IEnumerable<ItemV2> items = null;

          switch (category)
          {
            case ItemCategories.Weapons:
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

          _viewModel.ItemHeaders.Add(new ItemHeader()
          {
            Category = category.ToString(),
            Items = items,
          });
        }
      }
    }
  }
}
