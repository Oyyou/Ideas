using ItemManager.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TopDown.Items;
using TopDown.Models;

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
      if (string.IsNullOrEmpty(_viewModel.Name))
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

      if (!float.TryParse(_viewModel.ExperienceValue, out float experienceValue))
      {
        _viewModel.Status = "'ExperienceValue' needs to be a number";
        return false;
      }
      else if (experienceValue <= 0)
      {
        _viewModel.Status = "'ExperienceValue' needs to be a above '0'";
        return false;
      }

      if (!float.TryParse(_viewModel.CraftTime, out float craftTime))
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

      //var itemHeader = _viewModel.ItemHeaders.Where(c => c.Category == _viewModel.Category).FirstOrDefault();

      //if (itemHeader != null)
      //{
      //  if (itemHeader.Items.Any(c => string.Equals(c.Name.Trim(), _viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
      //  {
      //    _viewModel.Status = "Item already exists";
      //    return false;
      //  }
      //}

      _viewModel.Status = "Ready";
      return true;
    }

    public void Execute(object parameter)
    {
      var itemHeaders = _viewModel.ItemHeaders;

      foreach (var itemHeader in itemHeaders)
      {
        if (itemHeader.Category == _viewModel.Category)
        {
          var newItems = itemHeader.Items.ToList();

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

          _viewModel.ItemHeaders.Remove(itemHeader);
          _viewModel.ItemHeaders.Add(new Models.ItemHeader()
          {
            Category = category.ToString(),
            Items = newItems,
          });
          break;
        }
      }
    }
  }
}
