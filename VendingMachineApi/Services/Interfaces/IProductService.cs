namespace VendingMachineApi.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> GetProductsBySellerIdAsync(string sellerId);
        Task<Product> AddNewProduct(Product product);
        Task<Product?> UpdateProductAsync(ProductDto productDto, string sellerId);
        Task<Product?> DeleteProductAsync(int productId, string sellerId);
    }
}
