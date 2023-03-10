using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;

namespace MatchIt.Seeders
{
    public class UserDatabaseSeeder : IHostedService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserDatabaseSeeder(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var user = new IdentityUser { UserName = "matchit" };
            await _userManager.CreateAsync(user, "");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
