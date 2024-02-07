using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VendingMachineApi.Controllers;
using VendingMachineApi.Dtos;
using VendingMachineApi.Helpers;
using VendingMachineApi.Services.Interfaces;

namespace VendingMachineApi.Tests
{
    [TestFixture]
    // Testing only that different routes return correct type of response
    public class UsersControllerTests
    {
        UsersController _usersController;
        Mock<IUserService> _userServiceMock;
        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _usersController = new UsersController(_userServiceMock.Object);
        }
        // naming = (MethodTesting)_Input State_ Expected Output

        // RegisterAsync
        [Test]
        public async Task RegisterAsync_ValidInput_OkResult()
        {
            // Arrange
            RegisterModel registerModel = new RegisterModel
            {
                Email = "test@example.com",
                UserName = "testuser",
                Password = "TestPassword",
                UserTypeClaim = "Buyer"
            };
            _userServiceMock.Setup(s => s.RegisterUserAsync(registerModel))
                .ReturnsAsync(new AuthModel { IsAuthenticated = true });
            // Act
            var result = await _usersController.RegisterAsync(registerModel);

            // Assert
            _userServiceMock.Verify(x => x.RegisterUserAsync(registerModel), Times.Once());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<AuthModel>());

            var authModel = okObjectResult.Value as AuthModel;
            Assert.That(authModel.IsAuthenticated, Is.True);
        }
        [Test]
        public async Task RegisterAsync_InvalidInput_BadRequestResult()
        {
            // Arrange
            RegisterModel registerModel = new RegisterModel
            {
                Email = "",
                UserName = "testuser",
                Password = "TestPassword",
                UserTypeClaim = "Buyer"
            };
            _userServiceMock.Setup(s => s.RegisterUserAsync(registerModel))
                .ReturnsAsync(new AuthModel { IsAuthenticated = false, Message = "Auth" });
            // Act
            var result = await _usersController.RegisterAsync(registerModel);

            // Assert
            _userServiceMock.Verify(x => x.RegisterUserAsync(registerModel), Times.Once());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.That(badRequestObjectResult.Value, Is.InstanceOf<string>());
        }
        // LoginAsync
        [Test]
        public async Task LoginAsync_ValidInput_OkResult()
        {
            // Arrange
            LoginModel loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "TestPassword",
            };
            _userServiceMock.Setup(s => s.LoginUserAsync(loginModel))
                .ReturnsAsync(new AuthModel { IsAuthenticated = true });
            // Act
            var result = await _usersController.LoginAsync(loginModel);

            // Assert
            _userServiceMock.Verify(x => x.LoginUserAsync(loginModel), Times.Once());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<AuthModel>());

            var authModel = okObjectResult.Value as AuthModel;
            Assert.That(authModel.IsAuthenticated, Is.True);
        }
        [Test]
        public async Task LoginAsync_InvalidInput_BadRequestResult()
        {
            // Arrange
            LoginModel loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "TestPassword",
            };
            _userServiceMock.Setup(s => s.LoginUserAsync(loginModel))
                .ReturnsAsync(new AuthModel { IsAuthenticated = false, Message = "Message" });
            // Act
            var result = await _usersController.LoginAsync(loginModel);

            // Assert
            _userServiceMock.Verify(x => x.LoginUserAsync(loginModel), Times.Once());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.That(badRequestObjectResult?.Value, Is.InstanceOf<string>());
        }
        // AddToDeposit
        [Test]
        public async Task AddToDeposit_ValidInput_OkResult()
        {
            // Arrange
            int newDeposit = 10;
            int deposit = 50;
            int expectedDeposit = deposit + newDeposit;
            _usersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        // User id
                        new Claim(ClaimTypes.NameIdentifier, "userId"),
                        new Claim(CustomClaimTypes.ISBUYER, CustomClaimTypes.ISBUYER)
                    }, "mock"))
                }
            };
            _userServiceMock.Setup(x => x.BuyerAddToDepositAsync(It.IsAny<UpdateDepositModel>()))
                .ReturnsAsync(new UpdateDepositModel { Deposit = deposit + newDeposit, IsConfirmed = true });

            // Act
            var result = await _usersController.AddToDeposit(newDeposit);

            // Assert
            _userServiceMock.Verify(x => x.BuyerAddToDepositAsync(It.IsAny<UpdateDepositModel>()), Times.Once());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult?.Value, Is.InstanceOf<UpdateDepositModel>());

            var updateDepositModel = okObjectResult.Value as UpdateDepositModel;
            Assert.That(updateDepositModel?.Deposit, Is.EqualTo(expectedDeposit));
        }
        [Test]
        public async Task AddToDeposit_InvalidInput_BadRequestResult()
        {
            // Arrange
            int newDeposit = 0;
            int deposit = 50;
            int expectedDeposit = deposit + newDeposit;
            _usersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        // User id
                        new Claim(ClaimTypes.NameIdentifier, "userId"),
                        new Claim(CustomClaimTypes.ISBUYER, CustomClaimTypes.ISBUYER)
                    }, "mock"))
                }
            };
            _userServiceMock.Setup(x => x.BuyerAddToDepositAsync(It.IsAny<UpdateDepositModel>()))
                .ReturnsAsync(new UpdateDepositModel { Message = "Message" });

            // Act
            var result = await _usersController.AddToDeposit(newDeposit);

            // Assert
            _userServiceMock.Verify(x => x.BuyerAddToDepositAsync(It.IsAny<UpdateDepositModel>()), Times.Once());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.That(badRequestObjectResult?.Value, Is.InstanceOf<string>());
        }
        [Test]
        public async Task AddToDeposit_UnauthorizedUser_UnauthorizedResult()
        {
            // Arrange
            _usersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, "userId"),
                        new Claim(CustomClaimTypes.ISSELLER, CustomClaimTypes.ISSELLER)
                    }, "mock"))
                }
            };
            _userServiceMock.Setup(x => x.BuyerAddToDepositAsync(It.IsAny<UpdateDepositModel>()))
               .ReturnsAsync(new UpdateDepositModel());
            // Act
            var result = await _usersController.AddToDeposit(10);

            // Assert
            _userServiceMock.Verify(x => x.BuyerAddToDepositAsync(It.IsAny<UpdateDepositModel>()), Times.Never());
            var unauthorizedObjectResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedObjectResult?.Value, Is.InstanceOf<string>());
        }
        // ResetDeposit
    }
}
