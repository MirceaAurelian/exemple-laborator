using Laborator6_PSSC_DanMirceaAurelian.Domain.Models;
using static Laborator6_PSSC_DanMirceaAurelian.Domain.Models.ShoppingCartsPaidEvent;
using static Laborator6_PSSC_DanMirceaAurelian.Domain.ShoppingCartsOperations;
using System;
using static Laborator6_PSSC_DanMirceaAurelian.Domain.Models.ShoppingCarts;
using LanguageExt;
using System.Threading.Tasks;
using System.Collections.Generic;
using Laborator6_PSSC_DanMirceaAurelian.Domain.Repositories;
using System.Linq;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;
using Laborator6_PSSC_DanMirceaAurelian.Events;
using Laborator6_PSSC_DanMirceaAurelian.Dto.Models;
using Laborator6_PSSC_DanMirceaAurelian.Dto.Events;

namespace Laborator6_PSSC_DanMirceaAurelian.Domain
{
    public class PayShoppingCartWorkflow
    {
        private readonly IProductsRepository productsRepository;
        private readonly IOrderHeadersRepository orderHeadersRepository;
        private readonly IOrderLinesRepository orderLinesRepository;
        private readonly ILogger<PayShoppingCartWorkflow> logger;
        private readonly IEventSender eventSender;

        public PayShoppingCartWorkflow(IProductsRepository productsRepository, IOrderHeadersRepository orderHeadersRepository, IOrderLinesRepository orderLinesRepository, ILogger<PayShoppingCartWorkflow> logger, IEventSender eventSender)
        {
            this.productsRepository = productsRepository;
            this.orderHeadersRepository = orderHeadersRepository;
            this.orderLinesRepository = orderLinesRepository;
            this.logger = logger;
            this.eventSender = eventSender;
        }
        public async Task<IShoppingCartsPaidEvent> ExecuteAsync(PayShoppingCartCommand command)
        {
            EmptyShoppingCarts emptyShoppingCarts = new EmptyShoppingCarts(command.InputShoppingCarts);
            var result = from products in productsRepository.TryGetExistingProduct(emptyShoppingCarts.ShoppingCartList.Select(sp => sp.productCode))
                                          .ToEither(ex => new UnvalidatedShoppingCarts(emptyShoppingCarts.ShoppingCartList, ex) as IShoppingCarts)
                         from existingOrderHeaders in orderHeadersRepository.TryGetExistingOrderHeaders(emptyShoppingCarts.ShoppingCartList.Select(sp => sp.OrderId))
                                           .ToEither(ex => new UnvalidatedShoppingCarts(emptyShoppingCarts.ShoppingCartList, ex) as IShoppingCarts)
                         from existingOrderLines in orderLinesRepository.TryGetExistingOrderLines()
                                          .ToEither(ex => new UnvalidatedShoppingCarts(emptyShoppingCarts.ShoppingCartList, ex) as IShoppingCarts)
                         let checkProductExists = (Func<ProductCode, Option<ProductCode>>)(product => CheckProductExists(products, product))
                         let checkOrderHeaderExistsOrInsert = (Func<int, Option<int>>)(existingOrderHeader => CheckOrderHeaderExistsOrInsert(existingOrderHeaders, existingOrderHeader))
                         let checkProductStocValid = (Func<ProductCode, Option <ProductCode>>)(product => CheckProductStocValid(existingOrderLines, products, product))

                         from paidShoppingCarts in ExecuteWorkflowAsync(emptyShoppingCarts, existingOrderLines, existingOrderHeaders, checkProductExists, checkOrderHeaderExistsOrInsert, checkProductStocValid).ToAsync()
                         from saveResults in orderLinesRepository.TrySaveOrderLines(paidShoppingCarts)
                                          .ToEither(ex => new UnvalidatedShoppingCarts(emptyShoppingCarts.ShoppingCartList, ex) as IShoppingCarts)
                         let shoppingCarts = paidShoppingCarts.ShoppingCartList.Select(shoppingCart => new PaidShoppingCart(
                                                                                                            shoppingCart.productCode,
                                                                                                            quantity: shoppingCart.quantity,
                                                                                                            address: shoppingCart.address,
                                                                                                            price: shoppingCart.price,
                                                                                                            finalPrice: shoppingCart.finalPrice)
                         {
                             OrderLineId = shoppingCart.OrderLineId,
                             OrderId = shoppingCart.OrderId
                         })
                         let successfulEvent = new ShoppingCartsPaidScucceededEvent(shoppingCarts, paidShoppingCarts.PublishedDate)
                         let eventToPublish = new ShoppingCartsPublishEvent()
                         {
                             ShoppingCarts = shoppingCarts.Select(s => new OrderProductDto()
                             {
                                 Name = "Comanda #"+s.OrderId,
                                 Address = s.address._address,
                                 ProductCode = s.productCode.Code,
                                 Price = s.price.Value,
                                 Quantity = s.quantity.Value,
                                 FinalPrice = s.finalPrice.Value,
                                 OrderLineId = s.OrderLineId,
                                 OrderId = s.OrderId

                             }).ToList()
                         }
                         from paidEventResult in eventSender.SendAsync("shoppingcarts", eventToPublish)
                                                 .ToEither(ex => new UnvalidatedShoppingCarts(emptyShoppingCarts.ShoppingCartList, ex) as IShoppingCarts)
                         select successfulEvent;
            return await result.Match(
                    Left: shoppingCarts => GenerateFailedEvent(shoppingCarts) as IShoppingCartsPaidEvent,
                    Right: paidShoppingCarts => paidShoppingCarts
                );
    }
        private async Task<Either<IShoppingCarts, PaidShoppingCarts>> ExecuteWorkflowAsync(EmptyShoppingCarts emptyShoppingCarts,
                                                                                          IEnumerable<CalculatedShoppingCart> existingOrderLines,
                                                                                          IEnumerable<int> existingOrderHeaders,
                                                                                          Func<ProductCode, Option<ProductCode>> checkProductExists,
                                                                                          Func<int, Option<int>> checkOrderHeaderExistsOrInsert,
                                                                                          Func<ProductCode, Option<ProductCode>> checkProductStocValid)
        {

            IShoppingCarts shoppingCarts = await ValidateShoppingCarts(checkProductExists, checkOrderHeaderExistsOrInsert, checkProductStocValid, emptyShoppingCarts);
            shoppingCarts = CalculateFinalPrices(shoppingCarts);
            shoppingCarts = MergeShoppingCarts(shoppingCarts, existingOrderLines);
            shoppingCarts = PayShoppingCarts(shoppingCarts);

            return shoppingCarts.Match<Either<IShoppingCarts, PaidShoppingCarts>>(
                whenEmptyShoppingCarts: emptyShoppingCarts => Left(emptyShoppingCarts as IShoppingCarts),
                whenCalculatedShoppingCarts: calculatedShoppingCarts => Left(calculatedShoppingCarts as IShoppingCarts),
                whenUnvalidatedShoppingCarts: unvalidatedShoppingCarts => Left(unvalidatedShoppingCarts as IShoppingCarts),
                whenValidatedShoppingCarts: validatedShoppingCarts => Left(validatedShoppingCarts as IShoppingCarts),
                whenPaidShoppingCarts: paidShoppingCarts => Right(paidShoppingCarts)
            );
        }

        private Option<ProductCode> CheckProductExists(IEnumerable<ProductStoc> products, ProductCode productCode)
        {
            if (products.Any(p => p.productCode == productCode))
            {
                return Some(productCode);
            }
            else
            {
                return None;
            }
        }

        private Option<int> CheckOrderHeaderExistsOrInsert(IEnumerable<int> existingOrderHeaders, int existingOrderHeader)
        {
            if (existingOrderHeaders.Any(p => p == existingOrderHeader || p == 0))
            {
                return Some(existingOrderHeader);
            }
            else
            {
                return None;
            }
        }

        private Option<ProductCode> CheckProductStocValid(IEnumerable<CalculatedShoppingCart> orderLines, IEnumerable<ProductStoc> products, ProductCode product)
        {
            if (orderLines.Where(ol => ol.productCode == product).Sum(ol => ol.quantity.Value) <= products.Where(p => p.productCode == product).Select(p => p.quantity.Value).Single())
                return Some(product);
            else
                return None;
        }

        private ShoppingCartsPaidFailedEvent GenerateFailedEvent(IShoppingCarts shoppingCarts) =>
            shoppingCarts.Match<ShoppingCartsPaidFailedEvent>(
                whenEmptyShoppingCarts: emptyShoppingCarts => new($"Invalid state {nameof(EmptyShoppingCarts)}"),
                whenValidatedShoppingCarts: validatedShoppingCarts => new($"Invalid state {nameof(ValidatedShoppingCarts)}"),
                whenUnvalidatedShoppingCarts: unvalidatedShoppingCarts =>
                {
                    logger.LogError(unvalidatedShoppingCarts.Ex, unvalidatedShoppingCarts.Ex.Message);
                    return new(unvalidatedShoppingCarts.Ex.Message);
                },
                whenCalculatedShoppingCarts: calculatedExamGrades => new($"Invalid state {nameof(CalculatedShoppingCarts)}"),
                whenPaidShoppingCarts: publishedExamGrades => new($"Invalid state {nameof(PaidShoppingCarts)}"));
    }

}
