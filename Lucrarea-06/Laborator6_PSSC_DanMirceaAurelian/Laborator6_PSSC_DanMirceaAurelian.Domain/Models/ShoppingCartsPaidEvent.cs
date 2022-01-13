using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator6_PSSC_DanMirceaAurelian.Domain.Models
{
    [AsChoice]
    public static partial class ShoppingCartsPaidEvent
    {
        public interface IShoppingCartsPaidEvent { }

        public record ShoppingCartsPaidScucceededEvent : IShoppingCartsPaidEvent
        {

            public IEnumerable<PaidShoppingCart> shoppingCarts { get; }
            public DateTime PublishedDate { get; }

            internal ShoppingCartsPaidScucceededEvent(IEnumerable<PaidShoppingCart> shoppingCarts, DateTime publishedDate)
            {
                shoppingCarts = shoppingCarts;
                PublishedDate = publishedDate;
            }
        }

        public record ShoppingCartsPaidFailedEvent : IShoppingCartsPaidEvent
        {
            public string Reason { get; }

            internal ShoppingCartsPaidFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
