using System.Diagnostics;
using System.Threading;

using NLog;

namespace Laboratory.Consumer
{
	static class MessageCounter
	{
		static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static int received;
		static int preConsumed;
		static int postConsumed;
		static int faulted;
		static readonly Stopwatch Stopwatch = new Stopwatch();

		public static int Received => received;

		public static void Receive()
		{
			LogInitialSummary();

			Interlocked.Increment(ref received);

			LogSummary();
		}

		static void LogInitialSummary()
		{
			if (received != 0) return;

			Logger.Info($"Received {received} messages. Pre: {preConsumed}, post: {postConsumed}, faulted: {faulted}.");
			Stopwatch.Start();
		}

		static void LogSummary()
		{
			if (received % 100 == 0)
				Logger.Info($"Received {received} messages. Pre: {preConsumed}, post: {postConsumed}, faulted: {faulted}. " +
					$"{Stopwatch.Elapsed} since first message, {1000 * received / Stopwatch.ElapsedMilliseconds} msg/sec.");
		}

		public static void PreConsume()
		{
			Interlocked.Increment(ref preConsumed);
		}

		public static void PostConsume()
		{
			Interlocked.Increment(ref postConsumed);
		}
		public static void Faulted()
		{
			Interlocked.Increment(ref faulted);
		}
	}
}