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

## Получение состояния термопринтера через USB-соединение

Утилита **lsusb** позволяет вывести информацию о USB-шинах и подключенных к ним устройствах. Каждое из устройств имеет пару 16-битных идентификаторов: идентификатор производителя (Vendor ID) и идентификатор устройства (Device ID), которые следует использовать для идентификации устройств и поиска драйвера устройства.

Запуск утилиты **lsusb** без параметров позволяет вывести на экран список подключенных устройств:

``` console
Bus 001 Device 001: ID 1d6b:0002 Linux Foundation 2.0 root hub
Bus 002 Device 004: ID 0fe6:811e ICS Advent Parallel Adapter
Bus 002 Device 003: ID 0e0f:0002 VMware, Inc. Virtual USB Hub
Bus 002 Device 002: ID 0e0f:0003 VMware, Inc. Virtual Mouse
Bus 002 Device 001: ID 1d6b:0001 Linux Foundation 1.1 root hub
```

Команда `lsusb -v` выводит подробную информацию. Пример вывода для термопринтера:

```
Bus 002 Device 004: ID 0fe6:811e ICS Advent Parallel Adapter
Device Descriptor:
  bLength                18
  bDescriptorType         1
  bcdUSB               2.00
  bDeviceClass          239 Miscellaneous Device
  bDeviceSubClass         2 
  bDeviceProtocol         1 Interface Association
  bMaxPacketSize0        64
  idVendor           0x0fe6 ICS Advent
  idProduct          0x811e Parallel Adapter
  bcdDevice            1.00
  iManufacturer           1 GD32 Microelectronics
  iProduct                2 GD32 Composite CDC Printer
  iSerial                 3 6268784D0B34
  bNumConfigurations      1
  Configuration Descriptor:
    bLength                 9
    bDescriptorType         2
    wTotalLength       0x0020
    bNumInterfaces          1
    bConfigurationValue     1
    iConfiguration          0 
    bmAttributes         0xc0
      Self Powered
    MaxPower              100mA
    Interface Descriptor:
      bLength                 9
      bDescriptorType         4
      bInterfaceNumber        0
      bAlternateSetting       0
      bNumEndpoints           2
      bInterfaceClass         7 Printer
      bInterfaceSubClass      1 Printer
      bInterfaceProtocol      2 Bidirectional
      iInterface              0 
      Endpoint Descriptor:
        bLength                 7
        bDescriptorType         5
        bEndpointAddress     0x81  EP 1 IN
        bmAttributes            2
          Transfer Type            Bulk
          Synch Type               None
          Usage Type               Data
        wMaxPacketSize     0x0040  1x 64 bytes
        bInterval               0
      Endpoint Descriptor:
        bLength                 7
        bDescriptorType         5
        bEndpointAddress     0x01  EP 1 OUT
        bmAttributes            2
          Transfer Type            Bulk
          Synch Type               None
          Usage Type               Data
        wMaxPacketSize     0x0040  1x 64 bytes
        bInterval               0
Device Status:     0x0000
  (Bus Powered)
```

В предоставленных данных есть несколько важных параметров:

```
iProduct            2 GD32 Composite CDC Printer
idVendor            0x0fe6 ICS Advent
idProduct           0x811e Parallel Adapter
bEndpointAddress    0x81  EP 1 IN
bEndpointAddress    0x01  EP 1 OUT
```

Параметр **iProduct** позволяет увидеть, что подключенное USB-устройство является составным CDC-принтером. Идентификационные параметры **idVendor** (0x0fe6) и **idProduct** (0x811e) будут использованы для поиска устройства для подключения. Параметры **bEndpointAddress** позволяют определить, что через порт 0x81 можно получить данные от устройства, а 0x01 - записать данные на устройство.

Ещё интересной информацией является версия USB, которая указана в параметре **bcdUSB** (2.00). Также интересной является информация о классе устройства **bDeviceClass**. В конкретном случае класс устройства: _Miscellaneous Device_ (239).
