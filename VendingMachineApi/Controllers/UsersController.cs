﻿using Microsoft.AspNetCore.Mvc;
using VendingMachineApi.Dtos;
using VendingMachineApi.Services.Interfaces;

namespace VendingMachineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel registerModel)
        {
            var result = await _authService.RegisterUserAsync(registerModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel loginModel)
        {
            var result = await _authService.LoginUserAsync(loginModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            //Todo send confirmation mail
            return Ok(result);
        }
    }
}
