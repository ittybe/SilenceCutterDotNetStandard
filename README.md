# SilenceCutterDotNetStandard
Silence Cutter .Net Standard  version 

example of using my package 

```
using System;
using SilenceCutter.Detecting;
using System.IO;
using Xabe.FFmpeg.Enums;
using System.Diagnostics;
using SilenceCutter.VideoManipulating;
namespace SilenceCutterDebugging
{
    class Program
    {
        static void Main(string[] Args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string inputPath = @"ENTER YOU PATH ";  
            string outputPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
            string tempDir = "TempDir";

            string preferExtension = FileExtensions.Mp4;

            string noiseMark = "N";
            string silenceMark = "S";

            double silenceSpeed = 8;
            double noiseSpeed = 1;

            int millisec = 500;
            int millisecExt = 100;

            VolumeValue ToCut = VolumeValue.Noise;

            VideoRedactor videoRedactor = new VideoRedactor(inputPath, outputPath, tempDir, preferExtension, noiseMark, silenceMark);
            float SilenceThreshold = Math.Abs(videoRedactor.GetMaxAbsAmplitudeInAudioStream() / 200);

            videoRedactor.DetectVolume(SilenceThreshold, millisec, millisecExt);
            //videoRedactor.Cut(ToCut);
            videoRedactor.Start(silenceSpeed, noiseSpeed, 1);
            videoRedactor.RemoveTempDir();
            Console.WriteLine($"output: {outputPath}");
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed.TotalSeconds);
        }
    }
}
