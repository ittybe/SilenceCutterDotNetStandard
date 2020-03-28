using System;
using System.Collections.Generic;
using System.Text;
using SilenceCutter.Detecting;
using SilenceCutter.VideoManipulating;
using SilenceCutter.VideoPartNaming;

namespace SilenceCutter
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






    }
}
