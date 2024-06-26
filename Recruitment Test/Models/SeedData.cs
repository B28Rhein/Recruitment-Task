using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Recrutiment_Test.Controllers;
using System.Security.Cryptography;

namespace Recrutiment_Test.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RecruitmentDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<RecruitmentDbContext>>()))
            {
                // Look for any movies.
                if (context.AppUsers.Any())
                {
                    return;   // DB has been seeded
                }
                AppUser appUser = new AppUser() { UserName = "admin", Role = 3, Active = true };
                appUser.PasswordHashed = AccountController.hasher.HashPassword(appUser, "admin");
                context.AppUsers.Add(appUser);
                context.SaveChanges();
            }
        }
    }
}
