using System;
using System.Collections.Generic;
using System.Text;

using Xabe.FFmpeg.Streams;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;

using System.Threading.Tasks;

namespace SilenceCutter.VideoManipulating
{
    /// <summary>
    /// Start 
    /// </summary>
    public class ConversionQueueWait
    {
        private List<IConversion> conversionList;

        public ConversionQueueWait() 
        {
            conversionList = new List<IConversion>();
        }
        /// <summary>
        /// add to list conversion
        /// </summary>
        /// <param name="conversion">conversion object</param>
        public void Add(IConversion conversion) 
        {
            conversionList.Add(conversion);
        }
        /// <summary>
        /// clear conversions
        /// </summary>
        public void Clear() 
        {
            conversionList.Clear();
        }

        private void StartConversionWait(IConversion conversion) 
        {
            conversion.Start().Wait();
        }
        /// <summary>
        /// start conversion
        /// </summary>
        public void Start() 
        {
            Parallel.ForEach(conversionList, StartConversionWait);
        }
    }
}
