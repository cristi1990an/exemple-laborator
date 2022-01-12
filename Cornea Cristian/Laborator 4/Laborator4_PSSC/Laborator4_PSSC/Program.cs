using Laborator4_PSSC.Domain;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Laborator4_PSSC.Domain.ShoppingCarts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Laborator4_PSSC.Repositories;

namespace Laborator4_PSSC
{
    class Program
    {
        private static string ConnectionString = "Server=DESKTOP-NEQI476;Database=Laborator5_PSSC;Trusted_Connection=True;MultipleActiveResultSets=true";

        static void Main(string[] args)
        {
            Task.Run(async () => { await Start(args); }).GetAwaiter().GetResult();
        }

        static async Task Start(string[] args)
        {
            using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
            ILogger<PayShoppingCartWorkflow> logger = loggerFactory.CreateLogger<PayShoppingCartWorkflow>();

            var listOfGrades = ReadListOfShoppingCarts().ToArray();
            PayShoppingCartCommand command = new(listOfGrades);
            var dbContextBuilder = new DbContextOptionsBuilder<ShoppingCartsContext>().UseSqlServer(ConnectionString).UseLoggerFactory(loggerFactory);
            ShoppingCartsContext shoppingCartsContext = new ShoppingCartsContext(dbContextBuilder.Options);
            ProductsRepository productsRepository = new(shoppingCartsContext);
            OrderHeadersRepository orderHeadersRepository = new(shoppingCartsContext);
            OrderLinesRepository orderLinesRepository = new(shoppingCartsContext);

            PayShoppingCartWorkflow workflow = new(productsRepository, orderHeadersRepository, orderLinesRepository, logger);
            var result = await workflow.ExecuteAsync(command);

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

            Console.WriteLine("Hello World!");
        }

        private static ILoggerFactory ConfigureLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
                                builder.AddSimpleConsole(options =>
                                {
                                    options.IncludeScopes = true;
                                    options.SingleLine = true;
                                    options.TimestampFormat = "hh:mm:ss ";
                                })
                                .AddProvider(new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()));
        }

        private static List<EmptyShoppingCart> ReadListOfShoppingCarts()
        {
            List<EmptyShoppingCart> listOfShoppingCarts = new();
            do
            {
                var quantity = ReadValue("Cantitatea produsului comandat: ");
                if (string.IsNullOrEmpty(quantity))
                {
                    break;
                }

                var product_code = ReadValue("Codul produsului: ");
                if (string.IsNullOrEmpty(product_code))
                {
                    break;
                }

                var address = ReadValue("Adresa: ");
                if (string.IsNullOrEmpty(address))
                {
                    break;
                }

                var price = ReadValue("Pretul: ");
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
    }
}
