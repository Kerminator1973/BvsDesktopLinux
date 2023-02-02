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

```console
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

```console
iProduct            2 GD32 Composite CDC Printer
idVendor            0x0fe6 ICS Advent
idProduct           0x811e Parallel Adapter
bEndpointAddress    0x81  EP 1 IN
bEndpointAddress    0x01  EP 1 OUT
```

Параметр **iProduct** позволяет увидеть, что подключенное USB-устройство является составным CDC-принтером. Идентификационные параметры **idVendor** (0x0fe6) и **idProduct** (0x811e) будут использованы для поиска устройства для подключения. Параметры **bEndpointAddress** позволяют определить, что через порт 0x81 можно получить данные от устройства, а 0x01 - записать данные на устройство.

Ещё интересной информацией является версия USB, которая указана в параметре **bcdUSB** (2.00). Также интересной является информация о классе устройства **bDeviceClass**. В конкретном случае класс устройства: _Miscellaneous Device_ (239).

Для получения информации о состоянии устройства была использована статья [Driving a USB Thermal Printer with Linux/Raspberry Pi](https://vince.patronweb.com/2019/01/11/Linux-Zjiang-POS58-thermal-printer/). Пример кода взят из репозитория [https://github.com/vpatron/](https://github.com/vpatron/usb_receipt_printer). Однако, пример был модифицирован с целью адаптации под конкретный термопринтер и исправления ошибок в коде скрипта.

Для запуска примера требуется установить компоненты операционной системы:

``` shell
sudo apt install pip3
sudo apt install libusb-1.0-0-dev
```

Библиотека [libusb](https://github.com/libusb/libusb) является кросс-платформенной библиотекой для доступа к USB-устройствам. Эта библиотека написана на Си и используется во многих других проектах, включая [PyUSB](https://github.com/pyusb/pyusb) и [libusb.net](https://github.com/LibUsbDotNet/LibUsbDotNet).

Также необходимо загрузить библиотку PyUSB для Python:

``` shell
pip3 install pyusb
```

В скрипте на Python сначала осуществляется поиск устройство по idVendor и idProduct:

``` python
import usb.core
import usb.util

dev = usb.core.find(idVendor=0x0fe6, idProduct=0x811e)
if dev is None:
    raise ValueError('Device not found')
```

Затем следует проверить, что драйвер находится в активном состоянии:

``` python
needs_reattach = False
if dev.is_kernel_driver_active(0):
    needs_reattach = True
    dev.detach_kernel_driver(0)
```

Далее устанавливается активная конфигурации. Метод может быть вызван без параметров, что означает, что автоматически выбирается первая активная конфигурация:

``` python
dev.set_configuration()
```

Далее следует получить информацию об Endpoins (IN и OUT):

``` python
cfg = dev.get_active_configuration()
intf = cfg[(0,0)]

ep = usb.util.find_descriptor(
    intf,
    # match the first OUT endpoint
    custom_match = \
    lambda e: \
        usb.util.endpoint_direction(e.bEndpointAddress) == \
        usb.util.ENDPOINT_OUT)

ep_in = usb.util.find_descriptor(
    intf,
    # match the first IN endpoint
    custom_match = \
    lambda e: \
        usb.util.endpoint_direction(e.bEndpointAddress) == \
        usb.util.ENDPOINT_IN)
```

Затем можно записать данные в входной (OUT) Endpoint:

``` python
ep.write('\x1b\x76\x00')
```

Результат работы команды можно получить так:

``` python
try:
    data = dev.read(ep_in.bEndpointAddress, ep_in.wMaxPacketSize)
    print( data )

except usb.core.USBError as e:
    if e.args == ('Operation timed out',):
        print( "timeoutas" )
```

Завершающая часть скрипта:

``` python
dev.reset()
if needs_reattach:
    dev.attach_kernel_driver(0)
    print( "Reattached USB device to kernel driver")
```

Важно заметить, что использование методов **detach_kernel_driver**() и **attach_kernel_driver**() позволяет отключить работающий драйвер (CUPS), выполнить команду, а после выполнения команды, снова активировать CUPS-драйвер, продолжив работу как ни в чём не бывало.

## Решение проблем

Может потребоваться включение пользователя ОС (в примере - developer) в группу **lp**:

``` shell
sudo usermod -a -G lp developer
```

## Подключение других HID-устройств

Описанный выше пример работает и с другими типам HID-устройств, например, с DORS USB WDT.

Для того, чтобы найти нужное устройство можно использовать команду `lsusb`, которая перечисляет список подключенный USB HID устройств.

При подключении прибора DORS USB WDT в папке `/dev/usb` появилось новое устройство **hiddev0**. Подход описанный выше позволяет отправлять в WDT команду и получать ответ устройства, но это требует повышения привелений посредством использования команды **sudo**. Чтобы запускать скрипт можно было и без sudo, следует добавить для конкретного устройства правила использования. Для этого необходимо добавить в папке `/etc/udev/rules.d` файл с любым именем и расширением "rules", например "dors-wdt.rules". В этот файл нужно добавить всего одну строку и перезапустить систему:

```cfg
SUBSYSTEM=="usb", ATTR{idVendor}=="0483", ATTR{idProduct}=="5750", MODE="666
```

Приведённая выше слова указывает, что для USB-устройства с указанными VID и PID имеет все необходимые права доступа "666".

## LibUsbDotNet для проектов на C\#

Для использования HID-устройств в приложениях .NET может быть использована библиотека LibUsb.NET, которая является обёрткой над [libusb](https://github.com/libusb/libusb). Проект развивается на [площадке GitHub](https://github.com/LibUsbDotNet/LibUsbDotNet). В январе 2023 года последней версией была 3.0.0-alpha. Примеры доступны вместе с реализацией библиотеки.

На NuGet.org доступна [сборка от 25.09.2018](https://www.nuget.org/packages/LibUsbDotNet/) версии 2.2.29. Примеры использования LibUsbDotNet доступны [здесь](https://libusbdotnet.sourceforge.net/V2/html/acc0426e-9f5e-43a7-9c1d-841cdef2c663.htm)

Следует заметить, что примеры кода для 2.2.29 и 3.0.0-alpha не совместимы между собой, хотя имеют схожую структуру. Так, например, в третьей версии используется класс UsbContext, а в версии 2.2.29 - LibUsbRegistry.

Так же замечу, что первый опыт использования LibUsbDotNet 2.2.29, в целом, негативный. Взаимодействовать с DORS USB WDT удалось только после того, как был отключен драйвера usbhid с sudo:

```shell
sudo rmmod usbhid
```

Кроме этого, размер read-буфера пришлось установить в фиксированное значение - 32 байта. Если размер буфера будет другой, то мы получим либо ошибку _Overflow_ (данных больше, чем ожидали), либо _Timeout_ (ожидаемое количество данных не получено). Выполняются какие-то внутренние инициализационные процессы и повторный запуск приложения может занимать до 10 секунд. Надёжность решение низкая. В API отсутствует множество функций, доступных в оригинальном libusb, например, отсутствует возможность выполнить _Reset_ устройства.

Также было зафиксировано, что при обмене данными с DORS USB WDT первые две команды (GetStatus) не выполняются, но все последующие подаваемые команды (после, приблизительно, 200 мс) успешно выполняются.

Также был выполнен замер времени исполнения различных методов библиотеки посредством Stopwatch:

``` csharp
var sw = new Stopwatch();

sw.Start();
var zstart = sw.ElapsedTicks;

// Находим устройство по Pid и Vid
WdtUsbDevice = UsbDevice.OpenUsbDevice(DorsWdtUsbFinder);

var zdelta = sw.ElapsedTicks - zstart;
sw.Stop();
Console.WriteLine("UsbDevice.OpenUsbDevice():\t{0} ms", (zdelta * 1000.0) / Stopwatch.Frequency);
```

Было выявлено, что злодеями являются OpenUsbDevice() и SetConfiguration(), которые в среднем выполняются по 8 и 2 секунды соответственно.

### LibUsbDotNet 3.0.0-alpha

Далее была выполнена попытка использовать библиотеку LibUsbDotNet версии 3.0.0-alpha, которая пока не доступна через nuget.org, но её исходники можно скачать с [GitHub](https://github.com/LibUsbDotNet/LibUsbDotNet).

Использовался следующий базовый пример кода:

``` csharp
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace ConsoleTestWDT
{
    internal class Program
    {
        private const int ProductId = 0x5750;
        private const int VendorId = 0x0483;

        static void Main(string[] args)
        {
            using (var context = new UsbContext()) {
                // Получить полный список подключенных устройств
                var usbDeviceCollection = context.List();

                // Выбрать нужное нам HID-устройство по PID и VID
                var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == ProductId && d.VendorId == VendorId);

                // Открываем устройство для работы
                selectedDevice.Open();

                // Получаем интерфейс из первой конфигурации устройства
                selectedDevice.ClaimInterface(selectedDevice.Configs[0].Interfaces[0].Number);

                // Получаем Endpoints для записи и чтения данных
                var writeEndpoint = selectedDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
                var readEnpoint = selectedDevice.OpenEndpointReader(ReadEndpointID.Ep01);

                // Команда запроса состояния состоит только из одного байта
                var cmdGetStatus = new byte[] { 0x0B };
                writeEndpoint.Write(cmdGetStatus, 1000, out var bytesWritten);

                var readBuffer = new byte[32];
                readEnpoint.Read(readBuffer, 200, out var readBytes);
            }
        }
    }
}
```

В примере есть недостатки - если прибор не подключен, то selectedDevice будет равен null и приложение сгенерирует исключение.

Известные особенности версии 3.0.0-alpha:

- Тайм-ауты игнорируются, а их реальные значения - одна секунда
- Самая длительная операция selectedDevice.Configs[0], в среднем занимает 300 мс, но может выполняться и 900+ мс (но редко)
- размер буфера должен быть 32 байта не только для чтения, но и для записи. Т.е. ключевой код должен выглядеть так:

```csharp
var cmdGetStatus = new byte[32];
cmdGetStatus[0] = 0x0B;
writeEndpoint.Write(cmdGetStatus, 500, out var bytesWritten);
```

### Проблема отладки кода на машине с VMWare Player 16

В процессе разработки кода утилиты взаимодействия с HID-устройством были найдены различные ошибки при обмене данными. В результате поиска способов их устранения удалось выявить последовательность подключения прибора, которая позволила минимизировать (или даже исключить) вероятность возникновения подобных ошибок.

**Вариант №1:**

- USB WDT отключен
- Загружаем Ubuntu 22.04 в виртуальной машине
- Подключаем USB WDT
- Выполняем команду sudo rmmod usbhid
- Запускаем тестовую утилиту

**Вариант №2:**

- USB WDT подключен
- Загружаем Ubuntu 22.04 в виртуальной машине
- Через управляющую консоль VMware Player отключаем USB WDT
- Через управляющую консоль VMware Player включаем USB WDT
- Выполняем команду sudo rmmod usbhid
- Запускаем тестовую утилиту
