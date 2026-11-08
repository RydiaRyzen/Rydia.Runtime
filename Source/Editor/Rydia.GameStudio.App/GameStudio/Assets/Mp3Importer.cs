using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rydia.Engine;

namespace Rydia.GameStudio.Assets
{

    /// <summary>
    /// .aiff
    /// .aif
    /// .aifc
    /// .mp2
    /// .mp3
    /// .flac
    /// .wavに対応している<see cref="IResourceImporter{AudioData}"/>です
    /// </summary>
    public class Mp3Importer : IResourceImporter<AudioData>
    {

        private static MethodInfo LoadMethod;

        public Mp3Importer()
        {
            string nativeAssemblyName;
            string nativeAssemblyPath;
            if (Environment.Is64BitProcess)
            {
                nativeAssemblyName = @"SoXWrapper_x64.dll";
                nativeAssemblyPath = Path.GetFullPath(Path.Combine("x64", nativeAssemblyName));
            }
            else
            {
                nativeAssemblyName = @"SoXWrapper_x86.dll";
                nativeAssemblyPath = Path.GetFullPath(Path.Combine("x86", nativeAssemblyName));
            }
            Assembly nativeAssembly = Assembly.LoadFile(nativeAssemblyPath);
            Type wrapperClass = nativeAssembly.GetType("SoXWrapper.SoXWrapper");
            if (wrapperClass == null)
            {
                throw new InvalidOperationException("Failed to find required class in assembly: " + nativeAssemblyPath);
            }

            // Find Load method in SoXWrapper class
            LoadMethod = wrapperClass.GetMethod("LoadAudioAsVorbisStream");
            if (LoadMethod == null)
            {
                throw new InvalidOperationException("Failed to find required method in assembly: " + nativeAssemblyPath);
            }
        }

        public bool TryImport(byte[] data, out AudioData resource)
        {
            try
            {
                DecodeMp3(data, out resource);
                return true;
            }
            catch
            {
                resource = null;
                return false;
            }
        }

        private void DecodeMp3(byte[] data, out AudioData audioData)
        {
            var tempPath = Path.GetTempPath();
            tempPath = Path.Combine(tempPath, "Temp45963.mp3");
            File.WriteAllBytes(tempPath, data);
            byte[] oggData;
            try
            {
                oggData = LoadMethod.Invoke(null, new object[] { tempPath }) as byte[];
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke LoadAudioAsVorbisStream method.", e);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
            var importer = new OggImporter();
            importer.TryImport(oggData, out audioData);
        }

    }

}
