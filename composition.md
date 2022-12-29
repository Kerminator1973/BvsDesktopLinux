# Композиция элементов пользовательского интерфейса

Естественный способ снизить сложность кода отвечающего за пользовательский интерфейс - активно использовать композицию элементов пользовательского интерфейса. Выглядеть композиция может таким образом:

``` xml
<Window xmlns="https://github.com/avaloniaui"
        ...
		xmlns:controls="clr-namespace:approve.Views"
        Title="approve">
    ...
    <StackPanel Orientation="Vertical">
        <controls:PortSelector DeviceName="Купюпроприёмник D210" />
        <controls:PortSelector DeviceName="Принтер Cashino" />
        <controls:PortSelector DeviceName="Модуль спецэлектроники WDT" />
    </StackPanel>
</Window>
```

В приведённом выше примере, в главное окно встраивается три схожих элемента пользовательского интерфейса. Верстка такого компонента может выглядеть следующим образом:

``` xml
<UserControl xmlns="https://github.com/avaloniaui"
             ...
             x:Class="approve.Views.PortSelector">
	
	<StackPanel Orientation="Horizontal">
		<TextBlock Text="{Binding DeviceName}" />
		<ComboBox Name="FontComboBox" />
		<Button Content="Проверить" />
	</StackPanel>
</UserControl>
```

Связанный программный код:

``` csharp
using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace approve.Views
{
    public partial class PortSelector : UserControl
    {
        public String DeviceName
        {
            get { return (String)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public PortSelector()
        {
            InitializeComponent();

            DataContext = this;

            var fontComboBox = this.Find<ComboBox>("FontComboBox");
            if (null != fontComboBox)
            {
                fontComboBox.Items = new List<string> { "Порт 1", "Порт 2", "Порт 3", "Порт 4" };
                fontComboBox.SelectedIndex = 0;
            }
        }

        // Регистрируем зависимое свойство, которое можно будет считать и установить в том числе, в родительском органе управления
        public static readonly StyledProperty<String> DeviceNameProperty =
            AvaloniaProperty.Register<MainWindow, String>
            (
                nameof(DeviceName)
            );
    }
}
```

Ключевой частью являются т.н. зависимые свойства, через которые можно передать данные из верстки родительского элемента дочернему элементу. В приведённом выше примере, такое зависимое свойство называется DeviceName:

``` csharp
public String DeviceName
{
    get { return (String)GetValue(DeviceNameProperty); }
    set { SetValue(DeviceNameProperty, value); }
}
```

Чтобы иметь возможность использовать использовать это свойство его необходимо зарегистрировать:

``` csharp
public static readonly StyledProperty<String> DeviceNameProperty =
    AvaloniaProperty.Register<MainWindow, String>
    (
        nameof(DeviceName)
    );
```

Следует обратить внимание, что в дочернем элементе контекс данных установлен на текущий компонент: `DataContext = this;`

Эквивалентом **StyledProperty** в WPF является [DependencyProperty](https://github.com/Kerminator1973/BVSDesktopSupport/blob/main/ui_composition.md).
