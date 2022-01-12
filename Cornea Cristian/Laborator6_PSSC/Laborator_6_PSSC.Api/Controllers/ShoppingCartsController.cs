using Laborator_6_PSSC.Domain;
using Laborator_6_PSSC.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using Laborator_6_PSSC.Api.Models;
using Laborator_6_PSSC.Domain.Models;
using Laborator_6_PSSC.Data;

namespace Laborator_6_PSSC.Api.Controllers
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
            shoppingCart.OrderId,
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
            price: shoppingCart._Price)
            {
                OrderId = shoppingCart._OrderId
            };
    }
}
