# Упрощённая печать данных на принтере

Для печати документов на принтере в Linux используется [CUPS](https://openprinting.github.io/cups/) = _Common UNIX Printing System_. CUPS управляет очередями печати и предоставляет пользователям команды для использования в shell, а также API для программистов на языке C++, для интеграции новых приложений с CUPS. Для работы с очередью печати используются команды **lpstat** и **lpr**. Команда lpstat позволяет получить информацию о подключенных принтерах. Например, команда `lpstat -p` вернёт строку вида:

``` console
printer Printer-KPOS_58C is idle.  enabled since Чт 12 янв 2023 19:49:14
```

Подстрока `Printer-KPOS_58C` - это зарегистрированное имя принтера в системе.

Поместить файл в очередь печати принтера можно командой `lpr [имя файла]`.

Кроме утилит, в дистрибутовах с поддержкой Desktop часто реализованы утилиты для доступа к очередям принтера.

В зависимости от поддерживаемого принтером протокола, CUPS-драйвер может поддерживать как _PostScript_, так и _CUPS Raster Files_.

## Выполнение команд Shell из Avalonia-приложения

Для того, чтобы выполнять эти команды из кода Avalonia-приложения, можно использовать специализированную helper-функцию:

``` csharp
using System;
using System.Diagnostics;

public static class ShellHelper
{
    public static string Bash(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
}
```

Вызвать функцию можно следующим образом:

``` csharp
var cmd = $"lpstat -p";
var results = cmd.Bash();
```
