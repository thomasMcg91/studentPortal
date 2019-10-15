using System;
using Microsoft.EntityFrameworkCore;

// required to add console logging
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// import the Models (representing structure of tables in database)
using SMS.Core.Models; 

namespace SMS.Data.Repositories
{
    // The Context is How EntityFramework communicates with the database
    // We define DbSet properties for each table in the database
    public class DatabaseContext :DbContext
    {
        // autentication store
        public DbSet<User> Users { get; set; }

        // create DbSets for various models
        public DbSet<Student> Students { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<StudentModule> StudentModules { get; set; }
                 
        // Configure the context to use Specified database. We are using 
        // Sqlite database as it does not require any additional installations.
        // Could use SqlServer using connection below if installed
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging(true)
                .UseSqlite("Filename=SMS.db");
                //.UseSqlServer(@"Server=(localdb)\mssqllocaldb; Database=SMS; Trusted_Connection=True;ConnectRetryCount=0");
                //.UseLoggerFactory(GetConsoleLoggerFactory()); // enables logging of queries for debugging
        }

        // Convenience method to recreate the database thus ensuring  the new database takes 
        // account of any changes to the Models or DatabaseContext
        public void Initialise()
        {        
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        // Creates a Sql Query console logger that can be added to context for debugging 
        private static ILoggerFactory GetConsoleLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConsole()
                .AddFilter(DbLoggerCategory.Query.Name, LogLevel.Information)
            );
            return serviceCollection.BuildServiceProvider()
                .GetService<ILoggerFactory>();
        }
    }
}
