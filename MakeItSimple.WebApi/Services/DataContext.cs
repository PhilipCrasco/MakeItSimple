using JWT;
using MakeItSimple.WebApi.Domain;
using MakeItSimple.WebApi.Domain.Setup;
using MakeItSimple.WebApi.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MakeItSimple.WebApi.Services
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; } 

        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Department> Departments { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserRole>()
                .Property(x => x.Permissions)
                .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                new ValueComparer<ICollection<string>>(
                (c1 ,c2) => c1.SequenceEqual(c2) ,
                c => c.Aggregate(0 , (a , v) => HashCode.Combine(a , v.GetHashCode())),
                c => c.ToList()));



             modelBuilder.Entity<User>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<User>(u => u.AddedBy);

            modelBuilder.Entity<UserRole>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<UserRole>(u => u.AddedBy);

            modelBuilder.Entity<Department>()
           .HasOne(u => u.AddedByUser)
           .WithOne()
           .HasForeignKey<Department>(u => u.AddedBy);



        }



    }
}
