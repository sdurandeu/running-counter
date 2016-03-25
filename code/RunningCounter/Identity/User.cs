namespace RunningCounter.Identity
{
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        public virtual int? KilometersGoal { get; set; }
    }
}