# Использование DataTrigger в Avalonia (Xaml.Behaviors)

В WPF доступна условная настройка визуального представления по зависимым свойствам - для этого используется **DataTrigger**. Например:

``` csharp
<DataGrid.ItemContainerStyle>
    <Style TargetType="DataGridRow">
        <Style.Triggers>
            <DataTrigger Binding="{Binding Dest}" Value="Jammed">
                <Setter Property="Background" Value="Red"></Setter>
                <Setter Property="Foreground" Value="WhiteSmoke"></Setter>
```

В Avalonia для подобных задач используется отдельный Package [Avalonia XAML Behaviors](https://github.com/wieslawsoltes/AvaloniaBehaviors) от Wiesław Šoltés. Package реализует Interaction.Behaviors, DataTriggerBehavior и ChangePropertyAction. Найти библиотеку на nuget.org можно по имени: `Avalonia.Xaml.Behaviors`. Следует заметить, что документация по библиотеке размещена в [разделе Wiki](https://github.com/wieslawsoltes/AvaloniaBehaviors/wiki/DataTriggerBehavior).

Avalonia XAML Behaviors является портом библиотеки **Windows UWP version of XAML Behaviors**.

Установить Avalonia XAML Behaviors можно из командной строки:

``` shell
nuget install Avalonia.Xaml.Behaviors
```

Если утилита nuget не была загружена ранее, её можно получить [по ссылке](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe)

**XAML Behaviors** - это классы, которые привязаны (_attached_) к органу управления и слушают изменения внутри него (подписываются на событие, в том числе, на событие изменения свойств). Когда случается ожидаемое событие, выполняется триггер.

Xaml.Behaviors это подход, который применялся в WPF и UWP. Например, в UWP существуют совершенно разные виды Behaviors, каждый из которых выполняет некоторые специализированные действия: AutoFocusBehavior, AutoSelectBehavior, EventTriggerBehavior, ViewportBehavior, FadeHeaderBehavior. См. статью: [XAML Behaviors and WinUI 3](https://xamlbrewer.wordpress.com/2023/01/16/xaml-behaviors-and-winui-3/) by Diederik Krols. Очень часть Xaml.Behaviors используются для типовой настройки форм, или элементов пользовательского интерфейса.

DataTriggerBehavior срабатывает, когда данные, связанные с XamlBehaviors соответствуют указанным условиям. В [оригинальном package](https://github.com/microsoft/XamlBehaviors/wiki/DataTriggerBehavior) описываются условия: Equal, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, NotEqual.

Ниже приведён пример, в котором текст в TextBox зависит от значения свойства IsBanknoteSelected:

``` csharp
<Window 
    xmlns:int="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity"
    xmlns:ia="clr-namespace:Avalonia.Xaml.Interactions.Core;assembly=Avalonia.Xaml.Interactions" />
...
<TextBox Name="textbox" Margin="10,5,10,5">
    <int:Interaction.Behaviors>
        <ia:DataTriggerBehavior Binding="{Binding IsBanknoteSelected}" ComparisonCondition="Equal" Value="true">
            <ia:ChangePropertyAction TargetObject="{Binding #textbox}" PropertyName="Text" Value="Yes! An item selected" />
        </ia:DataTriggerBehavior>
        <ia:DataTriggerBehavior Binding="{Binding IsBanknoteSelected}" ComparisonCondition="Equal" Value="false">
            <ia:ChangePropertyAction TargetObject="{Binding #textbox}" PropertyName="Text" Value="There is no any selection here" />
        </ia:DataTriggerBehavior>
    </int:Interaction.Behaviors>
</TextBox>
```

### DataTriggerBehavior и изображения

Поход работает и в случае, если в зависимости от некоторого свойства должно отображаться некоторое изображение:

``` csharp
<Window xmlns:imaging="clr-namespace:Avalonia.Media.Imaging;assembly=Avalonia.Visuals">
...
<Window.Resources>
    <imaging:Bitmap x:Key="Banknote100">
        <x:Arguments>
            <x:String>banknote100.jpg</x:String>
        </x:Arguments>
    </imaging:Bitmap>
    <imaging:Bitmap x:Key="Banknote200">
        <x:Arguments>
            <x:String>banknote200.jpg</x:String>
        </x:Arguments>
    </imaging:Bitmap>
</Window.Resources>
...
<Image Name="image" Height="100" Margin="0,0,0,10">
    <int:Interaction.Behaviors>
        <ia:DataTriggerBehavior Binding="{Binding IsBanknoteSelected}" ComparisonCondition="Equal" Value="true">
            <ia:ChangePropertyAction TargetObject="{Binding #image}" PropertyName="Source"
                                        Value="{StaticResource Banknote100}" />
        </ia:DataTriggerBehavior>
        <ia:DataTriggerBehavior Binding="{Binding IsBanknoteSelected}" ComparisonCondition="Equal" Value="false">
            <ia:ChangePropertyAction TargetObject="{Binding #image}" PropertyName="Source" 
                                        Value="{StaticResource Banknote200}" />
        </ia:DataTriggerBehavior>
    </int:Interaction.Behaviors>
</Image>
```

Однако, приведённый вариант работает только в том случае, если изображения находятся по конкретному физическому пути.

Если же нам нужно использовать изображения находящиеся в ресурсах приложения, то следует использовать немного другой подход:

``` csharp
<Window.Resources>
    <ImageBrush x:Key="Banknote100" Source="avares://BvsDesktopLinux/Assets/banknote100.jpg" />
    <ImageBrush x:Key="Banknote200" Source="avares://BvsDesktopLinux/Assets/banknote200.jpg" />
</Window.Resources>
...
<Image Name="image" Height="100" Margin="0,0,0,10">
    <int:Interaction.Behaviors>
        <ia:DataTriggerBehavior Binding="{Binding IsBanknoteSelected}" ComparisonCondition="Equal" Value="true">
            <ia:ChangePropertyAction TargetObject="{Binding #image}" PropertyName="Source"
                                        Value="{Binding Source={StaticResource Banknote100}, Path=Source}" />
        </ia:DataTriggerBehavior>
        <ia:DataTriggerBehavior Binding="{Binding IsBanknoteSelected}" ComparisonCondition="Equal" Value="false">
            <ia:ChangePropertyAction TargetObject="{Binding #image}" PropertyName="Source" 
                                        Value="{Binding Source={StaticResource Banknote200}, Path=Source}" />
        </ia:DataTriggerBehavior>
    </int:Interaction.Behaviors>
</Image>
```

В приведённом выше примере изображение регистрируется как ImageBrush, а триггер извлекает значение поля Source конкретного объекта: `{Binding Source={StaticResource Banknote200}, Path=Source}`. Этот подход можно назвать трюковым, но он вполне эффективно работает.

## Выделить цветом строку DataGrid, используя XAML

В теории, можно использовать Avalonia.XAML.Behavior для установки цвета элементов DataGrid по значению некоторого поля. Для этого должен использоваться приблизительно вот такой код:

``` csharp
<Window.Styles>
	<Style Selector="DataGridCell.statusColumn">
		<Setter Property="FontSize" Value="24"/>
		<Setter Property="(int:Interaction.Behaviors)">
			<int:BehaviorCollectionTemplate>
				<int:BehaviorCollection>
					<ia:DataTriggerBehavior Binding="{Binding Status}" ComparisonCondition="Equal" Value="Rejected">
						<ia:ChangePropertyAction TargetObject="DataGridCell" PropertyName="Background" Value="Yellow" />
					</ia:DataTriggerBehavior>
				</int:BehaviorCollection>
			</int:BehaviorCollectionTemplate>
		</Setter>
	</Style>
</Window.Styles>
...
<DataGrid AutoGenerateColumns="False" Margin="10"
		  Items="{Binding Banknotes}" SelectedItem="{Binding SelectedBanknote}">
	<DataGrid.Columns>
		<DataGridTextColumn Header="{x:Static p:Resources.NoteId}" Binding="{Binding Id}" />
		<DataGridTextColumn Header="{x:Static p:Resources.NoteCurrency}" Binding="{Binding Currency}" />
		<DataGridTextColumn Header="{x:Static p:Resources.NoteDenomination}" Binding="{Binding Denomination}" />
		<DataGridTextColumn Header="{x:Static p:Resources.Status}" Binding="{Binding Status}" CellStyleClasses="statusColumn" />
	</DataGrid.Columns>
</DataGrid>
```

В примере использован селектор "DataGridCell.statusColumn", который позволит выбрать только те ячейки таблицы, у которых установлен стиль "statusColumn", т.е. только четвертую колонку (Status). Чтобы селектор применялся не к одному элементу, а ко всем, соответствующим ему, следует добавлить wrapper-ы:

``` csharp
<int:BehaviorCollectionTemplate>
    <int:BehaviorCollection>
        ...
    </int:BehaviorCollection>
</int:BehaviorCollectionTemplate>
```

Триггер и команда изменения свойства написаны корректно, но проблема состоит в том, что в момент привязки используется контекст всего View (т.е. ViewModel), а не контекст конкретной ячейки, или строки. Т.е. если указать какое-то свойство из ViewModel, то изменение свойства "Background" произойдёт вполне успешно.

При использовании Developer Console (F12) можно увидеть, что и у DataGridRow, и у DataGridCell контекст данных установлен на элемент ObservableCollection из ViewModel, а не на ViewModel. Т.е. скорее всего в коде Xaml.Behavior есть ограничения в части работы с DataContext.

Оставил запрос в [GitHub проекта](https://github.com/AvaloniaUI/Avalonia/discussions/8121).

## Выделить цветом строку DataGrid, используя программный код на C/#

Для выделения некоторой строки DataGrid, в зависимости от значения поля/полей используется обработчик свойства **LoadingRow**:

``` csharp
<DataGrid LoadingRow="DataGrid_OnLoadingRow" ...
```

Также необходимо добавить обработчик явным образом:

```csharp
private void DataGrid_OnLoadingRow(object? sender, DataGridRowEventArgs e)
{
    var dataObject = e.Row.DataContext as Models.Banknote;
    if (dataObject != null && dataObject.Status == "Rejected")
    {
        e.Row.Classes.Add("rejectedStatus");
    }
    else
    {
        e.Row.Classes.Remove("rejectedStatus");
    }
}
```

В приведённом выше примере используется стиль "rejectedStatus", который определяется в XAML-коде следующим образом:

``` csharp
<Window.Styles>
    <!-- Определяем стиль для отображения строки DataGrid, в которой отбражается отвергнутая купюра -->
    <Style Selector="DataGridRow.rejectedStatus">
        <Setter Property="Background" Value="Red"/>
        <Setter Property="Foreground" Value="White"/>
    </Style>
</Window.Styles>
```

Важно отметить, что в реализации обработчика события LoadingRow важно не только добавлять, но у удалять классы, так как показано в примере выше. Если этого не сделать, то стиль будет применяться хаотичным образом при сортировке DataGrid, либо при замене DataGrid другими данными.

## Обсуждения по описанным выше проблемам

Обсуждение в bug tracker-е [Avalonia: Trigger like solution for DataGridCell using DataTriggerBehavior (Avalonia.Xaml.Interactivity)](https://github.com/AvaloniaUI/Avalonia/discussions/8121). В результате обсуждения, появилось ещё одно решение задачи - с [использованием класса, реализующего интерфейс IValueConverter](./converters.md). Однако, в этом варианте, потенциально, возникает существенный drawback, который состоит в многократном вызове метода Convert() в том случае, если мы хотим установить фоновый цвет всей строки, а не одного элемента. Ниже дано разъяснение по данной проблеме c примером:

``` csharp
<Style Selector="DataGridCell" x:DataType="models:Banknote">
    <Setter Property="Background" Value="{Binding Status, Converter={StaticResource StatusConverter}}" />
</Style>
```

Допустим, в таблице есть восемь колонок и 100 строк. В случае использования приведённого выше селектора метод Convert() будет вызван 800 раз, т.е. для каждой ячейки. Теоретически, это в 8 раз больше вызовов, чем при использовании атрибута **LoadingRow**.

### Дополнительно

Информирование о результатах поиска на [StackOverflow - Avalonia UI C# XAML WPF - Adjust data grid row color based on column value](https://stackoverflow.com/questions/61589139/avalonia-ui-c-sharp-xaml-wpf-adjust-data-grid-row-color-based-on-column-value/75554247#75554247).
