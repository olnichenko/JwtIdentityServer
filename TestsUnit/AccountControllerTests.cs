using Microsoft.AspNetCore.Mvc;
using JwtIdentityServer.Controllers;
using Moq;
using JwtIdentityServer.Services;
using Microsoft.Extensions.Options;
using JwtIdentityServer.ConfigurationModels;
using AutoMapper;
using JwtIdentityServer.ViewModels;
using DAL.Models;

namespace TestsUnit
{
    public class AccountControllerTests
    {
        private string _token = "TestToken";
        private UserVM _userVM = new UserVM
        {
            Email = "Test@mail.com",
            Password = "TestPassword"
        };
        private User _mappedUser;
        private User _resultUser;
        private ResetPasswordKey _resetPasswordKey;
        private Mock<ITokenService> _mockTokenService;
        private Mock<IUserService> _mockUserService;
        private Mock<IMapper> _mockMapper;
        private Mock<IResetPasswordKeyService> _mockResetPasswordService;
        private Mock<IMailService> _mockMailService;
        private AccountController _accountController;

        [SetUp]
        public void Setup()
        {
            _mappedUser = new User
            {
                Email = _userVM.Email,
                Password = _userVM.Password
            };

            _resultUser = new User
            {
                Id = 1,
                Email = _userVM.Email,
                Password = _userVM.Password,
                IsEmailConfirmed = true
            };
            _resetPasswordKey = new ResetPasswordKey
            {
                ExpirationDate = DateTime.Now.AddDays(1),
                Id = Guid.NewGuid(),
            };

            _mockTokenService = new Mock<ITokenService>();
            _mockTokenService.Setup(x => x.GenerateToken(_resultUser)).Returns(_token);

            _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(x => x.CreateUser(_mappedUser)).Returns(Task.FromResult(_resultUser));
            _mockUserService.Setup(x => x.GetByEmailAndPassword(_userVM.Email, _userVM.Password)).Returns(_resultUser);

            _mockMailService = new Mock<IMailService>();

            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(x => x.Map<User>(_userVM)).Returns(_mappedUser);

            _mockResetPasswordService = new Mock<IResetPasswordKeyService>();

            _accountController = new AccountController(_mockTokenService.Object, _mockMapper.Object, _mockUserService.Object, _mockResetPasswordService.Object, _mockMailService.Object);
        }

        [Test]
        public void Register_ShouldRegisterUser()
        {
            // a
            var result = _accountController.Register(_userVM).Result;

            // a
            _mockMapper.Verify(x => x.Map<User>(_userVM), Times.Once);
            _mockUserService.Verify(x => x.CreateUser(_mappedUser), Times.Once);
            _mockMailService.Verify(x => x.SendEmailConfirmationMessage(_resultUser.Email, It.IsAny<Guid>()), Times.Once);
            Assert.IsFalse(result == "Error");
        }

        [Test]
        public void Register_ShoulNotRegisterUser()
        {
            // a
            _resultUser.Id = 0;

            // a
            var result = _accountController.Register(_userVM).Result;

            // a
            _mockMapper.Verify(x => x.Map<User>(_userVM), Times.Once);
            _mockUserService.Verify(x => x.CreateUser(_mappedUser), Times.Once);
            _mockMailService.Verify(x => x.SendEmailConfirmationMessage(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
            Assert.That(result, Is.EqualTo("Error"));
        }

        [Test]
        public void GetToken_ShouldReturnToken()
        {
            var result = _accountController.GetToken(_userVM.Email, _userVM.Password);

            _mockUserService.Verify(x => x.GetByEmailAndPassword(_userVM.Email, _userVM.Password), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Once);
            Assert.That(result, Is.EqualTo(_token));
        }

        [Test]
        public void GetToken_ShouldNotFoundUser()
        {
            _mockUserService.Setup(x => x.GetByEmailAndPassword(_userVM.Email, _userVM.Password)).Returns((User?)null);

            var result = _accountController.GetToken(_userVM.Email, _userVM.Password);

            _mockUserService.Verify(x => x.GetByEmailAndPassword(_userVM.Email, _userVM.Password), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetToken_ShouldNotSendToken()
        {
            _resultUser.IsEmailConfirmed = false;

            var result = _accountController.GetToken(_userVM.Email, _userVM.Password);

            _mockUserService.Verify(x => x.GetByEmailAndPassword(_userVM.Email, _userVM.Password), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ResetPassword_ShouldSendMail()
        {
            _mockUserService.Setup(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email)).Returns(_resultUser);
            _mockResetPasswordService.Setup(x => x.CreateResetPasswordKey(_resultUser)).Returns(Task.FromResult(_resetPasswordKey));

            var result = _accountController.ResetPassword(_mappedUser.Email).Result;

            _mockUserService.Verify(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email), Times.Once);
            _mockResetPasswordService.Verify(x => x.CreateResetPasswordKey(_resultUser), Times.Once);
            _mockMailService.Verify(x => x.SendResetPasswordMessage(_resultUser.Email, _resetPasswordKey.Id));
            Assert.IsFalse(result == "Error");
        }

        [Test]
        public void ResetPassword_ShouldNotFoundUser()
        {
            _mockUserService.Setup(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email)).Returns((User?)null);

            var result = _accountController.ResetPassword(_mappedUser.Email).Result;

            _mockUserService.Verify(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email), Times.Once);
            _mockResetPasswordService.Verify(x => x.CreateResetPasswordKey(_resultUser), Times.Never);
            _mockMailService.Verify(x => x.SendResetPasswordMessage(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
            Assert.That(result, Is.EqualTo("Error"));
        }

        [Test]
        public void ResetPassword_ShouldNotSendMail()
        {
            _resultUser.IsEmailConfirmed = false;

            var result = _accountController.ResetPassword(_mappedUser.Email).Result;

            _mockUserService.Verify(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email), Times.Once);
            _mockResetPasswordService.Verify(x => x.CreateResetPasswordKey(_resultUser), Times.Never);
            _mockMailService.Verify(x => x.SendResetPasswordMessage(It.IsAny<string>(), It.IsAny<Guid>()), Times.Never);
            Assert.That(result, Is.EqualTo("Error"));
        }

        [Test]
        public void ConfirmEmail_ShouldCallService()
        {
            _accountController.ConfirmEmail(_resetPasswordKey.Id);

            _mockUserService.Verify(x => x.ConfirmUserEmail(_resetPasswordKey.Id), Times.Once);
        }

        [Test]
        public void ResetPassword_ShouldReturnView()
        {
            var testKey = Guid.NewGuid();

            var result = _accountController.ResetPassword(testKey) as ViewResult;

            Assert.That(result?.Model, Is.EqualTo(testKey));
        }

        [Test]
        public void ResetPassword_ShouldResetPassword()
        {
            var testKey = Guid.NewGuid();
            var testPassword = "testPassword";
            _mockUserService.Setup(x => x.ResetUserPassword(testKey, testPassword)).Returns(Task.FromResult(true));
            _mockResetPasswordService.Setup(x => x.SetResetKeyAsUsed(testKey)).Returns(Task.FromResult(true));

            var result = _accountController.ResetPassword(testKey, testPassword).Result;

            _mockUserService.Verify(x => x.ResetUserPassword(testKey, testPassword), Times.Once);
            _mockResetPasswordService.Verify(x => x.SetResetKeyAsUsed(testKey), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public void ResetPassword_ShouldReturnFalse()
        {
            var testKey = Guid.NewGuid();
            var testPassword = "testPassword";
            _mockUserService.Setup(x => x.ResetUserPassword(testKey, testPassword)).Returns(Task.FromResult(false));
            _mockResetPasswordService.Setup(x => x.SetResetKeyAsUsed(testKey)).Returns(Task.FromResult(true));

            var result = _accountController.ResetPassword(testKey, testPassword).Result;

            _mockUserService.Verify(x => x.ResetUserPassword(testKey, testPassword), Times.Once);
            _mockResetPasswordService.Verify(x => x.SetResetKeyAsUsed(testKey), Times.Never);
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateToken_ShouldCallService()
        {
            _mockTokenService.Setup(x => x.ValidateToken(_token)).Returns(true);

            var result = _accountController.ValidateToken(_token);

            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateToken_ShouldReturnNotValid()
        {
            _mockTokenService.Setup(x => x.ValidateToken(_token)).Returns(false);

            var result = _accountController.ValidateToken(_token);

            Assert.IsFalse(result);
        }
    }
}
