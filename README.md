# DenuvoNetCredDecrypter

A small, parameterless console utility for **.NET Framework 4.7.2** that:

1. detects and prints the current machine **HWID**;
2. asks for three lines of input — `id`, `hwid` and `hardware info`;
3. requests credentials from a remote service over HTTPS;
4. decrypts the returned password (AES-256-CBC);
5. saves `login` / `password` plus the entered `hwid` and `hardware info` to a file named after the entered `id`.

## Requirements

- Windows
- .NET Framework 4.7.2 (or newer)
- .NET SDK 8+ (build/test only)

## Build

```powershell
dotnet build DenuvoNetCredDecrypter.sln -c Release
```

The executable is produced at
`src/DenuvoNetCredDecrypter/bin/Release/net472/DenuvoNetCredDecrypter.exe`.

## Test

```powershell
dotnet test DenuvoNetCredDecrypter.sln
```

## Run

```powershell
.\src\DenuvoNetCredDecrypter\bin\Release\net472\DenuvoNetCredDecrypter.exe
```

The program takes **no command line arguments** and is fully interactive.

## Configuration

Settings live in [`src/DenuvoNetCredDecrypter/appsettings.json`](src/DenuvoNetCredDecrypter/appsettings.json)
and are copied next to the executable on build:

| Key          | Meaning                                            |
|--------------|----------------------------------------------------|
| `apiUrl`     | Full URL of the credential service endpoint.       |
| `userAgent`  | `User-Agent` header sent with the request.         |
| `aesKeySeed` | Seed whose SHA-256 is used as the AES-256 key.     |

## How the HWID is computed

The HWID mirrors the reference algorithm:

- `cpuId` = `Win32_Processor.ProcessorId`; if empty, `Name`; if still empty, `Manufacturer`;
  the fallback path appends `MaxClockSpeed`.
- `baseId` = `Win32_BaseBoard.Manufacturer` + `Name` + `SerialNumber`.
- `HWID` = upper-case hex of `MD5(ASCII(cpuId + baseId))`.

## Password decryption

The `auth_pswd` value from the server is Base64 AES-256-CBC ciphertext where:

- key = `SHA-256(UTF8(aesKeySeed))`,
- IV = 16 zero bytes,
- padding = PKCS7,
- the first byte of the plaintext is a header and is dropped.

## Project layout

```
src/DenuvoNetCredDecrypter/
  Program.cs                 composition root
  Configuration/             options + loader
  Domain/                    models
  Abstractions/              interfaces
  Services/                  HWID, API client, decryptor, store
  App/                       application flow
  Infrastructure/Console/    console UI (theme, spinner)
tests/DenuvoNetCredDecrypter.Tests/   xUnit tests
```

## License

MIT — see [LICENSE](LICENSE).
