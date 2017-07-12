using System;
using System.Collections.Generic;
using YoutubeExtractor;

namespace ExampleApplication.Portable
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Run();

            Console.ReadLine();
        }

        private static async void Run()
        {
            IEnumerable<VideoInfo> videoInfos = await DownloadUrlResolver.GetDownloadUrlsAsync("https://www.youtube.com/watch?v=9bZkp7q19f0");

            foreach (VideoInfo videoInfo in videoInfos)
            {
                Console.WriteLine(videoInfo.DownloadUrl);
                Console.WriteLine();
            }
        }
    }
}