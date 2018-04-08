using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using Nop.Services.Stores;
using Nop.Services.Localization;

namespace Nop.Plugin.Widgets.Flexibee
{
    public static class OrderExporter
    {
        private static string CustomerOfOrderExport(IOrderService orderService, string flexibeeExternalIdPrefix, ICountryService countryService, Order order, Nop.Core.Domain.Customers.Customer customer, ITaxService taxService)
        {
            string result = "";
            var adresar = new Flexibee.Adresar.Import.winstromAdresar
            {
                id = new[] { new Flexibee.Adresar.Import.IdType() { Value = "ext:" + flexibeeExternalIdPrefix + ":C" + customer.Id } },
                nazev = String.IsNullOrEmpty(order.BillingAddress.Company)
                            ? new Flexibee.Adresar.Import.NazevType() { Value = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName }
                            : new Flexibee.Adresar.Import.NazevType() { Value = order.BillingAddress.Company },
                ulice = String.IsNullOrEmpty(order.BillingAddress.Address2)
                            ? new Flexibee.Adresar.Import.UliceType() { Value = order.BillingAddress.Address1 }
                            : new Flexibee.Adresar.Import.UliceType() { Value = order.BillingAddress.Address1 + ", " + order.BillingAddress.Address2 },
                mesto = new Flexibee.Adresar.Import.MestoType() { Value = order.BillingAddress.City },
                psc = new Flexibee.Adresar.Import.PscType() { Value = order.BillingAddress.ZipPostalCode },
                tel = new Flexibee.Adresar.Import.TelType() { Value = order.BillingAddress.PhoneNumber },
                email = new Flexibee.Adresar.Import.EmailType() { Value = customer.Email }
            };
            if (order.BillingAddress.CountryId.HasValue)
            {
                adresar.stat = new Flexibee.Adresar.Import.StatType
                {
                    Value =
                        "code:" +
                        countryService.GetCountryById(order.BillingAddress.CountryId.Value).
                            TwoLetterIsoCode
                };
                if (!String.IsNullOrEmpty(order.VatNumber))
                {
                    adresar.dic = new Flexibee.Adresar.Import.DicType() { Value = order.VatNumber };
                }
            }
            adresar.postovniShodna = new Flexibee.Adresar.Import.PostovniShodnaType()
            {
                Value =
                    order.ShippingAddress == null || (order.ShippingAddress.Address1 == order.BillingAddress.Address1 &&
                    order.ShippingAddress.Address2 == order.BillingAddress.Address2 &&
                    order.ShippingAddress.City == order.BillingAddress.City &&
                    order.ShippingAddress.Company == order.BillingAddress.Company &&
                    order.ShippingAddress.CountryId == order.BillingAddress.CountryId &&
                    order.ShippingAddress.ZipPostalCode == order.BillingAddress.ZipPostalCode) ? "true" : "false"
            };
            if (adresar.postovniShodna.Value == "false")
            {
                adresar.faJmenoFirmy = new Flexibee.Adresar.Import.FaJmenoFirmyType()
                {
                    Value =
                        String.IsNullOrEmpty(order.ShippingAddress.Company) ?
                        order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName :
                        order.ShippingAddress.Company
                };
                adresar.faUlice = new Flexibee.Adresar.Import.FaUliceType()
                {
                    Value =
                        String.IsNullOrEmpty(order.ShippingAddress.Address2) ?
                        order.ShippingAddress.Address1 :
                        order.ShippingAddress.Address1 + ", " + order.ShippingAddress.Address2
                };
                adresar.faMesto = new Flexibee.Adresar.Import.FaMestoType() { Value = order.ShippingAddress.City };
                adresar.faPsc = new Flexibee.Adresar.Import.FaPscType() { Value = order.ShippingAddress.ZipPostalCode };
                if (order.ShippingAddress.CountryId.HasValue)
                {
                    adresar.faStat = new Flexibee.Adresar.Import.FaStatType
                    {
                        Value = "code:" +
                           countryService.GetCountryById(
                              order.ShippingAddress.CountryId.Value).TwoLetterIsoCode
                    };
                }
            }
            adresar.typVztahuK = new Flexibee.Adresar.Import.TypVztahuKType { Value = "typVztahu.odberatel" };
            adresar.platceDph = new Flexibee.Adresar.Import.PlatceDphType { Value = taxService.GetVatNumberStatus(order.VatNumber) == VatNumberStatus.Valid ? "true" : "false" };
            var vysledek = new Flexibee.Adresar.Import.winstrom();
            vysledek.adresar = new[] { adresar };

            result += FlexibeeCommunicator.SendToFlexibee(vysledek, "adresar");
            return result;
        }

        private static string OrderExport(string flexibeeExternalIdPrefix, ICountryService countryService, Order order, Nop.Core.Domain.Customers.Customer customer, ITaxService taxService, out List<Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladuObjednavkaprijatapolozka> polozky)
        {
            string result = "";
            //objednavka
            var objednavka = new Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijata
            {
                id = new[] { new Flexibee.ObjednavkaPrijata.Import.IdType() { Value = "ext:" + flexibeeExternalIdPrefix + ":O" + order.Id } },
                typDokl = new Flexibee.ObjednavkaPrijata.Import.TypDoklType { Value = "code:OBPS" },
                cisDosle = new Flexibee.ObjednavkaPrijata.Import.CisDosleType() { Value = order.Id.ToString() },
                mena = new Flexibee.ObjednavkaPrijata.Import.MenaType { Value = "code:CZK" },
                firma = new Flexibee.ObjednavkaPrijata.Import.FirmaType { Value = "ext:" + flexibeeExternalIdPrefix + ":C" + customer.Id },
                nazFirmy = new Flexibee.ObjednavkaPrijata.Import.NazFirmyType
                {
                    Value = String.IsNullOrEmpty(order.BillingAddress.Company)
                        ? order.BillingAddress.FirstName + " " + order.BillingAddress.LastName
                        : order.BillingAddress.Company
                },
                ulice = new Flexibee.ObjednavkaPrijata.Import.UliceType
                {
                    Value = String.IsNullOrEmpty(order.BillingAddress.Address2)
                            ? order.BillingAddress.Address1
                            : order.BillingAddress.Address1 + ", " + order.BillingAddress.Address2
                },
                mesto = new Flexibee.ObjednavkaPrijata.Import.MestoType() { Value = order.BillingAddress.City },
                psc = new Flexibee.ObjednavkaPrijata.Import.PscType() { Value = order.BillingAddress.ZipPostalCode },
                poznam = String.IsNullOrWhiteSpace(order.CheckoutAttributeDescription) ? new ObjednavkaPrijata.Import.PoznamType { Value = order.CheckoutAttributeDescription} : null
            };

            if (order.BillingAddress.CountryId.HasValue)
            {
                objednavka.stat = new Flexibee.ObjednavkaPrijata.Import.StatType
                {
                    Value = "code:" + countryService.GetCountryById(
                                            order.BillingAddress.CountryId.Value).TwoLetterIsoCode
                };
            }
            objednavka.postovniShodna = new Flexibee.ObjednavkaPrijata.Import.PostovniShodnaType
            {
                Value = (order.PickUpInStore ||
                (order.ShippingAddress.Address1 == order.BillingAddress.Address1 &&
                order.ShippingAddress.Address2 == order.BillingAddress.Address2 &&
                order.ShippingAddress.City == order.BillingAddress.City &&
                order.ShippingAddress.Company == order.BillingAddress.Company &&
                order.ShippingAddress.CountryId == order.BillingAddress.CountryId &&
                order.ShippingAddress.ZipPostalCode == order.BillingAddress.ZipPostalCode)) ? "true" : "false"
            };
            if (objednavka.postovniShodna.Value == "false")
            {
                objednavka.faNazev = new Flexibee.ObjednavkaPrijata.Import.FaNazevType
                {
                    Value = String.IsNullOrEmpty(order.ShippingAddress.Company) ?
                    order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName :
                    order.ShippingAddress.Company
                };
                objednavka.faUlice = new Flexibee.ObjednavkaPrijata.Import.FaUliceType
                {
                    Value = String.IsNullOrEmpty(order.ShippingAddress.Address2) ?
                    order.ShippingAddress.Address1 :
                    order.ShippingAddress.Address1 + ", " + order.ShippingAddress.Address2
                };
                objednavka.faMesto = new Flexibee.ObjednavkaPrijata.Import.FaMestoType { Value = order.ShippingAddress.City };
                objednavka.faPsc = new Flexibee.ObjednavkaPrijata.Import.FaPscType { Value = order.ShippingAddress.ZipPostalCode };
                if (order.ShippingAddress.CountryId.HasValue)
                {
                    objednavka.faStat = new Flexibee.ObjednavkaPrijata.Import.FaStatType
                    {
                        Value =
                            "code:" +
                            countryService.GetCountryById(
                                order.ShippingAddress.CountryId.Value).TwoLetterIsoCode
                    };
                }
            }

            objednavka.datVyst = new Flexibee.ObjednavkaPrijata.Import.DatVystType { Value = order.CreatedOnUtc };
            objednavka.datObj = new Flexibee.ObjednavkaPrijata.Import.DatObjType { Value = System.Xml.XmlConvert.ToString(new DateTimeOffset(order.CreatedOnUtc.Date)) };
            objednavka.formaUhradyCis = new Flexibee.ObjednavkaPrijata.Import.FormaUhradyCisType();
            objednavka.typDoklNabFak = new Flexibee.ObjednavkaPrijata.Import.TypDoklNabFakType { Value = "code:FAKTURA" };
            switch (order.PaymentMethodSystemName)
            {
                case "Payments.CashOnDelivery":
                    objednavka.formaUhradyCis.Value = "code:DOBIRKA";
                    break;
                case "Payments.PayInStore":
                    objednavka.formaUhradyCis.Value = "code:HOTOVE";
                    break;
                case "Payments.PurchaseOrder":
                    objednavka.formaUhradyCis.Value = "code:PREVOD";
                    objednavka.typDoklNabFak.Value = "code:ZÁLOHA";
                    objednavka.zaokrJakDphK = new ObjednavkaPrijata.Import.ZaokrJakDphKType() { Value = "zaokrJak.nahoru" };
                    objednavka.zaokrJakSumK = new ObjednavkaPrijata.Import.ZaokrJakSumKType() { Value = "zaokrJak.matem" };
                    objednavka.zaokrNaDphK = new ObjednavkaPrijata.Import.ZaokrNaDphKType() { Value = "zaokrNa.setiny" };
                    objednavka.zaokrNaSumK = new ObjednavkaPrijata.Import.ZaokrNaSumKType() { Value = "zaokrNa.setiny" };
                    break;
                case "Payments.PayPalStandard":
                    objednavka.formaUhradyCis.Value = "code:PAYPAL";
                    objednavka.zaokrJakDphK = new ObjednavkaPrijata.Import.ZaokrJakDphKType() { Value = "zaokrJak.nahoru" };
                    objednavka.zaokrJakSumK = new ObjednavkaPrijata.Import.ZaokrJakSumKType() { Value = "zaokrJak.matem" };
                    objednavka.zaokrNaDphK = new ObjednavkaPrijata.Import.ZaokrNaDphKType() { Value = "zaokrNa.setiny" };
                    objednavka.zaokrNaSumK = new ObjednavkaPrijata.Import.ZaokrNaSumKType() { Value = "zaokrNa.setiny" };
                    break;
                default:
                    objednavka.formaUhradyCis.Value = "code:NESPECIFIKOVANO";
                    break;
            }
            objednavka.formaDopravy = new ObjednavkaPrijata.Import.FormaDopravyType { };
            if (order.PickUpInStore)
            {
                objednavka.formaDopravy.Value = "code:OSOBNĚ";
            }
            else
            {
                switch (order.ShippingMethod)
                {
                    case "Osobní odběr v Praze":
                        objednavka.formaDopravy.Value = "code:OSOBNĚ";
                        break;
                    case "PPL Firemní balík":
                        objednavka.formaDopravy.Value = "code:PPL-FIRMA";
                        break;
                    case "PPL Soukromá adresa":
                        objednavka.formaDopravy.Value = "code:PPL-SOUKROMA";
                        break;
                    case "PPL Slovensko":
                        objednavka.formaDopravy.Value = "code:PPL-SLOVENSKO";
                        break;
                    default:
                        objednavka.formaDopravy.Value = "code:DHL";
                        objednavka.doprava = new Flexibee.ObjednavkaPrijata.Import.DopravaType { Value = order.ShippingMethod };
                        break;
                }
            }
            polozky = order.OrderItems.Select(variant =>
                new Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladuObjednavkaprijatapolozka
                {
                    id = new[] { new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuIdType { Value = "ext:" + flexibeeExternalIdPrefix + ":PON" + variant.Id.ToString() } },
                    mnozMj = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuMnozMjType { Value = variant.Quantity.ToString() },
                    cenik = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuCenikType { Value = "ext:" + flexibeeExternalIdPrefix + ":P" + variant.ProductId },
                    typPolozkyK = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuTypPolozkyKType { Value = Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuTypPolozkyKTypeInner.typPolozkykatalog },
                    cenaMj = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuCenaMjType { Value = variant.UnitPriceExclTax.ToString(CultureInfo.InvariantCulture) },
                    sklad = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuSkladType { Value = "code:BYT" }
                }).ToList();

            if (!order.PickUpInStore && order.ShippingMethod != "Osobní odběr v Praze")
            {
                var doprava = new Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladuObjednavkaprijatapolozka
                {
                    id = new[] { new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuIdType { Value = "ext:" + flexibeeExternalIdPrefix + ":DOP" + order.Id } },
                    mnozMj = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuMnozMjType { Value = "1" },
                    cenik = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuCenikType
                    {
                        Value
                            =
                            "code:DOPRAVA"
                    },
                    typPolozkyK = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuTypPolozkyKType
                    {
                        Value =
                            Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuTypPolozkyKTypeInner
                            .typPolozkykatalog
                    },
                    cenaMj = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuCenaMjType { Value = order.OrderShippingExclTax.ToString(CultureInfo.InvariantCulture) }
                };
                polozky.Add(doprava);
            }

            if (order.PaymentMethodAdditionalFeeExclTax > 0)
            {
                var doberecne = new Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladuObjednavkaprijatapolozka
                {
                    id = new[] { new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuIdType { Value = "ext:" + flexibeeExternalIdPrefix + ":DOB" + order.Id } },
                    mnozMj = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuMnozMjType { Value = "1" },
                    cenik = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuCenikType
                    {
                        Value
                            =
                            "code:DOBERECNE"
                    },
                    typPolozkyK = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuTypPolozkyKType
                    {
                        Value =
                            Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuTypPolozkyKTypeInner
                            .typPolozkykatalog
                    },
                    cenaMj = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuCenaMjType { Value = order.PaymentMethodAdditionalFeeExclTax.ToString(CultureInfo.InvariantCulture) }
                };
                polozky.Add(doberecne);
            }

            if (order.RedeemedRewardPointsEntry != null && order.RedeemedRewardPointsEntry.UsedAmount > 0)
            {
                var rewardPossibilities = order.OrderItems.Select(item => new { item.Product.TaxCategoryId, item.PriceInclTax }).Union(new[] { new { TaxCategoryId = 7, PriceInclTax = order.OrderShippingInclTax }, new { TaxCategoryId = 7, PriceInclTax = order.PaymentMethodAdditionalFeeInclTax } })
                  .GroupBy(item => item.TaxCategoryId).Select(group =>
                new
                {
                    taxCategoryId = group.Key,
                    priceInclTax = group.Sum(item => item.PriceInclTax)
                }).OrderBy(item => item.taxCategoryId).ToList();

                var pointsToRedeem = -1 * order.RedeemedRewardPointsEntry.Points;
                var amountPerPoint = order.RedeemedRewardPointsEntry.UsedAmount / pointsToRedeem;
                foreach (var rewardPossibility in rewardPossibilities)
                {
                    if (pointsToRedeem > 0 && rewardPossibility.priceInclTax > amountPerPoint)
                    {
                        var possibleToRedeem = (int)Math.Floor(rewardPossibility.priceInclTax / amountPerPoint);
                        var currentRedeem = Math.Min(possibleToRedeem, pointsToRedeem);
                        pointsToRedeem -= currentRedeem;

                        var product = new Product() { };
                        decimal taxRate;
                        var pricePerPointWithoutTax = taxService.GetProductPrice(product, rewardPossibility.taxCategoryId, amountPerPoint, false, order.Customer, true, out taxRate);

                        var slevaZaBody = new Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladuObjednavkaprijatapolozka
                        {
                            id = new[] { new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuIdType { Value = "ext:" + flexibeeExternalIdPrefix + ":BOD" + order.Id + "T" + rewardPossibility.taxCategoryId } },
                            mnozMj = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuMnozMjType { Value = currentRedeem.ToString(CultureInfo.InvariantCulture) },
                            cenik = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuCenikType
                            {
                                Value
                                    =
                                    "code:SLEVABODY"
                            },
                            typPolozkyK = new Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuTypPolozkyKType
                            {
                                Value =
                                    Flexibee.ObjednavkaPrijata.Import.PolozkyObchDokladuTypPolozkyKTypeInner
                                    .typPolozkykatalog
                            },
                            cenaMj = new ObjednavkaPrijata.Import.PolozkyObchDokladuCenaMjType { Value = (-1 * pricePerPointWithoutTax).ToString(CultureInfo.InvariantCulture) },
                            typCenyDphK = new ObjednavkaPrijata.Import.PolozkyObchDokladuTypCenyDphKType { Value = "typCeny.bezDph" },
                            typSzbDphK = new ObjednavkaPrijata.Import.PolozkyObchDokladuTypSzbDphKType
                            {
                                Value = rewardPossibility.taxCategoryId == 7 ? "typSzbDph.dphZakl" :
                                            rewardPossibility.taxCategoryId == 8 ? "typSzbDph.dphSniz" : "typSzbDph.dphOsv"
                            },
                            mj = new ObjednavkaPrijata.Import.PolozkyObchDokladuMjType { Value = "code:B" }
                        };
                        polozky.Add(slevaZaBody);
                    }
                }
            }

            objednavka.polozkyObchDokladu = new ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladu { objednavkaprijatapolozka = polozky.ToArray() };
            var vysledek = new Flexibee.ObjednavkaPrijata.Import.winstrom();
            vysledek.objednavkaprijata = new[] { objednavka };

            result += FlexibeeCommunicator.SendToFlexibee(vysledek, "objednavka-prijata");
            return result;
        }

        private static string ReservationExport(IOrderService orderService, string flexibeeExternalIdPrefix, ICountryService countryService, Order order, Nop.Core.Domain.Customers.Customer customer)
        {
            string result = "";
            //rezervace
            var rezervace = order.OrderItems.Select(variant =>
                new Flexibee.Rezervace.Import.winstromRezervace
                {
                    mnozstvi = new Flexibee.Rezervace.Import.MnozstviType { Value = variant.Quantity },
                    cenik = new Flexibee.Rezervace.Import.CenikType { Value = "ext:" + flexibeeExternalIdPrefix + ":P" + variant.ProductId },
                    firma = new Rezervace.Import.FirmaType { Value = "ext:" + flexibeeExternalIdPrefix + ":C" + customer.Id },
                    sklad = new Flexibee.Rezervace.Import.SkladType { Value = "code:BYT" },
                    poznamka = new Rezervace.Import.PoznamkaType { Value = "Objednávka " + order.Id }
                }).ToList();

            var vysledek = new Flexibee.Rezervace.Import.winstrom();
            vysledek.rezervace = rezervace.ToArray();

            result += FlexibeeCommunicator.SendToFlexibee(vysledek, "rezervace");
            return result;
        }

        private static string InvoiceExport(Order order, List<Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladuObjednavkaprijatapolozka> polozkyObjednavky, string flexibeeExternalIdPrefix)
        {
            string result = "";

            string realizXmlFormat = @"<?xml version=""1.0""?>
<winstrom>
    <objednavka-prijata>
        <id>ext:{0}:O{1}</id>
        <realizaceObj type=""faktura-vydana"">
            <id>ext:{0}:FVO{1}</id>
            <polozkyObchDokladu>
{2}
            </polozkyObchDokladu>
        </realizaceObj>
    </objednavka-prijata>
</winstrom>";
            
            string polozkaXmlFormat = @"                <polozka>
                    <id>{0}</id>
                    <mj>{1}</mj>
                </polozka>";


            var polozkyXml = polozkyObjednavky.Select(polozka => String.Format(polozkaXmlFormat, polozka.id[0].Value, polozka.mnozMj.Value));

            string realizXml = String.Format(realizXmlFormat, flexibeeExternalIdPrefix, order.Id, String.Join(Environment.NewLine, polozkyXml));

            result += FlexibeeCommunicator.SendXmlToFlexibee(realizXml, "objednavka-prijata");
            return result;
        }

        private static string SendEmailWithInvoice(Order order, string flexibeeExternalIdPrefix, IStoreService storeService, ILanguageService languageService)
        {
            string result = "";

            var languages = languageService.GetAllLanguages();
            var czechLanguageId = languages.First(l => l.LanguageCulture == "cs-CZ").Id;

            bool czechCustomer = order.CustomerLanguageId == czechLanguageId;
            
            var store = storeService.GetStoreById(order.StoreId).Name;

            string email = czechCustomer ?
                String.Format(@"Na základě objednávky v obchodě {0} Vám v příloze posíláme zálohový doklad, který obsahuje informace k platbě. Prosím, při platbě uvádějte celý variabilní symbol.

Děkujeme,

Váš obchod {0}", store) :
                String.Format(@"Thank you for your order in store {0}. You can find attached a pro forma invoce that contains payment datails.

Your store {0}", store);
            string subject = czechCustomer ?
                String.Format("{0}: Údaje k platbě", store) :
                String.Format("{0}: Payment details", store);

            result += FlexibeeCommunicator.SendXmlToFlexibee(email, "faktura-vydana", 
                String.Format(@"/ext:{0}:FVO{1}/odeslani-dokladu.xml?to={2}&cc={3}&subject={4}",
                    flexibeeExternalIdPrefix,
                    order.Id,
                    HttpUtility.UrlEncode(order.BillingAddress.Email),
                    HttpUtility.UrlEncode("saskie@cincilka.cz"),
                    HttpUtility.UrlEncode(subject)));
            return result;
        }

        public static string OrdersExport(IOrderService orderService, string flexibeeExternalIdPrefix, ICountryService countryService, ITaxService taxService, bool reexport, DateTime createdFromUtc, DateTime createdToUtc, IStoreService storeService, ILanguageService languageService)
        {
            string result = "";

            var orders = orderService.SearchOrders(createdFromUtc: createdFromUtc, createdToUtc: createdToUtc);
            foreach (var order in orders.Reverse<Order>())
            {
                result += OrderExport(order, orderService, flexibeeExternalIdPrefix, countryService, taxService, reexport, storeService, languageService);
            }
            return result;
        }

        public static string OrderExport(Order order, IOrderService orderService, string flexibeeExternalIdPrefix, ICountryService countryService, ITaxService taxService, bool reexport, IStoreService storeService, ILanguageService languageService)
        {
            string result = "";

            result += CustomerOfOrderExport(orderService, flexibeeExternalIdPrefix, countryService, order, order.Customer, taxService);
            List<Flexibee.ObjednavkaPrijata.Import.winstromObjednavkaprijataPolozkyObchDokladuObjednavkaprijatapolozka> polozkyObjednavky;
            result += OrderExport(flexibeeExternalIdPrefix, countryService, order, order.Customer, taxService, out polozkyObjednavky);
            if (!reexport)
            {
                // reservations are overhead for us now
                //result += ReservationExport(orderService, flexibeeExternalIdPrefix, countryService, order, order.Customer);

                if (order.PaymentMethodSystemName == "Payments.PurchaseOrder")
                {
                    result += InvoiceExport(order, polozkyObjednavky, flexibeeExternalIdPrefix);
                    result += SendEmailWithInvoice(order, flexibeeExternalIdPrefix, storeService, languageService);
                }
            }

            return result;
        }

        #region Order Cancel

        public static string OrderCancel(string flexibeeExternalIdPrefix, Order order)
        {
            string result = "";

            result += OrderStorno(flexibeeExternalIdPrefix, order);

            result += ReservationStorno(flexibeeExternalIdPrefix, order);

            if (order.PaymentMethodSystemName == "Payments.PurchaseOrder")
            {
                result += InvoiceCancel(flexibeeExternalIdPrefix, order);
            }

            return result;
        }

        private static string OrderStorno(string flexibeeExternalIdPrefix, Order order)
        {
            string result = "";

            string orderXmlFormat = @"<?xml version=""1.0""?>
<winstrom>
    <objednavka-prijata action=""storno"">
        <id>ext:{0}:O{1}</id>
    </objednavka-prijata>
</winstrom>";

            string orderXml = String.Format(orderXmlFormat, flexibeeExternalIdPrefix, order.Id);
            result += FlexibeeCommunicator.SendXmlToFlexibee(orderXml, "objednavka-prijata");
            return result;
        }

        public static string ReservationStorno(string flexibeeExternalIdPrefix, Order order, bool writeToFile = true)
        {
            string result = "";

            string xml = FlexibeeCommunicator.SendXmlToFlexibee(null, "rezervace",
                String.Format(@"/(poznamka='Objednávka {0}').xml?detail=custom&limit=0", order.Id),
                writeToFile);

            xml = xml.Replace("<rezervace>", "<rezervace action=\"delete\">");

            result += FlexibeeCommunicator.SendXmlToFlexibee(xml, "rezervace", writeToFile: writeToFile);
            return result;
        }

        private static string InvoiceCancel(string flexibeeExternalIdPrefix, Order order)
        {
            string result = "";

            string invoiceXmlFormat = @"<?xml version=""1.0""?>
<winstrom>
    <faktura-vydana action=""storno"">
        <id>ext:{0}:FVO{1}</id>
    </faktura-vydana>
</winstrom>";

            string invoiceXml = String.Format(invoiceXmlFormat, flexibeeExternalIdPrefix, order.Id);

            result += FlexibeeCommunicator.SendXmlToFlexibee(invoiceXml, "faktura-vydana");
            return result;
        }

        #endregion
    }
}
