﻿
namespace Laborator3_PSSC.Domain
{
    public record CalculatedShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice);
}
