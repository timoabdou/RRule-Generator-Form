using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RRule_Generator_Form
{
    /// <summary>
    /// Interaction logic for W_RetrieveParamsFromRRule.xaml
    /// </summary>
    public partial class W_RetrieveParamsFromRRule : Window
    {
        private readonly RRuleViewModel rrule;

        public W_RetrieveParamsFromRRule(RRuleViewModel viewModel)
        {
            InitializeComponent();
            this.rrule = viewModel;
        }

        private void BTN_RetrieveParams_Click(object sender, RoutedEventArgs e)
        {
            rrule.FillFromRRule(rruleTextBox.Text);
            Close();
        }

        private void BTN_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
