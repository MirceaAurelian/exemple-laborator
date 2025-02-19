﻿using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator4_PSSC_DanMirceaAurelian.Domain.ShoppingCarts;

namespace Laborator4_PSSC_DanMirceaAurelian.Domain
{
    public static class ShoppingCartsOperations
    {
        public static Task<IShoppingCarts> ValidateShoppingCarts(Func<ProductCode, Option<ProductCode>> checkProductExists, EmptyShoppingCarts shoppingCarts) =>
            shoppingCarts.ShoppingCartList
                      .Select(ValidateShoppingCart(checkProductExists))
                      .Aggregate(CreateEmptyValidatedShoppingCartList().ToAsync(), ReduceValidShoppingCarts)
                      .MatchAsync(
                            Right: validatedShoppingCarts => new ValidatedShoppingCarts(validatedShoppingCarts),
                            LeftAsync: errorMessage => Task.FromResult((IShoppingCarts)new UnvalidatedShoppingCarts(shoppingCarts.ShoppingCartList, errorMessage))
                      );
        private static Func<EmptyShoppingCart, EitherAsync<string, ValidatedShoppingCart>> ValidateShoppingCart(Func<ProductCode, Option<ProductCode>> checkProductExists) =>
        emptyShoppingCart => ValidateShoppingCart(checkProductExists, emptyShoppingCart);

        private static EitherAsync<string, ValidatedShoppingCart> ValidateShoppingCart(Func<ProductCode, Option<ProductCode>> checkProductExists, EmptyShoppingCart emptyShoppingCart) =>
            from address in Address.TryParse(emptyShoppingCart.address)
                                    .ToEitherAsync(() => $"Invalid address ({emptyShoppingCart.productCode}, {emptyShoppingCart.address})")
            from productCode in ProductCode.TryParse(emptyShoppingCart.productCode)
                                    .ToEitherAsync(() => $"Invalid product code ({emptyShoppingCart.productCode})")
            from quantity in Quantity.TryParse(emptyShoppingCart.quantity)
                                    .ToEitherAsync(() => $"Invalid quantity ({emptyShoppingCart.productCode}, {emptyShoppingCart.quantity})")
            from price in Price.TryParse(emptyShoppingCart.price)
                        .ToEitherAsync(() => $"Invalid price ({emptyShoppingCart.productCode}, {emptyShoppingCart.price})")
            select new ValidatedShoppingCart(productCode, quantity, address, price);

        private static Either<string, List<ValidatedShoppingCart>> CreateEmptyValidatedShoppingCartList() =>
            Right(new List<ValidatedShoppingCart>());

        private static EitherAsync<string, List<ValidatedShoppingCart>> ReduceValidShoppingCarts(EitherAsync<string, List<ValidatedShoppingCart>> acc, EitherAsync<string, ValidatedShoppingCart> next) =>
            from list in acc
            from nextShoppingCart in next
            select list.AppendValidShoppingCart(nextShoppingCart);

        private static List<ValidatedShoppingCart> AppendValidShoppingCart(this List<ValidatedShoppingCart> list, ValidatedShoppingCart validShoppingCart)
        {
            list.Add(validShoppingCart);
            return list;
        }

        public static IShoppingCarts CalculateFinalPrices(IShoppingCarts shoppingCarts) => shoppingCarts.Match(
            whenEmptyShoppingCarts: emptyShoppingCart => emptyShoppingCart,
            whenUnvalidatedShoppingCarts: unvalidatedShoppingCart => unvalidatedShoppingCart,
            whenCalculatedShoppingCarts: calculatedShoppingCart => calculatedShoppingCart,
            whenPaidShoppingCarts: paidShoppingCart => paidShoppingCart,
            whenValidatedShoppingCarts: CalculateFinalPrice
        );

        private static IShoppingCarts CalculateFinalPrice(ValidatedShoppingCarts validShoppingCarts) =>
            new CalculatedShoppingCarts(validShoppingCarts.ShoppingCartList
                                                          .Select(CalculateShoppingCartFinalPrice)
                                                          .ToList()
                                                          .AsReadOnly());
        private static CalculatedShoppingCart CalculateShoppingCartFinalPrice(ValidatedShoppingCart validShoppingCart) =>
            new CalculatedShoppingCart(validShoppingCart.productCode,
                                      validShoppingCart.quantity,
                                      validShoppingCart.address,
                                      validShoppingCart.price,
                                      validShoppingCart.price * validShoppingCart.quantity);


        public static IShoppingCarts MergeGrades(IShoppingCarts shoppingCarts, IEnumerable<CalculatedShoppingCart> existingShoppingCarts) => shoppingCarts.Match(
            whenEmptyShoppingCarts: emptyShoppingCarts => emptyShoppingCarts,
            whenUnvalidatedShoppingCarts: unvalidatedShoppingCarts => unvalidatedShoppingCarts,
            whenValidatedShoppingCarts: validatedShoppingCarts => validatedShoppingCarts,
            whenPaidShoppingCarts: paidShoppingCarts => paidShoppingCarts,
            whenCalculatedShoppingCarts: calculatedShoppingCarts => MergeGrades(calculatedShoppingCarts.ShoppingCartList, existingShoppingCarts));

        private static CalculatedShoppingCarts MergeGrades(IEnumerable<CalculatedShoppingCart> newList, IEnumerable<CalculatedShoppingCart> existingList)
        {
            var updatedAndNewShoppingCarts = newList.Select(sp => sp with { OrderId = existingList.FirstOrDefault(s => s.productCode == sp.productCode)?.OrderId ?? 0, IsUpdated = true });
            var oldShoppingCarts = existingList.Where(sp => !newList.Any(s => s.productCode == sp.productCode));
            var allShoppingCarts = updatedAndNewShoppingCarts.Union(oldShoppingCarts)
                                               .ToList()
                                               .AsReadOnly();
            return new CalculatedShoppingCarts(allShoppingCarts);
        }
        public static IShoppingCarts PayShoppingCarts(IShoppingCarts shoppingCarts) => shoppingCarts.Match(
            whenEmptyShoppingCarts: emptyShoppingCart => emptyShoppingCart,
            whenUnvalidatedShoppingCarts: unvalidatedShoppingCart => unvalidatedShoppingCart,
            whenPaidShoppingCarts: paidShoppingCart => paidShoppingCart,
            whenValidatedShoppingCarts: validatedShoppingCart => validatedShoppingCart,
            whenCalculatedShoppingCarts: GenerateExport
        );

        private static IShoppingCarts GenerateExport(CalculatedShoppingCarts calculatedShoppingCart) =>
            new PaidShoppingCarts(calculatedShoppingCart.ShoppingCartList,
                                    calculatedShoppingCart.ShoppingCartList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(),
                                    DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedShoppingCart shoppingCart) =>
            export.AppendLine($"{shoppingCart.productCode.Code}, {shoppingCart.price}, {shoppingCart.quantity}, {shoppingCart.finalPrice}");
    }
}
