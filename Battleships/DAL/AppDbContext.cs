using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<GameSave> GameSaves
        {
            get;
            set;
        } = null!;
        public DbSet<Setting> Settings
        {
            get;
            set;
        } = null!;
        public DbSet<Boat> Boats
        {
            get;
            set;
        } = null!;

        private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                    .AddFilter("Microsoft", LogLevel.Information);
                    //.AddConsole();
                }
            );

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(_loggerFactory)
                //.UseSqlServer(@"Server = (localdb)\mssqllocaldb; Database = MyDatabase; Trusted_Connection=True;");
                .UseSqlServer(@"Server=barrel.itcollege.ee,1533; User Id=student; Password=Student.Bad.password.0; Database=capetr_BattleshipsDb; MultipleActiveResultSets=true;");
                //.UseSqlite("Data Source=C:/Users/petre/source/repos/DbDemo/app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.
            //    Entity<Student>().HasIndex(i => new {i.FirstName, i.LastName})
            //    .IsUnique();
        }
    }
}
