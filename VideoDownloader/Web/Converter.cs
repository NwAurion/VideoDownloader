using MediaToolkit;
using MediaToolkit.Model;

namespace VideoDownloader.Web
{
    class Converter
    {
        public static void DoConvert(string ToFileType)
        {
            string PathAndName = VideoFile.Path + VideoFile.Name;
            MediaFile inputFile = new MediaFile(PathAndName + VideoFile.Extension);
            MediaFile outputFile = new MediaFile(PathAndName + ToFileType);

            using (var engine = new Engine())
            {
                engine.Convert(inputFile, outputFile);
            }
        }
    }

}
