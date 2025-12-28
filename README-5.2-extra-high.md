### DiagnosticLink / Drumroll — обзор проекта (после декомпиляции)

Этот каталог выглядит как **распакованная/скопированная папка установленного Windows-приложения** (много `.dll/.exe`) + **две папки с декомпилированным C# кодом**: `source-code1/` и `source-code2/`.
По метаданным в декомпиляции: **Drumroll 8.19.5842.0**, целевой рантайм **.NET Framework 4.6.1** (в `source-code2` также указан `PlatformTarget=x64`).

Важный момент: **декомпилированный код покрывает в основном WinForms-обвязку (UI)**. Реальная диагностика (протоколы, работа с ECU/ACM, чтение параметров, тесты/сервисы, программирование) почти наверняка реализована в **внешних сборках** (`SAPI.dll`, `DataHub.dll`, `J2534Abstraction.dll`, `McdAbstraction.dll`, `Softing.Dts.dll`, и др.), которые в этом репозитории присутствуют как бинарники.

---

### Что лежит в корне проекта

- **`Drumroll.exe` / `Drumroll.exe.config`**: основное WinForms приложение + конфигурация (WCF endpoints, security extensions).
- **Много DLL**: логика диагностики, панели интерфейса, коммуникационные слои, инструменты, отчёты, лицензирование.
- **`source-code1/` и `source-code2/`**: декомпилированный C# код приложения `Drumroll.exe`.
- **`driver/`**: конфиги “CAESAR/toolkit” уровня драйвера/VCI (каналы, адаптеры, мониторинг, PassThru и т.п.).
- **`Detroit Diesel/SID/`**: PDU-API описания/настройки для устройства “SID” (ресурсы UDS/CAN, распиновка, mapping).
- **`addons/bosch/D-PDU API/`**: Bosch D-PDU API (ISO 22900-2 / MVCI) + конфиги (в т.ч. DoIP).
- **`addons/softing/`**: Softing компоненты/шаблоны + `SystemConfig.xml` (указывает путь в `C:\ProgramData\...`).
- **`gbf/`**: набор `.GBF` (похоже, бинарные файлы протоколов/описаний; как текст не читаются).
- **`CAESAR_MappingTable.pmf`**: текстовая таблица маппинга com-параметров протоколов (UDS/UDSDOIP и др.).

---

### `source-code1/` vs `source-code2/` (почему две папки)

Обе папки содержат **одинаковый набор (~37) C# файлов** с одинаковыми namespace’ами вида `DetroitDiesel.Windows.Forms.Diagnostics.Container.*`, но отличаются “упаковкой”:

- **`source-code1/`**
  - `.sln/.csproj` формата VS2010 (`ToolsVersion=4.0`) и пометка “Project was exported from assembly … Drumroll.exe”.
  - Пути файлов в стиле `Windows/Forms/Diagnostics/Container/*.cs`.

- **`source-code2/`**
  - SDK-style `Drumroll.csproj` (`Microsoft.NET.Sdk.WindowsDesktop`, `net461`, `UseWindowsForms=true`, `PlatformTarget=x64`).
  - Есть секция `<Reference Include="...">` на ключевые сборки (например `SAPI`, `DataHub`, `McdAbstraction` и т.п.).
  - Пути файлов в стиле `DetroitDiesel.Windows.Forms.Diagnostics.Container/*.cs`.

Практически: **читать можно любую**, но `source-code2/` удобнее как “нормализованный” layout.

---

### Высокоуровневая архитектура приложения

Упрощённо:

- **`Drumroll.exe`** (WinForms контейнер)
  - Инициализация (CrashHandler, проверка .NET, апгрейд, лицензия, подготовка SAPI)
  - Запуск **`MainForm`**
    - Внутри основная область — **`TabbedView`**
      - `TabbedView` динамически подгружает панели (views) из DLL:
        - `EcuInfo.dll`, `EcuStatus.dll`, `Troubleshooting.dll`, `Instruments.dll`, `Parameters.dll`, `Reprogramming.dll`
      - Меню “склеивается” через `MenuProxy` (если он объявлен у панели).
    - Диалог подключения к устройствам/ECU: **`ConnectionDialog`**
      - Показывает список ECU из `SapiManager.GlobalInstance.Sapi.Ecus`
      - Выбор “ресурса подключения” (`ConnectionResource`) и открытие канала через `SapiManager.OpenConnection(...)`

Ключевой вывод: **UI управляет выбором и настройками**, а “мозги” диагностики и общения с ECU — в `SapiManager/SapiLayer1` и других DLL.

---

### Поток запуска (Startup)

Точка входа: `source-code1/Windows/Forms/Diagnostics/Container/Program.cs`.

Что делает `Program.Main()` / `RunApplication()` (важные шаги):

- Инициализирует `CrashHandler`, выставляет текущую папку на папку exe.
- Может выполнять “миграции”/cleanup:
  - удаление legacy registry key: `Software\PassThruSupport\Pi Technology\...`
- Проверка необходимости “force upgrade”.
- Загружает настройки логирования (`TraceLogManager`) и запускает trace.
- Готовит “серверные данные” (`ServerDataManager`), делает информацию для crash report.
- Показ SplashScreen.
- **Подготовка коммуникаций**:
  - `SapiManager.GlobalInstance.ResetSapi()`
  - загрузка настроек `SapiManager.GlobalInstance.LoadSettings(...)`
- Проверка лицензии/регистрации (см. `ServerRegistrationDialog`).
- Создаёт `MainForm` и запускает `Application.Run(mainForm)`.

Отдельно: есть логика `RunSidConfigure()`:

- Читает реестр: `Software\PassThruSupport\Detroit Diesel\Devices\SID` → `FunctionLibrary`
- Загружает dll и вызывает экспорт `Configure` (через `LoadLibrary/GetProcAddress`)

Это важная подсказка: **SID/PassThru устройство конфигурируется отдельно** и участвует в стеке связи.

---

### Подключение к ECU: что именно “видно” в декомпилированном коде

#### 1) Выбор железа / авто-подключения / Roll-Call

`source-code1/Windows/Forms/Diagnostics/Container/ConnectionOptionsPanel.cs`:

- **Hardware Types**: список берётся из `SapiManager.GlobalInstance.HardwareTypes`, выбранный пишется через `sapi.SetHardwareType(...)`.
- **RollCall / Monitoring**:
  - `SapiManager.GlobalInstance.RollCallEnabled`
  - `SapiManager.GlobalInstance.MonitoringEnabled`
  - опция **DoIP roll-call** (`DoIPRollCallEnabled`) видна, если `ApplicationInformation.CanRollCallDoIP`.
- **MCD**:
  - переключатель `UseMcd`, `McdAvailable` (похоже, отдельный слой/адаптер, связанный с DoIP).
- **Auto-baud**:
  - `AllowAutoBaud` при наличии capability.
- **Auto-connect per ECU identifier**:
  - UI строит список идентификаторов ECU (например `UDS-61`, `UDS-33` и т.п.) и включает/выключает автоподключение через `sapi.SetAutoConnect(identifier, ...)`.

#### 2) Выбор ECU и “ресурса подключения”

`source-code1/Windows/Forms/Diagnostics/Container/ConnectionDialog.cs`:

- Показывает список ECU (фильтрует roll-call/virtual/offline).
- Для выбранного ECU показывает `ConnectionResource` из `ecu.GetConnectionResources()`.
- При нажатии Connect:
  - сохраняет выбранный ресурс в settings
  - вызывает **`SapiManager.OpenConnection(selectedResource, channelOptions)`**

`ChannelOptions` включает опции вроде:
- старт коммуникации (`ExecuteStartComm`)
- циклические сервисы (`CyclicRead`)
- авто-исполнение сконфигурированных сервисов

#### 3) Отображение статуса по протоколам (J1708/J1939/DoIP)

`source-code1/Windows/Forms/Diagnostics/Container/ConnectionState.cs` + `TabbedView.cs`:

- `ConnectionState` привязан к `Protocol`:
  - J1708: `71432`
  - J1939: `71993`
  - DoIP: `13400`
- Берёт `IRollCall` менеджер: `ChannelCollection.GetManager(protocol)`.
- Рисует “кружки” статуса и (опционально) bus load.

#### 4) Bus Monitor (CAN1/CAN2/ETHERNET)

`source-code1/Windows/Forms/Diagnostics/Container/BusMonitorForm.cs` + `BusMonitorSettingsForm.cs`:

- `BusMonitorForm` умеет стартовать монитор по физическому линку: **`CAN1`, `CAN2`, `ETHERNET`**.
- Ресурсы мониторинга приходят из `BusMonitorCollection.GetAvailableMonitoringResources()`.
- Настройки позволяют привязать “физический линк” к конкретному `ConnectionResource`.
- Сохранение:
  - CAN: `.trc`
  - Ethernet: `.pcapng`

---

### ACM / AdBlue / DEF: что удалось найти в этом проекте

В исходниках **нет** явных строк “AdBlue/DEF”, но есть прямое соответствие:

- **`ConnectionOptionsPanel_IdentifierUDS_61` → `Aftertreatment Control Module`**
  - встречается в:
    - `source-code1/Windows/Forms/Diagnostics/Container/Properties/Resources.resx`
    - `source-code2/DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.resx`

Это сильная подсказка, что **ACM представлен как UDS-устройство с идентификатором/адресом `61`** (в UI это отображается как “Aftertreatment Control Module”).

Что важно:

- В декомпилированных `source-code*` **нет логики “как читать параметры ACM / как запускать сервисы ACM”** — UI лишь выбирает ECU и вызывает `SapiManager.OpenConnection(...)`.
- Реальная работа с ACM почти наверняка “внутри”:
  - диагностических данных (CBF/GBF/прочие пакеты),
  - и/или DLL (`SAPI.dll`, `DataHub.dll`, `ECUInfo.dll`, `Parameters.dll`, `Reprogramming.dll` и т.д.).
- В корне репозитория `.cbf` действительно не лежат, **но в приложенном `ProgramData/` они есть**: `ProgramData/Application Data/Caesar/cbf/` (в т.ч. `ACM02T*`, `ACM21T*`, `ACM301T*`). Это локальные “описания ECU/сервисов”, которые использует стек `SAPI/CAESAR`.
- В `ProgramData/Application Data/Mappings.xml` есть явные маппинги по aftertreatment: **SCR/DEF температуры/давления, DPF regen state и т.д.** (таргеты вида `ACM21T.DT_AS018_SCR_Inlet_Temperature`, `ACM301T.DT_AS014_DEF_Pressure` и др.) — то есть чтение данных по AdBlue/ATS уже предусмотрено.
- В `ProgramData/Application Data/parameterWarnings.xml` есть предупреждения по параметру **Auto-Elevate** и пометка, что в EPA10 он “переехал” из MCM в ACM (это коррелирует с сервисной документацией DTNA).

---

### Что показывает ваш `ProgramData/` про транспорт (NEXIQ USB-Link)

У вас адаптер **NEXIQ USB-Link (RP1210)**. По `ProgramData/Technical Support/Trace Files/*.DrumrollTrace` видно:

- Приложение поднимает связку **`SID (SAE->RP1210)`** (то есть PassThru-устройство “SID”, работающее поверх RP1210).
- Оно пытается включить DoIP-детект (по `SapiConfig.xml`), но пишет: **`DoIP - RP1210 ethernet activation not supported ...`**.

Практический вывод: **в вашей текущей связке (RP1210/USB-Link) DoIP не используется**; доступ к ACM идёт **по CAN (J1939 / UDS-over-CAN/ISO-TP)** через PassThru/SID.

При этом стек DoIP в установке **присутствует** (MCD/Softing + Bosch D-PDU API):

- `ProgramData/Application Data/SapiConfig.xml` содержит `McdRootDescriptionFile` и `McdEthernetDetectionString` (DoIP-Collection).
- `ProgramData/Application Data/McdSystem/pdu_api_root.xml` содержит root для Bosch D-PDU API + DTNA SIDPDU.

Но чтобы реально работать по DoIP, обычно нужен **Ethernet/DoIP интерфейс**, а не RP1210 USB-Link.

---

### Как самому быстро проверить: CAN или DoIP

- **В DiagnosticLink**:
  - В `Bus Monitor` можно выбрать `CAN1/CAN2/ETHERNET`, а в свойствах соединения видно `PhysicalInterfaceLink` (CAN1/CAN2 vs ETHERNET).
  - В статусе протоколов (индикаторы J1708/J1939/DoIP) DoIP будет активным только если реально поднят DoIP канал.
- **По логам**:
  - Откройте `ProgramData/Technical Support/Trace Files/*.DrumrollTrace` и ищите `DoIP`, `ETHERNET`, `RP1210`. Если есть сообщение вида `RP1210 ethernet activation not supported` — значит DoIP через ваш адаптер не поднимется.
- **По списку ресурсов подключения** (в ConnectionDialog):
  - Ресурсы вида `SID_UDS_ON_CAN1_HS` / `SID_UDS_ON_CAN2_HS` → UDS over CAN.
  - Ресурсы с упоминанием `DoIP`/`ISO 13400` → DoIP.

---

### Полезная инфа из документа DTNA (D24R5) про перепрошивку ACM

Документ: [DTNA Recall Campaign D24R5 — “DD13 CARB 24 ACM Software Reprogramming”](https://static.nhtsa.gov/odi/tsbs/2024/MC-11008489-0001.pdf).

Что там важно для вас (и для понимания логики DiagnosticLink):

- **Перепрошивка ACM делается через DiagnosticLink с RP1210B-адаптером** (подходит к вашему NEXIQ USB-Link сценарию).
- Перед программированием рекомендуют:
  - подключить зарядник к 12В,
  - проверить VIN в модулях,
  - при необходимости запустить в меню Actions пункт **“Check VIN Synchronization”**.
- Программирование идёт через вкладку **Program Device**:
  - **Download data from server** (скачать пакеты/данные с сервера),
  - выбрать ACM (в примере **ACM301T**) и сделать **Update Device Software**,
  - после завершения — циклы key OFF/ON и повторное подключение.
- После ремонта рекомендуют нажать **Connect to Server** (обновить статус на сервере) и отдельно упоминают **Auto Elevate** (что совпадает с вашими `parameterWarnings.xml`).

Этот документ подтверждает: для записи/прошивки ACM используется именно стек DiagnosticLink + серверные данные, а не “локальный самописный протокол”.

### Как происходит подключение “на проводах” (по конфигам PDU/PassThru)

Здесь есть несколько уровней:

#### 1) SID (Detroit Diesel) через PDU-API (MVCI / ISO 22900-2)

Папка: `Detroit Diesel/SID/`

- `MDF_DTNA_SID.xml` определяет ресурсы:
  - `SID_UDS_ON_CAN1_HS` (UDS over CAN1)
  - `SID_UDS_ON_CAN2_HS` (UDS over CAN2)
  - `SID_RAWCAN_ON_CAN1_HS`, `SID_RAWCAN_ON_CAN2_HS`
- Там явно указано:
  - Bustype: `ISO_11898_2_DWCAN`
  - Protocol: `ISO_15765_3_on_ISO_15765_2` (ISO-TP / UDS transport поверх CAN)
- `CDF_DTNA_SID.xml` описывает распиновку J1939-кабеля SID (CAN1/CAN2, питание, GND).
- `mapping.ini` показывает, какие RP1210/адаптеры поддерживаются (NEXIQ, Noregon DLA+, USB-Link и т.д.) и какие “каналы” назначены под CAN/J1939/J1708.

Практический вывод: **ACM (UDS_61) в такой схеме будет доступен как UDS ECU через ресурсы SID_UDS_ON_CAN1/2**, если он сидит на соответствующей CAN-шине.

#### 2) Bosch D-PDU API (включая DoIP)

Папка: `addons/bosch/D-PDU API/`

- `pdu_api_root_x64.xml` — root, ссылается на `PDUAPI_Loader.dll`, `MDF_Bosch.xml`, `CDF_Bosch.xml`.
- `MDF_Bosch.xml` содержит модульные типы для **ISO 13400 DoIP** (например `MVCI_ISO_13400_DoIP_*`).
- `D_PDU_API_CFG.xml` содержит настройки логирования и флаги DoIP поддержки.

#### 3) CAESAR/toolkit конфиги драйвера

- `driver/SLAVE.INI`:
  - раздел `PartJ` содержит `PassThruVersion="02.02"` и `DeviceName="SID (SAE->RP1210)"`
  - описаны сетевые VCI, кабели, мониторинг, параметры TCP/UDP и т.п.
- `driver/Hardware.ini` / `driver/Mapping.plf`:
  - описания аппаратных “Part*” и кабельных пинов/мэппинга CAN/K-Line/J1708 и др.
- `CAESAR_MappingTable.pmf`:
  - маппинги com-параметров для `UDS` и `UDSDOIP` (P2/P3, block size, stmin, логические адреса и т.п.)

---

### Где копать дальше, если цель — доработки ACM (Aftertreatment/AdBlue)

Если задача — именно “доработать логику ACM” (читать/писать параметры, тесты, регенерация, сервисные процедуры, очистка ошибок, программирование), то `source-code*` почти наверняка недостаточно.

Что имеет смысл декомпилировать следующим шагом (по именам и тому, как их грузит UI):

- **`SAPI.dll` / `SapiLayer1`**: центральный слой “открыть канал / сервисы / лог-файлы”.
- **`DataHub.dll`**: диагностические данные/каталоги/описания.
- **`J2534Abstraction.dll`**, **`McdAbstraction.dll`**, **`Softing.Dts.dll`**, **`PDUAPI_DTNA_SID.DLL`**: транспорт/адаптеры.
- **`Parameters.dll`**, **`ServiceRoutineTabs.dll`**, **`FaultCodeTabs.dll`**, **`ECUInfo.dll`**, **`Reprogramming.dll`**: панели и, вероятно, прикладные сервисы поверх SAPI.

Полезный “якорь” для поиска ACM:

- строка/ключ **`UDS_61`** и текст **`Aftertreatment Control Module`**
- плюс поиск по `Aftertreatment`, `SCR`, `DEF`, `urea`, `ACM`, `UDS-61`
