using BullsAndCows.ViewModels;
using BullsAndCows.Views;
namespace BullsAndCows.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // No dependency injection: the View creates its own ViewModel.
        // All bindings in MainPage.xaml read from this object.
        BindingContext = new GameViewModel();
        
    }
}
