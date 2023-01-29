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

Чтобы иметь возможность использовать это свойство его необходимо зарегистрировать:

``` csharp
public static readonly StyledProperty<String> DeviceNameProperty =
    AvaloniaProperty.Register<MainWindow, String>
    (
        nameof(DeviceName)
    );
```

Следует обратить внимание, что в дочернем элементе контекс данных установлен на текущий компонент: `DataContext = this;`

Эквивалентом **StyledProperty** в WPF является [DependencyProperty](https://github.com/Kerminator1973/BVSDesktopSupport/blob/main/ui_composition.md).

## Если в дочернем элементе не установлен DataContext?

В этом случае, при попытке доступа к DataContext в любых методах, за исключением конструктора, будет получен контекст данных родительского элемента. Такое поведение очень удобно для разработчика, т.к. достаточно определить в родительском элементе объект - контекст данных и доступ к нему из дочерних элементов будет осуществляться без каких-либо дополнительных действий.

Это фундаментально важная особенность поведения Avalonia и WPF: один контекст данных совместно используется несколькими разными объектами, которые используются в качестве композиционной группы (View + UserControls). Хотя, при необходимости, конкретный элемент пользовательского элемента может получить свой собственный контекст данных.

## Передать сообытие родительскому элементу

При использовании композиции важно иметь возможность передать событие дочернего элемента родительскому элементу. Предположим, что у нас есть два блока разметки, которые должны сменять друг друга при нажатии некоторых управляющих элементов. Верстка родительского элемента может выглядеть следующим образом:

``` xml
<Grid>
    <controls:ConfigureView Name="ConfigureView" />
    <controls:TestsView Name="TestsView" IsVisible="False" />
</Grid>
```

Дочерний элемент с именем _TestsView_ является невидимым. Допустим, что дочерний элемент _ConfigureView_ содержит кнопку при которой нужно спрятать _ConfigureView_ и отобразить _TestsView_. Разметка дочернего элемента может выглядеть так:

``` xml
<Button Content="Дальше >>" Grid.Column="2" Margin="10" Command="{Binding ConfigurationApprove}" />
```

Обработчик нажатия кнопки проверяет, есть внешние подписчики события, определённого специально для подобной ситуации и, если такие подписчики есть, то вызывает зарегистрированный код:

``` csharp
public partial class ConfigureView : UserControl
{
    // Определяем событие, на которое может подписаться родительский элемент
    public event Action ConfigurationIsApproved;

    public ConfigureView()
    {
        InitializeComponent();

        DataContext = this;
    }

    public void ConfigurationApprove()
    {
        if (null != ConfigurationIsApproved)
        {
            ConfigurationIsApproved();
        }
    }
}
```

Соответственно, родительский элемент может подписаться на это событие и обработать его. Например так:

``` csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ConfigureView.ConfigurationIsApproved += () =>
        {
            TestsView.IsVisible = true;
            ConfigureView.IsVisible = false;
            TestsView.Focus();
        };
    }
}
```
