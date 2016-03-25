namespace RunningCounter.Models
{
    using Identity;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using Constants = RunningCounter.Constants;

    public class ActivitiesDbContextInitializer : DropCreateDatabaseIfModelChanges<ActivitiesDbContext>
    {
        protected override void Seed(ActivitiesDbContext context)
        {
            var userManager = new UserManager<User>(new UserStore<User>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            string name = "admin";
            string password = "123456";

            // create Role Admin if it does not exist
            if (!roleManager.RoleExists(name))
            {
                var roleresult = roleManager.Create(new IdentityRole(Constants.UserManagerRole));
            }

            // create admin user
            var adminUser = new User();
            adminUser.UserName = name;
            var adminresult = userManager.Create(adminUser, password);

            // add User Admin to Role Admin
            if (adminresult.Succeeded)
            {
                var result = userManager.AddToRole(adminUser.Id, Constants.UserManagerRole);
            }

            // create standard user
            var user = new User();
            user.UserName = "sebastian";
            userManager.Create(user, password);

            context.Activities.AddRange(new[]
            {
                new Activity { Title = "Morning run", Date = DateTime.Now, Kilometers = 1000, User = user },
                new Activity { Title = "Big Exercise", Date = DateTime.Now, Kilometers = 600, User = user },
                new Activity { Title = "Exercise at John's", Date = DateTime.Now.AddDays(-1).AddHours(-3), Kilometers = 1500, User = user },
                new Activity { Title = "Running at Home", Date = DateTime.Now.AddDays(-3), Kilometers = 1200, User = user },
                new Activity { Title = "Evening job", Date = DateTime.Now.AddDays(-3), Kilometers = 400, User = user },
                new Activity { Title = "Break run", Date = DateTime.Now.AddHours(-1), Kilometers = 500, User = user },
                new Activity { Title = "Night run", Date = DateTime.Now.AddHours(-1), Kilometers = 420, User = user },
                new Activity { Title = "Super Exercise!", Date = DateTime.Now.AddDays(-4), Kilometers = 3560, User = user }
            });

            base.Seed(context);
        }
    }
}