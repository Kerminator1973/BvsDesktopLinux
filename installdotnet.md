## Установка .NET 6 в Ubuntu 22.04

Установить Runtime:

``` shell
sudo apt-get update && \
  sudo apt-get install -y dotnet6
```

Установить SDK: `sudo apt-get install -y dotnet-sdk-6.0`

В случае возникновения ошибок из-за конфликтов зависимостей, может потребоваться выполнить настройку канала доступа к компонентам .NET:

``` shell
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```

Проверить список установленных SDK: `dotnet --list-sdks`

К сожалению, установка SDK не всегда работает. Я сталкивался с [проблемой потери информации о SDK](https://github.com/dotnet/sdk/issues/27129), в частности, при переходе на Ubuntu 22.10 Kinetic Kudu.

**Решение**, которое помогло:

1. Удалить предыдущие установки: `sudo apt remove --purge --autoremove *dotnet*`
2. Создать, или модифицировать файл `/etc/apt/preferences` и добавить в него, если нужно, следующие строки (нужен **sudo**):

```
Package: *net*
Pin: origin packages.microsoft.com
Pin-Priority: 1001
```

Подробнее о настройке можно почитать здесь: `man apt_preferences`

3. Загрузить .NET командой: `sudo apt install dotnet-sdk-6.0`

## Основные консольные команды

Команды создания приложения:

```
dotnet new avalonia.mvvm -o BvsDesktopLinux -n BvsDesktopLinux
cd BvsDesktopLinux
dotnet new sln
dotnet sln add BvsDesktopLinux.csproj
```

Сразу после команды new указывается имя шаболна. Список шаблонов можно посмотреть командой: `dotnet new list`. Установить шаблоны Avalonia можно командой `dotnet new install Avalonia.Templates`, см. [шаблоны Avalonia](https://github.com/AvaloniaUI/avalonia-dotnet-templates)

Если нужно указать конкретную платформу, то это можно сделать используя параметр `-F`:

``` shell
dotnet new avalonia.mvvm -F "net6.0" -o approve -n approve
```

Список параметров для конкретного шаблона можно посмотреть командой (пример): `dotnet new avalonia.mvvm -h`

Установка Git и загрузка репозитария из GitHub:

``` shell
sudo apt-get update
sudo apt-get install git
git clone https://github.com/Kerminator1973/BvsDesktopLinux.git
```

Команды сборки и запуска приложения (запуск из подкаталога с файлом .csproj):

``` shell
dotnet restore
dotnet build
dotnet run
```

Запуск Unit-тестов: `dotnet test`

## Передача приложения пользователям без исходных текстов

В случае, если нам необходимо передать пользователям версию продукта без исходных текстов, то следует установить .NET Runtime, собрать приложения в Release-версии:

``` shell
dotnet build --configuration Release
```

Далее следует скопировать приложение с зависимостями на компьютеры пользователей и запускать их явно указывая имя приложения, например:

``` shell
dotnet ./ADMCheck
```
