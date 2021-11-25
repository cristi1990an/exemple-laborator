using System.Collections.Generic;

namespace Laborator5_PSSC.Domain
{   
    public record PayShoppingCartCommand
    {
        public PayShoppingCartCommand(IReadOnlyCollection<EmptyShoppingCart> inputShoppingCarts)
        {
            InputShoppingCarts = inputShoppingCarts;
        }

        public IReadOnlyCollection<EmptyShoppingCart> InputShoppingCarts { get; }
    }
}
