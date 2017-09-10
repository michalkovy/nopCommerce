using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.WebImporter.Domain;
using System.Net;

namespace Nop.Plugin.Misc.WebImporter
{
    public enum SupportedSite : int
    {
        Zacuto,
        Profifoto,
        QualityCages,
        Switronix,
        MxlMics,
        LcdRacks
    }

    public static class Configurations
    {
        private static WebImporterSite CreateZacutoConfiguration()
        {
            var settings = new Nop.Plugin.Misc.WebImporter.Domain.WebImporterSite();
            settings.FullDescriptionExpression = @"//div[@class='product-description product-page-block ']/div[@class='product-page-block-content spacer'] | //div[starts-with(@class,'product-video')]/div[@class='product-page-block-content spacer']";
            settings.ProductNameExpression = "string(//div[@class='subsection pit']/div[@class='second-title-output'])";
            settings.ProductSkuExpression = @"Extensions:replace(string(//div[@class='subsection pit']/div[@class='product-id ']), '^Product ID : (.*)$', '$1')";
            settings.PriceExpression = @"number(Extensions:replace(Extensions:replace(string(//div[@class='subsection pit']/div[@class='product-price ']/div[@class='price']/span[@id='product_price']), '\$', ''), ',', ''))";
            settings.PriceOrigExpression = @"number(Extensions:replace(Extensions:replace(string(//div[@class='subsection pit']/div[@class='product-price ']/div[@class='price2']/span[@id='product_price2']), '\$', ''), ',', ''))";
            settings.PriceCostExpression = @"0.65*number(concat(
  substring(Extensions:replace(string(//div[@itemprop='description'] | //div[starts-with(@class,'product-video')]/div[@class='product-page-block-content spacer']), '^.*List Price: \$(\d+).*$','$1'), 1, number(contains(string(//div[@itemprop='description'] | //div[starts-with(@class,'product-video')]/div[@class='product-page-block-content spacer']), 'List Price: '))      * string-length(Extensions:replace(string(//div[@itemprop='description'] | //div[starts-with(@class,'product-video')]/div[@class='product-page-block-content spacer']), '^.*List Price: \$(\d+).*$','$1'))),
  substring(Extensions:replace(Extensions:replace(string(//div[@class='subsection pit']/div[@class='product-price ']/div[@class='price']/span[@id='product_price']), '\$', ''), ',', ''), 1, number(not(contains(string(//div[@itemprop='description'] | //div[starts-with(@class,'product-video')]/div[@class='product-page-block-content spacer']), 'List Price: '))) * string-length(Extensions:replace(Extensions:replace(string(//div[@class='subsection pit']/div[@class='product-price ']/div[@class='price']/span[@id='product_price']), '\$', ''), ',', '')))
))";
            
            //"0.65*number(Extensions:replace(string(//div[@itemprop='description'] | //div[starts-with(@class,'product-video')]/div[@class='product-page-block-content spacer']), '^.*List Price: \$(\d+).*$','$1') )";
            //if (/dgfh/retr) then Extensions:replace(Extensions:replace(string(//div[@class='gap-right lol']/div[@class='product-price ']/div[@itemprop='price']/span[@id='product_price']), '\$', ''), ',', '') else 
            settings.ShortDescriptionExpression = "Extensions:replace(Extensions:replace(string(/html/head/meta[@name='description']/@content),'Shop.*day!', ''), 'Buy high quality filmmaking accessories by Zacuto. Explore our store catalog or call us today for more details.', '')";
            settings.KeywordsExpression = "string(/html/head/meta[@name='keywords']/@content)";
            settings.FirstImageExpression = "string(/html/head/meta[@property='og:image']/@content)";
            settings.OtherImagesExpression = @"//div[@class='product-image ']/div[@class='gallery']/ul/li/a/img[@alt='']/@src";
            settings.ManufacturerExpression = @"Extensions:replace(string(//div[@class='subsection pit']/comment()[1]), '^.*Manufacturer: ([^<]*).*$', '$1')";
            settings.CategoryUrlExpression = @"//span[@class='product-bread-crumbs ']/a[last()]";
            settings.WeightExpression = "number(Extensions:replace(string(//div[@id='content']/div/script/text()), '^.*var product_weight = ([^;]*).*$', '$1'))";
            settings.EndWeightMultiplication = 0.45359237M; //lbs

            settings.CategoryUrlsExpression = "//div[starts-with(@class, 'panel-catalog-categories')]/div/ul[starts-with(@class, 'tree')]/li/a";
            settings.NextPageExpression = @"string(//a[.='Next &gt;&gt;']/@href)";
            settings.ProductDetailsExpression = "//div[@class='cp-top']/div[@class='catalog-product-title']/a/@href";
            settings.BaseUrl = @"https://store.zacuto.com/";

            settings.EndPriceWithouTaxMultiplication = 26M;
            settings.LanguageId = null;
            settings.UseProxy = true;
            settings.ManufacturerName = "Zacuto";
            settings.WarehouseId = 1;
            settings.VendorId = 0;
            settings.AvailableProductDetailsExpression = null;
            settings.Encoding = "UTF-8";
            settings.StoreIds = new int[] { 10, 11, 12 };
            settings.SkipManufacturerNames = new string[] { "Marshall" };
            return settings;
        }

        private static WebImporterSite CreateProfifotoConfiguration()
        {
            var settings = new Nop.Plugin.Misc.WebImporter.Domain.WebImporterSite();
            settings.FullDescriptionExpression = @"enter please";
            settings.ProductNameExpression = "Extensions:replace(Extensions:replace(string(/html/body/div[@id='body']/div[@id='content']/div[@id='productDetails']/h2), '^(.*)(Kód( +zboží)?|Objednací +(číslo|kód)).?.? [^ ]+( +[-–] +)?(.*)$', '$1$6'),' +[-–] +$','')";
            settings.ProductSkuExpression = @"Extensions:replace(string(/html/body/div[@id='body']/div[@id='content']/div[@id='productDetails']/h2), '^.*(Kód( +zboží)?|Objednací +(číslo|kód)).?.? ([^ ]+).*$', '$4')";
            settings.PriceExpression = @"number(/html/body/div[@id='body']/div[@id='content']/div[@id='productDetails']/div[@class='price']/span[@id='price'])";
            settings.PriceOrigExpression = null;
            settings.PriceCostExpression = @"number(/html/body/div[@id='body']/div[@id='content']/div[@id='productDetails']/div[@class='price']/span[@id='price']) * 0.85 div 1.21";
            settings.ShortDescriptionExpression = null;
            settings.KeywordsExpression = null;
            settings.CategoryUrlExpression = @"/html/body/div[@id='body']/div[@id='content']/div[@id='productDetails']/h3/a[last()]";
            settings.FirstImageExpression = @"Extensions:replace(string(/html/body/div[@id='body']/div[@id='content']/div[@id='productDetails']/div[@class='photo']/a/img[not(starts-with(@src, 'files/profifotolg'))]/@src), '_m\.(...)$', '.$1')";
            settings.OtherImagesExpression = @"Extensions:replaceMore(/html/body/div[@id='body']/div[@id='content']/div[@id='productDetails']/div[@id='photosList']/div[@class='photo']/a/img/@src, '_m\.(...)$', '.$1')";

            settings.CategoryUrlsExpression = @"/html/body/div[@id='body']/div[@id='column']/dl[@id='menu1']/dt/a";
            settings.NextPageExpression = @"string(/html/body/div[@id='body']/div[@id='content']/div[@id='productsList']/div[@id='pages']/a[starts-with(., 'Dal')]/@href)";
            settings.ProductDetailsExpression = @"/html/body/div[@id='body']/div[@id='content']/div[@id='productsList']/div[@class='entry']/h2/a/@href";
            settings.AvailableProductDetailsExpression = @"/html/body/div[@id='body']/div[@id='content']/div[@id='productsList']/div[@class='entry' and Extensions:replace(string(h4),'^.*[Ss]kladem(?! NE).*$','skladem') = 'skladem']/h2/a/@href";
            settings.WeightExpression = null;
            settings.EndWeightMultiplication = 1M;
            
            settings.BaseUrl = @"http://www.profifoto.cz/index.php";
            settings.ManufacturerExpression = null;

            settings.EndPriceWithouTaxMultiplication = 1.0M / 1.21M;
            settings.LanguageId = 3;
            settings.UseProxy = false;
            settings.ManufacturerName = null;
            settings.WarehouseId = 2;
            settings.VendorId = 1;
            settings.Encoding = "Windows-1250";
            settings.StoreIds = new int[] { 10, 11, 12 };
            return settings;
        }

        private static WebImporterSite CreateQualityCagesConfiguration()
        {
            var settings = new Nop.Plugin.Misc.WebImporter.Domain.WebImporterSite();
            settings.FullDescriptionExpression = @"/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']//span[@class='description_block']";
            settings.ProductNameExpression = "string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']//b[@class='smaller' and starts-with(@style,'color:#0675A7;')])";
            settings.ProductSkuExpression = @"Extensions:replace(string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']//text()[contains(.,'Model: ')][1]), '^.*Model: (.*)$', '$1')";
            settings.PriceExpression = @"number(Extensions:replace(Extensions:replace(string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']//b[@class='bigger'][1]), '\$', ''), ',', ''))";
            //settings.PriceExpression = @"number(Extensions:replace(Extensions:replace(string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']/form/b[@class='bigger'][1]), '\$', ''), ',', ''))";
            settings.PriceOrigExpression = null;
            settings.PriceCostExpression = @"0.8*number(Extensions:replace(Extensions:replace(string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']//b[@class='bigger'][1]), '\$', ''), ',', ''))";
            //settings.PriceCostExpression = @"0.8*number(Extensions:replace(Extensions:replace(string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']/form/b[@class='bigger'][1]), '\$', ''), ',', ''))";
            settings.ShortDescriptionExpression = @"string(/html/head/meta[@name='description']/@content)";
            settings.KeywordsExpression = @"string(/html/head/meta[@name='keywords']/@content)";
            settings.FirstImageExpression = @"string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']//div[@id='productMainImage']/noscript/div[@class='large']/a/img/@src)";
            settings.OtherImagesExpression = @"//div[@id='images']/a[@class='productSecondary']/img/@src";
            settings.ManufacturerExpression = null;
            settings.CategoryUrlExpression = @"/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='navBreadCrumb']/a[last()]";
            settings.WeightExpression = null;
            settings.EndWeightMultiplication = 0.45359237M; //lbs

            settings.CategoryUrlsExpression = @"/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdSidebar']/span[@id='spanMenuText']/a";
            //settings.NextPageExpression = @"string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productGeneral']/div[@class='main_block']/div[@id='productListing']/div[@id='productListingListingBottomLinks']/a[@title=' Next Page ']/@href)";
            settings.NextPageExpression = @"string(/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']//div[@class='main_block']/div[@id='productListing']//a[@title=' Next Page ']/@href)";
            //settings.ProductDetailsExpression = @"/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']/div[@id='productListing']/div[@class='table5']/div[@class='table_row5']/div[@class='right5']/div[@class='sub_content']/b/a/@href";
            settings.ProductDetailsExpression = @"/html/body/div[@id='divBg']/div[@id='divMain']/table[@id='tblMain']/tr/td[@id='tdContent']//div[@id='productListing']/div[@class='table5']/div[@class='table_row5']/div[@class='right5']/div[@class='sub_content']/b/a/@href";
            settings.BaseUrl = @"http://qualitycage.com/";

            settings.EndPriceWithouTaxMultiplication = 60M; //45M;
            settings.LanguageId = 2;
            settings.UseProxy = true;
            settings.ManufacturerName = "Quality Cage Company";
            settings.WarehouseId = 1;
            settings.VendorId = 0;
            settings.AvailableProductDetailsExpression = null;
            settings.Encoding = "Windows-1252";
            settings.StoreIds = new int[] { 1, 2, 3, 4, 5 };
            return settings;
        }

        private static WebImporterSite CreateSwitronixConfiguration()
        {
            var settings = new Nop.Plugin.Misc.WebImporter.Domain.WebImporterSite();
            settings.FullDescriptionExpression = @"/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div[@id='ja-container']/div[@id='ja-container2']/div[@id='ja-mainbody3']/div[@id='ja-contentwrap']/div[@id='ja-content']/div[@id='vmMainPage']/div/div[@id='jwts_tab']/div";

            settings.ProductNameExpression = "string(/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div[@id='ja-container']/div[@id='ja-container2']/div[@id='ja-mainbody3']/div[@id='ja-contentwrap']/div[@id='ja-content']/div[@id='vmMainPage']//h1)";

            settings.ProductSkuExpression = @"Extensions:replace(
concat(
  string(/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div[@id='ja-container']/div[@id='ja-container2']/div[@id='ja-mainbody3']/div[@id='ja-contentwrap']/div[@id='ja-content']/div[@id='vmMainPage']/div/div[@id='jwts_tab']/div/h2[a = 'Sku']/following-sibling::text()[not(normalize-space(.) = '')]),
  substring(string(/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div[@id='ja-container']/div[@id='ja-container2']/div[@id='ja-mainbody3']/div[@id='ja-contentwrap']/div[@id='ja-content']/div[@id='vmMainPage']//h1), 1, number(string(/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div[@id='ja-container']/div[@id='ja-container2']/div[@id='ja-mainbody3']/div[@id='ja-contentwrap']/div[@id='ja-content']/div[@id='vmMainPage']/div/div[@id='jwts_tab']/div/h2[a = 'Sku']/following-sibling::text()[not(normalize-space(.) = '')]) = '') * string-length(string(/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div[@id='ja-container']/div[@id='ja-container2']/div[@id='ja-mainbody3']/div[@id='ja-contentwrap']/div[@id='ja-content']/div[@id='vmMainPage']//h1)))
)
,'Part *#:', ''
)";
            settings.PriceExpression = null;
            settings.PriceOrigExpression = null;
            settings.PriceCostExpression = null;

            //"0.65*number(Extensions:replace(string(//div[@itemprop='description'] | //div[starts-with(@class,'product-video')]/div[@class='product-page-block-content spacer']), '^.*List Price: \$(\d+).*$','$1') )";
            //if (/dgfh/retr) then Extensions:replace(Extensions:replace(string(//div[@class='gap-right lol']/div[@class='product-price ']/div[@itemprop='price']/span[@id='product_price']), '\$', ''), ',', '') else 
            settings.ShortDescriptionExpression = "string(/html/head/meta[@name='description']/@content)";
            settings.KeywordsExpression = "string(/html/head/meta[@name='keywords']/@content)";
            settings.FirstImageExpression = @"Extensions:replace(string(/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div[@id='ja-container']/div[@id='ja-container2']/div[@id='ja-mainbody3']/div[@id='ja-contentwrap']/div[@id='ja-content']/div[@id='vmMainPage']/comment()[1]),'^.*a href=""(.*)"" title.*$', '$1')";
            /*html body#bd.fs3 div#ja-wrapper div#ja-containerwrap-fl div#ja-containerwrap2 div#ja-container div#ja-container2.clearfix div#ja-mainbody3.clearfix div#ja-contentwrap div#ja-content div#vmMainPage div div#main_gallery_wrapper191.main_gallery_wrapper div#main_middle_div191.main_middle_div div#main_thumbs_arrows_wrapper191.main_thumbs_arrows_wrapper div#main_thumb_container191.main_thumb_container div#main_thumb_div191.main_thumb_div div.thumbs_div a.no_link img*/

            settings.OtherImagesExpression = @"Extensions:replaceMore(/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div[@id='ja-container']/div[@id='ja-container2']/div[@id='ja-mainbody3']/div[@id='ja-contentwrap']/div[@id='ja-content']/div[@id='vmMainPage']/div/div/div/div[@id='main_thumbs_arrows_wrapper191']/div/div/div/a/img/@src,'/thumbs/','/lightbox/')";
            settings.ManufacturerExpression = null;
            settings.CategoryUrlExpression = @"/html/body/div[@id='ja-wrapper']/div[@id='ja-pathway']/span/a[last()]";

            settings.WeightExpression = null;
            settings.EndWeightMultiplication = 0.45359237M; //lbs

            settings.CategoryUrlsExpression = "/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div[@id='ja-containerwrap2']/div/div/div[@id='ja-col2']/div/div/div/div/div/a[@class='mainlevel']";
            settings.NextPageExpression = "string(/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div/div/div/div/div/div/div[@id='vmMainPage']/div/ul[@class='pagination']/li/a[@title='Next']/@href)";

            settings.ProductDetailsExpression = "/html/body/div[@id='ja-wrapper']/div[@id='ja-containerwrap-fl']/div/div/div/div/div/div/div[@id='vmMainPage']//h2/a/@href";

            settings.BaseUrl = @"http://www.switronix.com/products?page=shop";

            settings.EndPriceWithouTaxMultiplication = 26M;
            settings.LanguageId = null;
            settings.UseProxy = true;
            settings.ManufacturerName = "Switronix";
            settings.WarehouseId = 1;
            settings.VendorId = 0;
            settings.AvailableProductDetailsExpression = null;
            settings.Encoding = "UTF-8";
            settings.StoreIds = new int[] { 10, 11, 12 };
            return settings;
        }

        private static WebImporterSite CreateMxlMicsConfiguration()
        {
            var settings = new Nop.Plugin.Misc.WebImporter.Domain.WebImporterSite();
            settings.FullDescriptionExpression = @"//*[@id='contentsingle'] | //div[@id='content']";
//@"//div[@id='content']/div[@class='description'] | //dic[@id='contentsingle']/p[@class='divider'] | //div/ul[@class='productbullets'] | //dic[@id='contentsingle']/iframe";
            settings.ProductNameExpression = @"string(/html/head/meta[@name='page-topic']/@content)";
            settings.ProductSkuExpression = @"Extensions:replace(Extensions:replace(string(//h2), ' ', '-'),'®','')";
            settings.PriceExpression = null;
            settings.PriceOrigExpression = null;
            settings.PriceCostExpression = null;

            
            settings.ShortDescriptionExpression = "string(/html/head/meta[@name='description']/@content)";
            settings.KeywordsExpression = "string(/html/head/meta[@name='keywords']/@content)";
            settings.FirstImageExpression = "string(//div[@id='mic']/img/@src | //div[@id='contentsingle']/*[@class='right']/img/@src)";
            settings.OtherImagesExpression = @"//table[@id='gallery']/tr/td//a/@href";
            settings.ManufacturerExpression = null;
            settings.CategoryUrlExpression = @"//div[@id='breadcrumb']/div[@class='crumbs']/a[last()]";
            settings.WeightExpression = null;
            settings.EndWeightMultiplication = 0.45359237M; //lbs

            settings.CategoryUrlsExpression = "//ul[@id='menu']//li[a='PRODUCTS']/ul/li/ul/li/a";
            settings.NextPageExpression = null;
            settings.ProductDetailsExpression = "//div[@id='category']/div[@class='micthumb']/a[@class='micThumbnail']/@href";
            settings.BaseUrl = @"http://www.mxlmics.com/";

            settings.EndPriceWithouTaxMultiplication = 26M;
            settings.LanguageId = null;
            settings.UseProxy = false;
            settings.ManufacturerName = "Marshall Electronics";
            settings.WarehouseId = 1;
            settings.VendorId = 0;
            settings.AvailableProductDetailsExpression = null;
            settings.Encoding = "UTF-8";
            settings.StoreIds = new int[] { 10, 11, 12 };
            settings.SkipManufacturerNames = new string[] {};
            return settings;
        }

        private static WebImporterSite CreateLcdRacksConfiguration()
        {
            var settings = new Nop.Plugin.Misc.WebImporter.Domain.WebImporterSite();
            settings.FullDescriptionExpression = @"//div[@id='content']/div[@class='tabscontainer']";
            //@"//div[@id='content']/div[@class='description'] | //dic[@id='contentsingle']/p[@class='divider'] | //div/ul[@class='productbullets'] | //dic[@id='contentsingle']/iframe";
            settings.ProductNameExpression = @"string(//h2[@id='producttitle'])";
            settings.ProductSkuExpression = @"Extensions:replace(string(//h2[@id='producttitle']), ' ', '-')";
            settings.PriceExpression = null;
            settings.PriceOrigExpression = null;
            settings.PriceCostExpression = null;


            settings.ShortDescriptionExpression = "string(//h3[@id='subtitle'])";
            settings.KeywordsExpression = "string(/html/head/meta[@name='keywords']/@content)";

            settings.FirstImageExpression = "string(//div[@id='content']/div[@id='contentright']/img/@src | //div[@id='gallery']/p/a/img/@src)";
            settings.OtherImagesExpression = @"//div[@id='gallery']/a[@class='image']/@href";
            settings.ManufacturerExpression = null;
            settings.CategoryUrlExpression = @"//div[@id='main']/div[@class='breadcrumbs']/a[last()]";

            settings.WeightExpression = null;
            settings.EndWeightMultiplication = 0.45359237M; //lbs

            settings.CategoryUrlsExpression = "//div[@id='container']/div[@class='megamenu_fixed']//a";
            settings.NextPageExpression = null;
            settings.ProductDetailsExpression = "//div[@id='content']/table[@class='categorytable']/tbody/tr/td/a/@href";
            settings.BaseUrl = @"http://www.lcdracks.com/";

            settings.EndPriceWithouTaxMultiplication = 26M;
            settings.LanguageId = null;
            settings.UseProxy = false;
            settings.ManufacturerName = "Marshall Electronics";
            settings.WarehouseId = 1;
            settings.VendorId = 0;
            settings.AvailableProductDetailsExpression = null;
            settings.Encoding = "UTF-8";
            settings.StoreIds = new int[] { 10, 11, 12 };
            settings.SkipManufacturerNames = new string[] { };
            return settings;
        }

        public static WebImporterSite GetConfiguration(SupportedSite site)
        {
            switch (site)
            {
                case SupportedSite.Zacuto:
                    return CreateZacutoConfiguration();
                case SupportedSite.Profifoto:
                    return CreateProfifotoConfiguration();
                case SupportedSite.QualityCages:
                    return CreateQualityCagesConfiguration();
                case SupportedSite.Switronix:
                    return CreateSwitronixConfiguration();
                case SupportedSite.MxlMics:
                    return CreateMxlMicsConfiguration();
                case SupportedSite.LcdRacks:
                    return CreateLcdRacksConfiguration();
                default:
                    throw new NotImplementedException("this site is not implemented");
            }

        }

        //private static WebProxy _proxyHTTP = new WebProxy("46.20.154.146", 3128);
        public static WebClient GetWebClientProxy()
        {
            var webClient = new MyWebClient();
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            webClient.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            webClient.Headers.Add("Accept-Language", "cs,en-us;q=0.7,en;q=0.3");
            webClient.Headers.Add("Accept-Encoding", "gzip, deflate");
            //webClient.Proxy = _proxyHTTP;
            webClient.Encoding = Encoding.UTF8;
            return webClient;
        }

        public static WebClient GetWebClientClean()
        {
            var webClient = new MyWebClient();
            webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            webClient.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            webClient.Headers.Add("Accept-Language", "cs,en-us;q=0.7,en;q=0.3");
            webClient.Proxy = null;
            webClient.Encoding = Encoding.GetEncoding("Windows-1250");
            return webClient;
        }

        public static WebClient GetClient(WebImporterSite configuration)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            var client = configuration.UseProxy ? Configurations.GetWebClientProxy() : Configurations.GetWebClientClean();
            client.Encoding = Encoding.GetEncoding(configuration.Encoding);
            return client;
        }

        public class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.Timeout = 15 * 1000;
                return request;
            }
        }
    }
}
