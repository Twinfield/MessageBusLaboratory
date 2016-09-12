using System;
using System.Configuration;
using System.Linq;

using MassTransit;
using MassTransit.AzureServiceBusTransport;

using NLog;

namespace Laboratory.Consumer
{
	class Program
	{
		const string QueueName = "Laboratory";

		static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static IBusControl readOnlyBus;

		static void Main(string[] args)
		{
			var useAzureServiceBus = args.Any(a => a.ToLower() == "azure");

			ConfigureAndStartBus(useAzureServiceBus);
			WaitForInput();
			StopBus();

			Logger.Info("Finished");
		}

		static void ConfigureAndStartBus(bool useAzureServiceBus)
		{
			Logger.Info("Configuring bus...");
			readOnlyBus = useAzureServiceBus ? ConfigureAzureServiceBus(): ConfigureRabbitMqBus();
			DisplayBusConfiguration(readOnlyBus);

			Logger.Info("Starting bus...");
			readOnlyBus.Start();
			Logger.Info("Bus started.");
		}

		static IBusControl ConfigureAzureServiceBus()
		{
			Logger.Info("Configuring Azure bus...");
			var connectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];

			var bus = Bus.Factory.CreateUsingAzureServiceBus(busConfig =>
			{
				var host = busConfig.Host(connectionString, hostConfig => { });

				busConfig.UseRetry(Retry.Immediate(2));

				busConfig.ReceiveEndpoint(host, QueueName, endpointConfig =>
				{
					endpointConfig.Consumer<ColorEventConsumer>();
				});
			});

			bus.ConnectConsumeObserver(new CustomConsumeObserver());

			return bus;
		}

		static IBusControl ConfigureRabbitMqBus()
		{
			Logger.Info("Configuring RabbitMQ bus...");

			var server = ConfigurationManager.AppSettings["RabbitMqServer"];
			var virtualHost = ConfigurationManager.AppSettings["RabbitMqVirtualHost"];
			var username = ConfigurationManager.AppSettings["RabbitMqUsername"];
			var password = ConfigurationManager.AppSettings["RabbitMqPassword"];

			var bus = Bus.Factory.CreateUsingRabbitMq(busConfig =>
			{
				var host = busConfig.Host(BuildRabbitMqHostAddressUri(server, virtualHost), hostConfig =>
				{
					hostConfig.Username(username);
					hostConfig.Password(password);
					hostConfig.Heartbeat(10);
				});

				busConfig.UseRetry(Retry.Immediate(2));

				busConfig.ReceiveEndpoint(host, QueueName, endpointConfig =>
				{
					endpointConfig.Consumer<ColorEventConsumer>();
				});
			});

			bus.ConnectConsumeObserver(new CustomConsumeObserver());

			return bus;
		}

		static Uri BuildRabbitMqHostAddressUri(string server, string virtualHost)
		{
			return new UriBuilder("rabbitmq", server)
			{
				Path = virtualHost
			}.Uri;
		}

		static void StopBus()
		{
			Logger.Info("Stopping bus...");
			readOnlyBus.Stop();
			Logger.Info("Bus stopped.");
		}

		static void WaitForInput()
		{
			Console.WriteLine("Type 'quit' to quit.");
			var text = "";
			while (text != "quit")
			{
				text = Console.ReadLine();
			}
		}

		static void DisplayBusConfiguration(IBusControl bus)
		{
			var result = bus.GetProbeResult();
			Logger.Debug(result.ToJsonString());
		}
	}
}
