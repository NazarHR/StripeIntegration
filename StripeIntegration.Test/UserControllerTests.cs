using Microsoft.AspNetCore.Identity;
using Moq;
using Stripe.Identity;
using StripeItegration.Controllers;
using StripeItegration.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StripeItegration.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.Tax;

namespace StripeIntegration.Test
{
    public  class UserControllerTests
    {
        [Fact]
        public async Task Token_IfUserPasswordIsWrong_ReturnsUnauthorized_Async()
        {
            //Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            var configurationMock = new Mock<IConfiguration>();
            var testUser = new ApplicationUser();
            userManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(testUser);
            userManagerMock
                .Setup(x=>x.CheckPasswordAsync(It.IsAny<ApplicationUser>(),It.IsAny<string>()))
                .ReturnsAsync(false);

            var userController = new UserController(userManagerMock.Object, configurationMock.Object);
            var testLoginModel = new LoginModel();
            //Act
            var result = await userController.Token(testLoginModel);

            //Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Token_WithRightCredentials_ReturnsOkAndToken_Async()
        {
            
            //Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(x => x["JWT:Secret"]).Returns("Valid_Secret_Key_For_Testing_1111");
            configurationMock.SetupGet(x => x["JWT:ValidIssuer"]).Returns("ValidIssuer");
            configurationMock.SetupGet(x => x["JWT:ValidAudience"]).Returns("ValidAudience");

            var testUser = new ApplicationUser()
            {
                Id = "ValidUserId",
                UserName = "ValidUsername",

            };
            userManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(testUser);
            userManagerMock
                .Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            userManagerMock
                .Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());

            var userController = new UserController(userManagerMock.Object, configurationMock.Object);
            var testLoginModel = new LoginModel()
            {
                Username = "ValidUsername",
                Password = "ValidPassword"
            };
            //Act
            var result = await userController.Token(testLoginModel);
            
            //Assert
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task Register_RegisteringExistingUSer_ReturnsBadRequest_Async()
        {

            //Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var testUser = new ApplicationUser()
            {
                Id = "ExistingUserId",
                UserName = "ExistingUsername",

            };

            userManagerMock
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(testUser);

            var userController = new UserController(userManagerMock.Object, null);
            var testRegisterModel = new RegisterModel()
            {
                Username = "ValidUsername",
                Password = "ValidPassword",
                Email = "ValidEmail@mail.com"
            };
            //Act
            var result = await userController.Register(testRegisterModel);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_NewUsersuccessfulRegistration_ReturnsBadRequest_Async()
        {
            //Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var testUser = new ApplicationUser()
            {
                Id = "ExistingUserId",
                UserName = "ExistingUsername"

            };
            ApplicationUser returnedUser = null;
            userManagerMock
                .SetupGet(x => x.FindByNameAsync(It.IsAny<string>()).Result)
                .Returns(returnedUser);

            userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            var userController = new UserController(userManagerMock.Object, null);
            var testRegisterModel = new RegisterModel()
            {
                Username = "ValidUsername",
                Password = "ValidPassword",
                Email = "ValidEmail@mail.com"
            };
            //Act
            var result = await userController.Register(testRegisterModel);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
