using ItemManager.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using VillageBackend.Models;

namespace ItemManager.Commands
{
  public class EditItem : ICommand
  {
    private ViewModel _viewModel;

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public EditItem(ViewModel viewModel)
    {
      _viewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      var treeView = parameter as TreeViewItem;

      var type = typeof(Weapon);

      var item = treeView.Header as ItemV2;

      _viewModel.OpenItem(item);
    }
  }
}
