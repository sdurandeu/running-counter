namespace RunningCounter
{
    using Identity;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.OAuth;
    using Models;
    using Owin;
    using System;

    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ActivitiesDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Configure the application for OAuth based flow
            var oauthServerOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/auth/token"),
                Provider = new OAuthResourceOwnerAuthorizationProvider(),                
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AllowInsecureHttp = true
            };

            app.UseOAuthAuthorizationServer(oauthServerOptions);

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}