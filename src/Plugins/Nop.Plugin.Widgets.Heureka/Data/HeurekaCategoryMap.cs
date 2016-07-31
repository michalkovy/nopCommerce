using System.Data.Entity.ModelConfiguration;
using Nop.Plugin.Widgets.Heureka.Domain;

namespace Nop.Plugin.Widgets.Heureka.Data
{
    public class HeurekaCategoryMap : EntityTypeConfiguration<HeurekaCategory>
    {
        public HeurekaCategoryMap()
        {
            ToTable("HeurekaCategory");

            //Map the primary key
            HasKey(m => m.Id);
            //Map the additional properties
            Property(m => m.CategoryId);
            Property(m => m.HeurekaId);
        }
    }

}
