
using Nop.Core;

namespace Nop.Plugin.Widgets.Heureka.Domain
{
    public class HeurekaCategory : BaseEntity
    {
        public virtual int CategoryId { get; set; }
        public virtual int HeurekaId { get; set; }
    }
}
