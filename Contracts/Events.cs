namespace Laboratory.Contracts
{
	public interface IEvent
	{
		int Number { get; set; }
		string Type { get; set; }
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

	public class GreenEvent : IEvent
	{
		public GreenEvent()
		{
			Type = GetType().ToString();
		}

		public int Number { get; set; }
		public string Type { get; set; }
	}

	public class BlueEvent : IEvent
	{
		public BlueEvent()
		{
			Type = GetType().ToString();
		}

		public int Number { get; set; }
		public string Type { get; set; }
	}
}
