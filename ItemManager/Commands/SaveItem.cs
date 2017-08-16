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
  public class SaveItem : ICommand
  {
    private ViewModel _viewModel;

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public SaveItem(ViewModel viewModel)
    {
      _viewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      if (string.IsNullOrEmpty(_viewModel.Name))
        return false;

      if (string.IsNullOrEmpty(_viewModel.Category))
        return false;

      if (string.IsNullOrEmpty(_viewModel.Material))
        return false;

      if (string.IsNullOrEmpty(_viewModel.ImagePath))
        return false;

      var itemHeader = _viewModel.ItemHeaders.Where(c => c.Category == _viewModel.Category).FirstOrDefault();

      if (itemHeader != null)
      {
        if (itemHeader.Items.Any(c => string.Equals(c.Name.Trim(), _viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
          return false;
      }

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

          ItemCategories category = (ItemCategories)Enum.Parse(typeof(ItemCategories), _viewModel.Category);

          switch (category)
          {
            case ItemCategories.Weapon:
              newItems.Add(new Weapon()
              {
                Name = _viewModel.Name,                
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
