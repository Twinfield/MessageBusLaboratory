using System.Collections.Generic;

using Laboratory.Contracts;

namespace Laboratory.Publisher
{
	class MessageFactory
	{
		int sequenceNumber;

		public object Create()
		{
			return Create(++sequenceNumber);
		}

		public IEnumerable<object> CreateBatch(int batchSize)
		{
			for (var i = 0; i < batchSize; i++)
				yield return Create();
		}

		static object Create(int number)
		{
			if (number % 3 == 0)
				return new RedEvent { Number = number };

			if (number % 2 == 0)
				return new BlueEvent { Number = number };

			return new GreenEvent { Number = number };
		}
	}
}