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
  public class SaveItems : ICommand
  {
    private ViewModel _viewModel;

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public SaveItems(ViewModel viewModel)
    {
      _viewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {

    }
  }
}
