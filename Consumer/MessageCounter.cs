using System.Diagnostics;
using System.Threading;

using NLog;

namespace Laboratory.Consumer
{
	static class MessageCounter
	{
		static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		static readonly Stopwatch Stopwatch = new Stopwatch();
		static int received;
		static int preConsumed;
		static int postConsumed;
		static int faulted;

		static string Summary => $"Received {received} messages. Pre: {preConsumed}, post: {postConsumed}, faulted: {faulted}.";

		public static int Received => received;

		public static void Receive()
		{
			LogInitialSummary();

			Interlocked.Increment(ref received);

			LogSummary();
		}

		static void LogInitialSummary()
		{
			if (!IsFirstReceived()) return;

			Logger.Info(Summary);

			Stopwatch.Start();
		}

		static void LogSummary()
		{
			if (!IsHundredthReceived()) return;

			var messageRate = 1000 * received / Stopwatch.ElapsedMilliseconds;
			Logger.Info($"{Summary} {Stopwatch.Elapsed}, {messageRate} msg/sec (avg).");
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

		static bool IsFirstReceived()
		{
			return received == 0;
		}

		static bool IsHundredthReceived()
		{
			return received % 100 == 0;
		}
	}
}