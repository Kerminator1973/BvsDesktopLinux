using ReactiveUI;

namespace ComposeUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    // Создаём три разных свойства, которые будут использоваться в дочернем элементе
    private string _propertyA = "Cool";

    public string PropertyA
    {
        get => _propertyA;
        set => this.RaiseAndSetIfChanged(ref _propertyA, value);
    }

    private string _propertyB = "Avalonia UI";

    public string PropertyB
    {
        get => _propertyB;
        set => this.RaiseAndSetIfChanged(ref _propertyB, value);
    }

    private string _propertyC = "0.10.19";

    public string PropertyC
    {
        get => _propertyC;
        set => this.RaiseAndSetIfChanged(ref _propertyC, value);
    }
}
