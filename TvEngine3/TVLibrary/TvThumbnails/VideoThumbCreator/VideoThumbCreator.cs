﻿#region Copyright (C) 2005-2010 Team MediaPortal

// Copyright (C) 2005-2010 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using TvLibrary.Log;

namespace TvThumbnails.VideoThumbCreator
{
  public static class VideoThumbCreator
  {
    private static string _extractApp = "ffmpeg.exe";

    private static readonly string _extractorPath = ExtractorPath();

    private static int _previewColumns = 2;

    private static int _previewRows = 2;

    private static bool _leaveShareThumb = false;

    public static string ExtractorPath()
    {
      string currentPath = Assembly.GetCallingAssembly().Location;

      FileInfo currentPathInfo = new FileInfo(currentPath);

      return currentPathInfo.DirectoryName + "\\" + _extractApp;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static bool CreateVideoThumb(string aVideoPath, string aThumbPath, bool aOmitCredits)
    {
      if (!File.Exists(aVideoPath))
      {
        Log.Info("VideoThumbCreator: File {0} not found!", aVideoPath);
        return false;
      }

      if (!File.Exists(_extractorPath))
      {
        Log.Info("VideoThumbCreator: No {0} found to generate thumbnails of your video!", _extractorPath);
        return false;
      }

      //TODO Blacklist stuff
      /*bool isCachedThumbBlacklisted = IsCachedThumbBlacklisted(aVideoPath);
      IVideoThumbBlacklist blacklist = GlobalServiceProvider.Get<IVideoThumbBlacklist>();
      if (blacklist != null && blacklist.Contains(aVideoPath))
        if (isCachedThumbBlacklisted)
        {
          Log.Info("Skipped creating thumbnail for {0}, it has been blacklisted because last attempt failed", aVideoPath);
          return false;
        }*/

      int preGapSec = 5;
      int postGapSec = 5;

      if (aOmitCredits)
      {
        preGapSec = 420;
        postGapSec = 600;
      }

      bool Success = false;
      string strFilenamewithoutExtension = Path.ChangeExtension(aVideoPath, null);
      int width = (int)Thumbs.ThumbLargeResolution;

      string ffmpegArgs =
        string.Format("select=isnan(prev_selected_t)+gte(t-prev_selected_t\\,5),") +
        string.Format("yadif=0:-1:0,") +
        string.Format("scale={0}:{1},", width, -1) +
        string.Format("setsar=1:1,") +
        string.Format("tile={0}x{1}", _previewColumns, _previewRows);

      string ExtractorArgs =
        string.Format("-loglevel quiet -ss {0} ", preGapSec) +
        string.Format("-i \"{0}\" ", aVideoPath) + 
        string.Format("-vf {0} ", ffmpegArgs) + 
        string.Format("-vframes 1 -vsync 0 ") +
        string.Format("-an \"{0}_s.jpg\"", strFilenamewithoutExtension);

      string ExtractorFallbackArgs =
        string.Format("-loglevel quiet -ss 5 ") +
        string.Format("-i \"{0}\" ", aVideoPath) +
        string.Format("-vf {0} ", ffmpegArgs) +
        string.Format("-vframes 1 -vsync 0 ") +
        string.Format("-an \"{0}_s.jpg\"", strFilenamewithoutExtension);

      try
      {
        // Use this for the working dir to be on the safe side
        string TempPath = Path.GetTempPath();
        string OutputThumb = string.Format("{0}_s{1}", Path.ChangeExtension(aVideoPath, null), ".jpg");
        string ShareThumb = OutputThumb.Replace("_s.jpg", ".jpg");

        if ((_leaveShareThumb && !File.Exists(ShareThumb)) // No thumb in share although it should be there
            || (!_leaveShareThumb && !File.Exists(aThumbPath))) // No thumb cached and no chance to find it in share
        {
          //Log.Debug("VideoThumbCreator: No thumb in share {0} - trying to create one with arguments: {1}", ShareThumb, ExtractorArgs);
          Success = StartProcess(_extractorPath, ExtractorArgs, TempPath, 15000, true, GetMtnConditions());
          if (!Success)
          {
            // Maybe the pre-gap was too large or not enough sharp & light scenes could be caught
            Thread.Sleep(100);
            Success = StartProcess(_extractorPath, ExtractorFallbackArgs, TempPath, 30000, true, GetMtnConditions());
            if (!Success)
              Log.Info("VideoThumbCreator: {0} has not been executed successfully with arguments: {1}", _extractApp,
                       ExtractorFallbackArgs);
          }
          // give the system a few IO cycles
          Thread.Sleep(100);
          // make sure there's no process hanging
          KillProcess(Path.ChangeExtension(_extractApp, null));
          try
          {
            // remove the _s which mdn appends to its files
            File.Move(OutputThumb, ShareThumb);
          }
          catch (FileNotFoundException)
          {
            Log.Info("VideoThumbCreator: {0} did not extract a thumbnail to: {1}", _extractApp, OutputThumb);
          }
          catch (Exception)
          {
            try
            {
              // Clean up
              File.Delete(OutputThumb);
              Thread.Sleep(50);
            }
            catch (Exception)
            {
            }
          }
        }
        else
        {
          // We have a thumbnail in share but the cache was wiped out - make sure it is recreated
          if (_leaveShareThumb && !File.Exists(aThumbPath)) // && !File.Exists(aThumbPath))
            Success = true;
        }

        Thread.Sleep(30);

        if (File.Exists(ShareThumb))
        {
          if (Success)
          {
            CreateThumbnail(ShareThumb, aThumbPath, 400, 400, 0);
            //CreateThumbnail(ShareThumb, Utils.ConvertToLargeCoverArt(aThumbPath),
            //                        (int)Thumbs.ThumbLargeResolution, (int)Thumbs.ThumbLargeResolution, 0, false);
          }

          if (!_leaveShareThumb)
          {
            try
            {
              File.Delete(ShareThumb);
              Thread.Sleep(30);
            }
            catch (Exception)
            {
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("VideoThumbCreator: Thumbnail generation failed - {0}!", ex.ToString());
      }
      if (File.Exists(aThumbPath))
      {
        return true;
      }
      else
      {
        //TODO Blacklist stuff
        //if (blacklist != null)
        //{
        //  blacklist.Add(aVideoPath);
        //}
        //AddCachedThumbToBlacklist(aVideoPath);                
        return false;
      }
    }

    /// <summary>
    /// Interface for video thumbnails blacklisting
    /// </summary>
    public interface IVideoThumbBlacklist
    {
      bool Add(string path);
      bool Remove(string path);
      bool Contains(string path);
      void Clear();
    }


    /// <summary>
    /// Creates a thumbnail of the specified image
    /// </summary>
    /// <param name="aDrawingImage">The source System.Drawing.Image</param>
    /// <param name="aThumbTargetPath">Filename of the thumbnail to create</param>
    /// <param name="aThumbWidth">Maximum width of the thumbnail</param>
    /// <param name="aThumbHeight">Maximum height of the thumbnail</param>
    /// <param name="aRotation">
    /// 0 = no rotate
    /// 1 = rotate 90 degrees
    /// 2 = rotate 180 degrees
    /// 3 = rotate 270 degrees
    /// </param>
    /// <param name="aFastMode">Use low quality resizing without interpolation suitable for small thumbnails</param>
    /// <returns>Whether the thumb has been successfully created</returns>
    private static bool CreateThumbnail(Image aDrawingImage, string aThumbTargetPath, int aThumbWidth, int aThumbHeight,
                                        int aRotation, bool aFastMode)
    {
      if (string.IsNullOrEmpty(aThumbTargetPath) || aThumbHeight <= 0 || aThumbHeight <= 0) return false;

      Bitmap myBitmap = null;
      Image myTargetThumb = null;

      try
      {
        switch (aRotation)
        {
          case 1:
            aDrawingImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            break;
          case 2:
            aDrawingImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            break;
          case 3:
            aDrawingImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            break;
          default:
            break;
        }

        int iWidth = aThumbWidth;
        int iHeight = aThumbHeight;
        float fAR = (aDrawingImage.Width) / ((float)aDrawingImage.Height);

        if (aDrawingImage.Width > aDrawingImage.Height)
          iHeight = (int)Math.Floor((((float)iWidth) / fAR));
        else
          iWidth = (int)Math.Floor((fAR * ((float)iHeight)));

        /*try
        {
          Utils.FileDelete(aThumbTargetPath);
        }
        catch (Exception ex)
        {
          Log.Error("Picture: Error deleting old thumbnail - {0}", ex.Message);
        }*/

        if (aFastMode)
        {
          Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
          myBitmap = new Bitmap(aDrawingImage, iWidth, iHeight);
          myTargetThumb = myBitmap.GetThumbnailImage(iWidth, iHeight, myCallback, IntPtr.Zero);
        }
        else
        {
          myBitmap = new Bitmap(iWidth, iHeight, aDrawingImage.PixelFormat);
          //myBitmap.SetResolution(aDrawingImage.HorizontalResolution, aDrawingImage.VerticalResolution);
          using (Graphics g = Graphics.FromImage(myBitmap))
          {
            g.CompositingQuality = CompositingQuality.Default;
            g.InterpolationMode = InterpolationMode.Default;
            g.SmoothingMode = SmoothingMode.Default;
            g.DrawImage(aDrawingImage, new Rectangle(0, 0, iWidth, iHeight));
            myTargetThumb = myBitmap;
          }
        }

        return SaveThumbnail(aThumbTargetPath, myTargetThumb);
      }
      catch (Exception)
      {
        return false;
      }
      finally
      {
        if (myTargetThumb != null)
          myTargetThumb.Dispose();
        if (myBitmap != null)
          myBitmap.Dispose();
      }
    }

    private static bool SaveThumbnail(string aThumbTargetPath, Image myImage)
    {
      try
      {
        myImage.Save(aThumbTargetPath);
        return true;
      }
      catch (Exception ex)
      {
        Log.Error("Picture: Error saving new thumbnail {0} - {1}", aThumbTargetPath, ex.Message);
        return false;
      }
    }

    private static bool ThumbnailCallback()
    {
      return false;
    }

    /// <summary>
    /// Creates a thumbnail of the specified image filename
    /// </summary>
    /// <param name="aInputFilename">The source filename to load a System.Drawing.Image from</param>
    /// <param name="aThumbTargetPath">Filename of the thumbnail to create</param>
    /// <param name="aThumbWidth">Maximum width of the thumbnail</param>
    /// <param name="aThumbHeight">Maximum height of the thumbnail</param>
    /// <param name="aRotation">
    /// 0 = no rotate
    /// 1 = rotate 90 degrees
    /// 2 = rotate 180 degrees
    /// 3 = rotate 270 degrees
    /// </param>
    /// <param name="aFastMode">Use low quality resizing without interpolation suitable for small thumbnails</param>
    /// <returns>Whether the thumb has been successfully created</returns>
    private static bool CreateThumbnail(string aInputFilename, string aThumbTargetPath, 
                                        int iMaxWidth, int iMaxHeight, int iRotate)
    {
      if (string.IsNullOrEmpty(aInputFilename) || string.IsNullOrEmpty(aThumbTargetPath) || iMaxHeight <= 0 ||
          iMaxHeight <= 0) return false;

      if (!File.Exists(aInputFilename)) return false;

      Image myImage = null;

      try
      {
        myImage = ImageFast.FromFile(aInputFilename);
        return CreateThumbnail(myImage, aThumbTargetPath, iMaxWidth, iMaxHeight, iRotate, true);
      }
      catch (ArgumentException)
      {
        Log.Info("Picture: Fast loading of thumbnail {0} failed - trying safe fallback now", aInputFilename);

        try
        {
          myImage = Image.FromFile(aInputFilename, true);
          return CreateThumbnail(myImage, aThumbTargetPath, iMaxWidth, iMaxHeight, iRotate, true);
        }
        catch (OutOfMemoryException)
        {
          Log.Info("Picture: Creating thumbnail failed - image format is not supported of {0}", aInputFilename);
          return false;
        }
        catch (Exception ex)
        {
          Log.Error("Picture: CreateThumbnail exception err:{0} stack:{1}", ex.Message, ex.StackTrace);
          return false;
        }
      }
      finally
      {
        if (myImage != null)
          myImage.Dispose();
      }
    }

    private static void KillProcess(string aProcessName)
    {
      try
      {
        Process[] leftovers = System.Diagnostics.Process.GetProcessesByName(aProcessName);
        foreach (Process termProc in leftovers)
        {
          try
          {
            Log.Info("Util: Killing process: {0}", termProc.ProcessName);
            termProc.Kill();
          }
          catch (Exception exk)
          {
            Log.Error("Util: Error stopping processes - {0})", exk.ToString());
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("Util: Error getting processes by name for {0} - {1})", aProcessName, ex.ToString());
      }
    }


    private static ProcessFailedConditions GetMtnConditions()
    {
      ProcessFailedConditions mtnStat = new ProcessFailedConditions();
      // The input file is shorter than pre- and post-recording time
      mtnStat.AddCriticalOutString("net duration after -B & -E is negative");
      mtnStat.AddCriticalOutString("all rows're skipped?");
      mtnStat.AddCriticalOutString("step is zero; movie is too short?");
      mtnStat.AddCriticalOutString("failed: -");
      // unsupported video format by mtn.exe - maybe there's an update?
      mtnStat.AddCriticalOutString("couldn't find a decoder for codec_id");

      mtnStat.SuccessExitCode = 0;

      return mtnStat;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private static bool StartProcess(string aAppName, string aArguments, string aWorkingDir, int aExpectedTimeoutMs,
                                     bool aLowerPriority, ProcessFailedConditions aFailConditions)
    {
      bool success = false;
      Process ExternalProc = new Process();
      ProcessStartInfo ProcOptions = new ProcessStartInfo(aAppName, aArguments);

      ProcOptions.UseShellExecute = false; // Important for WorkingDirectory behaviour
      ProcOptions.RedirectStandardError = true; // .NET bug? Some stdout reader abort to early without that!
      ProcOptions.RedirectStandardOutput = true; // The precious data we're after
      //ProcOptions.StandardOutputEncoding = Encoding.GetEncoding("ISO-8859-1"); // the output contains "Umlaute", etc.
      //ProcOptions.StandardErrorEncoding = Encoding.GetEncoding("ISO-8859-1");
      ProcOptions.WorkingDirectory = aWorkingDir; // set the dir because the binary might depend on cygwin.dll
      ProcOptions.CreateNoWindow = true; // Do not spawn a "Dos-Box"      
      ProcOptions.ErrorDialog = false; // Do not open an error box on failure        

      ExternalProc.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
      ExternalProc.ErrorDataReceived += new DataReceivedEventHandler(ErrorDataHandler);
      ExternalProc.EnableRaisingEvents = true; // We want to know when and why the process died        
      ExternalProc.StartInfo = ProcOptions;
      if (File.Exists(ProcOptions.FileName))
      {
        try
        {
          ExternalProc.Start();
          if (aLowerPriority)
          {
            try
            {
              ExternalProc.PriorityClass = ProcessPriorityClass.BelowNormal;
              // Execute all processes in the background so movies, etc stay fluent
            }
            catch (Exception ex2)
            {
              Log.Error("Util: Error setting process priority for {0}: {1}", aAppName, ex2.Message);
            }
          }
          // Read in asynchronous  mode to avoid deadlocks (if error stream is full)
          // http://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.redirectstandarderror.aspx
          ExternalProc.BeginErrorReadLine();
          ExternalProc.BeginOutputReadLine();

          // wait this many seconds until the process has to be finished
          ExternalProc.WaitForExit(aExpectedTimeoutMs);

          success = (ExternalProc.HasExited && ExternalProc.ExitCode == aFailConditions.SuccessExitCode);

          ExternalProc.OutputDataReceived -= new DataReceivedEventHandler(OutputDataHandler);
          ExternalProc.ErrorDataReceived -= new DataReceivedEventHandler(ErrorDataHandler);
        }
        catch (Exception ex)
        {
          Log.Error("Util: Error executing {0}: {1}", aAppName, ex.Message);
        }
      }
      else
        Log.Info("Util: Could not start {0} because it doesn't exist!", ProcOptions.FileName);

      return success;
    }

    private static void OutputDataHandler(object sendingProcess,
                                          DataReceivedEventArgs outLine)
    {
      if (!String.IsNullOrEmpty(outLine.Data))
      {
        Log.Info("Util: StdOut - {0}", outLine.Data);
      }
    }

    private static void ErrorDataHandler(object sendingProcess,
                                         DataReceivedEventArgs errLine)
    {
      if (!String.IsNullOrEmpty(errLine.Data))
      {
        Log.Info("Util: StdErr - {0}", errLine.Data);
      }
    }
  }

  internal class ProcessFailedConditions
  {
    public List<string> CriticalOutputLines = new List<string>();
    public int SuccessExitCode = 0;

    internal void AddCriticalOutString(string aFailureString)
    {
      CriticalOutputLines.Add(aFailureString);
    }
  }
}