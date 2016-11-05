using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VideoDownloader
{
    public class FileTypes
    {
        static List<string> Types;

        public static List<string> GetFileTypes()
        {
            if (Types == null)
            {
                Types = new List<string>();

                var extensions = Enum.GetValues(typeof(Extension));

                foreach (Extension ext in extensions)
                {
                    Types.Add(ext.ToString().Replace("_","."));
                }
            }

            return Types;
        }
        ///<remarks>
        /// Supported extensions. Description takem from ffmep -formats
        ///</remarks>
        enum Extension
        { 
            [Description("MP3 (MPEG audio layer 3)")]
            _mp3,
            [Description("MP4 (MPEG-4 Part 14)")]
            _mp4,
            [Description("raw MPEG-4 video")]
            _m4v,
            [Description("Ogg")]
            _ogg,
            [Description("WebM")]
            _webm,
            [Description("MPEG-2 PS (VOB)")]
            _vob,
            /* not working: flv
            [Description("FLV (Flash Video)")]
            _flv,*/
            /* not working: 4xm
            [Description("4X Technologies")]
            _4xm,*/
            /* not working: aa
            [Description("Audible AA format files")]
            _aa,*/
            [Description("raw ADTS AAC (Advanced Audio Coding)")]
            _aac,
            [Description("raw FLAC")]
            _flac
        }

        /* Muxing only
              // _3dostr          3DO STR
        [Description("3GP2 (3GPP2 file format)")]
        3g2
        [Description("3GP (3GPP file format)")]
        3gp
        [Description("4X Technologies")]
        4xm   
        [Description("a64 - video for Commodore 64")}
        a64        
        */
    }
}
