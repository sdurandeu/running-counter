namespace RunningCounter.Models
{
    using Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public class ActivitiesDbContext : IdentityDbContext<User>
    {
        public ActivitiesDbContext() : base("DefaultConnection")
        {
            if (((IObjectContextAdapter)this).ObjectContext != null)
            {
                ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += (sender, e) => DateTimeKindAttribute.Apply(e.Entity);
            }
        }

        public virtual DbSet<Activity> Activities { get; set; } // marking as virtual allows mocking override

        public static ActivitiesDbContext Create()
        {
            return new ActivitiesDbContext();
        }

        public virtual void SetEntityStateModified(Activity activity)
        {
            this.Entry(activity).State = EntityState.Modified;
        }
    }
}