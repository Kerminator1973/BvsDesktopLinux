using Avalonia;
using Avalonia.Controls;

namespace ComposeUI.Views;

public partial class Child : UserControl
{
    public Child()
    {
        InitializeComponent();
    }

    // Создаём свойство, которое будет использоваться в XAML верстке, а также
    // будет использовано для привязки (binding) в XAML родительского элемента
    public string SomeString
    {
        get => this.GetValue(SomeStringProperty);
        set => this.SetValue(SomeStringProperty, value);
    }

    public static readonly StyledProperty<string> SomeStringProperty =
        AvaloniaProperty.Register<Child, string>(nameof(SomeString));
}