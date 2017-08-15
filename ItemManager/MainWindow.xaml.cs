using ItemManager.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ItemManager
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      DataContext = new ViewModel();

      //string jsonFile = JsonConvert.SerializeObject(weapons, Formatting.Indented);

      //var fileName = "Weapons.json";

      //using (var str = new StreamWriter(fileName))
      //{
      //  str.Write(jsonFile);
      //}

      //var t = JsonConvert.DeserializeObject<List<Weapon>>(File.ReadAllText(fileName),
      //  new JsonSerializerSettings() { Formatting = Formatting.Indented });
    }
  }
}
