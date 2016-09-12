using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;
using MassTransit.AzureServiceBusTransport;

using NLog;

namespace Laboratory.Publisher
{
	class Program
	{
		static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static IBusControl sendOnlyBus;
		static readonly MessageFactory MessageFactory = new MessageFactory(int.Parse(ConfigurationManager.AppSettings["MessageSize"]));

		static void Main(string[] args)
		{
			var useAzureServiceBus = args.Any(a => a.ToLower() == "azure");
			var batchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);

			SetupAndStartBus(useAzureServiceBus);

			PublishBatch(batchSize);

			StopBus();

			Logger.Info("Finished");
		}

		static void SetupAndStartBus(bool useAzureServiceBus)
		{
			Logger.Info("Configuring bus...");
			sendOnlyBus = useAzureServiceBus ? SetupSendOnlyAzureServiceBus() : SetupSendOnlyRabbitMqBus();

			Logger.Info("Starting bus...");
			sendOnlyBus.Start();
			Logger.Info("Bus started");
		}

		static IBusControl SetupSendOnlyAzureServiceBus()
		{
			Logger.Info("Configuring Azure bus...");

			var connectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];

			return Bus.Factory.CreateUsingAzureServiceBus(busConfig =>
			{
				busConfig.Host(connectionString, hostConfig => { });
			});
		}

		static IBusControl SetupSendOnlyRabbitMqBus()
		{
			Logger.Info("Configuring RabbitMQ bus...");

			var server = ConfigurationManager.AppSettings["RabbitMqServer"];
			var virtualHost = ConfigurationManager.AppSettings["RabbitMqVirtualHost"];
			var username = ConfigurationManager.AppSettings["RabbitMqUsername"];
			var password = ConfigurationManager.AppSettings["RabbitMqPassword"];

			return Bus.Factory.CreateUsingRabbitMq(busConfig =>
			{
				busConfig.Host(BuildRabbitMqHostAddressUri(server, virtualHost), hostConfig =>
				{
					hostConfig.Username(username);
					hostConfig.Password(password);
					hostConfig.Heartbeat(10);
				});
			});
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
			sendOnlyBus.Stop();
			Logger.Info("Bus stopped.");
		}

		static void PublishBatch(int batchSize)
		{
			Logger.Info($"Publishing batch of {batchSize} messages...");

			var messages = MessageFactory.CreateBatch(batchSize);
			Task.WaitAll(messages.Select(message => Publish(sendOnlyBus, message)).ToArray());

			Logger.Info("Messages published.");
		}

		static Task Publish<T>(IPublishEndpoint bus, T message)
		{
			return bus.Publish(message, ctx =>
			{
				SendFaultEventsToDedicatedQueue(ctx);
			});
		}

		static void SendFaultEventsToDedicatedQueue(SendContext context)
		{
			context.FaultAddress = new Uri(ConfigurationManager.AppSettings["ServiceBusFaultAddress"]);
		}
	}
}
