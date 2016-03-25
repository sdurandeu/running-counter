namespace RunningCounter.Controllers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Description;

    [Authorize]
    public class ActivitiesController : ApiController
    {
        private ActivitiesDbContext activitiesDbContext;

        private ApplicationUserManager userManager;

        public ActivitiesController()
        {
        }

        public ActivitiesController(ApplicationUserManager applicationUserManager, ActivitiesDbContext activitiesDbContext)
        {
            this.activitiesDbContext = activitiesDbContext;
            this.userManager = applicationUserManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this.userManager ?? this.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                this.userManager = value;
            }
        }

        public ActivitiesDbContext ActivitiesDbContext
        {
            get
            {
                return this.activitiesDbContext ?? this.Request.GetOwinContext().Get<ActivitiesDbContext>();
            }

            private set
            {
                this.activitiesDbContext = value;
            }
        }

        // GET api/activities     
        [ResponseType(typeof(IEnumerable<Activity>))]
        public async Task<IHttpActionResult> Get(DateTime? startDate = null, DateTime? endDate = null, int? startTime = null, int? endTime = null)
        {
            var currentUserId = this.User.Identity.GetUserId();

            var activitiesQuery = await this.ActivitiesDbContext.Activities.Where(m => m.User.Id == currentUserId).ToListAsync();

            IQueryable<Activity> activitiesResults = activitiesQuery.AsQueryable();

            // apply filters
            if (startDate != null)
            {
                activitiesResults = activitiesResults.Where(m => m.Date.Date >= startDate);
            }

            if (endDate != null)
            {
                activitiesResults = activitiesResults.Where(m => m.Date.Date <= endDate);
            }

            if (startTime != null)
            {
                activitiesResults = activitiesResults.Where(m => m.Date.Hour >= startTime);
            }

            if (endTime != null)
            {
                activitiesResults = activitiesResults.Where(m => m.Date.Hour <= endTime);
            }

            return this.Ok(activitiesResults.ToList());
        }

        // POST api/activities
        [ResponseType(typeof(Activity))]
        public async Task<IHttpActionResult> Post(Activity activity)
        {
            await this.UpdateUserAndRevalidateModelState(activity);

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.ActivitiesDbContext.Activities.Add(activity);

            await this.ActivitiesDbContext.SaveChangesAsync();

            return this.Created<Activity>(new Uri(this.Request.RequestUri, activity.Id.ToString()), activity);
        }

        // PUT api/activities/5
        public async Task<IHttpActionResult> Put(int id, Activity activity)
        {
            await this.UpdateUserAndRevalidateModelState(activity);

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var activityToUpdate = await this.ActivitiesDbContext.Activities.AsNoTracking().Where(item => item.Id == id).SingleOrDefaultAsync();

            // if activity does not exists, create it
            if (activityToUpdate == null)
            {
                activity.Id = id;
                this.ActivitiesDbContext.Activities.Add(activity);
                await this.ActivitiesDbContext.SaveChangesAsync();
                return this.Created<Activity>(new Uri(this.Request.RequestUri, activity.Id.ToString()), activity);
            }

            if (activityToUpdate.User.Id != this.User.Identity.GetUserId())
            {
                return this.StatusCode(HttpStatusCode.Forbidden);
            }

            // update activity
            this.ActivitiesDbContext.SetEntityStateModified(activity);

            await this.ActivitiesDbContext.SaveChangesAsync();

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE api/activities/5
        public async Task<IHttpActionResult> Delete(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();

            var activityToDelete = await this.ActivitiesDbContext.Activities.Where(item => item.User.Id == currentUserId && item.Id == id).FirstOrDefaultAsync();

            if (activityToDelete == null)
            {
                return this.NotFound();
            }

            if (activityToDelete.User.Id != currentUserId)
            {
                return this.StatusCode(HttpStatusCode.Forbidden);
            }

            this.ActivitiesDbContext.Activities.Remove(activityToDelete);

            await this.ActivitiesDbContext.SaveChangesAsync();

            return this.Ok();
        }

        private async Task UpdateUserAndRevalidateModelState(Activity activity)
        {
            var userId = this.User.Identity.GetUserId();
            var currentUser = await this.UserManager.FindByIdAsync(userId);
            activity.User = currentUser;
            this.ModelState.Clear();
            this.Validate(activity);
        }
    }
}