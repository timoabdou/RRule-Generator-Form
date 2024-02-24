using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace RRule_Generator_Form
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RRuleViewModel rrule;
        public MainWindow()
        {
            InitializeComponent();

            rrule = new RRuleViewModel();

            DataContext = rrule;
        }

        private void BTN_Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TB_RRule.Text);
        }

        private void BTN_RetrieveParamsFromRRule_Click(object sender, RoutedEventArgs e)
        {
            W_RetrieveParamsFromRRule retrieveParamsFromRRuleWindow = new W_RetrieveParamsFromRRule(rrule);
            retrieveParamsFromRRuleWindow.Owner = this;
            retrieveParamsFromRRuleWindow.ShowDialog();
        }
    }
}
