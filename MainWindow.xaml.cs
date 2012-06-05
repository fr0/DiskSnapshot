using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Path = System.IO.Path;
using System.Windows.Threading;

namespace DiskSnapshot
{
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      HeartbeatTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1.0)};
      HeartbeatTimer.Tick += HeartbeatTimer_Tick;
      AvailableDrives = System.IO.DriveInfo.GetDrives().Where(d => (d.DriveType != DriveType.NoRootDirectory)).ToArray();
      var bootdrive = AvailableDrives.First(x => x.Name.ToUpper() == "C:\\"); // hack
      LoadDirectoryStructure(bootdrive);
    }

    public static readonly DependencyProperty DirectoryStructureProperty =
        DependencyProperty.Register("DirectoryStructure", typeof(DirectoryStructure), typeof(MainWindow),
          new FrameworkPropertyMetadata());
    public DirectoryStructure DirectoryStructure
    {
      get { return (DirectoryStructure)GetValue(DirectoryStructureProperty); }
      set { SetValue(DirectoryStructureProperty, value); }
    }

    public static readonly DependencyProperty IsSelectingDriveProperty =
        DependencyProperty.Register("IsSelectingDrive", typeof(bool), typeof(MainWindow),
          new FrameworkPropertyMetadata(false));
    public bool IsSelectingDrive
    {
      get { return (bool)GetValue(IsSelectingDriveProperty); }
      set { SetValue(IsSelectingDriveProperty, value); }
    }

    public static readonly DependencyProperty AvailableDrivesProperty =
        DependencyProperty.Register("AvailableDrives", typeof(DriveInfo[]), typeof(MainWindow),
          new FrameworkPropertyMetadata());
    public DriveInfo[] AvailableDrives
    {
      get { return (DriveInfo[])GetValue(AvailableDrivesProperty); }
      set { SetValue(AvailableDrivesProperty, value); }
    }

    public static readonly DependencyProperty WorkingProperty =
        DependencyProperty.Register("Working", typeof(bool), typeof(MainWindow),
          new FrameworkPropertyMetadata(false));
    public bool Working
    {
      get { return (bool)GetValue(WorkingProperty); }
      set { SetValue(WorkingProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register("Progress", typeof(string), typeof(MainWindow),
          new FrameworkPropertyMetadata(""));
    public string Progress
    {
      get { return (string)GetValue(ProgressProperty); }
      set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressDirectoryProperty =
        DependencyProperty.Register("ProgressDirectory", typeof(string), typeof(MainWindow),
          new FrameworkPropertyMetadata(""));
    public string ProgressDirectory
    {
      get { return (string)GetValue(ProgressDirectoryProperty); }
      set { SetValue(ProgressDirectoryProperty, value); }
    }

    DateTime StartTime;
    void HeartbeatTimer_Tick(object sender, EventArgs e)
    {
      TimeSpan runningTime = DateTime.Now - StartTime;
      if (DirectoryStructure.LastScanTimeInSeconds > 0)
      {
        Progress = string.Format("{0} (ETC {1})", runningTime.ToString("mm\\:ss"), 
          (TimeSpan.FromSeconds(DirectoryStructure.LastScanTimeInSeconds) - runningTime).ToString("mm\\:ss"));
      }
      else
      {
        Progress = runningTime.ToString("mm\\:ss");
      }
    }


    readonly DispatcherTimer HeartbeatTimer;

    public const string ShortAlphaNumericProductName = "DiskSnapshot";
    public const string CompanyName = "Beckman Coulter";

    /// <summary> This is where the database and incident logs reside. </summary>
    public static string BaseAppDataDir
    {
      get
      {
        // ProgramData/Beckman Coulter/DiskSnapshot
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        return System.IO.Path.Combine(System.IO.Path.Combine(appDataDir, CompanyName), ShortAlphaNumericProductName);
      }
    }

    void LoadDirectoryStructure(DriveInfo drive)
    {
      LoadDirectoryStructure(drive.Name.Substring(0,1), drive.Name);
    }
    void LoadDirectoryStructure(string drive, string rootDirectory)
    {
      DirectoryStructure ds;
      string savedFile = System.IO.Path.Combine(BaseAppDataDir, string.Format("{0}.snapshot", drive));
      if (System.IO.File.Exists(savedFile))
      {
        var s = new XmlSerializer(typeof (DirectoryStructure));
        using (var stream = System.IO.File.OpenRead(savedFile))
          ds = (DirectoryStructure) s.Deserialize(stream);
      }
      else
      {
        ds = new DirectoryStructure(drive, rootDirectory);
      }
      DirectoryStructure = ds;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
      DirectoryStructure.Root.Snapshot();
      string savedFile = System.IO.Path.Combine(BaseAppDataDir, string.Format("{0}.snapshot", this.DirectoryStructure.Drive));
      System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savedFile));
      var s = new XmlSerializer(typeof(DirectoryStructure));
      using (var stream = System.IO.File.Create(savedFile))
        s.Serialize(stream, DirectoryStructure);
      MessageBox.Show(this, "Saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnSelectDrive_Click(object sender, RoutedEventArgs e)
    {
      IsSelectingDrive = true;
    }

    private void BtnOKDrive_Click(object sender, RoutedEventArgs e)
    {
      var drive = CbxDrives.SelectedItem as DriveInfo;
      if (drive != null)
        LoadDirectoryStructure(drive);
      IsSelectingDrive = false;
    }

    private void BtnCancelDrive_Click(object sender, RoutedEventArgs e)
    {
      IsSelectingDrive = false;
    }

    private void BtnScan_Click(object sender, RoutedEventArgs e)
    {
      Working = true;
      HeartbeatTimer.Start();
      StartTime = DateTime.Now;
      Scanner.Refresh(DirectoryStructure, dir => ProgressDirectory = dir, () =>
      {
        HeartbeatTimer.Stop();
        DirectoryStructure.Update();
        Working = false;
      });
    }
  }
}
