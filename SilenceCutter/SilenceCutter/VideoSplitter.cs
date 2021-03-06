﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg.Streams;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;

using SilenceCutter.Detecting;
using SilenceCutter.VideoPartNaming;
using Xabe.FFmpeg.Model;

using System.IO;
using Xabe.FFmpeg.Events;

namespace SilenceCutter.VideoManipulating
{
    /// <summary>
    /// split video by volume level
    /// </summary>
    public class VideoSplitter : VideoManipulator
    {
        /// <summary>
        /// temp directory for video parts
        /// </summary>
        public DirectoryInfo TempDir
        {
            get
            {
                return base.tempDir;
            }
            set
            {
                tempDir = value;
            }
        }
        /// <summary>
        /// list of time line with definition of volume level
        /// </summary>
        public List<TimeLineVolume> DetectedTime
        {
            get
            {
                return detectedTime;
            }
            set
            {
                detectedTime = value;
            }
        }
        /// <summary>
        /// input path
        /// </summary>
        public string InputPath { get; set; }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputPath">input path to video for split</param>
        /// <param name="detectedTime">result of method SilenceCutter.Detecting.VolumeDetector.DetectVolumeLevel()</param>
        public VideoSplitter(List<TimeLineVolume> detectedTime, string tempDir, string noiseMark, string silenceMark, string inputPath): base(detectedTime, tempDir, noiseMark, silenceMark) 
        {
            DetectedTime = detectedTime;
            InputPath = inputPath;    
            if (!this.tempDir.Exists)
                this.tempDir.Create();
        }
        /// <summary>
        /// split video on part with only silent or noise
        /// </summary>
        /// <param name="OnProgressHandler">handler for event OnProgress IConvertion's object </param>
        /// <param name="PreferExtension">prefer extension for splited parts of video</param>
        /// <param name="processNumber">number of process for one time</param>
        public void SplitVideo(string PreferExtension, int processNumber, ConversionProgressEventHandler OnProgressHandler = null)
        {
            //VideoPartsContainer container = VideoPartNamesGenerator.GenerateNames(DetectedTime, TempDir, PreferExtension);
            VideoPartsContainer container = new VideoPartsContainer(DetectedTime, TempDir.FullName, PreferExtension, noiseMark, silenceMark);
            ConversionQueueWait conversionQueue = new ConversionQueueWait();
            for (int i = 0; i < DetectedTime.Count; i++)
            {
                string outputPath = container[i].FullName;

                // split video

                IConversion conversion = Conversion.Split(InputPath, outputPath, DetectedTime[i].Start, DetectedTime[i].Duration);

                conversion.OnProgress += OnProgressHandler;

                // wait for finishing of conversion
                conversionQueue.Add(conversion);
                if ((i % processNumber == 0 && i != 0) || i == DetectedTime.Count - 1) 
                {
                    conversionQueue.Start();
                    conversionQueue.Clear();
                }
            }
        }
        
        /// <summary>
        /// Clear temp directory
        /// </summary>
        public void ClearTempDir() 
        {
            foreach (FileInfo file in TempDir.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in TempDir.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        
        /// <summary>
        /// remove directory
        /// </summary>
        public void RemoveTempDir() 
        {
            TempDir.Delete(true);
        }
    }
}
