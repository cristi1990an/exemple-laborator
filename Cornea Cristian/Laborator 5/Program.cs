using Laborator5_PSSC.Domain;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Laborator5_PSSC.Domain.ShoppingCarts;

namespace Laborator5_PSSC
{
    class Program
    {
        static void Main()
        {
            Task.Run(async () => { await Start(); }).GetAwaiter().GetResult();
        }

        static async Task Start()
        {
            var listOfGrades = ReadListOfShoppingCarts().ToArray();
            PayShoppingCartCommand command = new(listOfGrades);
            PayShoppingCartWorkflow workflow = new PayShoppingCartWorkflow();
            var result = await workflow.ExecuteAsync(command, CheckProductExists, CheckStock, CheckAddress);

            result.Match(
                    whenShoppingCartsPaidFailedEvent: @event =>
                    {
                        Console.WriteLine($"Pay failed: {@event.Reason}");
                        return @event;
                    },
                    whenShoppingCartsPaidScucceededEvent: @event =>
                    {
                        Console.WriteLine($"Pay succeeded.");
                        Console.WriteLine(@event.Csv);
                        return @event;
                    }
                );
        }

        private static List<EmptyShoppingCart> ReadListOfShoppingCarts()
        {
            List<EmptyShoppingCart> listOfShoppingCarts = new();
            do
            {
                var quantity = ReadValue("Quantity: ");
                if (string.IsNullOrEmpty(quantity))
                {
                    break;
                }

                var product_code = ReadValue("Product code: ");
                if (string.IsNullOrEmpty(product_code))
                {
                    break;
                }

                var address = ReadValue("Address: ");
                if (string.IsNullOrEmpty(address))
                {
                    break;
                }

                var price = ReadValue("Price: ");
                if (string.IsNullOrEmpty(price))
                {
                    break;
                }

                listOfShoppingCarts.Add(new(product_code, quantity, address, price));
            } while (true);
            return listOfShoppingCarts;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        private static TryAsync<bool> CheckProductExists(ProductCode product) => async () => true;
        private static TryAsync<bool> CheckStock(ProductCode product, Quantity quantity) => async () => true;
        private static TryAsync<bool> CheckAddress(Address address) => async () => true;
    }
}
