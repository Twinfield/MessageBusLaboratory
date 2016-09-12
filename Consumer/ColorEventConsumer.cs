using System;
using System.Threading.Tasks;
using Laboratory.Contracts;
using MassTransit;

using NLog;

namespace Laboratory.Consumer
{
	internal class ColorEventConsumer :
		IConsumer<RedEvent>,
		IConsumer<BlueEvent>,
		IConsumer<GreenEvent>
	{
		static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public Task Consume(ConsumeContext<RedEvent> context)
		{
			return Task.Run(() =>
			{
				Handle(context.Message.GetType(), context.Message.Number);
			});
		}

		public Task Consume(ConsumeContext<BlueEvent> context)
		{
			return Task.Run(() =>
			{
				Handle(context.Message.GetType(), context.Message.Number);
			});
		}

		public Task Consume(ConsumeContext<GreenEvent> context)
		{
			return Task.Run(() =>
			{
				Handle(context.Message.GetType(), context.Message.Number);
			});
		}

		static void Handle(Type type, int number)
		{
			MessageCounter.Receive();

			Logger.Debug($"Received message {number} of type {type}.");
		}
	}
}