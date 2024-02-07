using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace VendingMachineApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        // Get All
        [HttpGet("GetAllProducts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            var allProducts = await _productService.GetAllProductsAsync();
            if (allProducts.IsNullOrEmpty())
                return Ok("No products for selling");
            return Ok(allProducts);
        }
        // Get by id
        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product is null)
                return NotFound("Product not found");
            return Ok(product);
        }
        // Get User's products by user id
        [HttpGet("getSellerProducts/{sellerId}")]
        public async Task<IActionResult> GetProductBySellerId(string sellerId)
        {
            // if sending no seller id that is bad requset
            if (sellerId == null) return BadRequest(string.Empty);

            // check that user sending the requset is a seller if not return bad request 
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISSELLER)) return Unauthorized(ExceptionMessages.OnlySellerUser);

            // Check that seller user (id == target id), seller can only check owned products
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (sellerId != userId) return Unauthorized("Can't view another seller products");

            var sellerProducts = await _productService.GetProductsBySellerIdAsync(sellerId);
            return Ok(sellerProducts);
        }
        // Add new product
        [HttpPost]
        public async Task<IActionResult> AddNewProduct(ProductDto newProductDto)
        {
            // check that sender is a seller user
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISSELLER)) return Unauthorized(ExceptionMessages.OnlySellerUser);

            try
            {
                // get seller id
                var sellerId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                // Map dto to model and add user(seller) id into the new model
                var newProduct = new Product
                {
                    ProductName = newProductDto.ProductName,
                    Cost = newProductDto.Cost,
                    AmountAvailable = newProductDto.AmountAvailable,
                    SellerId = sellerId
                };

                var response = await _productService.AddNewProduct(newProduct);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
        }
        // Update product
        [HttpPut]
        public async Task<IActionResult> UpdateProductById(ProductDto productDto)
        {
            // check that sender is a seller user
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISSELLER)) return Unauthorized(ExceptionMessages.OnlySellerUser);
            try
            {
                // get seller id
                var sellerId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                var response = await _productService.UpdateProductAsync(productDto, sellerId);
                if (response is null) return NotFound("Product not found");

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized($"Unauthorized request: {ex.Message}");
            }
        }
        // Delete product
        [HttpDelete]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            // check that sender is a seller user
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISSELLER)) return Unauthorized(ExceptionMessages.OnlySellerUser);
            try
            {
                // get seller id
                var sellerId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                // send product and seller ids to product service to delete the product
                var response = await _productService.DeleteProductAsync(productId, sellerId);
                if (response is null) return NotFound("Product not found");

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized($"Unauthorized request: {ex.Message}");
            }
        }
    }
}
