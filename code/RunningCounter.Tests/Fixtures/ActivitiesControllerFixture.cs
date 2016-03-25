namespace RunningCounter.Tests
{
    using Controllers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Results;

    [TestClass]
    public class ActivitiesControllerFixture
    {
        [TestMethod]
        public void ShouldGetAllActivities()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var dbSetMock = MockFactory.GetActivitiesDbSetMock();
            var applicationDbContext = MockFactory.GetActivitiesDbContextMock(dbSetMock);

            var activitiesController = new ActivitiesController(applicationUserManager.Object, applicationDbContext.Object);
            activitiesController.User = MockFactory.GetUserPrincipal();

            var activities = activitiesController.Get().Result as OkNegotiatedContentResult<List<Activity>>;

            Assert.IsNotNull(activities);
            Assert.AreEqual(activities.Content.Count(), 2);
            Assert.AreEqual(activities.Content.First().Id, 1);
            Assert.AreEqual(activities.Content.First().User.Id, "1");
        }

        [TestMethod]
        public void ShouldPostOneActivity()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var dbSetMock = MockFactory.GetActivitiesDbSetMock();
            var applicationDbContext = MockFactory.GetActivitiesDbContextMock(dbSetMock);

            var activitiesController = new ActivitiesController(applicationUserManager.Object, applicationDbContext.Object);
            activitiesController.User = MockFactory.GetUserPrincipal();
            activitiesController.Request = MockFactory.GetHttpRequestMessage();

            Activity activity = new Activity() { Id = 3, Title = "New Activity", Kilometers = 1000 };

            var createdResult = activitiesController.Post(activity).Result as CreatedNegotiatedContentResult<Activity>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdResult.Content.Id, activity.Id);

            // verify change is applied in DbSet
            dbSetMock.Verify(set => set.Add(It.Is<Activity>(m => m.Id == activity.Id)), Times.Once);
            applicationDbContext.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldDeleteOneActivity()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var dbSetMock = MockFactory.GetActivitiesDbSetMock();
            var applicationDbContext = MockFactory.GetActivitiesDbContextMock(dbSetMock);

            var activitiesController = new ActivitiesController(applicationUserManager.Object, applicationDbContext.Object);
            activitiesController.User = MockFactory.GetUserPrincipal();

            var result = activitiesController.Delete(2).Result as OkResult;

            Assert.IsNotNull(result);

            // verify change is applied in DbSet            
            dbSetMock.Verify(set => set.Remove(It.Is<Activity>(m => m.Id == 2)), Times.Once);
            applicationDbContext.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldReturn404WhenDeletingUnexistingActivity()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var dbSetMock = MockFactory.GetActivitiesDbSetMock();
            var applicationDbContext = MockFactory.GetActivitiesDbContextMock(dbSetMock);

            var activitiesController = new ActivitiesController(applicationUserManager.Object, applicationDbContext.Object);
            activitiesController.User = MockFactory.GetUserPrincipal();

            var result = activitiesController.Delete(1234).Result as NotFoundResult;

            Assert.IsNotNull(result);

            // verify change is not applied in DbSet            
            dbSetMock.Verify(set => set.Remove(It.IsAny<Activity>()), Times.Never);
            applicationDbContext.Verify(m => m.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void ShouldNotUpdateActivityIfUserNotOwner()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var dbSetMock = MockFactory.GetActivitiesDbSetMock();
            var applicationDbContext = MockFactory.GetActivitiesDbContextMock(dbSetMock);

            var activitiesController = new ActivitiesController(applicationUserManager.Object, applicationDbContext.Object);
            activitiesController.User = MockFactory.GetUserPrincipal("2"); // id of current user and activity owner does not match
            activitiesController.Request = MockFactory.GetHttpRequestMessage();

            Activity updatedActivity = new Activity() { Id = 1, Title = "New Activity", Kilometers = 1000 };

            var result = activitiesController.Put(1, updatedActivity).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.Forbidden);

            // verify change is not saved in DbSet                        
            applicationDbContext.Verify(m => m.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void ShouldCreateActivityWhenPutIfEntityDoesNotExists()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var dbSetMock = MockFactory.GetActivitiesDbSetMock();
            var applicationDbContext = MockFactory.GetActivitiesDbContextMock(dbSetMock);

            var activitiesController = new ActivitiesController(applicationUserManager.Object, applicationDbContext.Object);
            activitiesController.User = MockFactory.GetUserPrincipal();
            activitiesController.Request = MockFactory.GetHttpRequestMessage();

            Activity newActivity = new Activity() { Id = 234, Title = "New Activity", Kilometers = 1000 };

            var createdResult = activitiesController.Put(newActivity.Id, newActivity).Result as CreatedNegotiatedContentResult<Activity>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdResult.Content.Id, newActivity.Id);

            // verify change is applied in DbSet
            dbSetMock.Verify(set => set.Add(It.Is<Activity>(m => m.Id == newActivity.Id)), Times.Once);
            applicationDbContext.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public void ShouldUpdateActivity()
        {
            var applicationUserManager = MockFactory.GetApplicationUserManagerMock();
            var dbSetMock = MockFactory.GetActivitiesDbSetMock();
            var applicationDbContext = MockFactory.GetActivitiesDbContextMock(dbSetMock);

            Activity updatedActivity = new Activity() { Id = 1, Title = "Updated Activity title", Kilometers = 1000 };

            applicationDbContext.Setup(m => m.SetEntityStateModified(It.IsAny<Activity>()));

            var activitiesController = new ActivitiesController(applicationUserManager.Object, applicationDbContext.Object);
            activitiesController.User = MockFactory.GetUserPrincipal();
            activitiesController.Request = MockFactory.GetHttpRequestMessage();

            var result = activitiesController.Put(updatedActivity.Id, updatedActivity).Result as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.NoContent);

            // verify change is applied in DbSet
            applicationDbContext.Verify(m => m.SetEntityStateModified(It.Is<Activity>(activity => activity.Title == updatedActivity.Title)), Times.Once);
            applicationDbContext.Verify(m => m.SaveChangesAsync(), Times.Once);
        }
    }
}
