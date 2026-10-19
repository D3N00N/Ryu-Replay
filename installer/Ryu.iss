; Ryu's install wizard: language, welcome, folder, options, install, finish.
; It carries Velopack's one-click Setup and runs it silently into the chosen folder, so Velopack still owns the
; install (updates, the Apps & features entry, the uninstaller); the wizard itself installs nothing to uninstall.
; Built by tools\release.ps1:  ISCC /DAppVersion=1.0.0 /DPackId=Ryu /DOutDir=<releases dir> Ryu.iss
; (it takes <OutDir>\<PackId>-win-Setup.exe, made there by vpk pack just before).
; A test build (another PackId) writes its choices file and start-with-Windows entry under its own name, away from
; the real Ryu's (a test never starts its app).

#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif
#ifndef PackId
  #define PackId "Ryu"
#endif
#ifndef OutDir
  #define OutDir "..\releases"
#endif
#define VeloSetup OutDir + "\" + PackId + "-win-Setup.exe"

[Setup]
AppId={#PackId}.Wizard
AppName={#PackId}
AppVersion={#AppVersion}
AppVerName={#PackId} {#AppVersion}
AppPublisher=Ryu
AppPublisherURL=https://github.com/D3N00N/Ryu-Replay
DefaultDirName={code:DefaultDir}
UsePreviousAppDir=no
DisableWelcomePage=no
DisableDirPage=no
DisableProgramGroupPage=yes
DirExistsWarning=no
PrivilegesRequired=lowest
Uninstallable=no
CloseApplications=no
RestartApplications=no
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.19041
WizardStyle=modern dark
WizardImageFile=wizard.png
WizardSmallImageFile=wizard-small.png
SetupIconFile=..\src\Ryu\app.ico
OutputDir={#OutDir}
OutputBaseFilename={#PackId}-Setup
; Velopack's Setup is mostly compressed already (LZMA would save about 6% and slow the install).
Compression=none

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "ar"; MessagesFile: "compiler:Languages\Arabic.isl"
Name: "zhHans"; MessagesFile: "Languages\ChineseSimplified.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "ptBR"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[CustomMessages]
en.LangCode=en
ar.LangCode=ar
zhHans.LangCode=zh-Hans
ru.LangCode=ru
es.LangCode=es
ptBR.LangCode=pt-BR
de.LangCode=de
en.StartWithWindows=Start Ryu with Windows, so instant replay is ready when you play
ar.StartWithWindows=تشغيل Ryu مع بدء ويندوز، لتكون الإعادة الفورية جاهزة حين تلعب
zhHans.StartWithWindows=随 Windows 启动 Ryu，玩游戏时即时回放已就绪
ru.StartWithWindows=Запускать Ryu вместе с Windows, чтобы мгновенный повтор был готов, когда вы играете
es.StartWithWindows=Iniciar Ryu con Windows, para que la repetición instantánea esté lista cuando juegues
ptBR.StartWithWindows=Iniciar o Ryu com o Windows, para o replay instantâneo estar pronto quando você jogar
de.StartWithWindows=Ryu mit Windows starten, damit Instant Replay beim Spielen bereit ist
en.InstallingApp=Installing Ryu…
ar.InstallingApp=جارٍ تثبيت Ryu…
zhHans.InstallingApp=正在安装 Ryu…
ru.InstallingApp=Установка Ryu…
es.InstallingApp=Instalando Ryu…
ptBR.InstallingApp=Instalando o Ryu…
de.InstallingApp=Ryu wird installiert…
en.RyuRunning=Ryu is running. Quit it first (right-click its icon by the clock, then Exit), then choose Retry.
ar.RyuRunning=يعمل Ryu الآن. أغلقه أولًا (انقر بزر الفأرة الأيمن على أيقونته بجوار الساعة، ثم «خروج»)، ثم اختر «إعادة المحاولة».
zhHans.RyuRunning=Ryu 正在运行。请先退出它（右键单击时钟旁的图标，然后选择“退出”），再选择“重试”。
ru.RyuRunning=Ryu запущен. Сначала закройте его (щёлкните правой кнопкой по его значку у часов и выберите «Выход»), затем нажмите «Повтор».
es.RyuRunning=Ryu está abierto. Ciérralo primero (clic derecho en su icono junto al reloj y luego «Salir») y después elige Reintentar.
ptBR.RyuRunning=O Ryu está aberto. Feche-o primeiro (clique com o botão direito no ícone ao lado do relógio e depois em «Sair») e escolha Repetir.
de.RyuRunning=Ryu läuft. Beende es zuerst (Rechtsklick auf sein Symbol neben der Uhr, dann „Beenden“) und wähle dann Wiederholen.
en.InstallFailed=Ryu could not be installed (error %1). Please try again.
ar.InstallFailed=تعذّر تثبيت Ryu (الخطأ %1). يُرجى المحاولة مرة أخرى.
zhHans.InstallFailed=无法安装 Ryu（错误 %1）。请重试。
ru.InstallFailed=Не удалось установить Ryu (ошибка %1). Попробуйте ещё раз.
es.InstallFailed=No se pudo instalar Ryu (error %1). Inténtalo de nuevo.
ptBR.InstallFailed=Não foi possível instalar o Ryu (erro %1). Tente novamente.
de.InstallFailed=Ryu konnte nicht installiert werden (Fehler %1). Bitte versuche es erneut.
en.FolderNotEmpty=Please choose an empty or new folder. Uninstalling Ryu removes its whole folder, so it must not share one with other files.
ar.FolderNotEmpty=يُرجى اختيار مجلد فارغ أو جديد. إزالة Ryu تحذف مجلده كاملًا، لذا يجب ألا يحوي ملفات أخرى.
zhHans.FolderNotEmpty=请选择一个空文件夹或新文件夹。卸载 Ryu 会删除其整个文件夹，因此该文件夹不能包含其他文件。
ru.FolderNotEmpty=Выберите пустую или новую папку. При удалении Ryu удаляется вся его папка, поэтому в ней не должно быть других файлов.
es.FolderNotEmpty=Elige una carpeta vacía o nueva. Al desinstalar Ryu se borra toda su carpeta, así que no debe compartirla con otros archivos.
ptBR.FolderNotEmpty=Escolha uma pasta vazia ou nova. Desinstalar o Ryu apaga a pasta inteira, então ela não pode ter outros arquivos.
de.FolderNotEmpty=Bitte wähle einen leeren oder neuen Ordner. Beim Deinstallieren entfernt Ryu seinen ganzen Ordner, er darf also keine anderen Dateien enthalten.
en.FolderNotWritable=Ryu cannot write to this folder. Choose one in your user folder or on another drive (folders such as Program Files need administrator rights).
ar.FolderNotWritable=لا يستطيع Ryu الكتابة في هذا المجلد. اختر مجلدًا داخل مجلد المستخدم أو على قرص آخر (مجلدات مثل Program Files تحتاج صلاحيات المسؤول).
zhHans.FolderNotWritable=Ryu 无法写入此文件夹。请选择用户文件夹中或其他磁盘上的文件夹（Program Files 等文件夹需要管理员权限）。
ru.FolderNotWritable=Ryu не может записывать в эту папку. Выберите папку в своём профиле или на другом диске (для папок вроде Program Files нужны права администратора).
es.FolderNotWritable=Ryu no puede escribir en esta carpeta. Elige una en tu carpeta de usuario o en otra unidad (carpetas como Program Files requieren permisos de administrador).
ptBR.FolderNotWritable=O Ryu não consegue gravar nesta pasta. Escolha uma na sua pasta de usuário ou em outra unidade (pastas como Arquivos de Programas exigem direitos de administrador).
de.FolderNotWritable=Ryu kann in diesen Ordner nicht schreiben. Wähle einen in deinem Benutzerordner oder auf einem anderen Laufwerk (Ordner wie Programme brauchen Administratorrechte).

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"
Name: "autostart"; Description: "{cm:StartWithWindows}"

[Files]
; Run from [Code] once the progress page shows; Velopack's Setup extracts and installs the app itself.
Source: "{#VeloSetup}"; DestName: "{#PackId}-win-Setup.exe"; Flags: dontcopy

[Icons]
; Velopack's uninstaller removes every shortcut that points into the install folder, this one too.
Name: "{userdesktop}\{#PackId}"; Filename: "{app}\{#PackId}.exe"; Tasks: desktopicon

[Registry]
; The same entry Ryu keeps in step with its setting (Autostart.CommandFor, AppInfo.RunValueName): the wizard writes
; the first one, or clears an old one, for when Ryu is not started from the last page.
; {#PackId}.exe is Velopack's launcher at the top of the install folder (named after the package).
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#PackId}"; ValueData: """{app}\{#PackId}.exe"" --background"; Tasks: autostart
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: none; ValueName: "{#PackId}"; Flags: deletevalue; Tasks: not autostart

[Run]
Filename: "{app}\{#PackId}.exe"; Description: "{cm:LaunchProgram,{#PackId}}"; Flags: nowait postinstall skipifsilent

[Code]
{ Message boxes are suppressible: a silent install (/VERYSILENT /SUPPRESSMSGBOXES) never shows one, it takes the default. }
const
  UninstallKey = 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{#PackId}';

{ The folder Ryu is in already (to install over it), or the usual one. }
function DefaultDir(Param: String): String;
begin
  if not RegQueryStringValue(HKCU, UninstallKey, 'InstallLocation', Result) or (Result = '') then
    Result := ExpandConstant('{localappdata}\{#PackId}');
  Result := RemoveBackslashUnlessRoot(Result);
end;

function ChosenDir(): String;
begin
  Result := RemoveBackslashUnlessRoot(WizardDirValue());
end;

{ A Velopack install of Ryu, as InstallPaths.RootFor sees one. }
function IsRyuInstall(Dir: String): Boolean;
begin
  Result := FileExists(AddBackslash(Dir) + 'Update.exe') and DirExists(AddBackslash(Dir) + 'current');
end;

function IsEmptyFolder(Dir: String): Boolean;
var
  Found: TFindRec;
begin
  Result := True;
  if FindFirst(AddBackslash(Dir) + '*', Found) then
    try
      repeat
        if (Found.Name <> '.') and (Found.Name <> '..') then
        begin
          Result := False;
          Break;
        end;
      until not FindNext(Found);
    finally
      FindClose(Found);
    end;
end;

function CanWriteTo(Dir: String): Boolean;
var
  Existed: Boolean;
  Probe: String;
begin
  Existed := DirExists(Dir);
  Result := ForceDirectories(Dir);
  if Result then
  begin
    Probe := AddBackslash(Dir) + '.Ryu-setup-probe';
    Result := SaveStringToFile(Probe, 'ok', False);
    DeleteFile(Probe);
    if not Existed then
      RemoveDir(Dir);
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  Dir: String;
begin
  Result := True;
  if CurPageID = wpSelectDir then
  begin
    Dir := ChosenDir();
    { Uninstalling removes the whole folder: never a drive's root, never a folder that holds other things. }
    if (Length(Dir) <= 3) or (DirExists(Dir) and not IsEmptyFolder(Dir) and not IsRyuInstall(Dir)) then
    begin
      SuppressibleMsgBox(CustomMessage('FolderNotEmpty'), mbError, MB_OK, IDOK);
      Result := False;
    end
    else if not CanWriteTo(Dir) then
    begin
      SuppressibleMsgBox(CustomMessage('FolderNotWritable'), mbError, MB_OK, IDOK);
      Result := False;
    end;
  end;
end;

{ A Ryu started from the chosen folder (another copy elsewhere, a test or a developer's, does not count). }
function RyuRunningIn(Dir: String): Boolean;
var
  Locator, Service, Found, Item: Variant;
  I: Integer;
  ExePath: String;
begin
  Result := False;
  if not IsRyuInstall(Dir) then
    Exit; { a new or empty folder: nothing can be running from it }
  try
    Locator := CreateOleObject('WbemScripting.SWbemLocator');
    Service := Locator.ConnectServer('.', 'root\CIMV2');
    Found := Service.ExecQuery('SELECT ExecutablePath FROM Win32_Process WHERE Name = ''Ryu.exe''');
    for I := 0 to Found.Count - 1 do
    begin
      Item := Found.ItemIndex(I);
      { A process Windows does not show the path of (another user's) has no ExecutablePath. }
      if VarIsNull(Item.ExecutablePath) then
        Continue;
      ExePath := Item.ExecutablePath;
      if Pos(Lowercase(AddBackslash(Dir)), Lowercase(ExePath)) = 1 then
      begin
        Result := True;
        Exit;
      end;
    end;
  except
    { No WMI: go on (the Setup itself still copes with a running copy). }
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  Result := '';
  while RyuRunningIn(ChosenDir()) do
    if SuppressibleMsgBox(CustomMessage('RyuRunning'), mbError, MB_RETRYCANCEL, IDCANCEL) <> IDRETRY then
    begin
      Result := CustomMessage('RyuRunning');
      Exit;
    end;
end;

procedure WriteChoices();
var
  DataDir, Choices: String;
begin
  DataDir := ExpandConstant('{userappdata}\{#PackId}');
  ForceDirectories(DataDir);
  Choices := 'language=' + CustomMessage('LangCode') + #13#10 + 'startWithWindows=';
  if WizardIsTaskSelected('autostart') then
    Choices := Choices + 'true'
  else
    Choices := Choices + 'false';
  SaveStringToFile(AddBackslash(DataDir) + 'install-choices.txt', Choices + #13#10, False);
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  Code: Integer;
begin
  if CurStep = ssInstall then
  begin
    WizardForm.StatusLabel.Caption := CustomMessage('InstallingApp');
    ExtractTemporaryFile('{#PackId}-win-Setup.exe');
    if not Exec(ExpandConstant('{tmp}\{#PackId}-win-Setup.exe'), '--silent --installto "' + ChosenDir() + '"', '',
        SW_HIDE, ewWaitUntilTerminated, Code) or (Code <> 0) then
      RaiseException(FmtMessage(CustomMessage('InstallFailed'), [IntToStr(Code)]));
    WriteChoices();
  end;
end;
