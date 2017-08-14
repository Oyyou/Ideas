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
    class Weapon
    {
      public string Name { get; set; }

      public int Damage { get; set; }

      public int AttackSpeed { get; set; }

      public int Range { get; set; }
    }

    public MainWindow()
    {
      InitializeComponent();

      DataContext = new ViewModel();

      //var weapons = new List<Weapon>()
      //{
      //  new Weapon()
      //  {
      //    Name = "Sword",
      //    Range = 1,
      //    Damage = 2,
      //    AttackSpeed = 3,
      //  },
      //  new Weapon()
      //  {
      //    Name = "Spear",
      //    Range = 2,
      //    Damage = 3,
      //    AttackSpeed = 2,
      //  },
      //  new Weapon()
      //  {
      //    Name = "Axe",
      //    Range = 1,
      //    Damage = 3,
      //    AttackSpeed = 2,
      //  },
      //};

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
