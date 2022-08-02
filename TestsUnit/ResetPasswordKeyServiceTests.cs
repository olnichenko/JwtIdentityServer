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
    public class ResetPasswordKeyServiceTests
    {
        private IOptions<IdentitySettingsModel> _identitySettings;
        private Mock<IResetPasswordKeyRepository<ResetPasswordKey, Guid>> _mockResetPasswordRepository;
        private ResetPasswordKeyService _resetPasswordKeyService;
        private ResetPasswordKey _resetPasswordKey;
        private User _user;

        [SetUp]
        public void Setup()
        {
            _identitySettings = Options.Create(new IdentitySettingsModel());
            _identitySettings.Value.PasswordResetLinkUrl = "testListUrl";
            _identitySettings.Value.PasswordResetLinkExpirationDateHours = 1;
            _mockResetPasswordRepository = new Mock<IResetPasswordKeyRepository<ResetPasswordKey, Guid>>();
            _resetPasswordKeyService = new ResetPasswordKeyService(_identitySettings, _mockResetPasswordRepository.Object);
            _resetPasswordKey = new ResetPasswordKey
            {
                Id = Guid.NewGuid()
            };
            _user = new User
            {
                Id = 1,
                Email = "test@mail.com",
                Password = "testPassword"
            };
        }

        [Test]
        public void CreateResetPasswordLink_ShoulCreateLink()
        {
            _mockResetPasswordRepository.Setup(x => x.Create(It.IsAny<ResetPasswordKey>(), _user)).Returns(Task.FromResult(_resetPasswordKey));

            var result = _resetPasswordKeyService.CreateResetPasswordLink(_user).Result;

            _mockResetPasswordRepository.Verify(x => x.Create(It.IsAny<ResetPasswordKey>(), _user), Times.Once);
            var expectedResult = _identitySettings.Value.PasswordResetLinkUrl + _resetPasswordKey.Id;
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CreateResetPasswordLink_ShoulNotCreateLink()
        {
            _mockResetPasswordRepository.Setup(x => x.Create(It.IsAny<ResetPasswordKey>(), _user)).Returns(Task.FromResult((ResetPasswordKey?)null));

            var result = _resetPasswordKeyService.CreateResetPasswordLink(_user).Result;

            _mockResetPasswordRepository.Verify(x => x.Create(It.IsAny<ResetPasswordKey>(), _user), Times.Once);
            Assert.IsEmpty(result);
        }

        [Test]
        public void SetResetKeyAsUsed_ShoulSetLinkAsUsed()
        {
            _resetPasswordKey.IsUsed = true;
            _mockResetPasswordRepository.Setup(x => x.FindById(_resetPasswordKey.Id)).Returns(Task.FromResult(_resetPasswordKey));
            _mockResetPasswordRepository.Setup(x => x.Update(_resetPasswordKey)).Returns(Task.FromResult(_resetPasswordKey));

            var result = _resetPasswordKeyService.SetResetKeyAsUsed(_resetPasswordKey.Id).Result;

            _mockResetPasswordRepository.Verify(x => x.FindById(_resetPasswordKey.Id), Times.Once);
            _mockResetPasswordRepository.Verify(x => x.Update(_resetPasswordKey), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public void SetResetKeyAsUsed_ShoulNotSetLinkAsUsed()
        {
            _mockResetPasswordRepository.Setup(x => x.FindById(_resetPasswordKey.Id)).Returns(Task.FromResult((ResetPasswordKey?)null));
 
            var result = _resetPasswordKeyService.SetResetKeyAsUsed(_resetPasswordKey.Id).Result;

            _mockResetPasswordRepository.Verify(x => x.FindById(_resetPasswordKey.Id), Times.Once);
            _mockResetPasswordRepository.Verify(x => x.Update(It.IsAny<ResetPasswordKey>()), Times.Never);
            Assert.IsFalse(result);

        }
    }
}
