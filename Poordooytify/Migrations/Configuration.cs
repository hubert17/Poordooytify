namespace Poordooytify.Migrations
{
    using Poordooytify.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Poordooytify.Models.PoordooytifyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Poordooytify.Models.PoordooytifyContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //

            context.Genres.AddOrUpdate(
              g => g.Name,
              new Genre { Name = "Pinoy Pop" },
              new Genre { Name = "OPM Novelty" },
              new Genre { Name = "K-pop" },
              new Genre { Name = "Blues" },
              new Genre { Name = "Novelty" },
              new Genre { Name = "Country" },
              new Genre { Name = "Easy listening" },
              new Genre { Name = "House" },
              new Genre { Name = "Electronic" },
              new Genre { Name = "Folk" },
              new Genre { Name = "Hip hop" },
              new Genre { Name = "Jazz" },
              new Genre { Name = "Latin" },
              new Genre { Name = "Asian" },
              new Genre { Name = "Pop" },
              new Genre { Name = "RnB and Soul" },
              new Genre { Name = "Rock" },
              new Genre { Name = "Alternative Rock" },
              new Genre { Name = "OPM Rock" },
              new Genre { Name = "OPM Alternative" }
            );

        }
    }
}
