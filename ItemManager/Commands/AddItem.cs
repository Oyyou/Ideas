using ItemManager.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VillageBackend.Models;
using static VillageBackend.Enums;

namespace ItemManager.Commands
{
  public class AddItem : ICommand
  {
    private ViewModel _viewModel;

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public AddItem(ViewModel viewModel)
    {
      _viewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      if (string.IsNullOrWhiteSpace(_viewModel.Name))
      {
        _viewModel.Status = "Need to assign 'Name'";
        return false;
      }

      if (string.IsNullOrEmpty(_viewModel.Category))
      {
        _viewModel.Status = "Need to assign 'Category'";
        return false;
      }

      if (string.IsNullOrEmpty(_viewModel.Material))
      {
        _viewModel.Status = "Need to assign 'Material'";
        return false;
      }

      float experienceValue = 0f;

      if (!float.TryParse(_viewModel.ExperienceValue, out experienceValue))
      {
        _viewModel.Status = "'ExperienceValue' needs to be a number";
        return false;
      }
      else if (experienceValue <= 0)
      {
        _viewModel.Status = "'ExperienceValue' needs to be a above '0'";
        return false;
      }

      float craftTime = 0f;

      if (!float.TryParse(_viewModel.CraftTime, out craftTime))
      {
        _viewModel.Status = "'CraftTime' needs to be a number";
        return false;
      }
      else if (craftTime <= 0)
      {
        _viewModel.Status = "'CraftTime' needs to be a above '0'";
        return false;
      }

      if (string.IsNullOrEmpty(_viewModel.ImagePath))
      {
        _viewModel.Status = "Need to assign 'ImagePath'";
        return false;
      }

      if (string.IsNullOrEmpty(_viewModel.ImagePath))
      {
        _viewModel.Status = "Need to assign 'Image Path'";
        return false;
      }

      _viewModel.Status = "Ready";
      return true;
    }

    public void Execute(object parameter)
    {
      var itemHeaders = _viewModel.ItemHeaders;

      var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), _viewModel.Category);

      var material = (ItemMaterials)Enum.Parse(typeof(ItemMaterials), _viewModel.Material);

      if (_viewModel.ItemHeaders != null && _viewModel.ItemHeaders.Count > 0)
      {
        var header = _viewModel.ItemHeaders.Where(c => c.Category == _viewModel.Category).FirstOrDefault();
        ItemV2 item = null;

        if (header != null)
        {
          item = header.Items.Where(c => string.Equals(c.Name.Trim(), _viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

          if (item != null)
          {
            var result = MessageBox.Show("Item already exists. Are you sure you'd like to override?", "Message", MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
              return;
          }
        }

        foreach (var itemHeader in itemHeaders)
        {
          if (itemHeader.Category == _viewModel.Category)
          {
            var newItems = itemHeader.Items.ToList();

            if (item != null)
              newItems.Remove(item);

            AddNewItem(newItems);

            if (!_viewModel.ImagesToCopy.ContainsKey(category))
            {
              _viewModel.ImagesToCopy.Add(category, new List<Tuple<string, string>>());
            }

            _viewModel.ImagesToCopy[category].Add(new Tuple<string, string>(_viewModel.Name, _viewModel.ImagePath));

            _viewModel.ItemHeaders.Remove(itemHeader);
            _viewModel.ItemHeaders.Add(new Models.ItemHeader()
            {
              Category = _viewModel.Category.ToString(),
              Items = newItems,
            });
            break;
          }
        }
      }
      else
      {
        var newItems = new List<ItemV2>();

        AddNewItem(newItems);

        if (!_viewModel.ImagesToCopy.ContainsKey(category))
        {
          _viewModel.ImagesToCopy.Add(category, new List<Tuple<string, string>>());
        }

        _viewModel.ImagesToCopy[category].Add(new Tuple<string, string>(_viewModel.Name, _viewModel.ImagePath));

        _viewModel.ItemHeaders = new System.Collections.ObjectModel.ObservableCollection<Models.ItemHeader>()
        {
          new Models.ItemHeader()
          {
            Category = _viewModel.Category.ToString(),
            Items = newItems,
          },
        };
      }
    }

    private void AddNewItem(List<ItemV2> newItems)
    {
      var category = (ItemCategories)Enum.Parse(typeof(ItemCategories), _viewModel.Category);

      var material = (ItemMaterials)Enum.Parse(typeof(ItemMaterials), _viewModel.Material);

      switch (category)
      {
        case ItemCategories.Weapon:
          newItems.Add(new Weapon()
          {
            Name = _viewModel.Name,
            Category = category,
            Material = material,
            ExperienceValue = float.Parse(_viewModel.ExperienceValue),
            CraftTime = float.Parse(_viewModel.CraftTime),
          });
          break;
        case ItemCategories.Armour:
          break;
        case ItemCategories.Tool:
          break;
        case ItemCategories.Clothing:
          break;
        case ItemCategories.Jewellery:
          break;
        case ItemCategories.Medicine:
          break;
        default:
          break;
      }
    }
  }
}
