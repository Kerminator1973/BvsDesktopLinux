# Подключение HID-устройств

В настоящее время, всё чаще и чаще подключение внешних устройств осуществляется через USB-соединение. Однако, существуют три различных подхода для взаимодействия с такими приборами:

- полноценное использование стандартов USB (LibUSB)
- взаимодействие с HID-устройством (HID API)
- тунеллирование COM-трафика через USB

HID - это подмножество USB, пригодное для обмена данными с устройствами, для которых не нужна высокая скорость обмена. Например, для клавиатур и мышек. Этот вариант подключения является одним из наиболее простых. Фактическое ограничение по скорости составлять 12 мегабод, т.е. 1.5 мегабайта в секунду.

Тунеллирование COM-трафика через USB применяется чаще всего в ситуациях, когда разработчики внешнего устройства не могут, или не хотят полноценно поддерживать USB-протокол. В этом случае, они подключают к UART микроконтроллера специальную микросхему (мост). Пример моста - CP210x USB to UART Bridge от Silicon Labs. В этом случае, в операционной системе должны быть установлены драйвера для моста и сконфигурировано подключение через виртуальный COM-порт.

Для полноценного использования USB-стандартов чаще всего используется библиотека LibUsb. В различных языках программирования применяются библиотеки wrapper-ы для этой библиотеки.

## Пример подключения в кода на Python

[Пример на Python](./printing.md) работает и с любыми типами HID-устройств, например, с **DORS USB WDT**.

Для того, чтобы найти нужное устройство можно использовать команду `lsusb`, которая перечисляет список подключенный USB HID устройств.

Получить подробную информацию можно командой `lsusb -v`. Например:

``` output
idVendor           0x0d9f Powercom Co., Ltd
idProduct          0x0004 
bcdDevice            0.02
iManufacturer           3 POWERCOM Co.,LTD
iProduct                1 HID UPS Battery
iSerial                 2 004-0D9F-000
```

При подключении прибора DORS USB WDT в папке `/dev/usb` появилось новое устройство **hiddev0**. Подход описанный выше позволяет отправлять в WDT команду и получать ответ устройства, но это требует повышения привелений посредством использования команды **sudo**. Чтобы запускать скрипт можно было и без sudo, следует добавить для конкретного устройства правила использования. Для этого необходимо добавить в папке `/etc/udev/rules.d` файл с любым именем и расширением "rules", например "dors-wdt.rules". В этот файл нужно добавить всего одну строку и перезапустить систему:

```cfg
SUBSYSTEM=="usb", ATTR{idVendor}=="0483", ATTR{idProduct}=="5750", MODE="666"
```

TODO: _необходимо проверить и убедиться в том, что можно использовать не только использовать ATTRS{}, но и ATTR{}_.

Приведённая выше команда указываем правило работы модуля: два знака равенства указывают на необходимость поиска модуля подсистемы **usb**, у которого идентификатор вендора равен 0x0483, а идентификатор продукта - 0x5750 и установки его свойства **MODE** в значение **666**, т.е. предоставляет как права на запись, так и на чтение (read/write).

## LibUsbDotNet для проектов на C\#

Для использования HID-устройств в приложениях .NET может быть использована библиотека LibUsb.NET, которая является обёрткой над [libusb](https://github.com/libusb/libusb). Проект развивается на [площадке GitHub](https://github.com/LibUsbDotNet/LibUsbDotNet).

Доступно несколько разных версий библиотеки, отличающихся набором функциональных возможностей:

- На NuGet.org доступна [сборка от 25.09.2018](https://www.nuget.org/packages/LibUsbDotNet/) версии 2.2.29. Примеры использования LibUsbDotNet доступны [здесь](https://libusbdotnet.sourceforge.net/V2/html/acc0426e-9f5e-43a7-9c1d-841cdef2c663.htm)
- Похоже, что ветка "v2" на GitHub является проектом сопровождения для версии 2.2.29. Она имеет статус "active" и содержит код, позволяющий выгружать драйвера в Linux. Модификация версии осуществляется, приблизительно, раз в три месяца, есть не выполненные pull requests. Результаты тестирования этой экспериментальной сборки: основной поток - работоспособен, в альтернативных потоках - случались зависания
- Ветка "main" на GitHub является работоспособной и имеет номер 3.0.0-alpha. Для обеспечения работы с USB-устройствами, с загруженными драйверами (под Linux) требуется вносить изменения в исходные тексты библиотеки. Версия не считается активной, но она модифицируется (приблизительно, раз в полгода) и у неё есть не выполненные pull requests

Следует заметить, что API и примеры кода для всех трёх версий не совместимы между собой, хотя и имеют очень похожую структуру. Так, например, в третьей версии используется класс UsbContext, а в версии 2.2.29 - LibUsbRegistry.

Информации от сообщества пользователей LibUsbDotNet не много, но можно предположить, что версию 3.0.0-alpha развивают qmfrederik и mcuee, включая изменения jaroban. Версию "v2" развивает mcuee, а основные контрибьюторы: MatCDev (libusbK) и GreedShadeZhang (поддержка .NET 6).

### Особенности версии 2.2.29

Так же замечу, что первый опыт использования LibUsbDotNet 2.2.29, в целом, негативный. Взаимодействовать с DORS USB WDT удалось только после того, как был отключен драйвер usbhid с эскалацией прав пользователя (sudo):

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

Было выявлено, что "злодеями" являются OpenUsbDevice() и SetConfiguration(), которые в среднем выполняются по 8 и 2 секунды соответственно.

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
- Размер буфера должен быть 32 байта (или 64 байта) не только для чтения, но и для записи. Т.е. ключевой код должен выглядеть так:

```csharp
var cmdGetStatus = new byte[32];
cmdGetStatus[0] = 0x0B;
writeEndpoint.Write(cmdGetStatus, 500, out var bytesWritten);
```

UPDATE February 2023: в действительности, если нам нужно отправить USB-устройству конкретное количество байт, то следует воспользоваться перегруженным методом с двумя дополнительными параметрами offset и count:

```csharp
var cmdGetStatus = new byte[64];
cmdGetStatus[0] = 0x1B;
cmdGetStatus[1] = 0x76;
cmdGetStatus[2] = 0x00;
writeEndpoint.Write(cmdGetStatus, 0, 3, 500, out var bytesWritten);
```

Особенность API состоит в том, что HID-устройства являются блочными, т.е. единицей передачи информации является блок (32, или 64 байта), а не байт. Соответственно, в первом параметре передаётся буфер, а затем окно в буфере, в котором находятся данные (offset и count). Буфер может быть переиспользован при выполнении других запросов.

### Структура проекта LibUsbDotNet

Основываясь на изучении исходных текстов, можно сделать заключение, что библиотека развивалась Travis Lee Robinson с 2006 по 2010 год, а затем, с 2011 по 2018 год, группой разработчиков, которые называют себя "LibUsbDotNet contributors". По каким-то причинам, следы активной работы Travis Robinson заканчиваются в 2012 году. В 2014 году на его имя был куплен сертификат для подписи других проектов автора, но дальнейшие следы утеряны.

Подкаталог Docs содержит автоматически сгенерированные файлы с расширением AML (Microsoft Assistance Markup Language File). Основываясь на содержимом файлов, источник информации для автоматической обработки - исходные тексты. Используется подход близкий к [DoxyGen](https://www.doxygen.nl/). Release Notes велся только до версии 2.2.7.

В коде есть папка BenchmarkCon, что предполагает, что автор кода осуществлял анализ производительности кода. По ссылкам в документации, похоже, что использовался специализированный инструмент - [Travis Robinson libUsbK](https://github.com/me21/usb-travis/tree/master/libusbK).

В папке "Examples" есть пять примеров приложений. Похоже, что в папке "MonoLibUsb" находятся примеры для сборки с использованием проекта [Mono](https://www.mono-project.com/). Mono - это проект от корпорации Xamarin, запущенной в 2004 году.

Часть исходных текстов проекта создаётся автоматически - для этого используется инструмент, исходники которого находятся в папке "LibUsbDotNet.Generator". Обрабатывается заголовочный файл библиотеки "libusb-1.0/libusb.h", на основании которого создаётся wrapper для библиотеки в папке "./LibUsbDotNet/Generated". Генерируются как структуры, так и обёртки для native-методов:

``` csharp
[DllImport(LibUsbNativeLibrary, EntryPoint = "libusb_init")]
public static extern Error Init(ref IntPtr ctx);

[DllImport(LibUsbNativeLibrary, EntryPoint = "libusb_exit")]
public static extern void Exit(IntPtr ctx);
```

Разработанные вручную классы в папке "\src\LibUsbDotNet" используют автоматически сгенерированные структуры и методы, предоставляя удобный, высокоуровневый API.

Обмен данными реализован через метод Transfer, реализованный в файле "UsbEndpointBase.cs". В вызовах, связанных с передачей данных используется буфер обмена, смешение и количество передаваемых байт данных. Буфер данных называется _the caller-allocated buffer_. Возможно, что именно с этим связаны систематические проблемы с использованием LibUsbDotNet - используются упрощённые вызовы Read() и Write(), в которых указывается только буфер, но не указывается offset и длина передаваемых данных.

Папка "LibUsbDotNet.Tests" содержит всего четыре простых теста библиотеки и, на февраль 2023 года, бесполезна для обеспечения качества продукта.

Проект в папке "Test_DeviceNotify" иллюстрирует, как можно подписаться на получение системных событий **WM_DEVICECHANGE** (см.: USB DEVICEINTERFACE, PORT, и VOLUME).

Утилита "Test_Info" выводит список подключенных USB-устройств, используя LibUsbDotNet. Эта утилита похожа на утилиту **lsusb**, используемую в ОС Linux.

Исходя из предоставленной структуры проекта, в проекты использующие LibUsbDotNet достаточно скопировать только подпапку "\src\LibUsbDotNet" репозитария.

### Работа с термопринтером, с установленным CUPS-драйвером

В случае, если в Linux установлен CUPS-драйвер для принтера, можно использовать бибилиотеку LibUsbDotNet для получения состояния принтера, но это потребует внести изменения в исходный код библиотеки. Изменить пришлось метод Open в классе UsbDevice:

``` csharp
namespace LibUsbDotNet.LibUsb
{
    public partial class UsbDevice
    {
        public void Open()
        {
            this.OpenNative().ThrowOnError();

            var error = NativeMethods.SetAutoDetachKernelDriver(this.DeviceHandle, 1);
            // Результат error пока не используеся
        }
```

Расширяюшие изменения базовой библиотеки (не требующие изменения кода уже существующих методов), следует добавить новый метод SetAutoDetachKernelDriver(): прототип в **IUsbDevice**, а также в реализацию **UsbDevice**:

``` csharp
/// <summary>
/// Sets the auto-detach kernel driver mode for the device.
/// </summary>
/// <param name="enable">The true means that the kernel driver has to be detached</param>
/// <returns>True on success.</returns>
bool SetAutoDetachKernelDriver(bool enable);
```

``` csharp
/// <summary>
/// Sets the auto-detach kernel driver mode.
/// </summary>
public bool SetAutoDetachKernelDriver(bool enable)
{
    return NativeMethods.SetAutoDetachKernelDriver(
        this.DeviceHandle, enable ? 1 : 0) == Error.Success;
}
```

Соответственно код получения состояния принтера Xiamen Cashino **CSN-A1K-U** выглядит так:

``` csharp
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace ConsoleTestWDT
{
    internal class Program
    {
        private const int VendorId = 0x0fe6;
        private const int ProductId = 0x811e;

        static void Main(string[] args)
        {
            using (var _context = new UsbContext())
            {
                var usbDeviceCollection = _context.List();
                var device = usbDeviceCollection.FirstOrDefault(d => 
                    d.ProductId == ProductId && d.VendorId == VendorId);
                if (null != device)
                {
                    // Открываем соединение с USB HID
                    device.Open();

                    // Устанавливаем режим автоматической выгрузки CUPS-драйвера
                    device.SetAutoDetachKernelDriver(true);

                    // Настройка интерфейса
                    device.ClaimInterface(device.Configs[0].Interfaces[0].Number);

                    var writeEndpoint = device?.OpenEndpointWriter(WriteEndpointID.Ep01);
                    var readEnpoint = device?.OpenEndpointReader(ReadEndpointID.Ep01);

                    if (null != writeEndpoint && null != readEnpoint) {

                        var cmdGetStatus = new byte[64];    // Команда запроса состояния принтера
                        cmdGetStatus[0] = 0x1B;
                        cmdGetStatus[1] = 0x76;
                        cmdGetStatus[2] = 0x00;
                        writeEndpoint.Write(cmdGetStatus, 2000, out var bytesWritten);
                        if (0 != bytesWritten) {

                            var readBuffer = new byte[64];
                            var error = readEnpoint.Read(readBuffer, 500, out var readBytes);

                            if (readBytes > 0) {
                                string r = BitConverter.ToString(readBuffer);                    
                                Console.WriteLine(r);
                            }
                        }
                    }
                }
            }
        }
    }
}
```

Экспериментально установлено, что для получения состояния конкретного термопринтера, тайм-аут для записи данных необходимо использовать не 500 мс, а две секунды.

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

Классы USB-устройств:

**HID (Human Interface Device)** _generally requires no Drivers to be installed by the User. It is limited to 64 Bytes/ms (~64K/second) per endpoint used. It is guaranteed to get the timeslot because it uses INT transfers._ Код класса: 3, обычно это: Клавиатура, мышь, джойстик

**CDC (Comunication Device Class)** _requires drivers (INF file) to be installed and then simulates a serial port. It uses Bulk transfers so theoretically can have good bandwidth but is NOT guaranteed. There are also other code and packet overheads involved._ Код класса: 2, Модем, сетевая карта, COM-порт

### Работа приложений под Microsoft Windows

Обе библиотеки являются кросс-платформенными и могут работать как под Linux, так и под Windows. Для их работы под Windows, необходимо скопировать в подкаталог с приложением Avalonia файл **libusb-1.0.dll** для использования LibUsbDotNet, или файл **hidapi.dll** для использования HidApi.NET.
