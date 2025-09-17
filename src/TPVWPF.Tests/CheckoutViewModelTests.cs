using FluentAssertions;
using Moq;
using System.Collections.ObjectModel;
using TPVWPF.Models;
using TPVWPF.Models.Cart;
using TPVWPF.ViewModels;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.Tests
{
    public class TestableObservableCollection<T> : ObservableCollection<T>
    {
        public bool ClearCalled { get; private set; }

        public TestableObservableCollection(IEnumerable<T> collection) : base(collection) { }

        protected override void ClearItems()
        {
            ClearCalled = true;
            base.ClearItems();
        }
    }

    public class CheckoutViewModelTests
    {
        private readonly Mock<ICartViewModel> _cartViewModelMock;
        private readonly CheckoutViewModel _viewModel;

        public CheckoutViewModelTests()
        {
            _cartViewModelMock = new Mock<ICartViewModel>();

            _cartViewModelMock.SetupGet(x => x.Items).Returns(new ObservableCollection<CartItem>
            {
                new CartItem()
                {
                    Product = new Product { Id = 1, Name = "Test Product", Price = 10.99m },
                    Quantity = 2
                }
            });
            _cartViewModelMock.SetupGet(x => x.Total).Returns(21.98m);

            _viewModel = new CheckoutViewModel(_cartViewModelMock.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Assert
            _viewModel.PaymentMethods.Should().NotBeNull();
            _viewModel.PaymentMethods.Should().HaveCount(3);
            _viewModel.PaymentMethods.Should().Contain("Efectivo");
            _viewModel.FormattedTotal.Should().Be("Total a Pagar: €21.98");
            _viewModel.FinalizeOrderCommand.Should().NotBeNull();
        }

        [Fact]
        public void PropertyChanges_ShouldRaiseNotifications()
        {
            // Arrange
            var monitor = _viewModel.Monitor();

            // Act & Assert
            _viewModel.CustomerFirstName = "John";
            monitor.Should().RaisePropertyChangeFor(x => x.CustomerFirstName);

            _viewModel.CustomerLastName = "Doe";
            monitor.Should().RaisePropertyChangeFor(x => x.CustomerLastName);

            _viewModel.CustomerAddress = "123 Main St";
            monitor.Should().RaisePropertyChangeFor(x => x.CustomerAddress);

            _viewModel.SelectedPaymentMethod = "Efectivo";
            monitor.Should().RaisePropertyChangeFor(x => x.SelectedPaymentMethod);
        }

        [Fact]
        public void FinalizeOrder_WithMissingData_ShouldShowWarning()
        {
            // Arrange
            var initialItems = new ObservableCollection<CartItem>(_cartViewModelMock.Object.Items);
            var initialTotal = _cartViewModelMock.Object.Total;

            // Act
            _viewModel.FinalizeOrderCommand.Execute(null);

            // Assert
            _cartViewModelMock.Object.Items.Should().BeEquivalentTo(initialItems);
            _cartViewModelMock.Object.Total.Should().Be(initialTotal);
        }

        [Fact]
        public void FinalizeOrder_WithValidData_ShouldClearCart()
        {
            // Arrange
            var testItems = new TestableObservableCollection<CartItem>(_cartViewModelMock.Object.Items.ToList());
            _cartViewModelMock.SetupGet(x => x.Items).Returns(testItems);

            _viewModel.CustomerFirstName = "John";
            _viewModel.CustomerLastName = "Doe";
            _viewModel.CustomerAddress = "123 Main St";
            _viewModel.SelectedPaymentMethod = "Efectivo";

            // Act
            _viewModel.FinalizeOrderCommand.Execute(null);

            // Assert
            testItems.ClearCalled.Should().BeTrue("Cart items should be cleared");
            testItems.Should().BeEmpty("Cart should be empty after checkout");
        }


        [Fact]
        public void FinalizeOrder_OnSuccess_ShouldRequestNavigation()
        {
            // Arrange
            var navigationRequested = false;
            _viewModel.RequestNavigationToCatalog += () => navigationRequested = true;

            // Set valid data
            _viewModel.CustomerFirstName = "John";
            _viewModel.CustomerLastName = "Doe";
            _viewModel.CustomerAddress = "123 Main St";
            _viewModel.SelectedPaymentMethod = "Efectivo";

            // Act
            _viewModel.FinalizeOrderCommand.Execute(null);

            // Assert
            navigationRequested.Should().BeTrue();
        }

        [Fact]
        public void Items_ShouldReturnCartItems()
        {
            // Arrange
            var expectedItem = new CartItem
            {
                Product = new Product { Id = 1, Name = "Test" },
                Quantity = 2
            };
            _cartViewModelMock.SetupGet(x => x.Items)
                .Returns(new ObservableCollection<CartItem> { expectedItem });

            // Act
            var items = _viewModel.Items;

            // Assert
            items.Should().ContainSingle();
            items.First().Should().BeEquivalentTo(expectedItem);
        }

        [Fact]
        public void Total_ShouldReturnCartTotal()
        {
            // Arrange
            const decimal expectedTotal = 50.99m;
            _cartViewModelMock.SetupGet(x => x.Total).Returns(expectedTotal);

            // Act
            var total = _viewModel.Total;

            // Assert
            total.Should().Be(expectedTotal);
        }
    }
}