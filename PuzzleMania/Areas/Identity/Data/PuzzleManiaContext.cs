using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Models;
using System.Drawing.Drawing2D;

namespace PuzzleMania.Data;

public class PuzzleManiaContext : IdentityDbContext<PuzzleManiaUser>
{
    public PuzzleManiaContext(DbContextOptions<PuzzleManiaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
