using Microsoft.Extensions.DependencyInjection;
using BullsAndCows.Views;
namespace BullsAndCows;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage());
	}
	
	
}