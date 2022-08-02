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
        private Mock<ITokenService> _mockTokenService;
        private IOptions<IdentitySettingsModel> _identitySettings;
        private Mock<IUserService> _mockUserService;
        private Mock<IMapper> _mockMapper;
        private Mock<IResetPasswordKeyService> _mockResetPasswordService;
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
                Password = _userVM.Password
            };

            _mockTokenService = new Mock<ITokenService>();
            _mockTokenService.Setup(x => x.GenerateToken(_resultUser)).Returns(_token);

            _identitySettings = Options.Create(new IdentitySettingsModel());

            _mockUserService = new Mock<IUserService>();
            _mockUserService.Setup(x => x.CreateUser(_mappedUser)).Returns(Task.FromResult(_resultUser));
            _mockUserService.Setup(x => x.GetByEmailAndPassword(_userVM.Email, _userVM.Password)).Returns(_resultUser);

            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(x => x.Map<User>(_userVM)).Returns(_mappedUser);

            _mockResetPasswordService = new Mock<IResetPasswordKeyService>();

            _accountController = new AccountController(_mockTokenService.Object, _identitySettings, _mockMapper.Object, _mockUserService.Object, _mockResetPasswordService.Object);
        }

        [Test]
        public void Register_ShouldReturnToken()
        {
            // a
            var result = _accountController.Register(_userVM).Result;

            // a
            _mockMapper.Verify(x => x.Map<User>(_userVM), Times.Once);
            _mockUserService.Verify(x => x.CreateUser(_mappedUser), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(_resultUser), Times.Once);
            Assert.That(result, Is.EqualTo(_token));
        }

        [Test]
        public void Register_ShouldReturnEmptyString()
        {
            // a
            _resultUser.Id = 0;

            // a
            var result = _accountController.Register(_userVM).Result;

            // a
            _mockMapper.Verify(x => x.Map<User>(_userVM), Times.Once);
            _mockUserService.Verify(x => x.CreateUser(_mappedUser), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
            Assert.That(result, Is.Empty);
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
        public void GetToken_ShouldReturnEmptyString()
        {
            _mockUserService.Setup(x => x.GetByEmailAndPassword(_userVM.Email, _userVM.Password)).Returns((User?)null);

            var result = _accountController.GetToken(_userVM.Email, _userVM.Password);

            _mockUserService.Verify(x => x.GetByEmailAndPassword(_userVM.Email, _userVM.Password), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ResetPassword_ShouldReturnLink()
        {
            var link = "testLink";
            _mockUserService.Setup(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email)).Returns(_resultUser);
            _mockResetPasswordService.Setup(x => x.CreateResetPasswordLink(_resultUser)).Returns(Task.FromResult(link));

            var result = _accountController.ResetPassword(_mappedUser.Email).Result;

            _mockUserService.Verify(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email), Times.Once);
            _mockResetPasswordService.Verify(x => x.CreateResetPasswordLink(_resultUser), Times.Once);
            Assert.That(result, Is.EqualTo(link));
        }

        [Test]
        public void ResetPassword_ShouldReturnEmptyString()
        {
            _mockUserService.Setup(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email)).Returns((User?)null);

            var result = _accountController.ResetPassword(_mappedUser.Email).Result;

            _mockUserService.Verify(x => x.GetByEmailWithResetPasswordKeys(_mappedUser.Email), Times.Once);
            _mockResetPasswordService.Verify(x => x.CreateResetPasswordLink(_resultUser), Times.Never);
            Assert.That(result, Is.Empty);
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
