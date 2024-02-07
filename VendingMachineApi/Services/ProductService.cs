using Microsoft.EntityFrameworkCore;
using VendingMachineApi.Dtos;
using VendingMachineApi.Helpers;
using VendingMachineApi.Models;
using VendingMachineApi.Services.Interfaces;

namespace VendingMachineApi.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _dbContext;
        public ProductService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync() =>
            await _dbContext.Products.ToListAsync();

        public async Task<Product?> GetProductByIdAsync(int productId) =>
            await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

        public async Task<IEnumerable<Product>> GetProductsBySellerIdAsync(string sellerId) =>
            await _dbContext.Products.Where(p => p.SellerId == sellerId).ToListAsync();

        public async Task<Product> AddNewProduct(Product product)
        {
            // Check if entry is null or product has null values
            if (product is null || string.IsNullOrEmpty(product.ProductName) || string.IsNullOrEmpty(product.SellerId))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);
            // If product cost and AmountAvailable <= 0 throw error
            if (product.Cost <= 0 || product.Cost % 5 != 0 || product.AmountAvailable <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidProductCostOrAmount);
            // Check that entry id is unassigned 0 (the database should assign the productId not user)  
            if (product.ProductId != 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntitytId);

            // Try to store product in database 
            var createdProduct = await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Return created product
            return createdProduct.Entity;
        }
        public async Task<Product?> UpdateProductAsync(ProductDto productDto, string sellerId)
        {
            // Check if entryDto is null or seller id is null or product has null values then throw exception 
            if (productDto is null || string.IsNullOrEmpty(productDto.ProductName))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (string.IsNullOrEmpty(sellerId) || productDto.ProductId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntitytId);
            // Check if new values for product cost and AmountAvailable are invalid <=0
            if (productDto.Cost <= 0 || productDto.Cost % 5 != 0 || productDto.AmountAvailable <= 0) 
                throw new ArgumentException(ExceptionMessages.InvalidProductCostOrAmount);

            // Get product to be Updated
            var productToBeUpdated = await GetProductByIdAsync(productDto.ProductId);

            // if couldn't find return null
            if (productToBeUpdated is null) return null;
            // check that sent sellerId matches sellerId on return product (user sent in sellerId owns this product)
            if (productToBeUpdated.SellerId != sellerId) 
                throw new UnauthorizedAccessException(ExceptionMessages.UnAuthorizedSeller);

            // map new values to the product entity
            productToBeUpdated.ProductName = productDto.ProductName;
            productToBeUpdated.AmountAvailable = productDto.AmountAvailable;
            productToBeUpdated.Cost = productToBeUpdated.Cost;

            // update product
            var updateResult = _dbContext.Products.Update(productToBeUpdated);
            _dbContext.SaveChanges();

            // Return updated product
            return updateResult.Entity;
        }
        public async Task<Product?> DeleteProductAsync(int productId, string sellerId)
        {
            // Check for valid product id (positive number) and seller id (non null/empty string)
            if (productId <= 0 || string.IsNullOrEmpty(sellerId)) 
                throw new ArgumentException(ExceptionMessages.InvalidEntitytId);

            // Chech the product exists in database
            var productToBeDeleted = await GetProductByIdAsync(productId);
            if (productToBeDeleted is null) return null;
            // check that sent sellerId matches sellerId on return product (user sent in sellerId owns this product)
            if (productToBeDeleted.SellerId != sellerId) 
                throw new UnauthorizedAccessException(ExceptionMessages.UnAuthorizedSeller);

            // Delete product the save changes
            _dbContext.Products.Remove(productToBeDeleted);
            _dbContext.SaveChanges();

            // Return product if necessary
            return productToBeDeleted;
        }
    }
}
