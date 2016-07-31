using System.Data.Entity.ModelConfiguration;
using Nop.Plugin.Misc.WebImporter.Domain;

namespace Nop.Plugin.Misc.WebImporter.Data
{
    public class WebImporterLinkMap : EntityTypeConfiguration<WebImporterLink>
    {
        public WebImporterLinkMap()
        {
            ToTable("WebImporterLink");

            //Map the primary key
            HasKey(m => m.Id);
            //Map the additional properties
            Property(m => m.Url);
            Property(m => m.CategoryId);
            Property(m => m.ManufacturerId);
        }
    }

}
