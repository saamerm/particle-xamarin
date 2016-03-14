using System;
using System.Collections.Generic;

namespace Particle
{
	public class ParticleEvent
	{
		public ParticleEvent(Dictionary<string, string> eventDictionary)
		{
			DeviceId = eventDictionary["coreid"];
			Data = eventDictionary["data"];
			Event = eventDictionary["event"];
			TimeToLive = Convert.ToInt32(eventDictionary["ttl"]);
			Time = DateTime.Parse(eventDictionary["published_at"]);
		}

		public string DeviceId { get; set; }
		public string Data { get; set; }
		public string Event { get; set; }
		public DateTime Time { get; set; }
		public int TimeToLive { get; set; }
		public string Desctiption
		{
			get { return "<Event: {Event}, DeviceID: {DeviceId}, Data: {Data}, Time: {Time:o}, TTL: {TimeToLive}>"; }
		}
	}
}