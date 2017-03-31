using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using YoutubeExtractor;
using System.Collections.Generic;

//IMPORTANT!!!!!! to download a vid, send http://www.youtubeinmp3.com/fetch/?video=<enter the link>

namespace socket_tester
{
    public static class SocketHandler
    {
        static string downloadPrefix = "/fetch/?video=";

        public static Socket accessSite(string url)
        {
	        Socket sc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	
	        IPAddress[] IPOptions = Dns.GetHostAddresses(url);

            Console.WriteLine(IPOptions[0]);

	        IPEndPoint endPoint = new IPEndPoint(IPOptions[0], 80);

            sc.Connect(endPoint);

            return sc;
        }


        public static void getSong(string youtubeLink)
        {
            List<VideoInfo> videoInfos = new List<VideoInfo>(DownloadUrlResolver.GetDownloadUrls(youtubeLink));
            VideoInfo optimalVid = videoInfos[0];
            int i;
            bool ok = true;
            for (i = 0; i < videoInfos.Count; i++)
            {
                if (videoInfos[i].CanExtractAudio && videoInfos[i].AudioBitrate >= 128)
                {
                    optimalVid = videoInfos[i];
                    break;
                }
            }
            if (i == videoInfos.Count)
                i = 0;

                /*
                 * If the video has a decrypted signature, decipher it
                 */
                

            /*
             * Create the audio downloader.
             * The first argument is the optimalVid where the audio should be extracted from.
             * The second argument is the path to save the audio file.
             */
            do
            {
                ok = true;
                optimalVid = videoInfos[i];
                if (optimalVid.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(optimalVid);
                }
                var audioDownloader = new AudioDownloader(optimalVid, Path.Combine("D:/Downloads", optimalVid.Title + optimalVid.AudioExtension));

                // Register the progress events. We treat the download progress as 85% of the progress and the extraction progress only as 15% of the progress,
                // because the download will take much longer than the audio extraction.
                audioDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
                audioDownloader.AudioExtractionProgressChanged += (sender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);

                /*
                 * Execute the audio downloader.
                 * For GUI applications note, that this method runs synchronously.
                 */
                try
                {
                    audioDownloader.Execute();
                }
                catch
                {
                    ok = false;
                    i++;
                }
            } while (!ok) ;
            
        }
    }
}


/*serverStart = response.IndexOf("Server: ") + 8;
serverEnd = response.IndexOf("CF-RAY: ") - 2;
string server = response.Substring(serverStart, serverEnd - serverStart);

cfStart = response.IndexOf()

getSong(redirection, sc, server);*/


/*private static void getSong(string youtubeLink, Socket sc, string suffix)
{
    int suffixStart;
    int redStart, redEnd;
    string getMessage = "GET "; //constructing the HTTP get packet.
    getMessage += downloadPrefix;
    getMessage += youtubeLink;
    getMessage += " HTTP/1.1\r\n";
    getMessage += suffix;
    getMessage += "\r\n\r\n";

    Console.WriteLine("\nTHE GET MESSGAE: ");
    Console.WriteLine(getMessage); //DEBUG ONLY

    byte[] buf = Encoding.Default.GetBytes(getMessage); //sending and receiving the response
    sc.Send(buf, 0, buf.Length, 0);
    byte[] rawResponse = new byte[9999];
    int rec = sc.Receive(rawResponse, 0, rawResponse.Length, 0);
    Array.Resize<byte>(ref rawResponse, rec);

    string response = Encoding.Default.GetString(rawResponse); //making a sring out of it to check for redirections

    Console.WriteLine(response); //DEBUG ONLY

    if (response.IndexOf("HTTP/1.1 302 Moved Temporarily") != -1)
    {
        redStart = response.IndexOf("Location: ") + 10;
        redEnd = response.IndexOf("Server: ") - 2;
        string redirection = response.Substring(redStart, redEnd - redStart);


        suffixStart = response.IndexOf("Server: ");
        string newSuffix = response.Substring(suffixStart, response.Length - suffixStart - 8);
        Console.WriteLine(newSuffix);

        getSong(redirection, sc, newSuffix);
    }
*/