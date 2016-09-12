using System;
using System.Threading.Tasks;
using Laboratory.Contracts;
using MassTransit;

using NLog;

namespace Laboratory.Consumer
{
	class MixedMessageConsumer :
		IConsumer<SomeKindOfMessage>,
		IConsumer<JustAnEvent>
	{
		static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public Task Consume(ConsumeContext<SomeKindOfMessage> context)
		{
			return Task.Run(() => Handle(context) );
		}

		public Task Consume(ConsumeContext<JustAnEvent> context)
		{
			return Task.Run(() => Handle(context));
		}

		static void Handle(ConsumeContext<SomeKindOfMessage> context)
		{
			MessageCounter.Receive();
			SynchronousAction(context.Message.GetType(), context.Message.Number);
		}

		static void Handle(ConsumeContext<JustAnEvent> context)
		{
			MessageCounter.Receive();
			SynchronousAction(context.Message.GetType(), context.Message.Number);
		}

		static void SynchronousAction(Type type, int number)
		{
			Logger.Debug($"{type} {number} started.");
			Logger.Debug($"{type} {number} finished.");
		}

	}
}
