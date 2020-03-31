using System;
using System.Collections.Generic;
using System.Text;
using SilenceCutter.Detecting;
using SilenceCutter.VideoManipulating;
using SilenceCutter.VideoPartNaming;
using Xabe.FFmpeg.Downloader;
using System.IO;

namespace SilenceCutter.VideoManipulating
{
    /// <summary>
    /// main class that uses all instrument of this package
    /// it can cut silence(noise), speed up silence(noise)
    /// </summary>
    public class VideoRedactor
    {
        /// <summary>
        /// string to mark noise parts
        /// </summary>
        public string NoiseMark { get; set; }
        /// <summary>
        /// string to mark silence parts 
        /// </summary>
        public string SilenceMark { get; set; }
        /// <summary>
        /// prefer extension for video parts in temp dir
        /// and for output video
        /// </summary>
        public string PreferExtension { get; set; }
        /// <summary>
        /// Temp directory for video parts
        /// </summary>
        public string TempDir { get; set; }
        /// <summary>
        /// input path for raw video
        /// </summary>
        public string InputPath { get; set; }
        /// <summary>
        /// output path for edited video
        /// </summary>
        public string OutputPath { get; set; }

        private List<TimeLineVolume> detectedTime;
        /// <summary>
        /// Detected Timelines in video
        /// </summary>
        public List<TimeLineVolume> DetectedTime
        {
            get { return detectedTime; }
            protected set { detectedTime = value; }
        }

        /// <summary>
        /// get container for parts container
        /// WARNING: every get create new VideoPartsContainer (for sync List with this container)
        /// </summary>
        public VideoPartsContainer PartsContainer 
        {
            get 
            {
                return new VideoPartsContainer(DetectedTime, TempDir, PreferExtension, NoiseMark, SilenceMark);
            }
        }

        // detecting staff
        private VolumeDetector volumeDetector;

        // video staff
        private VideoMerger videoMerger;
        private VideoSplitter videoSplitter;
        private VideoSpeedManipulator speedManipulator;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="inputPath">input video path</param>
        /// <param name="outputPath">output video path</param>
        /// <param name="tempDir">temp directory for video parts</param>
        /// <param name="preferExtension">prefer extension</param>
        /// <param name="noiseMark">noise mark for noises parts</param>
        /// <param name="silenceMark">silence mark for silence parts</param>
        public VideoRedactor(string inputPath, string outputPath, string tempDir, string preferExtension, string noiseMark, string silenceMark) 
        {
            InputPath = inputPath;
            OutputPath = outputPath;

            TempDir = tempDir;
            
            PreferExtension = preferExtension;
            
            NoiseMark = noiseMark;
            SilenceMark = silenceMark;

            PreferExtension = preferExtension;

            volumeDetector = new VolumeDetector(inputPath);
        }
        /// <summary>
        /// you have to call this method, because we can't cut or speed up noise or silence with out detecting
        /// </summary>
        /// <param name="amplitudeSilenceThreshold">silence amplitude</param>
        /// <param name="Millisec">millisec to calc average amlitude of block of samples</param>
        /// <param name="millisecExtension">after detecting we will add to noise this number of millisec</param>
        public void DetectVolume(float amplitudeSilenceThreshold, int Millisec, int millisecExtension) 
        {
            volumeDetector.DetectVolume(amplitudeSilenceThreshold, Millisec, millisecExtension);
            
            DetectedTime = volumeDetector.DetectedTime;

            videoSplitter = new VideoSplitter(DetectedTime, TempDir, NoiseMark, SilenceMark, InputPath);
            speedManipulator = new VideoSpeedManipulator(DetectedTime, TempDir, NoiseMark, SilenceMark);
            videoMerger = new VideoMerger(DetectedTime, TempDir, NoiseMark, SilenceMark, OutputPath);
        }

        /// <summary>
        /// cut from video 
        /// </summary>
        /// <param name="volume">volume level (silence, noise) to cut out</param>
        public void Cut(VolumeValue volume) 
        {
            DetectedTime.RemoveAll(timeLine => timeLine.Volume == volume);
        }

        /// <summary>
        /// start conversion
        /// WARNING: if you set SPEED param more than ~ 10 (depends of the millisec arg in DetectVolume method), it will corrupt the output final video
        /// if you want to cut silence out , We recommended method Cut in this instance
        /// </summary>
        /// <param name="silenceSpeed">silence speed. WARNING: if you set this param more than ~ 10 (depends of the millisec arg in DetectVolume method), it will corrupt the output final video</param>
        /// <param name="noiseSpeed">noise speed. WARNING: if you set this param more than ~ 10 (depends of the millisec arg in DetectVolume method), it will corrupt the output final video</param>
        /// <param name="processNumber">number of process for one time</param>
        public void Start(double silenceSpeed, double noiseSpeed, int processNumber) 
        {
            videoSplitter.SplitVideo(PreferExtension, processNumber);
            speedManipulator.ChangeSpeed(silenceSpeed, noiseSpeed, PreferExtension);
            videoMerger.MergeVideo(PreferExtension);
        }
        /// <summary>
        /// this method should help to choose right threshold amplitude for method VolumeDetect
        /// </summary>
        /// <returns>max abs amplitude in audio stream</returns>
        public float GetMaxAbsAmplitudeInAudioStream() 
        {
            return volumeDetector.GetMaxAbsAmplitude();
        }
        /// <summary>
        /// remove temp dir with all contain files
        /// </summary>
        public void RemoveTempDir() 
        {
            DirectoryInfo tempDir = new DirectoryInfo(TempDir);
            tempDir.Delete(true);
        }
    }
}
