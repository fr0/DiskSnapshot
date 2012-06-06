using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace DiskSnapshot
{
  [Serializable]
  public class DirectoryStructure
  {
    public DirectoryStructure()
    {
    }
    public DirectoryStructure(string drive, string rootDirectory)
    {
      Drive = drive;
      Root = new DirectoryStructureEntry {DirectoryName = rootDirectory};
    }

    public string Drive { get; set; }
    public DirectoryStructureEntry Root { get; set; }
    public double LastScanTimeInSeconds { get; set; }

    public void Refresh(Action<string> progress)
    {
      Root.Refresh("", 0, progress);
    }

    public void Update()
    {
      Root.Update();
    }
  }

  [Serializable]
  public class DirectoryStructureEntry : INotifyPropertyChanged
  {
    public DirectoryStructureEntry()
    {
      Children = new List<DirectoryStructureEntry>();
      State = DirectoryState.Unchanged;
    }

    public string DirectoryName { get; set; }
    public DirectoryState State { get; set; }
    public long PreviousSize { get; set; }
    public long CurrentSize { get; set; }
    public List<DirectoryStructureEntry> Children { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void Update()
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(null));
      foreach (var child in Children)
        child.Update();
    }

    [XmlIgnore]
    public DirectorySizeComparison SizeComparison
    {
      get
      {
        if (PreviousSize > CurrentSize)
          return DirectorySizeComparison.Smaller;
        else if (PreviousSize < CurrentSize)
          return DirectorySizeComparison.Larger;
        else
          return DirectorySizeComparison.Smaller;
      }
    }
    [XmlIgnore]
    public string CurrentSizeString
    {
      get { return SizeHelpers.ToDisplayString(CurrentSize); }
    }
    [XmlIgnore]
    public string DeltaString
    {
      get { return SizeHelpers.ToDisplayString(CurrentSize - PreviousSize); }
    }

    public void Snapshot()
    {
      foreach (var child in new List<DirectoryStructureEntry>(Children))
      {
        if (child.State == DirectoryState.Deleted)
          Children.Remove(child);
      }
      foreach (var child in Children)
        child.Snapshot();
      State = DirectoryState.Unchanged;
      PreviousSize = CurrentSize;
    }


    public void Refresh(string parentPath, int level, Action<string> progress, bool isNew=false)
    {
      string fullPath = Path.Combine(parentPath, DirectoryName);
      bool exists = Directory.Exists(fullPath);
      if (exists)
      {
        if (level < 3)
          progress(fullPath);
        Children = new List<DirectoryStructureEntry>(Children.OrderBy(x => x.DirectoryName, LogicalStringComparer.Comparer));
        long size = Directory.GetFiles(fullPath).Select(file => new FileInfo(file)).Select(info => info.Length).Sum();
        foreach (var child in Children)
        {
          child.Refresh(fullPath, level+1, progress, isNew);
          size += child.CurrentSize;
        }
        foreach (var dir in Directory.GetDirectories(fullPath).Where(x => !IsLink(x)))
        {
          string dirName = Path.GetFileName(dir);
          if (!Children.Any(c => string.Compare(c.DirectoryName, dirName, true) == 0))
          {
            var child = new DirectoryStructureEntry {DirectoryName = dirName, State = DirectoryState.Added};
            try
            {
              child.Refresh(fullPath, level+1, progress, true);
              size += child.CurrentSize;
              Children.AddSorted(child, (x,y) => LogicalStringComparer.Comparer.Compare(x.DirectoryName, y.DirectoryName));
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
          }
        }
        this.CurrentSize = size;
        if (isNew)
          State = DirectoryState.Added;
        else if (CurrentSize == PreviousSize)
          State = DirectoryState.Unchanged;
        else
          State = DirectoryState.Changed;
      }
      else
      {
        this.Children.Clear();
        this.CurrentSize = 0;
        this.State = DirectoryState.Deleted;
      }
    }

    public static bool IsLink(string shortcutFilename)
    {
      try
      {
        string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
        string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

        Shell32.Shell shell = new Shell32.Shell();
        Shell32.Folder folder = shell.NameSpace(pathOnly);
        Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
        if (folderItem != null)
        {
          return folderItem.IsLink;
        }
        return false; // not found
      }
      catch (Exception)
      {
        return false;
      }
    }
  }

  public enum DirectoryState
  {
    Unchanged,
    Changed,
    Added,
    Deleted
  }
  public enum DirectorySizeComparison
  {
    Same,
    Smaller,
    Larger
  }

  public static class SizeHelpers
  {
    public static long ToKB(long size)
    {
      return size/1024;
    }
    public static long ToMB(long size)
    {
      return size/(1024*1024);
    }
    public static string ToDisplayString(long size)
    {
      if (Math.Abs(size) < 1024)
        return string.Format("{0} b", size);
      else if (Math.Abs(size) < 1024 * 1024)
        return string.Format("{0} KB", ToKB(size));
      else
        return string.Format("{0} MB", ToMB(size));
    }
  }
}
