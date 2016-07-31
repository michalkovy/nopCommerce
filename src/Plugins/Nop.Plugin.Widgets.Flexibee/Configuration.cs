using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.Flexibee
{
    public static class FlexibeeConfiguration
    {
        public static void ConfigureFlexibeeRequest(WebRequest request, bool isTextPut = true)
        {
            if (isTextPut)
            {
                request.Method = "PUT";
                request.ContentType = "text/xml";
            }
            request.Timeout = -1;
            request.Credentials = new System.Net.NetworkCredential("mic"+
                "hal", "N" + "e"
                +"m" + "am1" +
                "0ho" + "lk");
        }
    }
}
