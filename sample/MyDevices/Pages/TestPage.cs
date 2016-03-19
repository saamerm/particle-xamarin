using System;
using Xamarin.Forms;
using Particle;
using System.Threading.Tasks;
using Particle.Helpers;
namespace MyDevices.Pages
{
	public class TestPage : ContentPage
	{
		public TestPage()
		{
			Button button = new Button { Text = "Test" };
			Label results = new Label();

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children = {
					button,
					results
				}
			};

			button.Clicked += async (object sender, EventArgs e) =>
			{
				//var test = await ParticleCloud.SharedInstance.PublishEventWithName("test", "Data", false, 1000);
				await ParticleCloud.SharedInstance.SubscribeToAllEventsWithPrefix("test", WriteMessageToLine);
			};

		}

		public void WriteMessageToLine(object sender, ParticleEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(e.EventData.Event);
		}



		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await ParticleCloud.SharedInstance.CreateOAuthClientAsync(App.Token, "xamarin");
			var response = await ParticleCloud.SharedInstance.LoginWithUserAsync("michael.watson@xamarin.com", "Da2188MW");

			//StartPublish();

		}

		async Task StartPublish()
		{
			bool keepRunning = true;

			while (keepRunning)
			{
				await ParticleCloud.SharedInstance.PublishEventWithName("test", "Data", false, 1000);
				await Task.Delay(1000);
			}
		}
	}
}

