using System.ComponentModel;

namespace TPVWPF.Models.Cart
{
    public class CartItem : INotifyPropertyChanged
    {
        private Product _product;
        private int _quantity;

        public Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }
        public decimal Subtotal
        {
            get { return Quantity * Product?.Price ?? 0; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
