namespace RunningCounter.Tests
{
    using RunningCounter.Identity;
    using RunningCounter.Models;
    using Microsoft.AspNet.Identity;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Hosting;

    public class MockFactory
    {
        public static Mock<ApplicationUserManager> GetApplicationUserManagerMock()
        {
            var userStore = new Mock<IUserStore<User>>();

            var firstAppUser = new Mock<User>();
            firstAppUser.Setup(u => u.Id).Returns("1");
            firstAppUser.SetupProperty(u => u.KilometersGoal);

            var secondAppUser = new Mock<User>();
            secondAppUser.Setup(u => u.Id).Returns("2");

            var usersList = new List<User>  {
                    firstAppUser.Object,
                    secondAppUser.Object
                  }.AsQueryable();

            var rolesList = new List<string> { "mockRole", RunningCounter.Constants.UserManagerRole };

            var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            userManager.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns<string>(userId => Task.FromResult<User>(usersList.FirstOrDefault(ul => ul.Id == userId)));
            userManager.Setup(u => u.Users).Returns(usersList);
            userManager.Setup(u => u.GetRolesAsync(It.IsAny<string>())).ReturnsAsync(rolesList);
            var identityResult = new IdentityResult();
            userManager.Setup(u => u.DeleteAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(u => u.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            return userManager;
        }

        public static Mock<ActivitiesDbContext> GetActivitiesDbContextMock(Mock<DbSet<Activity>> mockSet)
        {
            var applicationDbContext = new Mock<ActivitiesDbContext>();
            applicationDbContext.Setup(c => c.Activities).Returns(mockSet.Object);

            return applicationDbContext;
        }

        public static Mock<DbSet<Activity>> GetActivitiesDbSetMock()
        {
            // single user
            var appUser = new User();
            appUser.Id = "1";

            var data = new List<Activity>
                  {
                    new Activity { Id = 1, Title = "Activity 1", Kilometers = 1000, User = appUser, Date = DateTime.Now },
                    new Activity { Id = 2, Title = "Activity 2", Kilometers = 1000, User = appUser, Date = DateTime.Now },
                  }.AsQueryable();

            var mockSet = new Mock<DbSet<Activity>>();

            mockSet.As<IDbAsyncEnumerable<Activity>>()
               .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<Activity>(data.GetEnumerator()));

            mockSet.As<IQueryable<Activity>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Activity>(data.Provider));

            mockSet.As<IQueryable<Activity>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Activity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Activity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.Setup(x => x.AsNoTracking()).Returns(mockSet.Object);

            return mockSet;
        }

        public static ClaimsPrincipal GetUserPrincipal(string userId = "1", bool isAdmin = false)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

            if (isAdmin)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, RunningCounter.Constants.UserManagerRole));
            }

            return new ClaimsPrincipal(identity);
        }

        public static HttpRequestMessage GetHttpRequestMessage()
        {
            return new HttpRequestMessage()
            {
                Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } },
                RequestUri = new System.Uri("http://testuri")
            };
        }
    }
}
