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

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

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
        _viewModel.WorkingDirectory = dialog.SelectedPath;

        _viewModel.LoadJsonContent();
      }
    }
  }
}
