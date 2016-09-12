using System;
using System.Collections.Generic;

using Laboratory.Contracts;

namespace Laboratory.Publisher
{
	class MessageFactory
	{
		readonly int messageSize;
		int sequenceNumber;

		public MessageFactory(int messageSize)
		{
			this.messageSize = messageSize;
		}

		public object Create()
		{
			return Create(++sequenceNumber);
		}

		public IEnumerable<object> CreateBatch(int batchSize)
		{
			for (var i = 0; i < batchSize; i++)
				yield return Create();
		}

		object Create(int number)
		{
			if (number % 3 == 0)
				return new RedEvent { Number = number, Content = new String('R', messageSize) };

			if (number % 2 == 0)
				return new GreenEvent { Number = number, Content = new String('G', messageSize) };

			return new BlueEvent { Number = number, Content = new String('B', messageSize) };
		}
	}
}