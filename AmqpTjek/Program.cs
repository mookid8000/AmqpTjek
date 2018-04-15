using System;
using System.Configuration;
using Autofac;
using Rebus.Config;
using Serilog;

namespace AmqpTjek
{
    class Program
    {
        static Program()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Warning()
                .CreateLogger();
        }

        static int Main()
        {
            try
            {
                Start();

                return 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine($@"Bummer dude: {exception}");
                return -1;
            }
        }

        static void Start()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["rabbitmq"]?.ConnectionString
                                   ?? throw new ConfigurationErrorsException("Please add 'rabbitmq' connection string");

            var builder = new ContainerBuilder();

            builder.RegisterRebus(configure => configure
                .Logging(l => l.Serilog(Log.ForContext("queue", "connection-test")))
                .Transport(t => t.UseRabbitMq(connectionString, "connection-test")));

            builder.RegisterHandler<StringHandler>();

            using (var container = builder.Build())
            using (new PeriodicPublisher(container))
            {
                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
