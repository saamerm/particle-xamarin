using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using static System.Diagnostics.Debug;

using static Newtonsoft.Json.JsonConvert;
using ModernHttpClient;
using Particle.Models;

namespace Particle.Helpers
{
	public delegate void ParticleEventHandler(object myObject, ParticleEventArgs myArgs);

	public class EventSource : IDisposable
	{
		public EventSource(string url, string accessToken, int timeoutInterval, int retryInterval = 1)
		{
			EventUrl = url;
			Listeners = new Dictionary<string, ParticleEventHandler>();
			TimeoutInterval = timeoutInterval;
			RetryInterval = retryInterval;
			AccessToken = accessToken;
		}

		public HttpClient client = new HttpClient(new NativeMessageHandler());
		public string EventUrl { get; internal set; }
		public Dictionary<string, ParticleEventHandler> Listeners { get; internal set; }
		public int TimeoutInterval { get; internal set; }
		public int RetryInterval { get; internal set; } = 1;
		public int LastEventID { get; internal set; }
		public string AccessToken { get; internal set; }
		public Event SourceEvent { get; internal set; }

		public void AddEventListener(string eventName, ParticleEventHandler handler)
		{
			if (Listeners[eventName] == null)
			{
				Listeners.Add(eventName, handler);
				OnMessage += handler;
			}
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

		//public event ParticleEventHandler OnError;
		//public event ParticleEventHandler OnOpen;

		void ReceivedMessage(ParticleEvent receivedEvent, string error = null)
		{
			if (!String.IsNullOrEmpty(error))
			{
				if (OnMessage != null)
					OnMessage(this, new ParticleEventArgs(error));
			}

			if (OnMessage != null)
				OnMessage(this, new ParticleEventArgs(receivedEvent));
		}

		async Task StartHandlingEvents()
		{
			var keepChecking = true;
			while (keepChecking)
			{
				//Get info from url endpoint
				try
				{
					var response = await client.GetAsync(EventUrl + "?access_token=" + AccessToken);
					var particleResponse = DeserializeObject<ParticleGeneralResponse>(await response.Content.ReadAsStringAsync());

					//if (particleResponse.Success)
					//{
					//	OAuthClientId = particleResponse.ClientDetails.Id;
					//	OAuthClientSecret = particleResponse.ClientDetails.Secret;
					//	return true;
					//}

				}
				catch (Exception e)
				{
					WriteLine(e.Message);
				}


				if (keepChecking)
					await Task.Delay(TimeSpan.FromSeconds(RetryInterval));
			}
			//Create ParticleEvent from http response, pass into EventHandler
			SourceEvent.State = EventState.Connecting;

		}

		public void Dispose()
		{
			client = null;
			EventUrl = null;
			Listeners = null;
			AccessToken = null;
			SourceEvent = null;
		}
	}
}