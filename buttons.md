# Обработка нажатий на кнопки (Buttons)

Существуют два разных подхода: использование команды и разработка обработчика.

Для создания расширяемой архитектуры предпочтительным является использование команды - в этом случае команда размещается в DataConext и это создаёт слабую связь между представлением и бизнес-логикой (ModelView).

Команда описывается в XAML следующим образом:

``` csharp
<Button Content="Дальше >>" Command="{Binding ConfigurationApprove}" />
```

Обработчик команды выглядит так:

``` csharp
public void ConfigurationApprove()
{
    ...
}
```

Этот подход требует проектирования приложения и его использование может быть затруднено при "быстром" создании прототипа приложения.

## Прямолинейный подход - использование обработчика нажатия на кнопку

Однако, возможен и более прямолинейный подход - в коде на C#, являющегося частью View может быть определён обработчик нажатия на кнопку, например:

``` csharp
void bConfigurationApprove(object sender, RoutedEventArgs args)
{
    ...
}
```

В этом случае, XAML может выглядеть так:

``` csharp
<Button Content="Дальше >>" Click="bConfigurationApprove" />
```

## CheckBox - это тоже Button

Связать состояние CheckBox со свойством можно используя атрибут **IsChecked**:

``` csharp
<CheckBox IsChecked="{Binding IsSopButtonPressed}">SOP Button</CheckBox>
```

Свойство может быть определено следующим образом:

``` csharp
private Boolean isSopButtonPressed = false;

public Boolean IsSopButtonPressed
{
    get { return isSopButtonPressed; }
    set
    {
        this.RaiseAndSetIfChanged(ref isSopButtonPressed, value);
    }
}
```

Как и у экземпляра класса Button, мы можем использовать атрибут **Click** для обработки нажатия на поле выбора состояния CheckBox:

``` csharp
<CheckBox Click="btnSopButton">SOP Button</CheckBox>
```
