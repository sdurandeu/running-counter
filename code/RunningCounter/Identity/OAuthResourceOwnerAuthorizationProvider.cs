namespace RunningCounter.Identity
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security.OAuth;
    using Models;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class OAuthResourceOwnerAuthorizationProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            User user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {                
                context.SetError("access_denied", "The resource owner credentials are invalid or resource owner does not exist.");
                context.Rejected();
                return;
            }

            ClaimsIdentity identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ExternalBearer);

            context.Validated(identity);
        }
    }
}