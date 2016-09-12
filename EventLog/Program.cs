using System;
using MassTransit;
using MassTransit.Monitoring.Introspection.Contracts;

namespace Laboratory.EventLog
{
	class Program
	{
		static void Main(string[] args)
		{
			var readOnlyBus = ConfigureBus();

			ProbeResult result = readOnlyBus.GetProbeResult();
			Console.WriteLine(result.ToJsonString());

			readOnlyBus.Start();

			var text = "";
			while (text != "quit")
			{
				text = Console.ReadLine();
			}

			readOnlyBus.Stop();
		}

		private static IBusControl ConfigureBus()
		{
			var readOnlyBus = Bus.Factory.CreateUsingRabbitMq(sbc =>
			{
				var host = sbc.Host(new Uri("rabbitmq://localhost/lab"), h =>
				{
					h.Username("guest");
					h.Password("guest");
					h.Heartbeat(10);
				});
				sbc.EnablePerformanceCounters();

				sbc.ReceiveEndpoint(host, "EventLog", ec =>
				{
					ec.Consumer<EventLogConsumer>();
				});
			});
			return readOnlyBus;
		}
	}
}
