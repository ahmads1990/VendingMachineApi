using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VendingMachineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel registerModel)
        {
            var result = await _userService.RegisterUserAsync(registerModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel loginModel)
        {
            var result = await _userService.LoginUserAsync(loginModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            //Todo send confirmation mail
            return Ok(result);
        }
        [HttpPut("Deposit/{addDeposit}")]
        [Authorize]
        public async Task<IActionResult> AddToDeposit(int addDeposit)
        {
            // check that user sending the requset is a buyer if not return bad request 
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISBUYER))
                return Unauthorized(ExceptionMessages.OnlyBuyerUser);

            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            UpdateDepositModel updateDepositModel = new UpdateDepositModel
            {
                BuyerId = userId,
                Deposit = addDeposit,
            };

            var result = await _userService.BuyerAddToDepositAsync(updateDepositModel);
            if (!string.IsNullOrEmpty(updateDepositModel.Message))
                BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPut("Reset")]
        [Authorize]
        public async Task<IActionResult> ResetDeposit()
        {
            // check that user sending the requset is a buyer if not return bad request 
            if (!User.Claims.Any(c => c.Type == CustomClaimTypes.ISBUYER))
                return Unauthorized(ExceptionMessages.OnlyBuyerUser);

            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            UpdateDepositModel updateDepositModel = new UpdateDepositModel
            {
                BuyerId = userId,
                Deposit = 0,
            };

            var result = await _userService.BuyerResetDepositAsync(updateDepositModel);
            if (!string.IsNullOrEmpty(updateDepositModel.Message))
                BadRequest(result.Message);

            return Ok(result);
        }
    }
}
