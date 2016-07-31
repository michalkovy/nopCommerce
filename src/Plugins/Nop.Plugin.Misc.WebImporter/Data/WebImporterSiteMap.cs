using System.Data.Entity.ModelConfiguration;
using Nop.Plugin.Misc.WebImporter.Domain;

namespace Nop.Plugin.Misc.WebImporter.Data
{
    public class WebImporterSiteMap : EntityTypeConfiguration<WebImporterSite>
    {
        public WebImporterSiteMap()
        {
            ToTable("WebImporterSite");

            //Map the primary key
            HasKey(m => m.Id);
            //Map the additional properties
            Property(m => m.BaseUrl);
            Property(m => m.CategoryUrlsExpression);
            Property(m => m.NextPageExpression);
            Property(m => m.ProductDetailsExpression);
            Property(m => m.ProductSkuExpression);
            Property(m => m.ProductNameExpression);
            Property(m => m.ShortDescriptionExpression);
            Property(m => m.FullDescriptionExpression);
            Property(m => m.EndPriceWithouTaxMultiplication);
            Property(m => m.FirstImageExpression);
            Property(m => m.KeywordsExpression);
            Property(m => m.LanguageId);
            Property(m => m.OtherImagesExpression);
            Property(m => m.PriceExpression);
            Property(m => m.PriceOrigExpression);
            Property(m => m.PriceCostExpression);
            Property(m => m.UseProxy);
            Property(m => m.ManufacturerName);
            Property(m => m.ManufacturerExpression);
            Property(m => m.CategoryUrlExpression);
            Property(m => m.WeightExpression);
            Property(m => m.EndWeightMultiplication);
            Property(m => m.WarehouseId);
            Property(m => m.VendorId);
            Property(m => m.AvailableProductDetailsExpression);
        }
    }

}
