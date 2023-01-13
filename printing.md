# Упрощённая печать данных на принтере

Для работы с очередью печати в Linux используются команды **lpstat** и **lpr**. Команда lpstat позволяет получить информацию о подключенных принтерах. Например, команда `lpstat -p` вернёт строку вида:

``` console
printer Printer-KPOS_58C is idle.  enabled since Чт 12 янв 2023 19:49:14
```

Подстрока `Printer-KPOS_58C` - это зарегистрированное имя принтера в системе.

Поместить файл в очередь печати принтера можно командой `lpr [имя файла]`.

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
