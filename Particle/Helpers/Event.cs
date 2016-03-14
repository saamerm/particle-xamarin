using System;
using System.Collections.Generic;
namespace Particle.Helpers
{
	public class Event : EventArgs
	{
		public Event(string error)
		{
			Error = error;
		}

		public Event(string name, Dictionary<string, ParticleEvent> data, EventState state)
		{
			Name = name;
			Data = data;
			State = state;
		}

		public string Name { get; set; }
		public Dictionary<string, ParticleEvent> Data { get; set; }
		public EventState State { get; set; }
		public string Error { get; set; }
	}
}