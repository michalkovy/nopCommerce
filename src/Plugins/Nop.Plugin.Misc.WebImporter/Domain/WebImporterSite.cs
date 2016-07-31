using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Misc.WebImporter.Domain
{
    public class WebImporterSite : BaseEntity
    {
        public virtual string BaseUrl  { get; set; }
        public virtual string CategoryUrlsExpression { get; set; }
        public virtual string NextPageExpression { get; set; }
        public virtual string ProductDetailsExpression { get; set; }
        public virtual string ProductSkuExpression { get; set; }
        public virtual string ProductNameExpression { get; set; }
        public virtual string ShortDescriptionExpression { get; set; }
        public virtual string FullDescriptionExpression { get; set; }
        public virtual string PriceExpression { get; set; }
        public virtual string PriceOrigExpression { get; set; }
        public virtual string PriceCostExpression { get; set; }
        public virtual decimal EndPriceWithouTaxMultiplication { get; set; }
        public virtual string WeightExpression { get; set; }
        public virtual decimal EndWeightMultiplication { get; set; }
        public virtual string CategoryUrlExpression { get; set; }
        public virtual string KeywordsExpression { get; set; }
        public virtual string FirstImageExpression { get; set; }
        public virtual string OtherImagesExpression { get; set; }
        public virtual int? LanguageId { get; set; }
        public virtual bool UseProxy { get; set; }
        public virtual string ManufacturerName { get; set; }
        public virtual string ManufacturerExpression { get; set; }
        public virtual int VendorId { get; set; }
        public virtual int WarehouseId { get; set; }
        public virtual string AvailableProductDetailsExpression { get; set; }
        public virtual string Encoding { get; set; }
        public virtual int[] StoreIds { get; set; }
        public virtual string[] SkipManufacturerNames { get; set; }
    }
}
