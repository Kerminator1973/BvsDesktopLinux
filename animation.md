# Аниманиция в Avalonia

Анимация в Avalonia работает на тех же приниципах, что и в CSS - определяется некоторый "класс" анимации, который применяется к конкретному органу управления. Пример определения анимации:

``` csharp
<UserControl.Styles>
    <Style Selector="Border.red">
        <Style.Animations>
            <Animation Duration="0:0:1">
                <KeyFrame Cue="0%">
                    <Setter Property="Opacity" Value="0.0"/>
                </KeyFrame>
                <KeyFrame Cue="100%">
                    <Setter Property="Opacity" Value="1.0"/>
                </KeyFrame>
            </Animation>
        </Style.Animations>
    </Style>
</UserControl.Styles>
```

В приведённом выше примере указывается тип органа управления (Border) и имя стиля (red) к которым может быть применена анимация. Также определяются ключевые фреймы - 0% и 100%, с каждым из которых связано определённое свойство.

Продолжительность анимации можно задавать и в частях секунды, например: `0:0:0.7`.

Предположим, что у нас определён некоторый орган управления:

``` csharp
<Border Name="ErrorBanner" ... />
```

Теперь в кода на C# мы можем запустить анимацию добавив стиль "red", либо убрать этот стиль:

``` csharp
ErrorBanner.Classes.Add("red");
ErrorBanner.Classes.Remove("red");
```
