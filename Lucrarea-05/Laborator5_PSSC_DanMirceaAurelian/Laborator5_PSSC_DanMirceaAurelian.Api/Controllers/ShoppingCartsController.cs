using Laborator5_PSSC_DanMirceaAurelian.Domain;
using Laborator5_PSSC_DanMirceaAurelian.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using Laborator5_PSSC_DanMirceaAurelian.Api.Models;
using Laborator5_PSSC_DanMirceaAurelian.Domain.Models;
using Laborator5_PSSC_DanMirceaAurelian.Data;

namespace Laborator5_PSSC_DanMirceaAurelian.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingCartsController : ControllerBase
    {
        private ILogger<ShoppingCartsController> logger;

        public ShoppingCartsController(ILogger<ShoppingCartsController> logger)
        {
            this.logger = logger;
        }


        /*[HttpGet("{id}")]
        public async Task PrintFunc()
        {
            var result = from ol in .OrderLines
                         join p in orderLinesRepository.GetShoppingCartsContext().Products on ol.OrderId equals p.ProductId
                         join oh in orderLinesRepository.GetShoppingCartsContext().OrderHeaders on ol.OrderId equals oh.OrderId
                         select new { oh.OrderId, p.Code, ol.Quantity, oh.Address, ol.Price, oh.Total };
            await Task.Delay(1000);
            
        }*/
        [HttpGet]
        public async Task<IActionResult> GetAllShoppingCarts([FromServices] IOrderLinesRepository orderLinesRepository) =>
            await orderLinesRepository.TryGetExistingOrderLines().Match(
               Succ: GetAllShoppingCartsHandleSuccess,
               Fail: GetAllShoppingCartsHandleError
            );

        private ObjectResult GetAllShoppingCartsHandleError(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllShoppingCartsHandleSuccess(List<CalculatedShoppingCart> shoppingCarts) =>
        Ok(shoppingCarts.Select(shoppingCart => new
        {
            ProductCode = shoppingCart.productCode.Code,
            shoppingCart.quantity,
            shoppingCart.address,
            shoppingCart.price,
	        shoppingCart.finalPrice
        }));

        [HttpPost]
        public async Task<IActionResult> PayShoppingCarts([FromServices]PayShoppingCartWorkflow payShoppingCartWorkflow, [FromBody]InputShoppingCart[] shoppingCarts)
        {
            var emptyShoppingCarts = shoppingCarts.Select(MapInputShoppingCartToEmptyShoppingCart)
                                          .ToList()
                                          .AsReadOnly();
            PayShoppingCartCommand command = new(emptyShoppingCarts);
            var result = await payShoppingCartWorkflow.ExecuteAsync(command);
            return result.Match<IActionResult>(
                whenShoppingCartsPaidFailedEvent: failedEvent => StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason),
                whenShoppingCartsPaidScucceededEvent: successEvent => Ok()
            );
        }

        private static EmptyShoppingCart MapInputShoppingCartToEmptyShoppingCart(InputShoppingCart shoppingCart) => new EmptyShoppingCart(
            productCode: shoppingCart._ProductCode,
            quantity: shoppingCart._Quantity,
            address: shoppingCart._Address,
	        price: shoppingCart._Price);
    }
}
