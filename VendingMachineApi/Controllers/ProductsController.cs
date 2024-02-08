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
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public ProductsController(IProductService productService, ILogger<ProductsController> logger, IUserService userService)
        {
            _productService = productService;
            _logger = logger;
            _userService = userService;
        }
        // Get All
        [HttpGet("GetAllProducts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Seri Log is Working");

            var allProducts = await _productService.GetAllProductsAsync();
            if (allProducts.IsNullOrEmpty())
                return Ok("No products for selling");

            _logger.LogInformation("Returned AllProducts");
            return Ok(allProducts);
        }
        // Get by id
        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product is null)
                return NotFound(ExceptionMessages.EntityDoesntExist);

            _logger.LogInformation("Returned Product By Id");
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
            var userId = User.Claims.First(c => c.Type == CustomClaimTypes.UserId).Value;
            if (sellerId != userId) return Unauthorized(ExceptionMessages.UnAuthorizedSeller);

            var sellerProducts = await _productService.GetProductsBySellerIdAsync(sellerId);

            _logger.LogInformation("Returned Product By SellerId");
            return Ok(sellerProducts);
        }
        // Add new product
        [HttpPost]
        public async Task<IActionResult> AddNewProduct(ProductDto newProductDto)
        {
            // check that sender is a seller user
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISSELLER))
                return Unauthorized(ExceptionMessages.OnlySellerUser);

            try
            {
                // get seller id
                var sellerId = User.Claims.First(c => c.Type == CustomClaimTypes.UserId).Value;
                // Map dto to model and add user(seller) id into the new model
                var newProduct = new Product
                {
                    ProductName = newProductDto.ProductName,
                    Cost = newProductDto.Cost,
                    AmountAvailable = newProductDto.AmountAvailable,
                    SellerId = sellerId
                };

                var response = await _productService.AddNewProduct(newProduct);
                _logger.LogInformation($"New product added successfully by seller {sellerId}");
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid request");
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
                var sellerId = User.Claims.First(c => c.Type == CustomClaimTypes.UserId).Value;

                var response = await _productService.UpdateProductAsync(productDto, sellerId);
                if (response is null)
                {
                    _logger.LogInformation($"Product not found for update by seller {sellerId}");
                    return NotFound(ExceptionMessages.EntityDoesntExist);
                }

                _logger.LogInformation($"Product updated successfully by seller {sellerId}");
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid request");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized request");
                return Unauthorized($"Unauthorized request: {ex.Message}");
            }
        }
        // Delete product
        [HttpDelete]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            // check that sender is a seller user
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISSELLER))
                return Unauthorized(ExceptionMessages.OnlySellerUser);
            try
            {
                // get seller id
                var sellerId = User.Claims.First(c => c.Type == CustomClaimTypes.UserId).Value;
                // send product and seller ids to product service to delete the product
                var response = await _productService.DeleteProductAsync(productId, sellerId);
                if (response is null)
                {
                    _logger.LogInformation($"Product not found for removal by seller {sellerId}");
                    return NotFound(ExceptionMessages.EntityDoesntExist);
                }

                _logger.LogInformation($"Product removed successfully by seller {sellerId}");
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized request");
                return Unauthorized($"Unauthorized request: {ex.Message}");
            }
        }
        [HttpPost("Buy")]
        public async Task<IActionResult> BuyProduct(BuyProductModel buyProductModel)
        {
            if (buyProductModel.Quantity <= 0)
                return BadRequest(ExceptionMessages.InvalidProductCostOrAmount);
            // check that sender is a buyer user
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISBUYER))
                return Unauthorized(ExceptionMessages.OnlyBuyerUser);
            try
            {
                // get buyer id
                var buyerId = User.Claims.First(c => c.Type == CustomClaimTypes.UserId).Value;
                // get product
                var product = await _productService.GetProductByIdAsync(buyProductModel.ProductId);
                if (product is null)
                    return NotFound(ExceptionMessages.EntityDoesntExist);
                // Check amount
                if (product.AmountAvailable < buyProductModel.Quantity)
                    return BadRequest($"Required quantity ({buyProductModel.Quantity}) is higher than Amount Available {product.AmountAvailable}");
                // Enough amount can be purchased, Check users balance
                int orderCost = buyProductModel.Quantity * product.Cost;
                var canUserAffordIt = await _userService.CheckHaveEnoughDeposit(buyerId, orderCost);
                if (!canUserAffordIt)
                    return BadRequest("You cannot afford it");
                // All Checks done procceed to do it
                // create ProductDto to update the product remaning quantity
                var productDto = new ProductDto
                {
                    ProductId = product.ProductId,
                    AmountAvailable = product.AmountAvailable,
                    Cost = product.Cost,
                    ProductName = product.ProductName
                };
                // update product amount
                productDto.AmountAvailable -= buyProductModel.Quantity;
                var updatedProduct = await _productService.UpdateProductAsync(productDto, product.SellerId);
                // TODO add money to the seller

                // send user their cash
                return Ok("Here is your change");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
