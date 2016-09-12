namespace Laboratory.Contracts
{
	public interface IEvent
	{
		int Number { get; set; }
		string Type { get; set; }
	}

	public class JustAnEvent : IEvent
	{
		public JustAnEvent()
		{
			Type = GetType().ToString();
		}

		public int Number { get; set; }
		public string Type { get; set; }
	}

	public class GreenEvent : IEvent
	{
		public GreenEvent()
		{
			Type = GetType().ToString();
		}

		public int Number { get; set; }
		public string Type { get; set; }
	}

	public class RedEvent : IEvent
	{
		public RedEvent()
		{
			Type = GetType().ToString();
		}

		public int Number { get; set; }
		public string Type { get; set; }
	}

	public class UnhandledEvent : IEvent
	{
		public UnhandledEvent()
		{
			Type = GetType().ToString();
		}

		public int Number { get; set; }
		public string Type { get; set; }
	}
}
