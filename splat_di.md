# Dependency Injection в Avalonia

В Avalonia с момента перехода на ReactiveUI 6 используется [Splat](https://github.com/reactiveui/splat), в качестве sevice locator и [dependency injection](https://www.reactiveui.net/docs/handbook/dependency-inversion/). Маркерами использования этих механизмов являются Locator.Current и Locator.CurrentMutable.

Splat's Service Location сервис решает несколько проблем, которые были в RxUI:

- Service Location быстрый и у него почти нет overhead
- он включает несколько разных моделей управления жизненным циклом приложений ("создавать новый каждый раз", singleton, laze)

Дополнительно стоит почитать: https://github.com/reactiveui/Splat.DI.SourceGenerator
