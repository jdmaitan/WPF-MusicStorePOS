using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace TPVWPF.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private CultureInfo _currentCulture;
        private readonly ResourceManager _resourceManager = new ResourceManager("TPVWPF.Resources.Strings", typeof(LocalizationService).Assembly);

        public LocalizationService()
        {
            _currentCulture = CultureInfo.DefaultThreadCurrentCulture;
        }

        public CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                if (_currentCulture != value)
                {
                    _currentCulture = value;
                    CultureInfo.DefaultThreadCurrentCulture = value;
                    CultureInfo.DefaultThreadCurrentUICulture = value;
                    OnPropertyChanged(nameof(CurrentCulture));
                    OnPropertyChanged(nameof(MainWindowTitle));
                    OnPropertyChanged(nameof(CatalogButtonText));
                    OnPropertyChanged(nameof(CartButtonText));
                    OnPropertyChanged(nameof(SalesHistoryButtonText));
                    OnPropertyChanged(nameof(ManageProductsButtonText));
                    OnPropertyChanged(nameof(ReportsButtonText));
                    OnPropertyChanged(nameof(SettingsButtonText));
                    OnPropertyChanged(nameof(CatalogViewTitle));
                    OnPropertyChanged(nameof(ProductDetailsButtonText));
                    OnPropertyChanged(nameof(AddToCartButtonText));
                    OnPropertyChanged(nameof(CartViewTitle));
                    OnPropertyChanged(nameof(EmptyCartMessage));
                    OnPropertyChanged(nameof(DeleteButtonText));
                    OnPropertyChanged(nameof(TotalLabelText));
                    OnPropertyChanged(nameof(GoToCheckoutButtonText));
                    OnPropertyChanged(nameof(CheckoutViewTitle));
                    OnPropertyChanged(nameof(OrderSummaryLabel));
                    OnPropertyChanged(nameof(CustomerDetailsLabel));
                    OnPropertyChanged(nameof(FirstNameLabel));
                    OnPropertyChanged(nameof(LastNameLabel));
                    OnPropertyChanged(nameof(AddressLabel));
                    OnPropertyChanged(nameof(PaymentMethodLabel));
                    OnPropertyChanged(nameof(FinalizePurchaseButtonText));
                    OnPropertyChanged(nameof(OrderDetailViewTitle));
                    OnPropertyChanged(nameof(CustomerInformationHeader));
                    OnPropertyChanged(nameof(CustomerFirstNameLabel));
                    OnPropertyChanged(nameof(CustomerLastNameLabel));
                    OnPropertyChanged(nameof(CustomerAddressLabel));
                    OnPropertyChanged(nameof(CustomerPaymentMethodLabel));
                    OnPropertyChanged(nameof(OrderTotalLabel));
                    OnPropertyChanged(nameof(ProductAdminViewTitle));
                    OnPropertyChanged(nameof(NewProductButtonText));
                    OnPropertyChanged(nameof(EditProductButtonText));
                    OnPropertyChanged(nameof(DeleteProductButtonText));
                    OnPropertyChanged(nameof(ProductIdColumnHeader));
                    OnPropertyChanged(nameof(ProductNameColumnHeader));
                    OnPropertyChanged(nameof(ProductDescriptionColumnHeader));
                    OnPropertyChanged(nameof(ProductPriceColumnHeader));
                    OnPropertyChanged(nameof(ProductCategoryColumnHeader));
                    OnPropertyChanged(nameof(ProductDetailPriceLabel));
                    OnPropertyChanged(nameof(ProductDetailCategoryLabel));
                    OnPropertyChanged(nameof(ProductDetailDescriptionLabel));
                    OnPropertyChanged(nameof(AddToCartButtonTextDetail));
                    OnPropertyChanged(nameof(ProductEditViewTitle));
                    OnPropertyChanged(nameof(ProductNameLabelEdit));
                    OnPropertyChanged(nameof(ProductDescriptionLabelEdit));
                    OnPropertyChanged(nameof(ProductPriceLabelEdit));
                    OnPropertyChanged(nameof(ProductCategoryLabelEdit));
                    OnPropertyChanged(nameof(SaveButtonTextEdit));
                    OnPropertyChanged(nameof(CancelButtonTextEdit));
                    OnPropertyChanged(nameof(ReportsViewTitle));
                    OnPropertyChanged(nameof(TotalProductsLabel));
                    OnPropertyChanged(nameof(TotalSalesLabel));
                    OnPropertyChanged(nameof(HistoricalTotalRevenueLabel));
                    OnPropertyChanged(nameof(CurrentMonthRevenueLabel));
                    OnPropertyChanged(nameof(SalesHistoryViewTitle));
                    OnPropertyChanged(nameof(SettingsViewTitle));
                    OnPropertyChanged(nameof(LanguageLabelSettings));
                    OnPropertyChanged(nameof(SaveButtonTextSettings));
                    OnPropertyChanged(nameof(DateColumnHeader));
                    OnPropertyChanged(nameof(FirstNameColumnHeader));
                    OnPropertyChanged(nameof(LastNameColumnHeader));
                    OnPropertyChanged(nameof(TotalColumnHeader));
                    OnPropertyChanged(nameof(LoginViewTitle));
                    OnPropertyChanged(nameof(UsernameLabel));
                    OnPropertyChanged(nameof(PasswordLabel));
                    OnPropertyChanged(nameof(LoginButton));
                    OnPropertyChanged(nameof(ExportInvoiceButton));
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        public string MainWindowTitle => GetLocalizedString("MainWindowTitle");
        public string CatalogButtonText => GetLocalizedString("CatalogButtonText");
        public string CartButtonText => GetLocalizedString("CartButtonText");
        public string SalesHistoryButtonText => GetLocalizedString("SalesHistoryButtonText");
        public string ManageProductsButtonText => GetLocalizedString("ManageProductsButtonText");
        public string ReportsButtonText => GetLocalizedString("ReportsButtonText");
        public string SettingsButtonText => GetLocalizedString("SettingsButtonText");
        public string CatalogViewTitle => GetLocalizedString("CatalogViewTitle");
        public string ProductDetailsButtonText => GetLocalizedString("ProductDetailsButtonText");
        public string AddToCartButtonText => GetLocalizedString("AddToCartButtonText");
        public string CartViewTitle => GetLocalizedString("CartViewTitle");
        public string EmptyCartMessage => GetLocalizedString("EmptyCartMessage");
        public string DeleteButtonText => GetLocalizedString("DeleteButtonText");
        public string TotalLabelText => GetLocalizedString("TotalLabelText");
        public string GoToCheckoutButtonText => GetLocalizedString("GoToCheckoutButtonText");
        public string CheckoutViewTitle => GetLocalizedString("CheckoutViewTitle");
        public string OrderSummaryLabel => GetLocalizedString("OrderSummaryLabel");
        public string CustomerDetailsLabel => GetLocalizedString("CustomerDetailsLabel");
        public string FirstNameLabel => GetLocalizedString("FirstNameLabel");
        public string LastNameLabel => GetLocalizedString("LastNameLabel");
        public string AddressLabel => GetLocalizedString("AddressLabel");
        public string PaymentMethodLabel => GetLocalizedString("PaymentMethodLabel");
        public string FinalizePurchaseButtonText => GetLocalizedString("FinalizePurchaseButtonText");
        public string OrderDetailViewTitle => GetLocalizedString("OrderDetailViewTitle");
        public string CustomerInformationHeader => GetLocalizedString("CustomerInformationHeader");
        public string CustomerFirstNameLabel => GetLocalizedString("CustomerFirstNameLabel");
        public string CustomerLastNameLabel => GetLocalizedString("CustomerLastNameLabel");
        public string CustomerAddressLabel => GetLocalizedString("CustomerAddressLabel");
        public string CustomerPaymentMethodLabel => GetLocalizedString("CustomerPaymentMethodLabel");
        public string OrderTotalLabel => GetLocalizedString("OrderTotalLabel");
        public string ProductAdminViewTitle => GetLocalizedString("ProductAdminViewTitle");
        public string NewProductButtonText => GetLocalizedString("NewProductButtonText");
        public string EditProductButtonText => GetLocalizedString("EditProductButtonText");
        public string DeleteProductButtonText => GetLocalizedString("DeleteProductButtonText");
        public string ProductIdColumnHeader => GetLocalizedString("ProductIdColumnHeader");
        public string ProductNameColumnHeader => GetLocalizedString("ProductNameColumnHeader");
        public string ProductDescriptionColumnHeader => GetLocalizedString("ProductDescriptionColumnHeader");
        public string ProductPriceColumnHeader => GetLocalizedString("ProductPriceColumnHeader");
        public string ProductCategoryColumnHeader => GetLocalizedString("ProductCategoryColumnHeader");
        public string ProductDetailPriceLabel => GetLocalizedString("ProductDetailPriceLabel");
        public string ProductDetailCategoryLabel => GetLocalizedString("ProductDetailCategoryLabel");
        public string ProductDetailDescriptionLabel => GetLocalizedString("ProductDetailDescriptionLabel");
        public string AddToCartButtonTextDetail => GetLocalizedString("AddToCartButtonTextDetail");
        public string ProductEditViewTitle => GetLocalizedString("ProductEditViewTitle");
        public string ProductNameLabelEdit => GetLocalizedString("ProductNameLabelEdit");
        public string ProductDescriptionLabelEdit => GetLocalizedString("ProductDescriptionLabelEdit");
        public string ProductPriceLabelEdit => GetLocalizedString("ProductPriceLabelEdit");
        public string ProductCategoryLabelEdit => GetLocalizedString("ProductCategoryLabelEdit");
        public string SaveButtonTextEdit => GetLocalizedString("SaveButtonTextEdit");
        public string CancelButtonTextEdit => GetLocalizedString("CancelButtonTextEdit");
        public string ReportsViewTitle => GetLocalizedString("ReportsViewTitle");
        public string TotalProductsLabel => GetLocalizedString("TotalProductsLabel");
        public string TotalSalesLabel => GetLocalizedString("TotalSalesLabel");
        public string HistoricalTotalRevenueLabel => GetLocalizedString("HistoricalTotalRevenueLabel");
        public string CurrentMonthRevenueLabel => GetLocalizedString("CurrentMonthRevenueLabel");
        public string SalesHistoryViewTitle => GetLocalizedString("SalesHistoryViewTitle");
        public string SettingsViewTitle => GetLocalizedString("SettingsViewTitle");
        public string LanguageLabelSettings => GetLocalizedString("LanguageLabelSettings");
        public string SaveButtonTextSettings => GetLocalizedString("SaveButtonTextSettings");
        public string DateColumnHeader => GetLocalizedString("DateColumnHeader");
        public string FirstNameColumnHeader => GetLocalizedString("FirstNameColumnHeader");
        public string LastNameColumnHeader => GetLocalizedString("LastNameColumnHeader");
        public string TotalColumnHeader => GetLocalizedString("TotalColumnHeader");
        public string LoginViewTitle => GetLocalizedString("LoginViewTitle");
        public string UsernameLabel => GetLocalizedString("UsernameLabel");
        public string PasswordLabel => GetLocalizedString("PasswordLabel");
        public string LoginButton => GetLocalizedString("LoginButton");
        public string ExportInvoiceButton => GetLocalizedString("ExportInvoiceButton");


        public string GetLocalizedString(string key)
        {
            return _resourceManager.GetString(key, _currentCulture);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}