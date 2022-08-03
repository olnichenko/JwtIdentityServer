using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JwtIdentityServer.Controllers;
using Moq;
using JwtIdentityServer.Services;
using Microsoft.Extensions.Options;
using JwtIdentityServer.ConfigurationModels;
using AutoMapper;
using JwtIdentityServer.ViewModels;
using DAL.Models;
using DAL.Repositories;
using JwtIdentityServer.Validators;
using System.Linq.Expressions;

namespace TestsUnit
{
    public class UserServiceTests
    {
        private Mock<IUserRepository<User, long>> _mockUserRepository;
        private Mock<IValidator<User>> _mockUserValidator;
        private UserService _userService;
        private User _user;
        private User _resultUser;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository<User, long>>();
            _mockUserValidator = new Mock<IValidator<User>>();
            _userService = new UserService(_mockUserValidator.Object, _mockUserRepository.Object);

            _user = new User
            {
                Email = "testEmail",
                Password = "testPassword"
            };

            _resultUser = new User
            {
                Id = 1,
                Email = _user.Email,
                Password = _user.Password
            };
        }

        [Test]
        public void CreateUser_ShouldCreateUser()
        {
            _mockUserValidator.Setup(x => x.Validate(_user)).Returns(true);
            _mockUserRepository.Setup(x => x.Create(_user)).Returns(Task.FromResult(_resultUser));

            var result = _userService.CreateUser(_user).Result;

            _mockUserValidator.Verify(x => x.Validate(_user), Times.Once);
            _mockUserRepository.Verify(x => x.Create(_user), Times.Once);
            Assert.IsFalse(result.Password == _user.Password);
            Assert.That(result, Is.EqualTo(_resultUser));
        }

        [Test]
        public void CreateUser_ShouldNotCreate()
        {
            _mockUserValidator.Setup(x => x.Validate(_user)).Returns(false);

            var result = _userService.CreateUser(_user).Result;

            _mockUserValidator.Verify(x => x.Validate(_user), Times.Once);
            _mockUserRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Never);

            Assert.That(result.Id, Is.EqualTo(0));
        }

        [Test]
        public void GetByEmailAndPassword_ShouldFindUser()
        {
            var users = new List<User>();
            users.Add(_resultUser);
            _mockUserRepository.Setup(x => x.Get(It.IsAny<Func<User, bool>>())).Returns(users);

            var result = _userService.GetByEmailAndPassword(_resultUser.Email, _resultUser.Password);

            _mockUserRepository.Verify(x => x.Get(It.IsAny<Func<User, bool>>()), Times.Once);
            Assert.That(result, Is.EqualTo(_resultUser));
        }

        [Test]
        public void GetByEmailAndPassword_ShouldNotFindUser()
        {
            var users = new List<User>();
            _mockUserRepository.Setup(x => x.Get(It.IsAny<Func<User, bool>>())).Returns(users);

            var result = _userService.GetByEmailAndPassword(_resultUser.Email, _resultUser.Password);

            _mockUserRepository.Verify(x => x.Get(It.IsAny<Func<User, bool>>()), Times.Once);
            Assert.IsNull(result);
        }

        [Test]
        public void GetByEmailWithResetPasswordKeys_ShouldFindUser()
        {
            var users = new List<User>();
            users.Add(_resultUser);
            _mockUserRepository.Setup(x => x.GetWithInclude(It.IsAny<Func<User, bool>>(), It.IsAny<Expression<Func<User, object>>[]>())).Returns(users);

            var result = _userService.GetByEmailWithResetPasswordKeys(_resultUser.Email);

            _mockUserRepository.Verify(x => x.GetWithInclude(It.IsAny<Func<User, bool>>(), It.IsAny<Expression<Func<User, object>>[]>()), Times.Once);
            Assert.That(result, Is.EqualTo(_resultUser));
        }

        [Test]
        public void GetByEmailWithResetPasswordKeys_ShouldNotFindUser()
        {
            var users = new List<User>();
            _mockUserRepository.Setup(x => x.GetWithInclude(It.IsAny<Func<User, bool>>(), It.IsAny<Expression<Func<User, object>>[]>())).Returns(users);

            var result = _userService.GetByEmailWithResetPasswordKeys(_resultUser.Email);

            _mockUserRepository.Verify(x => x.GetWithInclude(It.IsAny<Func<User, bool>>(), It.IsAny<Expression<Func<User, object>>[]>()), Times.Once);
            Assert.IsNull(result);
        }

        [Test]
        public void ResetUserPassword_ShouldCallRepository()
        {
            var testResetKey = Guid.NewGuid();
            var testPassword = "testPassword";
            _mockUserRepository.Setup(x => x.ChangePassword(testResetKey, It.IsAny<string>())).Returns(Task.FromResult(true));

            var result = _userService.ResetUserPassword(testResetKey, testPassword).Result;

            _mockUserRepository.Verify(x => x.ChangePassword(testResetKey, It.IsAny<string>()), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public void ResetUserPassword_ShouldCallRepositoryNotChangePassword()
        {
            var testResetKey = Guid.NewGuid();
            var testPassword = "testPassword";
            _mockUserRepository.Setup(x => x.ChangePassword(testResetKey, It.IsAny<string>())).Returns(Task.FromResult(false));

            var result = _userService.ResetUserPassword(testResetKey, testPassword).Result;

            _mockUserRepository.Verify(x => x.ChangePassword(testResetKey, It.IsAny<string>()), Times.Once);
            Assert.IsFalse(result);
        }

        [Test]
        public void ConfirmUserEmail_ShouldUpdateUserData()
        {
            _resultUser.IsEmailConfirmed = false;
            var users = new List<User>();
            users.Add(_resultUser);
            _mockUserRepository.Setup(x => x.Get(It.IsAny<Func<User, bool>>())).Returns(users);
            var confirmKey = Guid.NewGuid();

            var result = _userService.ConfirmUserEmail(confirmKey).Result;

            _mockUserRepository.Verify(x => x.Update(_resultUser), Times.Once);
            Assert.IsTrue(result);
            Assert.IsTrue(_resultUser.IsEmailConfirmed);
        }

        [Test]
        public void ConfirmUserEmail_ShouldNotFoundUser()
        {
            var users = new List<User>();

            _mockUserRepository.Setup(x => x.Get(It.IsAny<Func<User, bool>>())).Returns(users);
            var confirmKey = Guid.NewGuid();

            var result = _userService.ConfirmUserEmail(confirmKey).Result;

            _mockUserRepository.Verify(x => x.Update(_resultUser), Times.Never);
            Assert.False(result);
        }
    }
}
