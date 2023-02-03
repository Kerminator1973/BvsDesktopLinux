# Подключение HID-устройств

[Пример на Python](./printing.md) работает и с любыми типами HID-устройств, например, с **DORS USB WDT**.

Для того, чтобы найти нужное устройство можно использовать команду `lsusb`, которая перечисляет список подключенный USB HID устройств.

При подключении прибора DORS USB WDT в папке `/dev/usb` появилось новое устройство **hiddev0**. Подход описанный выше позволяет отправлять в WDT команду и получать ответ устройства, но это требует повышения привелений посредством использования команды **sudo**. Чтобы запускать скрипт можно было и без sudo, следует добавить для конкретного устройства правила использования. Для этого необходимо добавить в папке `/etc/udev/rules.d` файл с любым именем и расширением "rules", например "dors-wdt.rules". В этот файл нужно добавить всего одну строку и перезапустить систему:

```cfg
SUBSYSTEM=="usb", ATTR{idVendor}=="0483", ATTR{idProduct}=="5750", MODE="666"
```

Приведённая выше команда указываем правило работы модуля: два знака равенства указывают на необходимость поиска модуля подсистемы **usb**, у которого идентификатор вендора равен 0x0483, а идентификатор продукта - 0x5750 и установки его свойства **MODE** в значение **666**, т.е. предоставляет как права на запись, так и на чтение (read/write).

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

                // Команда запроса состояния состоит только из одного байта. Однако, чтобы
                // код работал, необходимо передавать 32 байта (размер блока)
                var cmdGetStatus = new byte[32];
                cmdGetStatus[0] = 0x0B;
                writeEndpoint.Write(cmdGetStatus, 1000, out var bytesWritten);

                var readBuffer = new byte[32];
                readEnpoint.Read(readBuffer, 200, out var readBytes);
            }
        }
    }
}
```

В примере есть недостатки - если прибор не подключен, то selectedDevice будет равен null и приложение сгенерирует исключение.

Выявленные нами особенности версии 3.0.0-alpha:

- Тайм-ауты игнорируются, а их реальные значения - одна секунда
- Самая длительная операция selectedDevice.Configs[0], в среднем занимает 300 мс, но может выполняться и 900+ мс (но редко)
- Размер буфера должен быть 32 байта не только для чтения, но и для записи. Т.е. ключевой код должен выглядеть так:

```csharp
var cmdGetStatus = new byte[32];
cmdGetStatus[0] = 0x0B;
writeEndpoint.Write(cmdGetStatus, 500, out var bytesWritten);
```

### Дополнительные материалы для изучения (USB HID)

Вместе с тем, сами [разработчики LibUsb](https://github.com/libusb/libusb/wiki/FAQ#user-content-Does_libusb_support_USB_HID_devices) для работы с HID рекомендуют использовать библиотеку [HidApi.Net](https://github.com/badcel/HidApi.Net).

Предварительно требуется установить библиотеку libhidapi-hidraw0:

``` shell
sudo apt install libhidapi-hidraw0
```

А также добавить в rules настройки доступа для модулей HID-устройств (например, в файл `/etc/udev/rules.d/90-hid.rules`) строку:

``` config
KERNEL=="hidraw*", ATTRS{idVendor}=="0483", ATTRS{idProduct}=="5750", MODE="666"
```

Перегрузить настройки доступа можно командой:

``` shell
sudo udevadm control --reload-rules && sudo udevadm trigger
```

В проект сборки следует добавить библиотеку [HidApi.Net](https://www.nuget.org/packages/HidApi.Net), используя команду:

``` shell
dotnet add package HidApi.Net --version 0.2.1
```

Пример перечисления подключенных HID:

``` csharp
using HidApi;

foreach (var deviceInfo in Hid.Enumerate())
{
    using var device = deviceInfo.ConnectToDevice();
    Console.WriteLine($"VID: {deviceInfo.VendorId} PID: {deviceInfo.ProductId} : {device.GetManufacturer()}");
    
}
Hid.Exit(); //Call at the end of your program
```

Пример обмена данными через hidraw:

``` csharp
var device1 = new Device(0x0483, 0x5750); //Fill vendor id and product id
Console.WriteLine($"{device1.GetManufacturer()}");

byte[] w = new byte[] { 0x00, 0x0B };
device1.Write(w);
ReadOnlySpan<byte> ff = device1.ReadTimeout(65, 1000);

Hid.Exit(); //Call at the end of your program
```

Библиотека HidApi.Net существенно проще, чем LibUsbDotNet и работает через hidraw, а не через usb, что позволяет избежать необходимости выгружать модуль **usbhid** перед запуском приложения командой: `sudo rmmod usbhid`

### Работа приложений под Microsoft Windows

Обе библиотеки являются кросс-платформенными и могут работать как под Linux, так и под Windows. Для их работы под Windows, необходимо скопировать в подкаталог с приложением Avalonia файл libusb-1.0.dll для использования LibUsbDotNet, или файл hidapi.dll для использования HidApi.NET.
