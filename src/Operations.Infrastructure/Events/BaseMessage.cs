using System;
using Newtonsoft.Json;

namespace Operations.Infrastructure.Events
{
	public class BaseMessage
	{
		public BaseMessage(string correlationId = null)
		{
			Init(correlationId);
		}

		[JsonConstructor]
		public BaseMessage(string id, DateTime createDate)
		{
			Id = id;
			CreatedDate = createDate;
		}

		[JsonProperty]
		public string Id { get; private set; }

		[JsonProperty]
		public DateTime CreatedDate { get; private set; }

		[JsonProperty]
		public virtual string CorrelationId { get; set; }

		private void Init(string correlationId = null)
		{
			Id = Guid.NewGuid().ToString();
			CreatedDate = DateTime.UtcNow;
			CorrelationId = correlationId ?? Id;
		}
	}
}
