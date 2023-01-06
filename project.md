# Рекомендуемая структура проекта

В корневой папке рекомендуется хранить sln-файл. Проекты, с их описаниями в виде csproj-файлов, рекомендуется хранить в дочерних подкаталогах. Например:

``` text
-+ All Solution (sln)
 +-- Project A (csproj)
 +-- Project B (csproj)
 +-- Project C (csproj)
 ```

Соответственно, сначала необходимо создать папку высокого уровня, затем в этой папке следует сгенерировать проект, а потом генерировать по проекту/проектам sln-файл.

Например, мы создаём папку верхнего уровня:

``` shell
mkdir approve.repo
cd approve.repo
```

Создаём в ней отдельный проект:

``` shell
dotnet new avalonia.mvvm -F "net6.0" -o approve -n approve
```

Генерируем sln-файл и включаем в него проект:

``` shell
dotnet new sln
dotnet sln add ./approve/approve.csproj
```
