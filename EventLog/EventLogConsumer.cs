using System;
using System.Linq;
using System.Threading.Tasks;
using Laboratory.Contracts;
using MassTransit;

namespace Laboratory.EventLog
{
	/// <summary>
	/// Example of interface routing
	/// </summary>
	internal class EventLogConsumer :
		IConsumer<IEvent>
	{
		public Task Consume(ConsumeContext<IEvent> context)
		{
			return Task.Run(() =>
			{
				Console.WriteLine($"{DateTime.Now} {context.GetMessageType()} {context.Message.Number} logged");
			});
		}
	}

	static class Extensions
	{
		public static string GetMessageType(this ConsumeContext<object> context)
		{
			var typeUrn = context.SupportedMessageTypes.FirstOrDefault();

			return String.IsNullOrEmpty(typeUrn)
				? context.Message.GetType().ToString()
				: typeUrn.Substring(typeUrn.LastIndexOf(':') + 1);
		}
	}
}