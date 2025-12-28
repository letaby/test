# DiagnosticLink (Drumroll) — обзор декомпилированных исходников

## Что за проект
- Windows Forms приложение диагностики Detroit Diesel/Drumroll v8.19.5842 (целится в .NET Framework 4.6.1, x64).
- Основная логика общения с ЭБУ инкапсулирована во внешних библиотеках (`SAPI.dll`, `Interfaces.dll`, `Common*.dll`, `Net.dll` и др.) — в исходниках в `source-code1/2` преимущественно UI-обвязка и управление настройками.
- Поддерживаются подключение к ЭБУ, захват/воспроизведение логов, монитор шины, сетевой тестер, лицензирование и регистрация.

## Структура репозитория
- Корень: множество бинарей (*.dll/*.exe), ресурсных файлов и две папки с декомпилированным кодом: `source-code1/`, `source-code2/`.
- `addons/, gbf/, Detroit Diesel/, driver/` и другие каталоги с данными и ресурсами, используемыми внешними библиотеками.

## source-code1 (классический проект)
- Проект `Drumroll.csproj` (WinExe, TargetFrameworkVersion v4.6.1, RootNamespace `DetroitDiesel`). Список компилируемых файлов — формы и панели UI.
- Ключевые элементы:
  - `Program.cs`: инициализация CrashHandler, проверка версии .NET, запуск `SidConfigure` (J2534 драйвер), проверка лицензии (`LicenseManager` + `ServerRegistrationDialog`), загрузка настроек `SapiManager`, показ SplashScreen, затем `MainForm`.
  - `ConnectionDialog.cs`: перечисляет доступные ЭБУ (`SapiManager.GlobalInstance.Sapi.Ecus`), фильтрует по категории Vehicle/Engine и семейству, показывает ресурсы подключения, варианты диагностических описаний и advanced-опции канала (автозапуск StartComm, циклическое чтение, авто‑выполнение конфигурированных сервисов). При выборе вызывает `sapi.OpenConnection(resource, channelOptions)`.
  - `ConnectionOptionsPanel.cs`: выбор аппаратного интерфейса (VCI/J2534), запуск `SidConfigure`, включение RollCall/Monitoring, DoIP auto-connect, AutoBaud, UseMcd, управление автоподключением к конкретным идентификаторам ЭБУ, сброс/перезапуск SAPI.
  - `BusMonitorForm.cs`: отдельное окно мониторинга шины (CAN1/CAN2/Ethernet), может запускаться в отдельном STA-потоке; поддерживает фильтры, паузу, сохранение/загрузку, статистику.
  - Другие формы: настройки сервера (`ServerOptionsPanel`), сетевой тестер (`Net/NetworkDebugger.cs`), лицензирование (`Settings/ServerRegistrationDialog.cs`), просмотр логов (`Common/LogFileInfoDialog.cs`), статус ЭБУ, опции отображения и печати, диалоги предупреждений, фильтров и т.п.
- Ресурсы (*.resx) содержат строки и иконки UI; в `Resources.resx` встречается строка `Aftertreatment Control Module` (идентификатор `ConnectionOptionsPanel_IdentifierUDS_61`).

## source-code2 (SDK-проект, те же формы)
- `Drumroll.csproj` в стиле SDK; цели: `net461`, `UseWindowsForms=true`, `PlatformTarget=x64`, `LangVersion 12`. Ссылки на внешние dll (`CommonCS`, `SAPI`, `Interfaces`, `Net`, `CrashHandler`, `Help`, `Extensions`, `DataHub`, `OpenIdConnect`, `Adr`, и др.).
- Кодовые файлы зеркалируют `source-code1`, но в более чистом виде (меньше артефактов декомпиляции). Названия пространств имён и классов совпадают.
- Расширенный набор *.resx, *.ico, *.png — соответствуют ресурсам UI контейнера диагностики.

## Поток запуска и подключение к ЭБУ
- Старт `Program.Main`: проверка обязательного обновления продукта, очистка устаревшего SID PassThru ключа, запуск SidConfigure (если нужно), применение локали, загрузка настроек/логов (`TraceLogManager`), инициализация `SapiManager` (коммуникации), проверка лицензии, затем `MainForm`.
- `MainForm` управляет вкладками диагностики, воспроизведением логов, подключением/отключением каналов, печатью и т.п. Кнопка Connect открывает `ConnectionDialog`.
- `ConnectionDialog`:
  - Список ЭБУ формируется из `SapiManager.GlobalInstance.Sapi.Ecus`, исключая roll-call и offline‑only; сортировка по приоритету.
  - Фильтр по категории Vehicle/Engine и ElectronicsFamily; опция «Show all» показывает «restricted» ресурсы (через другие ЭБУ).
  - Для выбранного ЭБУ — перечень `ConnectionResource` (VCI + протокол), выбор фиксированного DiagnosticVariant, advanced channel options (битовые флаги).
  - Канал открывается через `sapi.OpenConnection(resource, options)`; описание файла (CBF/SMR) выводится, есть подготовка к смене описания (обработчик пуст).
- `ConnectionOptionsPanel`: глобальные настройки подключения (выбор аппаратного типа, автоконнект на идентификаторы, RollCall/DoIP, AutoBaud, UseMcd). При изменении может вызывать `SapiManager.ResetSapi()` или `Suspend/ResumeAutoConnect`.

## Сеть, лицензия и обновления
- `ServerRegistrationDialog`: ввод регистрационного ключа, описание компьютера, вызов `ServerClient` для активации; поддержка деавторизации.
- `ServerOptionsPanel`: сервер/порт DL Broker, адреса LogOn/Techlane/Edex, флаги сохранения выгрузок, ручная разблокировка, авто‑загрузка материалов; деавторизация очищает зашифрованный файл лицензии.
- `NetworkDebugger`: проверяет сетевые настройки, OIDC/Siteminder аутентификацию, пингует узлы; выводит результат в HTML-панель, позволяет копировать отчёт.
- Обновление: `ApplicationForceUpgrade` (проверка), `ExtensionLibrary.UpdateExtensions` (обновление расширений/плагинов), `PrintHelper.Initialize` (зависит от IE11).

## Мониторинг, логи и вспомогательные функции
- Логи: `LogFileInfoDialog` читает `.DrumrollLog`, выводит сессии/инциденты/варианты и версии описаний; печать через `PrintHelper`.
- `MainForm` содержит управление воспроизведением логов (скорость, метки, загрузка/сохранение), фильтрацию, полноэкранный режим, связи с панели инструментов и статус-баром.
- Bus Monitor: просмотр трафика шины, сохранение/загрузка, фильтрация, статистика, пауза.
- Статусы ЭБУ: `EcuStatusView/Item` отображают состояние подключённых каналов.

## Что известно про ACM/AdBlue
- Явных упоминаний ACM/AdBlue в коде нет. Единственное текстовое совпадение — ресурс `Aftertreatment Control Module`, что косвенно подтверждает поддержку соответствующего блока через стандартный список ЭБУ.
- Список ЭБУ, варианты диагностики и параметры каналов берутся из `SapiManager.GlobalInstance.Sapi.Ecus`, `ConnectionResource`, `DiagnosisVariant` — их реализации и описания находятся в внешних библиотеках/файлах описаний (CBF/SMR) и данных (gbf/). Для целевых доработок по ACM/AdBlue нужно работать с этими источниками или с API `SapiLayer1`.

## Рекомендации для дальнейших доработок
- Если требуется добавить/изменить поддержку ACM/AdBlue:
  - Изучить доступные ECU в рантайме (`SapiManager.GlobalInstance.Sapi.Ecus`) и их `Identifier/Name/DiagnosisVariants`.
  - Проверить описания CBF/SMR в каталоге данных и соответствие идентификаторам (например, строке `Aftertreatment Control Module`).
  - При необходимости расширять UI в `ConnectionDialog`/`ConnectionOptionsPanel` (например, доп. фильтры или предустановленные варианты) и дорабатывать SAPI‑слой (внешние DLL/описания).
- Для сборки SDK‑проекта `source-code2` нужны внешние dll из корня; `source-code1` можно использовать как ориентир для ресурсов и старых msbuild настроек.
