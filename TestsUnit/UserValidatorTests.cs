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
    public class UserValidatorTests
    {
        private UserValidator _userValidator;
        private User _user;

        [SetUp]
        public void Setup()
        {
            _userValidator = new UserValidator();
            _user = new User
            {
                Email = "testEmail",
                Password = "testPassword"
            };
        }

        [Test]
        public void Validate_SholdBeValid()
        {
            var result = _userValidator.Validate(_user);

            Assert.IsTrue(result);
        }

        [Test]
        public void Validate_SholdBeNotValidByEmail()
        {
            _user.Email = String.Empty;

            var result = _userValidator.Validate(_user);

            Assert.IsFalse(result);
        }

        [Test]
        public void Validate_SholdBeNotValidByPassword()
        {
            _user.Password = String.Empty;

            var result = _userValidator.Validate(_user);

            Assert.IsFalse(result);
        }
    }
}
