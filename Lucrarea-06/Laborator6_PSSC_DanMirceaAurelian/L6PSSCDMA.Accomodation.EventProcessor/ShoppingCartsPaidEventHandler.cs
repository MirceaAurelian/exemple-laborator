using Laborator6_PSSC_DanMirceaAurelian.Dto;
using Laborator6_PSSC_DanMirceaAurelian.Events;
using Laborator6_PSSC_DanMirceaAurelian.Dto.Events;
using Laborator6_PSSC_DanMirceaAurelian.Events;
using Laborator6_PSSC_DanMirceaAurelian.Events.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator6_PSSC_DanMirceaAurelian.Domain.Models.ShoppingCartsPaidEvent;
using System.IO;

namespace Laborator6_PSSC_DanMirceaAurelian.Accomodation.EventProcessor
{
    internal class ShoppingCartsPaidEventHandler : AbstractEventHandler<ShoppingCartsPublishEvent>
    {
        public override string[] EventTypes => new string[]{typeof(ShoppingCartsPublishEvent).Name};

        protected override Task<EventProcessingResult> OnHandleAsync(ShoppingCartsPublishEvent eventData)
        {
            var orders = eventData.ShoppingCarts.ToLookup(oh => oh.OrderId);
            var bills = orders.Select(oh => 
                                    { var html = "<!DOCTYPE HTML><html><head><meta charset = \"utf-8\"><title>Factura</title></head><body style=\"width: 100%; height: 100%; margin: 0;\"><h3 style=\"width: 100%; text-align: center;\">" + oh.FirstOrDefault().Name + "</h3><table border=\"1\" style=\"width: 100%;\"><tr><th>Cod Produs</th><th>Cantitate</th><th>Pret</th></tr>";
                                        eventData.ShoppingCarts.Where(sp => sp.OrderId == oh.Key).Select(sp => "<tr><td align=\"center\">" + sp.ProductCode + "</td><td align=\"center\">" + sp.Quantity + "</td><td align=\"center\">" + sp.Price + "</td></tr>").ToList().ForEach(s => html += s);
                                        html += "</table><h3 style=\"width: 100%; text-align: center;\">Total: " + oh.FirstOrDefault().FinalPrice + "<h3></body></html>";
                                        return html;
                                    });
            int i = 0;
            bills.ToList().ForEach(s => {
                StreamWriter sr = new StreamWriter("factura" + i + ".html");
                sr.WriteAsync(s);
                sr.Close();
                i++;
            });

            return Task.FromResult(EventProcessingResult.Completed);
        }
    }
}
