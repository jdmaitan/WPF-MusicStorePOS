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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TPVWPF.Models.Cart;
using TPVWPF.ViewModels;

namespace TPVWPF.Views
{
    /// <summary>
    /// Interaction logic for CartView.xaml
    /// </summary>
    public partial class CartView : UserControl
    {
        public CartView()
        {
            InitializeComponent();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is CartViewModel viewModel)
            {
                var textBox = (TextBox)sender;
                if (int.TryParse(textBox.Text, out int quantity))
                {
                    var cartItem = (CartItem)textBox.DataContext;
                    cartItem.Quantity = quantity;
                    viewModel.UpdateQuantityCommand.Execute(cartItem);
                }
                else
                {
                    MessageBox.Show("Por favor, ingrese una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
