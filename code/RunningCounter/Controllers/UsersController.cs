namespace RunningCounter.Controllers
{
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Constants = RunningCounter.Constants;

    [Authorize(Roles = Constants.UserManagerRole)]
    public class UsersController : IdentityBaseController
    {
        public UsersController() : base()
        {
        }

        public UsersController(ApplicationUserManager userManager) : base(userManager)
        {
        }

        // GET api/users     
        [ResponseType(typeof(IEnumerable<UserManagerViewModel>))]
        public IHttpActionResult GetUsers()
        {
            var users = this.UserManager.Users.ToList().Select(u => new UserManagerViewModel()
            {
                Id = u.Id,
                Name = u.UserName,
                IsAdmin = this.UserManager.GetRolesAsync(u.Id).Result.Contains(Constants.UserManagerRole),
                KilometersGoal = u.KilometersGoal
            });

            return this.Ok(users);
        }

        // PATCH api/users/5
        [HttpPatch]
        public async Task<IHttpActionResult> PatchUser(string id, UpdateUserViewModel updatedUser)
        {
            var user = await this.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return this.NotFound();
            }

            if (string.IsNullOrEmpty(updatedUser.Password))
            {
                return this.BadRequest();
            }

            user.PasswordHash = this.UserManager.PasswordHasher.HashPassword(updatedUser.Password);

            await this.UserManager.UpdateSecurityStampAsync(id);

            var identityResult = await this.UserManager.UpdateAsync(user);

            return identityResult.Succeeded ? this.StatusCode(HttpStatusCode.NoContent) : this.GetErrorResult(identityResult);
        }

        // DELETE api/users/5
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var user = await this.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return this.NotFound();
            }

            var identityResult = await this.UserManager.DeleteAsync(user);

            return identityResult.Succeeded ? this.Ok() : this.GetErrorResult(identityResult);
        }
    }
}
