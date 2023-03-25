using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Wiakut.IdentityServer4.Context;

public class AspNetIdentityDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public AspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options) 
        : base(options) { }
    
    
}