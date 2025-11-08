using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsVerified { get; set; }

    // Optional — flattened address info (or a separate table if you model Address)
}