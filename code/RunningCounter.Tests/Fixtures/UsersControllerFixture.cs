namespace RunningCounter.Tests
{
    using Controllers;
    using Identity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Results;

    [TestClass]
    public class UsersControllerFixture
    {
        [TestMethod]
        public void ShouldGetAllUsers()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var usersController = new UsersController(applicationUserManager.Object);

            var response = usersController.GetUsers() as OkNegotiatedContentResult<IEnumerable<UserManagerViewModel>>;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.Content.Count(), 2);            
        }

        [TestMethod]
        public void ShouldDeleteUser()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var usersController = new UsersController(applicationUserManager.Object);

            var response = usersController.DeleteUser("1").Result as OkResult;

            Assert.IsNotNull(response);

            applicationUserManager.Verify(m => m.DeleteAsync(It.Is<User>(user => user.Id == "1")), Times.Once);
        }

        [TestMethod]
        public void ShouldReturn404IfUserNotFound()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var usersController = new UsersController(applicationUserManager.Object);

            var response = usersController.DeleteUser("123").Result as NotFoundResult;

            Assert.IsNotNull(response);

            applicationUserManager.Verify(m => m.DeleteAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
