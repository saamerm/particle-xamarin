using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using static System.Diagnostics.Debug;

using static Newtonsoft.Json.JsonConvert;
using ModernHttpClient;
using Particle.Models;
using System.Threading;

namespace Particle.Helpers
{
	public delegate void ParticleEventHandler(object myObject, ParticleEventArgs myArgs);

	public class EventSource : IDisposable
	{
		public EventSource(string url, string accessToken)//, int timeoutInterval = 30, int retryInterval = 1)
		{
			EventUrl = url;
			Listeners = new Dictionary<string, ParticleEventHandler>();
			//TimeoutInterval = timeoutInterval;
			//RetryInterval = retryInterval;
			AccessToken = accessToken;
		}

		public string EventUrl { get; internal set; }
		public Dictionary<string, ParticleEventHandler> Listeners { get; internal set; }
		//public int TimeoutInterval { get; internal set; }
		//public int RetryInterval { get; internal set; } = 1;
		public int LastEventID { get; internal set; }
		public string AccessToken { get; internal set; }
		public EventRepository AllEvents { get; internal set; }

		public void AddEventListener(string eventName, ParticleEventHandler handler)
		{
			if (!Listeners.ContainsKey(eventName))
			{
				Listeners.Add(eventName, handler);
				OnMessage += handler;
			}
			else
				throw new Exception("Event Name is present in dictionary. Consider using a GUID to ensure a unique number is generated");
		}

		public void RemoveEventListener(string eventName)
		{
			if (Listeners[eventName] != null)
			{
				OnMessage -= Listeners[eventName];
				Listeners.Remove(eventName);
			}
		}

		public event ParticleEventHandler OnMessage;
		public event ParticleEventHandler OnError;
		public event ParticleEventHandler OnOpen;

		void ReceivedMessage(ParticleEvent receivedEvent, string error = null)
		{
			if (!String.IsNullOrEmpty(error) && OnError != null)
				OnError(this, new ParticleEventArgs(error));

			if (OnMessage != null)
				OnMessage(this, new ParticleEventArgs(receivedEvent));
		}

		void CloseConnection()
		{
			isSubscribed = false;
			AllEvents.State = EventState.Closed;
		}

		bool isSubscribed = false;
		public async Task StartHandlingEvents()
		{
			AllEvents.State = EventState.Connecting;
			var url = EventUrl + "?access_token=" + AccessToken;
			string eventName = "";
			isSubscribed = true;

			using (var client = new HttpClient(new NativeMessageHandler()))
			{
				client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

				var request = new HttpRequestMessage(HttpMethod.Get, url);
				using (var response = await client.SendAsync(
					request,
					HttpCompletionOption.ResponseHeadersRead))
				{
					AllEvents.State = EventState.Open;
					using (var body = await response.Content.ReadAsStreamAsync())
					using (var reader = new StreamReader(body))
					{
						if (OnOpen != null)
							OnOpen(this, new ParticleEventArgs());

						while (!reader.EndOfStream && isSubscribed)
						{
							var outputString = reader.ReadLine();
							if (outputString.Contains("event"))
							{
								eventName = outputString.Substring(6);
							}
							else if (outputString.Contains("data"))
							{
								Dictionary<string, string> values = DeserializeObject<Dictionary<string, string>>(outputString.Substring(5));
								var lastevent = new ParticleEvent(values, eventName);
								ReceivedMessage(lastevent);
							}
						}
					}
				}
			}
		}

		public void Dispose()
		{
			EventUrl = null;
			AccessToken = null;
			AllEvents = null;

			foreach (var listener in Listeners)
				OnMessage -= listener.Value;

			Listeners = null;
		}
	}
}