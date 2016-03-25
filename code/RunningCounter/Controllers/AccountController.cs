namespace RunningCounter.Controllers
{
    using Identity;
    using Microsoft.AspNet.Identity;
    using Models;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Constants = RunningCounter.Constants;

    [Authorize]
    public class AccountController : IdentityBaseController
    {
        public AccountController() : base()
        {
        }

        public AccountController(ApplicationUserManager userManager) : base(userManager)
        {
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/account/register")]
        public async Task<IHttpActionResult> Register(RegisterUserViewModel userViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            User newUser = new User
            {
                UserName = userViewModel.UserName,
            };

            IdentityResult result = await this.UserManager.CreateAsync(newUser, userViewModel.Password);

            return result.Succeeded ? this.Ok() : this.GetErrorResult(result);
        }

        [Route("api/account/settings")]
        [ResponseType(typeof(UserSettingsViewModel))]
        public async Task<IHttpActionResult> GetSettings()
        {
            var userId = this.User.Identity.GetUserId();
            var currentUser = await this.UserManager.FindByIdAsync(userId);
            var isAdmin = this.User.IsInRole(Constants.UserManagerRole);

            if (currentUser == null)
            {
                return this.NotFound();
            }

            return this.Ok(new UserSettingsViewModel
            {
                KilometersGoal = currentUser.KilometersGoal ?? Constants.DefaultKilometersGoal,
                IsAdmin = isAdmin
            });
        }

        [HttpPost]
        [Route("api/account/settings")]
        public async Task<IHttpActionResult> UpdateSettings(UserSettingsViewModel settings)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUserId = this.User.Identity.GetUserId();

            var currentUser = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());

            if (currentUser == null)
            {
                return this.NotFound();
            }

            currentUser.KilometersGoal = settings.KilometersGoal;

            IdentityResult result = await this.UserManager.UpdateAsync(currentUser);

            return result.Succeeded ? this.StatusCode(HttpStatusCode.NoContent) : this.GetErrorResult(result);
        }
    }
}
