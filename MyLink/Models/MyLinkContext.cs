using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MyLink.Models
{
    public class MyLinkContext : DbContext
    {
        #region Contructor
        public MyLinkContext() : base("DefaultConnection")
        {
            
        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }


        public DbSet<Department> Departments { get; set; }

        public DbSet<City> Cities { get; set; }
        
        public DbSet<UserRol> UserRols { get; set; }

        public DbSet<User> Users { get; set; }        

        public System.Data.Entity.DbSet<MyLink.Models.Language> Languages { get; set; }

        public System.Data.Entity.DbSet<MyLink.Models.LinkCategory> LinkCategories { get; set; }

        public System.Data.Entity.DbSet<MyLink.Models.Link> Links { get; set; }
    }
}