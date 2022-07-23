using System.IO;
using System.Diagnostics;
using System.Windows.Automation;

namespace Echeneis;

public class OrcaWrapper : IDisposable
{
    Process? _process;
    IntPtr mainWindow;

    AutomationElement btnConnect;
    AutomationElement btnCompile;
    AutomationElement btnRun;
    AutomationElement btnCancel;

    public OrcaWrapper()
    {
        _process = Process.Start(new ProcessStartInfo(Path.Join(AppContext.BaseDirectory, Path.Join("ORCA GC Controller", "ORCA GC Controller.exe"))));
        if (_process == null) throw new Exception("ORCA GC Controller.exe dose not start.");
        while (_process.MainWindowHandle == IntPtr.Zero) Thread.Sleep(1);

        // 最前面表示とメニューボタン無効化
        mainWindow = _process.MainWindowHandle;
        mainWindow.SetWindowTopMost();
        mainWindow.DisableCloseButton();
        mainWindow.DisableMinimizeButton();
        mainWindow.DisableMaximizeButton();

        const string CLASSNAME_BUTTON = "WindowsForms10.BUTTON.app.0.141b42a_r7_ad1";

        btnConnect = AutomationElement.FromHandle(mainWindow.GetChildHandle(CLASSNAME_BUTTON, "Connect"));
        btnCompile = AutomationElement.FromHandle(mainWindow.GetChildHandle(CLASSNAME_BUTTON, "Compile"));
        btnRun = AutomationElement.FromHandle(mainWindow.GetChildHandle(CLASSNAME_BUTTON, "Run"));
        btnCancel = AutomationElement.FromHandle(mainWindow.GetChildHandle(CLASSNAME_BUTTON, "Cancel"));
    }

    public bool Connected = false;
    public bool Compiled = false;
    public bool Running = false;

    public void Connect()
    {
        if (Connected)
        {
            return;
        }
        btnConnect.Invoke();
        Connected = true;
        Compiled = false;

        Thread.Sleep(100);
    }
    public void Ternimate()
    {
        if (!Connected)
        {
            return;
        }
        btnConnect.Invoke();
        Connected = false;
        Compiled = false;

        Thread.Sleep(100);
    }
    public void Compile()
    {
        if (Compiled || Running)
        {
            return;
        }
        btnCompile.Invoke();
        Compiled = true;

        Thread.Sleep(100);
    }
    public async Task Run()
    {
        if (!Connected || !Compiled || Running)
        {
            return;
        }
        btnRun.Invoke();
        Running = true;

        Thread.Sleep(100);

        await Task.Run(() =>
        {
            while (!(bool)btnRun.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty)) ;
            Running = false;
        });
    }
    public void Cancel()
    {
        if (!Running)
        {
            return;
        }
        btnCancel.Invoke();
        Running = false;

        Thread.Sleep(100);
    }

    #region IDisposable
    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                if (_process != null)
                {
                    _process.Kill();
                    _process.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
