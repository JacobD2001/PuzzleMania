using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Models;
using System.Drawing.Drawing2D;
using System.Reflection.Emit;

namespace PuzzleMania.Data;

public class PuzzleManiaContext : IdentityDbContext<PuzzleManiaUser>
{
    public PuzzleManiaContext(DbContextOptions<PuzzleManiaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Team> Teams { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<Riddle> Riddles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Game>()
                 .HasOne(g => g.Team)
                 .WithMany()
                 .HasForeignKey(g => g.TeamId);

        builder.Entity<Riddle>()
            .HasOne(r => r.Game)
                    .WithMany()
                    .HasForeignKey(r => r.GameId);
    }
}
