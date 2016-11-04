using System.IO;
using VideoLibrary;

namespace VideoDownloader.Web

{
    class Downloader
    {

        static string baseURL = "http://youtube.com/watch?v=";

        public static string SaveVideoToDisk(string videoID, string path)
        {
            var youTube = YouTube.Default; // starting point for YouTube actions
           
            var video = youTube.GetVideo(baseURL + videoID);
            // video.Start();
            // video.Wait();
            // video.GetAwaiter().OnCompleted()
            // var video = youTube.GetVideo(baseURL + videoID); // gets a Video object with info about the video
            string fileName = video.FullName.Substring(0, video.FullName.LastIndexOf("."));
            File.WriteAllBytes(path + video.FullName, video.GetBytes());
            return fileName;
        }
    }
}