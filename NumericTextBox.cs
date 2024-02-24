using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace RRule_Generator_Form
{

    public class NumericTextBox : TextBox
    {
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out int result))
            {
                e.Handled = true;
            }
            else
            {
                // Check if the resulting value is within the allowed range
                string newText = this.Text.Remove(this.SelectionStart, this.SelectionLength).Insert(this.SelectionStart, e.Text);
                if (string.IsNullOrEmpty(newText))
                {
                    newText = "0";
                }
                if (int.TryParse(newText, out int value) && (value < 0 || value > 999))
                {
                    e.Handled = true;
                }
            }

            base.OnPreviewTextInput(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = "0";
            }

            base.OnLostFocus(e);
        }
    }
}
