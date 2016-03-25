namespace RunningCounter.Tests
{
    using Controllers;
    using Identity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Moq;
    using System.Web.Http.Results;

    [TestClass]
    public class AccountControllerFixture
    {
        [TestMethod]
        public void ShouldRegisterNewUser()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var accountController = new AccountController(applicationUserManager.Object);

            var newUserViewModel = new RegisterUserViewModel()
            {
                UserName = "New user",
                Password = "123456"
            };

            var result = accountController.Register(newUserViewModel).Result as OkResult;

            Assert.IsNotNull(result);

            applicationUserManager.Verify(m => m.CreateAsync(It.Is<User>(user => user.UserName == newUserViewModel.UserName), It.Is<string>(s => s == newUserViewModel.Password)), Times.Once);
        }

        [TestMethod]
        public void ShouldGetUserSettings()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var accountController = new AccountController(applicationUserManager.Object);
            accountController.User = MockFactory.GetUserPrincipal();

            var result = accountController.GetSettings().Result as OkNegotiatedContentResult<UserSettingsViewModel>;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Content.IsAdmin, false);
            Assert.AreEqual(result.Content.KilometersGoal, RunningCounter.Constants.DefaultKilometersGoal);
        }

        [TestMethod]
        public void ShouldGetAdminUserSettings()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var accountController = new AccountController(applicationUserManager.Object);
            accountController.User = MockFactory.GetUserPrincipal(isAdmin: true);

            var result = accountController.GetSettings().Result as OkNegotiatedContentResult<UserSettingsViewModel>;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Content.IsAdmin, true);
        }

        [TestMethod]
        public void ShouldUpdateUserSettings()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var accountController = new AccountController(applicationUserManager.Object);
            accountController.User = MockFactory.GetUserPrincipal(isAdmin: true);

            var settingsViewModel = new UserSettingsViewModel()
            {
                KilometersGoal = 100
            };

            var result = accountController.UpdateSettings(settingsViewModel).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.NoContent);

            applicationUserManager.Verify(m => m.UpdateAsync(It.Is<User>(user => user.KilometersGoal == settingsViewModel.KilometersGoal)), Times.Once);
        }
    }
}
