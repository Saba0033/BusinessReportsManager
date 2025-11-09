using Microsoft.AspNetCore.Identity;

namespace BusinessReportsManager.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
}