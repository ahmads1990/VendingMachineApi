using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachineApi.Models;
using VendingMachineApi.Services;
using VendingMachineApi.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace VendingMachineApi.Tests
{
    public class ProductServiceTests
    {
        AppDbContext _appDbContext;
        IProductService _productService;
        SqliteConnection _connection;
        static int seedDataCount = 5;
        static int nonExistingId = 1000;
        public List<Product> GetProductsSeedData()
        {
            return new List<Product>()
            {
                new Product { ProductId = 1, ProductName = "Product1", AmountAvailable = 5, Cost = 10, SellerId = "s1" },
                new Product { ProductId = 2, ProductName = "Product2", AmountAvailable = 8, Cost = 15, SellerId = "s2" },
                new Product { ProductId = 3, ProductName = "Product3", AmountAvailable = 3, Cost = 20, SellerId = "s1" },
                new Product { ProductId = 4, ProductName = "Product4", AmountAvailable = 10, Cost = 25, SellerId = "s3" },
                new Product { ProductId = 5, ProductName = "Product5", AmountAvailable = 12, Cost = 30, SellerId = "s2" }
            };
        }
        [SetUp]
        public void Setup()
        {
            // create and open new SQLite Connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            // configure options 
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseSqlite(_connection)
               .Options;
            // seeding new context 
            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Products.AddRange(GetProductsSeedData());
                context.SaveChanges();
            }
            // testing context
            _appDbContext = new AppDbContext(options);
            // Arrange
            _productService = new ProductService(_appDbContext);
        }
        [TearDown]
        public void TearDown()
        {
            _appDbContext.Dispose();
            _connection.Close();
        }
        // naming = (MethodTesting)_Input State_ Expected Output

        // GetAllProductsAsync
        [Test]
        public async Task GetAllProductsAsync_ValidData_NotNull()
        {
            // Act
            var result = await _productService.GetAllProductsAsync();
            // Assert
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task GetAllProductsAsync_ValidData_CountEqualSeed()
        {
            // Act
            var result = await _productService.GetAllProductsAsync();
            // Assert
            Assert.That(result.Count(), Is.EqualTo(seedDataCount));
        }
        // GetProductByIdAsync
        [Test]
        public async Task GetProductByIdAsync_ValidExistingId_NotNull()
        {
            // Assert 
            int testId = 1;
            // Act
            var result = await _productService.GetProductByIdAsync(testId);
            // Assert
            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task GetProductByIdAsync_ValidNonExistingId_Null()
        {
            // Assert 
            int testId = nonExistingId;
            // Act
            var result = await _productService.GetProductByIdAsync(testId);
            // Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public async Task GetProductByIdAsync_InvalidId_Throw()
        {
            // Assert 
            int testId = -1;
            // Act
            var exception = Assert.Throws<ArgumentException>(() =>
                _productService.GetProductByIdAsync(testId));
            // Assert
            //Assert.That(exception.Message, Is.EqualTo(""));
        }
        // GetProductsBySellerIdAsync
        // AddNewProduct
        [Test]
        public async Task AddNewProduct_ValidProduct_ValidProduct()
        {
            // Arrange
            var newProduct = new Product { ProductName = "Product6", AmountAvailable = 12, Cost = 30, SellerId = "s2" };
            // Act
            var result = await _productService.AddNewProduct(newProduct);
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo(newProduct.ProductName));
            Assert.That(result.ProductId, Is.EqualTo(6));
        }
        [Test]
        public async Task AddNewProduct_InValidId_Throw()
        {
            // Arrange
            var newProduct = new Product { ProductId = 6, ProductName = "Product6", AmountAvailable = 12, Cost = 30, SellerId = "s2" };
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));
        }
        [Test]
        public async Task AddNewProduct_InValidName_Throw()
        {
            // Arrange
            var newProduct = new Product { ProductName = null, AmountAvailable = 12, Cost = 30, SellerId = "s2" };
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));
        }
        [Test]
        public void AddNewProduct_InValidAmountAvailable_Throw()
        {
            // Arrange
            var newProduct = new Product { ProductName = "Product6", AmountAvailable = 0, Cost = 30, SellerId = "s2" };
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));
        }
        [Test]
        public async Task AddNewProduct_InValidCost_Throw()
        {
            // Arrange
            var newProduct = new Product { ProductName = "Product6", AmountAvailable = 12, Cost = 0, SellerId = "s2" };
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));
        }
        [Test]
        public async Task AddNewProduct_InValidCostNotDividableBy5_Throw()
        {
            // Arrange
            var newProduct = new Product { ProductName = "Product6", AmountAvailable = 12, Cost = 3, SellerId = "s2" };
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.AddNewProduct(newProduct));
        }
        // UpdateProductAsync
        [Test]
        public async Task UpdateProductAsync_ValidProduct_ValidProduct()
        {
            // Arrange
            var updatedProductDto = new Dtos.ProductDto { ProductId = 1, ProductName = "UpdatedProduct1", AmountAvailable = 6, Cost = 15 };
            string sellerId = "s1";
            // Act
            var result = await _productService.UpdateProductAsync(updatedProductDto, sellerId);
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo(updatedProductDto.ProductName));
            Assert.That(result.ProductId, Is.EqualTo(updatedProductDto.ProductId));
        }
        [Test]
        public async Task UpdateProductAsync_InvalidId_Throw()
        {
            // Arrange
            var updatedProductDto = new Dtos.ProductDto { ProductId = -1, ProductName = "UpdatedProduct1", AmountAvailable = 6, Cost = 15 };
            string sellerId = "s1";
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProductDto, sellerId));
        }
        [Test]
        public async Task UpdateProductAsync_InvalidName_Throw()
        {
            // Arrange
            var updatedProductDto = new Dtos.ProductDto { ProductId = 1, ProductName = "", AmountAvailable = 6, Cost = 15 };
            string sellerId = "s1";
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProductDto, sellerId));
        }
        [Test]
        public async Task UpdateProductAsync_InvalidAmountAvailable_Throw()
        {
            // Arrange
            var updatedProductDto = new Dtos.ProductDto { ProductId = 1, ProductName = "UpdatedProduct1", AmountAvailable = 0, Cost = 15 };
            string sellerId = "s1";
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProductDto, sellerId));
        }
        [Test]
        public async Task UpdateProductAsync_InvalidCost_Throw()
        {
            // Arrange
            var updatedProductDto = new Dtos.ProductDto { ProductId = 1, ProductName = "UpdatedProduct1", AmountAvailable = 6, Cost = 0 };
            string sellerId = "s1";
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProductDto, sellerId));
        }
        [Test]
        public async Task UpdateProductAsync_InValidCostNotDividableBy5_Throw()
        {
            // Arrange
            var updatedProductDto = new Dtos.ProductDto { ProductId = 1, ProductName = "UpdatedProduct1", AmountAvailable = 6, Cost = 3 };
            string sellerId = "s1";
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProductDto, sellerId));
        }
        [Test]
        public async Task UpdateProductAsync_NotOwnerSeller_Throw()
        {
            // Arrange
            var updatedProductDto = new Dtos.ProductDto { ProductId = 1, ProductName = "UpdatedProduct1", AmountAvailable = 6, Cost = 15 };
            string sellerId = "s2";
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProductDto, sellerId));
        }
        [Test]
        public async Task UpdateProductAsync_InvalidSellerId_Throw()
        {
            // Arrange
            var updatedProductDto = new Dtos.ProductDto { ProductId = -1, ProductName = "UpdatedProduct1", AmountAvailable = 6, Cost = 15 };
            string sellerId = "";
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _productService.UpdateProductAsync(updatedProductDto, sellerId));
        }
        // DeleteProductAsync
        [Test]
        public async Task DeleteProductAsync_ValidId_NotNull()
        {
            // Arrange
            int productIndex = 0;
            var product = GetProductsSeedData()[productIndex];
            int testId = product.ProductId;
            string sellerId = product.SellerId;
            // Act
            var result = await _productService.DeleteProductAsync(testId, sellerId);
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductId, Is.EqualTo(testId));
        }
        [Test]
        public async Task DeleteProductAsync_ValidNonExistingId_Null()
        {
            // Arrange
            int productIndex = 0;
            var product = GetProductsSeedData()[productIndex];
            int testId = 6;
            string sellerId = product.SellerId;
            // Act
            var result = await _productService.DeleteProductAsync(testId, sellerId);
            // Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public async Task DeleteProductAsync_InvalidId_Throw()
        {
            // Arrange
            int productIndex = 0;
            var product = GetProductsSeedData()[productIndex];
            int testId = 0;
            string sellerId = product.SellerId;
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async()=>
                await _productService.DeleteProductAsync(productIndex, sellerId));
        }
        [Test]
        public async Task DeleteProductAsync_NotOwnerSeller_Throw()
        {
            // Arrange
            int productIndex = 0;
            var product = GetProductsSeedData()[productIndex];
            int testId = product.ProductId;
            string sellerId = "s2";
            // Act
            var result = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _productService.DeleteProductAsync(testId, sellerId));
        }
        [Test]
        public async Task DeleteProductAsync_InvalidSellerId_Throw()
        {
            // Arrange
            int productIndex = 0;
            var product = GetProductsSeedData()[productIndex];
            int testId = product.ProductId;
            string sellerId = "";
            // Act
            var result = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _productService.DeleteProductAsync(productIndex, sellerId));
        }
    }
}
