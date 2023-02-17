# Использование графических изображений

Чтобы добавить графические изображения в исполняемый файл как ресурсы, в csproj должно быть добавлено описание блока файлов:

``` xml
<ItemGroup>
    <AvaloniaResource Include="Assets\**" />
</ItemGroup>
```

После этого все файлы из папки "Assets" будут добавляться в исполняемый файл и к ним можно обращаться посредством префикса "avares://".

В простейшем случае, XAML-код будет выглядеть так:

``` csharp
<Image Source="avares://approve/Assets/dors-logo.jpg" />
```

Загрузить файл из ресурса в объект Bitmap можно так:

``` csharp
Bitmap SopButtonPressed = new Bitmap(assets.Open(new Uri(@"avares://approve/Assets/button-pressed.png")));
```

Определить XAML-элемент Image можно следующим образом:

``` csharp
<Image x:Name="SopButton" Margin="5,5,0,5" Width="100" Height="100" />
```

Связать изображение с XAML-элементом Image можно выполнив присвоение:

``` csharp
SopButton.Source = SopButtonPressed;
```

Также можно использовать Bitmap как зависимое свойство, например:

``` csharp
private Bitmap? printerOnlinePicture;

public Bitmap? PrinterOnlinePicture
{
    get { return printerOnlinePicture; }
    set
    {
        this.RaiseAndSetIfChanged(ref printerOnlinePicture, value);
    }
}

private Bitmap? printerStatusPicture;
```

В XAML указать зависимость можно следующим образом:

``` sharp
<Image Source="{Binding PrinterOnlinePicture}" />
```

Установка свойства в коде может выглядеть так:

``` csharp
this.PrinterOnlinePicture = new Bitmap(assets.Open(
    new Uri(@"avares://approve/Assets/138x208_ADM1071_Disabled.png")));
```
