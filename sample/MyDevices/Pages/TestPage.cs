using System;
using Xamarin.Forms;
using Particle;
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
				var test = await ParticleCloud.SharedInstance.PublishEventWithName("test", "Data", false, 1000);
			};
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await ParticleCloud.SharedInstance.CreateOAuthClientAsync(App.Token, "xamarin");
			var response = await ParticleCloud.SharedInstance.LoginWithUserAsync("michael.watson@xamarin.com", "Da2188MW");

		}
	}
}

