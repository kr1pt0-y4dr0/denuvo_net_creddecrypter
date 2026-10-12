# Результаты анализа: семейство `DenuvoNet_launcher` (DNL_launcher)

**Объект исследования:** grey-market игровые лончеры `DenuvoNet_launcher` (a.k.a. `DNL_launcher`),
распространяемые через `games-launcher.top` / `denuvo.net` как «офлайн-активация» игр.
**Исследованные образцы:** `DenuvoNet_launcher.exe` (v1.3.3.0), `DenuvoNet_launcher.exe` (v1.3.3.98), сопутствующий `zteam_offliner.exe`.
**Дата:** 2026-10-10 (обновлено 2026-10-12).

---

## 1. Итоговый вердикт

`DenuvoNet_launcher` — **обфусцированный .NET (WPF) лончер** под названием `DenuvoNet_launcher` /
`DNL_launcher`. По ключу (ID) он получает **с сервера** учётные данные **чужого игрового аккаунта**
(Steam / Epic / Ubisoft / EA / Microsoft Store), расшифровывает пароль и **автоматически вводит его**
в клиент магазина через AutoHotkey, переводя Steam/Ubisoft в offline-режим.

Это **не классический инфостилер** (не ворует локальные пароли/куки/браузер пользователя), но
**PUA / Riskware уровня «credential abuse / shared-account launcher»**: принимает и расшифровывает
чужие пароли, глушит аналитические/античит-процессы, самообновляется по HTTP без подписи и отключает
проверку TLS.

**Классификация:** PUA / Riskware — gray-market shared-account game/Steam launcher (credential abuse).

**Признаки вредоносности/нежелательного ПО:**

- **Self-update**: скачивает `update.xml` и `DenuvoNet_launcher.zip` с `games-launcher.top` и выполняет
  автозамену/распаковку (подпись/хеш обновления не проверяются).
- **Работа с логинами Steam**: классы `SteamLoginsParser`, `SteamLoginsChanger`, `SteamLoginConfData`,
  `LoginData` (поля `AccountName`, `PersonaName`, `RememberPassword`, `AllowAutoLogin`, `MostRecent`,
  `steamloginusersPath`, файл `loginusers.vdf`) — чтение/подмена данных автологина Steam.
- **HWID-фингерпринтинг**: `FingerPrint` (cpu/base/video/mac id через WMI), `UserID`,
  `Product {act, login, hwid, pc}`.
- **Скачивание и запуск внешних файлов**: `ExtractZip`, `GetFileFromResource`, `AutoUpdate` (AutoUpdater.NET).
- **Kill-лист процессов**: метод `DoCheaterskiller` (убивает процессы по имени).
- **Интеграция с AutoHotkey**: `sharpAHK`, `Load_ahkString` / `Load_ahkFile`, ресурс `AhkAnimateGif`.
- **Обход проверки сертификатов TLS** в HTTP-колбэках.
- **Запись в реестр** (Tracing-ключи, `Direct3D\MostRecentApplication`).
- **Anti-analysis**: анти-отладка (`Debugger Detected`), анти-тампер (` is tampered.`),
  JIT-hook шифрование тел методов.

---

## 2. Идентификация файлов

### 2.1 `DenuvoNet_launcher.exe` (v1.3.3.0)

| Параметр | Значение |
|---|---|
| Размер | `8 926 720` байт |
| MD5 | `F887E2BEECCC3D6CB5EE3A7D7B7AA9C7` |
| SHA-1 | `0959311436AA82CCEBBF7E71578DB4532F1576B3` |
| SHA-256 | `F119196293DF44033A700D07A66B18F5561A1608E75170CA0EBFBFFD95621250` |
| Тип | PE32, .NET assembly (WPF), GUI |
| Machine | `0x14C` (i386 / 32-bit) |
| Подсистема | Windows GUI |
| Секции | `.text` (entropy **7.997**), `.rsrc`, `.reloc` |
| Импорты | только `mscoree.dll!_CorExeMain` (классический .NET-хост) |
| TimeDateStamp | `0xD3C35757` → **2082** (поддельная/битая метка) |
| CLR EntryPointToken | `0x0600008F` → **`DNL_launcher.App.Main`** (`Program.Main` — пустышка-`ret`) |

.NET-метаданные:

| Параметр | Значение |
|---|---|
| AssemblyName | `DenuvoNet_launcher` |
| AssemblyVersion / FileVersion | `1.3.3.0` |
| AssemblyTitle | `DNL_launcher_2023` |
| AssemblyProduct | `DNL_launcher` |
| AssemblyDescription | `Konstantin Yurchenko (vk.com/konstantin_yurch)` |
| AssemblyCompany | `https://games-launcher.top` |
| AssemblyCopyright | `games-launcher.top Copyright 2023` |
| TargetFramework | `net472` (.NET Framework 4.7.2) |
| InternalName (VERSIONINFO) | `DenuvoNet_launcher.exe` |
| Прочее | `[assembly: SuppressIldasm]`, `ComVisible(false)`, `NeutralResourcesLanguage("en")` |

Технологии/зависимости:

- WPF + WindowsForms, `AllowUnsafeBlocks=true`
- `MaterialDesignThemes.Wpf`, `AutoUpdater.NET` (авто-обновление), `sharpAHK` (AutoHotkey interop),
  `Newtonsoft.Json`
- `System.Net.Http`, `System.Management` (WMI), `System.IO.Compression.FileSystem`
- `Microsoft.Web.WebView2` `1.0.1418.22` (нативный `WebView2Loader.dll` для win-x86/x64/arm64)
- **Costura.Fody** (`Costura.AssemblyLoader`, `DenuvoNet_launcher_ProcessedByFody`) — упаковка зависимостей в exe
- `PresentationBuildTasks 4.0.0.0` (WPF BAML)

### 2.2 `DenuvoNet_launcher.exe` (v1.3.3.98)

| Параметр | Значение |
|---|---|
| Размер | `9 160 704` байт (дата файла 2026-02-10) |
| MD5 | `07D27376DFA48C9715F755BE41E78328` |
| SHA-1 | `38DE725905EB20570D6A91426E69460E2751D1C6` |
| SHA-256 | `6D10C70BE8809CBF2FDA3CBC955F04047EBA2131D6DA28EB6034786494FCB246` |
| AssemblyVersion | `1.3.3.98` |
| Протектор | .NET Reactor (x86, 3 секции, `.text` entropy 7.997) |

### 2.3 `zteam_offliner.exe` (сопутствующий)

| Параметр | Значение |
|---|---|
| Размер | `1 455 104` байт |
| MD5 | `51D1368AED3ECA449852B58B1B9B4E23` |
| SHA-1 | `0C987B746023CDEE420F6A5BD33D2A338C2FBE1A` |
| SHA-256 | `D204C7D85C4859958F5E030205648FC9D66A748F13A3C4B091B2E178506DA81F` |
| Тип | Native x64 GUI, собран **AutoHotkey v1.1.36.02** (Ahk2Exe), `TimeDateStamp 0x639071DC` |
| Runtime | не .NET |

---

## 3. Протектор и обфускация

**Протектор: .NET Reactor (подтверждено).** Применённые защиты:

1. **Шифрование тел методов через JIT-hook.** Ключевой класс: `dg3ypDAonQcOidMs0w.WP6RZJql8gZrNhVA9v`
   (~372 KB исходника, реальный IL).
   - Делегат `QZEOeHRUkDiNqCWT0p` = сигнатура `ICorJitCompiler::compileMethod`:
     `uint (IntPtr classthis, IntPtr comp, IntPtr info, uint flags, IntPtr nativeEntry, ref uint nativeSizeOfCode)`.
   - `prXoP4RuYp()` — установка JIT-хука (находит `clrjit.dll`, патчит vtable JIT; P/Invoke
     `VirtualAlloc` / `WriteProcessMemory` / `VirtualProtect` / `OpenProcess` / `CloseHandle`).
   - `iXLoKRSpAw()` — **анти-тампер**: читает файл сборки, считает SHA, сравнивает с зашитым значением;
     при несовпадении — строка **` is tampered.`**
   - `w65ov7siki()` — **анти-отладка**: бросает **`Debugger Detected`**.
2. **Шифрование строк.** Центральный декриптор: `WP6RZJql8gZrNhVA9v.L3hoFlcqP6(int)` (AES/Rijndael,
   «flattened» switch). Пример: `MainWindow.tmpdir = %AppData% + L3hoFlcqP6(2578)`.
   Вторичный 2-аргументный: `qcBC7nbplYPB6DW0yw.G9skPDgcXb(string, string)`.
   Класс `ZaCryptography`: `_iterations=1`, `_keySize=256`, `_hash=…`, `_salt=""`, `_vector=…`.
3. **Control-flow flattening** — тела методов превращены в switch-машины.
4. **Method proxies** — вспомогательные `static object <random>(object P_0, ...)`.
5. **Переименование типов/неймспейсов** в ~19-символьные случайные имена.
6. **`[MethodImpl(MethodImplOptions.NoInlining)]`** на всех методах; `[SuppressIldasm]`.
7. Каждый класс имеет `static cctor`, который дергает `WP6RZJql8gZrNhVA9v.prXoP4RuYp()`,
   `iXLoKRSpAw()`, `w65ov7siki()`.

**Точка входа CLR — `App.Main`:**

```csharp
public static void Main() {
    XliSbN2oAPxhVpUoRhK.luCoX8hlsIILm();   // инициализация рантайма протектора
    App app = new App(); app.InitializeComponent(); app.Run();
}
static App() {                                  // cctor App
    XliSbN2oAPxhVpUoRhK.luCoX8hlsIILm();
    WP6RZJql8gZrNhVA9v.prXoP4RuYp();            // JIT hook
    WP6RZJql8gZrNhVA9v.iXLoKRSpAw();           // anti-tamper
    WP6RZJql8gZrNhVA9v.iXLoKRSpAw();
}
```

Профиль снятия защит (для v1.3.3.0): `641 Methods decrypted`, `768 Strings decrypted`,
`Assembly resources decrypted`, `Anti tamper removed`, `Anti debugger removed`,
`25 Embedded assemblies dumped (Costura.Fody)`, `46 Metadata tokens deobfuscated`,
`163 Calls to obfuscator types removed`, `742 Methods inlined`.
Для v1.3.3.98: `845 Methods decrypted`, `927 Strings decrypted`, `27 Embedded assemblies dumped`,
`30 Booleans decrypted`, `115 Calls to obfuscator types removed`, `957 Methods inlined`.

---

## 4. Функциональность приложения (классы)

Пространство имён `DNL_launcher`:

| Класс | Назначение / ключевые члены |
|---|---|
| `App : Application` | точка входа WPF (`Main`, `nLgvDX2ZbdsTY5wLIEk`) |
| `MainWindow : Window` | главное окно. Поля UI + методы (см. ниже) |
| `Program` | decoy `Main()` (пустой), `OnResolveAssembly` |
| `SteamLoginsParser` | `Parse(string) -> Dictionary<string, Dictionary<string,string>>` — парсинг логинов Steam |
| `SteamLoginsChanger` | смена/подмена логинов Steam |
| `SteamLoginConfData` | `Users` + `LoadFromFile(steamloginusersPath)` — чтение `loginusers.vdf` |
| `LoginData` | `Users, AccountName, PersonaName, RememberPassword, WantsOfflineMode, SkipOfflineModeWarning, AllowAutoLogin, MostRecent, Timestamp` |
| `FingerPrint` | `Value()`, `GetHash()`, `PC_data()` + `cpuId/baseId/videoId/macId` (WMI) — HWID |
| `UserID` | `IDcode` |
| `Product` | `act, login, hwid, pc` — данные «продукта/активации» |
| `ZaCryptography` | AES-обёртка: `Encrypt/Decrypt`, `EncryptString/DecryptString`, `_keySize=256` |
| `aes256cryptic` | ещё один AES-хелпер `EncryptString/DecryptString` |
| `DialogBox`, `DialogWindow`, `ErrorExplainWindow`, `PopupUserControl` | UI-диалоги, показ ответа сервера (`OpenDialogMessageFromServer`, `OpenErrorExplainWindow`) |
| `DenuvoNet_launcher.Properties.Resources` | картинки: `AhkAnimateGif`, `dnl_skin`, `EAsplash`, `MSsplash`, `splasho`, `Steamsplash_gif`, `Ubisoftsplash_gif` |
| `DenuvoNet_launcher.Properties.Settings` | настройки приложения |

Ключевые методы `MainWindow`:

- `BTN_ACTIVATE_Click` — кнопка **«Активировать»**: HTTP-запрос к серверу + обход проверки сертификатов.
- `AutoUpdate()` / `AutoUpdater_ApplicationExitEvent()` — самообновление.
- `ExtractZip(zipFilePath, outputDirectory)` — распаковка скачанного ZIP.
- `GetFileFromResource(Filename)` — достаёт файл из ресурсов.
- `GetRegistryValue(RegistryHive, keyPath, valueName)` — чтение реестра.
- `DoCheaterskiller()` — **убийство процессов** (kill-list).
- `LogSuccesstoLabel/LogInfo/LogError` — логирование в UI.
- `cmdDeleteID_Clicked`, `cmdIDSelection_Clicked`, `btn_last_IDs_Click_1`, `Btn_Free_Click`, `btn_site_Click`,
  `btn_paste_from_buffer_Click(_1)`, `Help_btn_Click_1`, `Window_Activated`, `DrawerHost_Initialized` — обработчики UI.
- `OpenDialogMessageFromServer(ServerResponse)`, `OpenErrorExplainWindow(MessageFromServer)` — вывод ответов сервера.

**Итог по функционалу:** лончер авторизуется на сервере (activation/login/hwid/pc), показывает UI,
работает с логинами Steam, качает/обновляет/распаковывает внешние бинари (в т.ч. через AutoUpdater.NET и AHK),
и убивает «читерские»/лишние процессы.

---

## 5. Встроенные ресурсы

Все встроенные ресурсы изначально зашифрованы/сжаты (нет `MZ`/`PK`/`deflate` без снятия протектора).
После снятия обфускации:

| Роль | Размер | SHA256 |
|---|---|---|
| `AhkAnimateGif.ahk` (вспом. AHK-анимация сплэша) | 958 | `D3B9C2BF80032CAC678386115E0F88DC27083206BBF44CDACDA219201F6D49E4` |
| `dnl_skin.zip` (Steam-скин `dnl`: `steam.styles` 77 KB + layout/menu) | 14 974 | `43F8859CAE29948305A40009B84A70A21EAAF5391B200F9833DDA9F8B14A39B6` |
| `splasho.png` | 349 217 | — |
| `EAsplash.png` | 116 232 | — |
| `MSsplash.png` | 129 975 | — |
| `Ubisoftsplash_gif.gif` | 173 452 | — |
| `Steamsplash_gif.gif` | 44 219 | — |
| `xnBJ4u7KTRU7qajYe6.axBde1JB9cZPfVuFCf` (служебный ресурс Reactor) | 256 | — |

Крупнейший исходный «ресурс 2» (8 688 835 Б, entropy 8.00) — упакованный **Costura-бандл зависимостей**;
после снятия протектора разложен на 25 сборок.

---

## 6. Сетевой протокол активации

Все запросы — `POST https://games-launcher.top/api.php`, `User-Agent: Games-launcher413`,
`Content-Type: application/json`.

**Запрос 1 — проверка ключа:**

```json
{
  "act":   "checkAccount",
  "login": "<введённый ID (длина 2..13)>",
  "hwid":  "<MD5 hex>",
  "pc":    "PC_Name: <machine> | CPU: <Win32_Processor.Name> | Videocard: <Win32_VideoController.Name>"
}
```

Ответ: `{"success": true|false, "errors": [ ... ]}`. При `false` показывается текст ошибок
(`OpenErrorExplainWindow`; коды **427** = ключ привязан к другому ПК, **428** = ключ истёк).

**Запрос 2 — выдача учётки (при success):**

```json
{ "act": "reqcreds", "hwid": "<hwid>", "login": "<ID>" }
```

Ответ: `{"result": { "auth_pswd": "<base64 AES>", "authlogin": "<login>", "type": "<store>" }}`.

**Запрос 3 (только v1.3.3.98) — EA 2FA:**

```json
{ "act": "EA_2FA_tst", "login": "<EA_login>" }
```

Сервер возвращает пин-код (`messages`), который **авто-вводится** в окно EA.

**Тип магазина (`type`):** `valve.steam`, `epic.games`, `ubisoft.uplay`, `microsoft.store`, `ea.origin`.

**Поведение TLS:** в колбэк добавляется `ServerCertificateValidationCallback => true` (принимает любой
сертификат); перед этим отдельно проверяется, что CN сертификата содержит `games-launcher.top`, иначе процесс
убивается (`Process.GetCurrentProcess().Kill()`). `SecurityProtocol` выставляется в `Ssl3`/`Tls12`.

---

## 7. Криптосхема паролей

Расшифровка серверного пароля (`aes256cryptic.DecryptString`):

```
AES-256-CBC
key = SHA256( ASCII("voron2020") )
IV  = 16 нулевых байт
затем .Substring(1)   // отбрасывается первый символ-маркер
```

Захардкоженный общий ключ `voron2020` → серверные пароли защищены **слабо** (единый ключ для всех аккаунтов).

---

## 8. HWID-фингерпринтинг

`FingerPrint.Value()` = `MD5hex_uppercase( cpuId() + baseId() )`:

- `cpuId` ← `Win32_Processor.ProcessorId` (fallback: `Name`, затем `Manufacturer`, + `MaxClockSpeed`);
- `baseId` ← `Win32_BaseBoard` (`Manufacturer` + `Name` + `SerialNumber`).

Методы `biosId`/`diskId`/`videoId`/`macId` в коде присутствуют, но в `Value()` **не используются**.
`PC_data()` возвращает три WMI-строки: `PC_Name`, `CPU` (`Win32_Processor.Name`),
`Videocard` (`Win32_VideoController.Name`).

---

## 9. Kill-list (`DoCheaterskiller`)

Убиваются процессы, **имя которых содержит** одну из подстрок:

```
note, cheat, cheatengine, wireshark, tcpdump, HTTPDebuggerSvc, HTTPDebuggerUI
```

Через `cmd /c taskkill /F /IM <proc>.exe` (fallback — `Process.Kill()`), с логом `Gotcha <proc>`.
Подстрока `note` ловит `notepad.exe` (и любое `*note*`); `cheat` — любые `*cheat*`.
Вызывается как на старте активации, так и в процессе ввода логина в Steam.

---

## 10. Ветки игровых магазинов

**`valve.steam`:**

1. `taskkill /IM steam.exe /F`; ставит `HKCU\Software\Valve\Steam` → `RememberPassword=1`, `AutoLoginUser=<login>`.
2. Чистит `%LocalAppData%\Steam` (рекурсивно, в цикле).
3. Парсит `%Steam%\config\loginusers.vdf`; при совпадении `AccountName` переписывает блок так, чтобы
   `WantsOfflineMode`, `SkipOfflineModeWarning`, `AllowAutoLogin`, `MostRecent`, `RememberPassword` = `"1"`.
4. Распаковывает `dnl_skin.zip` в `<Steam>\skins\`, ставит `SkinV5=dnl`, запускает
   `steam.exe -noverifyfiles -norepairfiles [--nointro]`.
5. Через AutoHotkey (`sharpAHK` + встроенный `AutoHotkey.dll`) вводит **логин и пароль** в окно
   `ahk_class vguiPopupWindow`, включая `BlockInput("On")` и проверку захвата фокуса «тестовым» логином
   `testlogin`/`failedtestlogin`, затем `SendRaw` логина/пароля и `{Enter}`.
6. Пишет `<Steam>\loginsteam.login`, правит все `*localconfig.vdf` (`EnableStreaming` `1`→`0`),
   ставит `SuppressAutoRun=1`, чистит `AutoLoginUser`.
7. Сохраняет `<SteamPath>` в `HKCU\Software\DenuvoNetLauncher\Steam\users\<login.toLowerCase()>\SteamPath`.
8. `FileRemoveDir <Steam>\skins\dnl` после показа `vguiPopupWindow`.

**Прочие ветки** (везде — авто-ввод логина/пароля и `BlockInput`):

- `epic.games` — окно `ahk_class UnrealWindow`, ищет `EpicGamesLauncher.exe`/ярлык по всем дискам;
- `ea.origin` — `EADesktop.exe`, окно `Qt5152QWindowOwnDCIcon`/`Qt5152QWindowOwnDCICon`, чистка `cookie.ini`
  (в v1.3.3.98 — плюс 2FA-пин от сервера через `EA_2FA_tst`);
- `ubisoft.uplay` — `upc.exe`, `ahk_class uplay_start`; в v1.3.3.98 — правка `settings.yaml` и управление сетью;
- `microsoft.store` — `ApplicationFrameHost.exe`, `ahk_class ApplicationFrameWindow`.

---

## 11. Self-update

Проверка/обновление через `AutoUpdater.NET`:

```
GET http://games-launcher.top/source/app/update.xml   User-Agent: AutoUpdater
```

`update.xml` (публичный, без авторизации):

```xml
<item><version>1.3.3.97</version>
      <url>http://games-launcher.top/source/app/DenuvoNet_launcher.zip</url>
      <mandatory>true</mandatory></item>
```

- Автозапуск скачанного ZIP; **подпись/хеш обновления не проверяются** → риск подмены
  (MITM / компрометация домена).
- Динамическая проверка (в изолированной среде, без выхода в реальную инфраструктуру) подтвердила
  единственный HTTP-GET от лончера — `GET /source/app/update.xml` (UA `AutoUpdater`); создан
  `%AppData%\Gameslauncher\Recent_ID` (0 байт); реестр на старте не изменялся.

---

## 12. Различия версий: v1.3.3.0 → v1.3.3.98

1. **EA (Origin) 2FA** — новый класс `DialogBox_2FA_Pincode`:
   `POST … {"act":"EA_2FA_tst","login":"<EA_login>"}`; сервер возвращает пин-код (`messages`), который
   **авто-вводится** в окно EA (`ahk_class Qt5152QWindowOwnDCICon`) через AutoHotkey.
   → у сервера есть доступ к 2FA-кодам расшаренных EA-аккаунтов.
2. **`NetworkManager`** — `ipconfig /release` и `/renew`: для **Ubisoft Connect** сеть отключается на время
   «офлайн-режима», затем восстанавливается (управление сетью для офлайн-запуска, не HWID-обход).
3. **Ubisoft `settings.yaml`**: в `%LocalAppData%\Ubisoft Game Launcher\settings.yaml` пишется встроенный
   `ubiusersettings.yaml` (`user.offline:false`), удаляются `user.dat` и `ConnectSecureStorage.dat`
   (сброс предыдущей авторизации пользователя). Ресурс `offlinesettings.yaml` (`user.offline:true`)
   в коде **не используется** (неиспользуемый встроенный ресурс).
4. `AppInfo.cs` — хелпер версии; новый ресурс `EGSsplash0.png` (Epic Games Store).
5. Сеть/домены без изменений: `games-launcher.top`, `denuvo.net`, `t.me/dnoff`, `vk.com/...`.

---

## 13. Сопутствующий образец `zteam_offliner.exe`

- Native x64 GUI, собран **AutoHotkey v1.1.36.02** (Ahk2Exe). Не .NET.
- Встроенный AHK-скрипт — ресурс `RCDATA "(>AUTOHOTKEY SCRIPT<)"`
  (SHA256 `B827DFB6EA2BBC115FA5C2F6BBC988705AF0E3C9307D6D8DFE4271397FD53C89`):
  **Steam offline account-switcher**. Читает `%A_ScriptDir%\config\loginusers.vdf`, показывает список аккаунтов;
  по выбору: `taskkill steam.exe` → `REG ADD HKCU\Software\Valve\Steam /v AutoLoginUser /d <акк>` и `RememberPassword=1`
  → перезапись `loginusers.vdf` с `WantsOfflineMode=1 / SkipOfflineModeWarning=1 / mostrecent=1 / RememberPassword=1`
  → запуск `%A_ScriptDir%\steam.exe`; кнопка **Run Steam Clean** удаляет `loginusers.vdf` и перезапускает Steam.
  **Сеть не использует.**
- Брендирование: ресурс `DENUVONET_SPLASH.PNG` + `FileInstall → %AppData%\Gameslauncher\Denuvonet_splash.png` —
  это **«Steamoffline»-компаньон из инструкции `denuvo.net`**, переупакованный под «zteam».
- **Оценка:** PUA/riskware (grey) той же кампании; включает offline-режим Steam для чужого/расшаренного
  аккаунта. Стилером/бэкдором не является.

---

## 14. Атрибуция разработчика

- `AssemblyDescription("Konstantin Yurchenko (vk.com/konstantin_yurch)")`.
- `AssemblyCompany("https://games-launcher.top")`, `AssemblyCopyright("games-launcher.top Copyright 2023")`.
- Встроенные YAML-ресурсы содержат `screenshot_root_path: C:/Users/ksyur/Pictures/UbisoftConnect/` —
  реальный путь с машины разработчика (`ksyur` = **Konstantin Yurchenko**), что усиливает атрибуцию.
- UI-текст «Made by Konstnt».
- OSINT: GridinSoft/ThreatInfo — «General Threat»/«Risk.Gen» для `DenuvoNet_launcher.exe`;
  ANY.RUN task `5ea103ee-f0a5-48f6-a79e-806cfbf88b49` — вердикт **«Malicious activity»**
  (дроп `ZipExtractor.exe` + `DenuvoNet_launcher.zip`, изменение системных сертификатов, загрузка
  `AutoHotkey.dll`, сетевые GET на `games-launcher.top`).

> Атрибуция по метаданным может быть прикрытием; тем не менее след разработчика (`ksyur`) в ресурсах согласуется с ней.

---

## 15. Инфраструктура и IOC

### 15.1 Сетевые индикаторы

```
Домены/IP:
  games-launcher.top   → 2.56.214.209 / 2a0c:b9c0:f:42ac::1   (в раннем наблюдении: 185.68.16.82, Hosting Ukraine)
  denuvo.net           → 169.40.15.144 / 2a01:8740:1:6c9::2
URL:
  http://games-launcher.top/source/app/update.xml        (AutoUpdate, UA "AutoUpdater")
  https://games-launcher.top/api.php                      (активация, UA "Games-launcher413")
  http://games-launcher.top/source/app/DenuvoNet_launcher.zip
  https://denuvo.net                                      (закрыт анти-бот защитой: «Making sure you're not a bot!»)
User-Agents: "AutoUpdater", "Games-launcher413"
Протокол: act = "checkAccount" | "reqcreds" | "EA_2FA_tst"
Ключ AES (base64-пароли): SHA256("voron2020"), IV = 16×00
```

Веб-сервер — **nginx**. Сертификаты — **Let's Encrypt** (DV):

| Домен | CN / SAN | Эмитент | Действует |
|---|---|---|---|
| games-launcher.top | `games-launcher.top`, `www.…` | Let's Encrypt YR2 | 25.09.2026 → 24.12.2026 |
| denuvo.net | `denuvo.net`, `www.…` | Let's Encrypt YR1 | 27.08.2026 → 25.11.2026 |

SPKI SHA256: `games-launcher.top` = `4daa1bc792b0e6460d6d21632579964ac4c46154b7e4e6c32bf8fc1c65825fd1`;
`denuvo.net` = `e8ba5bdb1e7fcabfd942e8c5c7d845425152beafdd38890fbd0b1b2b22ede346`.

### 15.2 Файлы/пути

```
%AppData%\Gameslauncher\Recent_ID, Recent_IDs
%AppData%\Gameslauncher\{AhkAnimateGif.ahk, dnl_skin.zip, *splash*, Denuvonet_splash.png}
<Steam>\config\loginusers.vdf (правка), <Steam>\loginsteam.login, <Steam>\skins\dnl
%LocalAppData%\Ubisoft Game Launcher\settings.yaml  (v1.3.3.98)
```

### 15.3 Реестр

```
HKCU\Software\Valve\Steam: RememberPassword, AutoLoginUser, SkinV5=dnl, SuppressAutoRun
HKCU\Software\DenuvoNetLauncher\Steam\users\<login>\SteamPath
HKLM\SOFTWARE\Microsoft\Tracing\DenuvoNet_launcher_RASAPI32
HKLM\SOFTWARE\Microsoft\Tracing\DenuvoNet_launcher_RASMANCS
HKCU\Software\Microsoft\Direct3D\MostRecentApplication
```

### 15.4 Карта ошибок API (`POST /api.php`)

| Запрос | HTTP | Ответ |
|---|---|---|
| `checkAccount` (тестовые/заглушечные креды) | 200 | `{"success":false,"errors":{"404":"ID … Not found"}}` |
| `reqcreds` (заглушечные креды) | 200 | `{"success":false,"errors":[],"messages":[]}` |
| `EA_2FA_tst` | **500** | пусто (обработчик падает) |
| неизвестный `act` / пустой JSON / GET без параметров | 200 | `{"success":false,"errors":{"5001":"Action unidentified"}}` |
| POST по **http** (порт 80) вместо https | 200 | работает (JSON тот же) |

- CORS-заголовков на `api.php` нет (для `.zip` — `Access-Control-Allow-Origin: *`).
- `EA_2FA_tst` роняет сервер (500) — необработанный случай на стороне бэкенда.

### 15.5 Публичный payload

`DenuvoNet_launcher.zip` — 9 071 766 Б, zip SHA256
`6011B27DB557598B7080FF31124905479CA3224D437E0576718C0B07A83A1F16`.
Внутри единственный файл `DenuvoNet_launcher.exe` (9 160 704 Б, 2026-02-10), **AssemblyVersion = 1.3.3.98**.

**Вывод:** исследуемый `DenuvoNet_launcher.exe` **побайтово идентичен** публичной раздаче (SHA256 `6D10C70B…`) —
это актуальная живая версия. Строка версии в `update.xml` отстаёт (`1.3.3.97`) от реальной сборки
(`1.3.3.98`) — расхождение метаданных у разработчика.

### 15.6 Хэши образцов (сводка)

```
DenuvoNet_launcher.exe         MD5 F887E2BEECCC3D6CB5EE3A7D7B7AA9C7
                    SHA256 F119196293DF44033A700D07A66B18F5561A1608E75170CA0EBFBFFD95621250
DenuvoNet_launcher.exe        MD5 07D27376DFA48C9715F755BE41E78328
                    SHA256 6D10C70BE8809CBF2FDA3CBC955F04047EBA2131D6DA28EB6034786494FCB246
zteam_offliner.exe  MD5 51D1368AED3ECA449852B58B1B9B4E23
                    SHA256 D204C7D85C4859958F5E030205648FC9D66A748F13A3C4B091B2E178506DA81F
```

---

**Выводы:**

1. **Модель HWID-привязки подтверждена эмпирически.** Пересчитанный на «родной» машине HWID
   (= `MD5(ProcessorId + BaseBoard.Manufacturer + BaseBoard.Name + BaseBoard.SerialNumber)`) был **принят
   сервером** для действующего аккаунта → формула `FingerPrint.Value()` из декомпила корректна.
   HWID привязан к железу (CPU + материнская плата) и не воспроизводится на другой машине.
2. **Различение ошибок:** несуществующий ID → `404` даже при верном HWID; значит `404` = «запись не
   найдена по паре (login, hwid)», наличие логина не раскрывается. Ожидаемый код «логин есть, но HWID
   чужой» (в клиенте зашит **427**) не наблюдался.
3. **Бэкенд активен** — отдаёт `success:true` и живые тексты сообщений (RU/EN), соответствующие
   ветвям декомпилированного `MainWindow`.
4. **Схема дешифрования подтверждена на реальных данных.** Ответ `reqcreds`: `type = "microsoft.store"`;
   `auth_pswd` = 24 символа base64 → 16 байт (один AES-блок). Расшифровка
   `AES-256-CBC, key = SHA256("voron2020"), IV = 16×00` дала осмысленный печатный текст (15 байт; после
   `Substring(1)` — 14 байт) ⇒ **захардкоженный ключ `voron2020` корректно расшифровывает серверные пароли**.
   Необратимый отпечаток расшифрованного секрета (sha256[:16] = `698d0b161e32ce13`) подтверждает успех
   без раскрытия самого секрета. `authlogin` присутствовал (длина 27).
   - **Значения логина/пароля намеренно не выводились и не сохранялись** (принцип минимизации данных).

**Итог:** полный протокол
(`checkAccount` → `reqcreds` → AES-расшифровка → авто-ввод в клиент магазина)
эмпирически подтверждён на живом бэкенде; ключ/IV восстановлены точно, ветка `microsoft.store` подтверждена.

---

## 17. Результаты по восстановлению исходного кода

- Логика приложения **полностью реконструирована** из снятого обфусцированного образа и **успешно
  пересобрана** в компилируемое WPF-решение (`net472`): v1.3.3.0 — сборка без ошибок; v1.3.3.98 —
  сборка без ошибок и без предупреждений (`DenuvoNet_launcher` v1.3.3.98).
- Подтверждена **воспроизводимость**: собранный лончер повторяет поведение оригинала (запускается,
  проходит инициализацию, обращается к тому же эндпоинту).
- `Class1.cs`/`Class5.cs` — неубранный остаток обфускатора (на работу не влияет).

---

## 18. YARA-правило

```yara
rule DNL_launcher_DenuvoNet
{
    meta:
        description  = "DenuvoNet_launcher / DNL_launcher loader (obfuscated .NET, games-launcher.top)"
        author       = "research"
        date         = "2026-10-10"
        reference    = "https://app.any.run/tasks/5ea103ee-f0a5-48f6-a79e-806cfbf88b49/"
        hash_md5     = "f887e2beeccc3d6cb5ee3a7d7b7aa9c7"
        hash_sha256  = "f119196293df44033a700d07a66b18f5561a1608e75170ca0ebfbffd95621250"

    strings:
        $mz          = { 4D 5A }
        $s1  = "DenuvoNet_launcher"            ascii wide
        $s2  = "DNL_launcher"                  ascii wide
        $s3  = "games-launcher.top"            ascii wide
        $s4  = "Debugger Detected"             ascii wide
        $s5  = " is tampered."                 ascii wide
        $s6  = "clrjit.dll"                    ascii wide
        $s7  = "m_pData"                       ascii wide
        $s8  = "sharpAHK"                      ascii
        $s9  = "steamloginusersPath"           ascii
        $s10 = "AutoUpdater.NET"               ascii
        $s11 = "QoObXhL7IkeoF2IeyK"            ascii
        $s12 = "BCrj0huO3m1t2fSrIm"            ascii
        $s13 = "GetDelegateForFunctionPointer" ascii wide

    condition:
        $mz at 0 and 4 of ($s*) and filesize < 20MB
}
```

---

## 19. Оценка опасности и выводы

- **Категория:** PUA / Riskware — gray-market shared-account game launcher (credential-abuse),
  распространяется через `denuvo.net` / `games-launcher.top` как «офлайн-активация» игр.
- **Опасность для пользователя:** средняя-высокая. Прямая кража данных пользователя из системы не выявлена,
  однако:
  1. на сервер уходит HWID и характеристики ПК;
  2. на ПК **приходит и расшифровывается чужой пароль** (слабая крипто-схема, общий ключ `voron2020`);
  3. выполняется авто-ввод учётки в клиенты магазинов + правка `loginusers.vdf`/реестра Steam;
  4. отключаются античит/аналитические утилиты (kill-list);
  5. HTTP-самообновление без подписи — **RCE-вектор** (MITM / компрометация домена);
  6. обход проверки TLS-сертификатов.
- Для владельцев аккаунтов это инструмент перепродажи/шаринга их аккаунтов.
- **Протектор:** .NET Reactor.

---

*Отчёт содержит только результаты исследования. Сведения об окружении, в котором проводился анализ,
и конкретные идентификаторы проверенных аккаунтов/HWID намеренно исключены.*
