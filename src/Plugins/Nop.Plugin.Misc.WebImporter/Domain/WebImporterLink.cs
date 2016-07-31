using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Misc.WebImporter.Domain
{
    public class WebImporterLink : BaseEntity
    {
        public virtual string Url  { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual int ManufacturerId { get; set; }
    }
}
