using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web;

using org.newpointe.PrivateChat.Model;

namespace org.newpointe.PrivateChat.Data
{
    public partial class PrivateChatContext : Rock.Data.DbContext
    {
        public DbSet<PrivatePrayerRequest> PrivatePrayerRequests { get; set; }

        public PrivateChatContext()
            :base ("RockContext")
        {
            // intentionally left blank
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // we don't want this context to create a database or look for EF Migrations, do set the Initializer to null
            Database.SetInitializer<PrivateChatContext>(new NullDatabaseInitializer<PrivateChatContext>());

            Rock.Data.ContextHelper.AddConfigurations(modelBuilder);
            modelBuilder.Configurations.AddFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}
