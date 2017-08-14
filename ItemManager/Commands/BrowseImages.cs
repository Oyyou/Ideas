using ItemManager.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ItemManager.Commands
{
  public class BrowseImages : ICommand
  {
    private ViewModel _viewModel;

    public event EventHandler CanExecuteChanged;

    public BrowseImages(ViewModel viewModel)
    {
      _viewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      var dialog = new OpenFileDialog()
      {
        Filter = "Files|*.png;",
      };
      var result = dialog.ShowDialog();

      if (result.Value)
      {
        _viewModel.ImagePath = dialog.FileName;
      }
    }
  }
}
