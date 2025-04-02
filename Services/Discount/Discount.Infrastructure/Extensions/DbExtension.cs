using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Infrastructure.Extensions
{
    public static class DbExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var config = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {
                    logger.LogInformation("Discount DB Migration Started");
                    ApplyMigrations(config);
                    logger.LogInformation("Discount DB Migration Completed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occured during DB Migration");
                    throw;
                }
            }
            return host;
        }

        private static void ApplyMigrations(IConfiguration config)
        {
            var retry = 5;
            while (retry > 0)
            {
                try
                {
                    using var connection = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();
                    using var cmd = new NpgsqlCommand()
                    {
                        Connection = connection,
                    };
                    cmd.CommandText = "Drop Table If Exists Coupon";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"Create Table Coupon(Id Serial Primary Key,
                                                ProductName Varchar(500) Not Null,
                                                Description Text,
                                                Amount INT)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "Insert into Coupon (ProductName, Description, Amount) Values ('Adidas Quick Force Indoor Badminton Shoes', 'Shoe Discount', 500)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "Insert into Coupon (ProductName, Description, Amount) Values ('Yonex VCORE Pro 100 A Tennis Racquet (270gm, Strung)', 'Racquet Discount', 700)";
                    cmd.ExecuteNonQuery();

                    break;
                }
                catch (Exception ex)
                {
                    retry--;
                    if (retry == 0)
                    {
                        throw;
                    }
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
