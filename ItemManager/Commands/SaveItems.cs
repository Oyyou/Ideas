using ItemManager.ViewModels;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
      if (string.IsNullOrEmpty(_viewModel.WorkingDirectory))
      {
        MessageBox.Show("Please set the working directory");
        return;
      }

      var itemHeaders = _viewModel.ItemHeaders;

      foreach (var itemHeader in itemHeaders)
      {
        string jsonFile = JsonConvert.SerializeObject(itemHeader.Items, Formatting.Indented);

        var fileName = $"{_viewModel.WorkingDirectory}\\{itemHeader.Category}.json";

        using (var str = new StreamWriter(fileName))
        {
          str.Write(jsonFile);
        }
      }

      foreach (var image in _viewModel.ImagesToCopy)
      {
        var folderDirectory = $"{_viewModel.WorkingDirectory}\\{image.Key}";

        if (!Directory.Exists(folderDirectory))
        {
          Directory.CreateDirectory(folderDirectory);
        }

        foreach (var imagePath in image.Value.Distinct())
        {
          var newImagesPath = $"{folderDirectory}\\{Path.GetFileName(imagePath.Item1)}{Path.GetExtension(imagePath.Item2)}";

          File.Copy(imagePath.Item2, newImagesPath, true);
        }
      }
    }
  }
}
