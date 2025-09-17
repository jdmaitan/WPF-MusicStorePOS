using FluentAssertions;
using Moq;
using System.Collections.ObjectModel;
using TPVWPF.Models;
using TPVWPF.Models.Cart;
using TPVWPF.ViewModels;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.Tests
{
    public class CartViewModelTests
    {
        private readonly CartViewModel _cartViewModel;
        private readonly Mock<ICartViewModel> _cartViewModelMock;

        public CartViewModelTests()
        {
            _cartViewModel = new CartViewModel();
            _cartViewModelMock = new Mock<ICartViewModel>();
        }

        [Fact]
        public void AddSameItemToCartShouldIncrementQuantityAndTotal()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product", Price = 10.00m };

            // Act
            _cartViewModel.AddProduct(product);
            _cartViewModel.AddProduct(product);

            // Assert
            _cartViewModel.Items.Should().HaveCount(1);
            _cartViewModel.Items.First().Quantity.Should().Be(2);
            _cartViewModel.Total.Should().Be(20.00m);
        }

        [Fact]
        public void ICartViewModel_RequestCheckoutEvent_ShouldBeRaised()
        {
            // Arrange
            var eventRaised = false;
            _cartViewModelMock.Object.RequestCheckout += () => eventRaised = true;

            // Act
            _cartViewModelMock.Raise(x => x.RequestCheckout += null);

            // Assert
            eventRaised.Should().BeTrue();
        }

        [Fact]
        public void ICartViewModel_AddProduct_ShouldAddToItems()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product", Price = 10.00m };
            var items = new ObservableCollection<CartItem>();

            _cartViewModelMock.SetupGet(x => x.Items).Returns(items);
            _cartViewModelMock.Setup(x => x.AddProduct(It.IsAny<Product>()))
                .Callback<Product>(p => items.Add(new CartItem { Product = p, Quantity = 1 }));

            // Act
            _cartViewModelMock.Object.AddProduct(product);

            // Assert
            items.Should().ContainSingle();
            items.First().Product.Should().BeEquivalentTo(product);
        }
    }
}