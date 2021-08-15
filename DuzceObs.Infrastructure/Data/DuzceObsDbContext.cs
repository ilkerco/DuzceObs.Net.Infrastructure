using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DuzceObs.Core.Model.Entities;
using DuzceObs.Core.Model.Interfaces;

namespace DuzceObs.Infrastructure.Data
{
    public class DuzceObsDbContext:IdentityDbContext<User>
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext CurrentHttpContext => _httpContextAccessor.HttpContext;
        public DuzceObsDbContext(DbContextOptions<DuzceObsDbContext> options) : base(options) { }
        public DuzceObsDbContext(DbContextOptions<DuzceObsDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Ders> Ders { get; set; }
        public DbSet<DersDegerlendirme> DersDegerlendirme { get; set; }
        public DbSet<Notlar> Notlar { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Ignore<Student>();
            builder.Ignore<Instructor>();
            builder.Entity<User>(entity =>
            {
                entity.HasDiscriminator<string>("UserType")
                .HasValue<Student>("Student")
                .HasValue<Instructor>("Instructor");
                entity.HasIndex(x => x.Tc).IsUnique();
            });
            base.OnModelCreating(builder);
            ConfigureEntities(builder);
        }
        public void ConfigureEntities(ModelBuilder builder)
        {
            builder.Entity<Ders>(ConfigureDers);
            builder.Entity<Instructor>(ConfigureInstructor);
            builder.Entity<Student>(ConfigureStudent);
            builder.Entity<Notlar>(ConfigureNotlar);
            builder.Entity<DersDegerlendirme>(ConfigureDersDegerlendirme);
        }
        public void ConfigureDers(EntityTypeBuilder<Ders> builder)
        {
            builder.HasOne(x => x.Instructor);
            builder.HasIndex(x => x.DersAdi).IsUnique();
            builder.HasIndex(x => x.DersKodu).IsUnique();
            //builder.HasIndex(prop => prop.Title).IsUnique();
        }
        public void ConfigureInstructor(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasMany<Ders>(x => x.Dersler).WithOne(c => c.Instructor);
            builder.HasIndex(x => x.Tc).IsUnique();
            //builder.HasIndex(prop => prop.Title).IsUnique();
        }
        public void ConfigureStudent(EntityTypeBuilder<Student> builder)
        {
            builder.HasMany<Ders>(x => x.Dersler).WithMany(c => c.Students);
            builder.HasIndex(x => x.Tc).IsUnique();
            builder.HasIndex(x => x.OgrNo).IsUnique();
            //builder.HasIndex(prop => prop.Title).IsUnique();
        }
        public void ConfigureNotlar(EntityTypeBuilder<Notlar> builder)
        {
            builder.HasOne(x => x.DersDegerlendirme);
            builder.HasOne(x => x.Student);
            //builder.HasIndex(prop => prop.Title).IsUnique();
        }
        public void ConfigureDersDegerlendirme(EntityTypeBuilder<DersDegerlendirme> builder)
        {
            builder.HasOne(x => x.Ders);
        }
        public Func<DateTime> TimestampProvider { get; set; } = () => DateTime.Now;
        public Func<string> UserProvider = () => GetCurrentUser(_httpContextAccessor.HttpContext);
        public static string GetCurrentUser(HttpContext ctx)
        {
            var claim = ctx?.User?.FindFirst(ClaimTypes.Name);
            if (claim != null) return claim.Value;
            return Environment.UserName;
        }
        public override int SaveChanges()
        {
            TrackChanges();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                TrackChanges();
                var result = await base.SaveChangesAsync(cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        private void TrackChanges()
        {
            try
            {
                foreach (var entry in ChangeTracker.Entries().Where(e =>
                e.State == EntityState.Added || e.State == EntityState.Modified))
                {

                    if (entry.Entity is IBaseEntity audible)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            audible.CreatedAt = TimestampProvider();
                            audible.CreatedBy = UserProvider.Invoke();
                        }
                        else
                        {
                            audible.UpdatedAt = TimestampProvider();
                            audible.UpdatedBy = UserProvider.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
