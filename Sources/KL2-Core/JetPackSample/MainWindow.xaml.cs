using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JetPackSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.ListOfItems = new string[]
            {
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.",
                "Curabitur consequat metus vitae est.",
                "Duis quis metus.",
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.",
                "Quisque scelerisque.",
            };

            this.ListOfData = this.ListOfItems.Select(i => new Data
                {
                    String = i,
                    Length = i.Length,
                    Spaces = i.ToCharArray().Where(c => c == ' ').Count()
                }).ToArray();


            this.DataContext = this;
        }


        public string[] ListOfItems { get; set; }
        public Data[] ListOfData { get; set; }

        public class Data
        {
            public string String { get; set; }
            public int Length { get; set; }
            public int Spaces { get; set; }
            public bool Check { get; set; }
        }
    }
}
