using Avalonia;
using Avalonia.Controls;

namespace ComposeUI.Views;

public partial class Child : UserControl
{
    public Child()
    {
        InitializeComponent();
    }

    // ������ ��������, ������� ����� �������������� � XAML �������, � �����
    // ����� ������������ ��� �������� (binding) � XAML ������������� ��������
    public string SomeString
    {
        get => this.GetValue(SomeStringProperty);
        set => this.SetValue(SomeStringProperty, value);
    }

    public static readonly StyledProperty<string> SomeStringProperty =
        AvaloniaProperty.Register<Child, string>(nameof(SomeString));
}