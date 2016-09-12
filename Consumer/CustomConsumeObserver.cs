using System;
using System.Threading.Tasks;

using MassTransit;
using MassTransit.Pipeline;
using MassTransit.Util;

using NLog;

namespace Laboratory.Consumer
{
	internal class CustomConsumeObserver : IConsumeObserver
	{
		static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public Task PreConsume<T>(ConsumeContext<T> context) where T : class
		{
			MessageCounter.PreConsume();
			if (context.GetRetryAttempt() > 0)
				Logger.Debug("Retry attempt received.");

			return TaskUtil.Completed;
		}

		public Task PostConsume<T>(ConsumeContext<T> context) where T : class
		{
			MessageCounter.PostConsume();
			if (context.GetRetryAttempt() > 0)
				Logger.Debug("Retry attempt succeeded.");

			return TaskUtil.Completed;
		}

		public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
		{
			MessageCounter.Faulted();
			if (context.GetRetryAttempt() > 0)
				Logger.Debug("Retry attempt failed.");

			return TaskUtil.Completed;
		}
	}
}