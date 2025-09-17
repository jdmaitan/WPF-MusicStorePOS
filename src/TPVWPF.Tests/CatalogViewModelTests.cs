using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Windows.Data;
using TPVWPF.Data;
using TPVWPF.Models;
using TPVWPF.ViewModels;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.Tests
{
    public class CatalogViewModelTests
    {
        private readonly Mock<IApplicationDbContext> _dbContextMock;
        private readonly Mock<ICartViewModel> _cartViewModelMock;
        private readonly CatalogViewModel _viewModel;

        public CatalogViewModelTests()
        {
            _dbContextMock = new Mock<IApplicationDbContext>();
            _cartViewModelMock = new Mock<ICartViewModel>();

            var emptyProducts = new List<Product>().AsQueryable();
            var mockDbSet = MockDbSet.Create(emptyProducts);
            _dbContextMock.Setup(x => x.Products).Returns(mockDbSet.Object);

            _viewModel = new CatalogViewModel(_dbContextMock.Object, _cartViewModelMock.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product", Price = 10.99m }
            };

            var mockDbSet = MockDbSet.Create(products);
            _dbContextMock.Setup(x => x.Products).Returns(mockDbSet.Object);

            // Act
            var vm = new CatalogViewModel(_dbContextMock.Object, _cartViewModelMock.Object);

            // Assert
            vm.Products.Should().NotBeNull();
            vm.Products.Should().HaveCount(1);
            vm.Products.First().Name.Should().Be("Test Product");
        }

        [Fact]
        public void Constructor_ShouldInitializeCommands()
        {
            // Assert
            _viewModel.ShowProductDetailsCommand.Should().NotBeNull();
            _viewModel.AddToCartCommand.Should().NotBeNull();
        }

        [Fact]
        public void SelectedProduct_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product" };
            var monitor = _viewModel.Monitor();

            // Act
            _viewModel.SelectedProduct = product;

            // Assert
            _viewModel.SelectedProduct.Should().Be(product);
            monitor.Should().RaisePropertyChangeFor(x => x.SelectedProduct);
        }

        [Fact]
        public void AddToCartCommand_WithValidProduct_ShouldAddProductToCart()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product" };

            // Act
            _viewModel.AddToCartCommand.Execute(product);

            // Assert
            _cartViewModelMock.Verify(x => x.AddProduct(product), Times.Once);
        }

        [Fact]
        public void AddToCartCommand_WithNullParameter_ShouldNotThrow()
        {
            // Act
            var act = () => _viewModel.AddToCartCommand.Execute(null);

            // Assert
            act.Should().NotThrow();
            _cartViewModelMock.Verify(x => x.AddProduct(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public void ShowProductDetailsCommand_WithValidProduct_ShouldNotThrow()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product" };

            // Act
            var act = () => _viewModel.ShowProductDetailsCommand.Execute(product);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void LoadProducts_WhenDbThrowsException_ShouldNotCrash()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>()
                .Setup(m => m.Provider)
                .Throws(new Exception("Test error"));

            _dbContextMock.Setup(x => x.Products)
                .Returns(mockDbSet.Object);

            // Act
            var vm = new CatalogViewModel(_dbContextMock.Object, _cartViewModelMock.Object);

            // Assert
            vm.Products.Should().BeNull();
        }

        [Fact]
        public void SearchText_WhenChanged_ShouldFilterProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Apple", Price = 1.99m },
                new Product { Id = 2, Name = "Banana", Price = 0.99m }
            };

            var mockDbSet = MockDbSet.Create(products);
            _dbContextMock.Setup(x => x.Products).Returns(mockDbSet.Object);

            var vm = new CatalogViewModel(_dbContextMock.Object, _cartViewModelMock.Object);

            // Act
            vm.SearchText = "app";

            // Assert
            vm.Products.Should().HaveCount(2);
            CollectionViewSource.GetDefaultView(vm.Products)
                .Cast<Product>()
                .Should().ContainSingle(p => p.Name == "Apple");
        }
    }
}