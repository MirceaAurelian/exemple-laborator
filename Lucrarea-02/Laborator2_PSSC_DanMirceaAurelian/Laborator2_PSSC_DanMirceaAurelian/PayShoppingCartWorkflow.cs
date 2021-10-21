using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator2_PSSC_DanMirceaAurelian.Domain.ShoppingCartsPaidEvent;
using static Laborator2_PSSC_DanMirceaAurelian.Domain.ShoppingCarts;
using static Laborator2_PSSC_DanMirceaAurelian.Domain.ShoppingCartsOperations;

namespace Laborator2_PSSC_DanMirceaAurelian.Domain
{
    public class PayShoppingCartWorkflow
    {
        public IShoppingCartsPaidEvent Execute(PayShoppingCartCommand command, Func<ProductCode, bool> checkProductExists)
        {
            EmptyShoppingCarts emptyShoppingCarts = new EmptyShoppingCarts(command.InputShoppingCarts);
            IShoppingCarts shoppingCarts = ValidateShoppingCarts(checkProductExists, emptyShoppingCarts);
            shoppingCarts = CalculateFinalPrice(shoppingCarts);
            shoppingCarts = PayShoppingCart(shoppingCarts);

            return shoppingCarts.Match(
                    whenEmptyShoppingCarts: emptyShoppingCarts => new ShoppingCartsPaidFailedEvent("Unexpected unvalidated state") as IShoppingCartsPaidEvent,
                    whenUnvalidatedShoppingCarts: unvalidatedShoppingCarts => new ShoppingCartsPaidFailedEvent(unvalidatedShoppingCarts.Reason),
                    whenValidatedShoppingCarts: validatedShoppingCarts => new ShoppingCartsPaidFailedEvent("Unexpected validated state"),
                    whenCalculatedShoppingCarts: calculatedShoppingCarts => new ShoppingCartsPaidFailedEvent("Unexpected calculated state"),
                    whenPaidShoppingCarts: paidShoppingCarts => new ShoppingCartsPaidScucceededEvent(paidShoppingCarts.Csv, paidShoppingCarts.PublishedDate)
                );
        }
    }
}
