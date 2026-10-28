## プロジェクト構成

**ソリューションファイルを開くときにはC:\などの浅い階層で開いてください**

**WindowsとAndroidは正常にビルド・起動できています**

Rydia Engine は、以下のプロジェクトで構成されています。

| プロジェクト | 説明 |
|---|---|
| `Rydia.Runtime` | クロスプラットフォーム対応のための共通ランタイム |
| `Rydia.Runtime.Windows` | Windows 向けランタイム |
| `Rydia.Runtime.Macos` | macOS 向けランタイム (未実装)|
| `Rydia.Runtime.Linux` | Linux 向けランタイム (未実装)|
| `Rydia.Runtime.Droid` | Android 向けランタイム |
| `Rydia.Runtime.iOS` | iOS 向けランタイム (未実装)|
| `Rydia.Runtime.NintendoSwitch` | Nintendo Switch 向けランタイム (未実装)|
| `Rydia.Engine` | ECS(Entity Component System)などのゲームの実行に必要な機能を提供するゲームエンジン |
| `Rydia.GameStudio` | ゲームオブジェクトやコンポーネントを作成・管理するためのエディター |



## `Rydia.Runtime`

主なインターフェースは以下のとおりです。
- `IMessageService` — メッセージサービスに関するインターフェース
- `IDesktopPlatform` — Desktop プラットフォーム向けのインターフェース
- `IMobilePlatform` — Mobile プラットフォーム向けのインターフェース
- `IGameConsolePlatform` — Game Console プラットフォーム向けのインターフェース

などDesktop および Mobile, Game Console プラットフォームでクロスプラットフォームを実現するためにプラットフォーム固有の処理を抽象化し、異なる実行環境で共通のコードを利用できるようにすることを目的としています。


### 対応プラットフォーム

| 分類 | プラットフォーム |
|---|---|
| Desktop | Windows、macOS、Linux |
| Mobile | Android、iOS |
| Game Console | Nintendo Switch |

## Rydia.Runtime.Windows

`Rydia.Runtime.Windows` は、Windows 環境で OpenGL ES および OpenAL を実行するために必要なランタイムを提供します。

主な機能は以下のとおりです。

- Google Angle for Direct3Dを使用したOpenGL ES 2.0 / 3.0 によるグラフィックス処理
- OpenAL によるオーディオ処理
- Windows 環境向けのプラットフォーム固有処理

## Rydia. Runtime.Macos, Linuxはまだ作っていません
MacosはGoogle Angle for Metalを使用する予定です。

LinuxはGoogle Angle for Vulkan or OpenGLを使用する予定です。

## Rydia.Runtime.Droid

`Rydia.Runtime.Droid` は、Android 環境で OpenGL ES および OpenAL を実行するために必要なランタイムを提供します。

主な機能は以下のとおりです。

- OpenGL ES 2.0 / 3.0 によるグラフィックス処理
- OpenAL によるオーディオ処理
- Android 環境向けのプラットフォーム固有処理

## Rydia. Runtime.iOSはまだ作っていません
Google Angle for Metalを使用する予定です。

## Rydia. Runtime.NintendoSwitchはまだ作っていません
Nintendo SwitchはOpenGL ES, OpenALをスイッチ用に書き換えるラッパークラスを作成し、使用する予定です。

## Rydia Engine

Rydia Engine は、クロスプラットフォームでのゲーム開発を目指すゲームエンジンプロジェクトです。

共通ランタイム、ゲームエンジン、エディターを分離することで、ゲームの実行に必要な機能を提供し、
プラットフォーム固有の処理をランタイム側に分離することで、複数のプラットフォームに対応したゲーム開発環境を提供します。

## Rydia.GameStudio

`Rydia.GameStudio` は、`Rydia.Engine` で使用するゲームオブジェクトやコンポーネントを動的に作成・管理するためのツールです。

エディターを通じてゲームオブジェクトを扱い、Rydia Engine を利用したゲーム開発を支援します。

.NET10.0および.slnxに対応したので、Visual Studio 2026が必要です

## グラフィックスとオーディオ

Rydia Engine では、以下の API を利用したグラフィックスおよびオーディオ処理をサポートします。

| 分野 | API |
|---|---|
| グラフィックス | OpenGL ES 2.0 / 3.0 |
| オーディオ | OpenAL |

各プラットフォーム向けのランタイムが、対応する実行環境でこれらの API を利用するための機能を提供します。

## ドキュメント

導入方法、プロジェクト構成、開発に関する情報については、公式ドキュメントを参照してください。

**[Rydia Engine 公式ドキュメント](https://rydiaryzen.github.io/RydiaEngine.Docs/index.html)**

## ロードマップ
|対応|補足|
|---|---|
|ECS(Entity Component System)の構築||
|エディターの構築(WinForms)||
|エディターの構築(ImGui)| ImGuiの実装によりエディターがWindows, Macos, Linuxへの対応になる|
|Rydia.Runtime.Linux, Macos, iOS, NintendoSwitchへの対応|これによりランチャー側が6プラットフォーム対応になる|
|難読化への対応||
|Live2Dへの対応||