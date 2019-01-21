using AviFile;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahaDel
{
    class Program
    {

        static string testWavFile = @"..\..\..\..\Files\sound.wav";
        static string testOutWavFile = @"..\..\..\..\Files\out.wav";
        static string testOutWavFile2 = @"..\..\..\..\Files\sound2.wav";
        static string testAviFile = @"..\..\..\..\Files\1.avi";


        static string filesDir = @"..\..\..\Files\";
        static string soundLibraryDir = @"..\..\..\Files\samples_from_library\";
        

       [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // ExtractSound();
                // ReadSound();
                // PlaySound();
                //MusicFileOperations.ReadAndWriteSound(testWavFile, testOutWavFile);
                //MusicFileOperations.ReadAndSaveSoundPart(testWavFile, Path.Combine(filesDir,"short.wav"), 15,30);

                // FouriesForthAndBack();
                MixFiles();
                
            }
            catch (Exception ex)
            {
                LogError("Ошибка при работе приложения: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            LogInfo("Нажмите любую клавишу");
            Console.ReadKey();
        }

        private static void MixFiles()
        {
            var file1 = Path.Combine(soundLibraryDir, @"background\12693__connum__melancholic-burble-in-f-major.mp3");
            var file2 = Path.Combine(soundLibraryDir, @"crowd_lough\25296__freesound__laughs-ses2.wav");
            var outFile = Path.Combine(filesDir, "out_mixed.wav");
            MusicFileOperations.OverlapFiles(file1, file2, 10, outFile);
        }

        private static void FouriesForthAndBack()
        {
            var soundArray = MusicFileOperations.GetArraysFromFile(Path.Combine(filesDir, "short.wav"));
            var math = new MathOperations();
            var outArray = new List<float[]>();
            //math.DoSomeFourier(soundArray, outArray);
            var res = math.DoFourierForthAndBack(soundArray);
            MusicFileOperations.SaveArraysToFile(Path.Combine(filesDir, "short_out.wav"), res);
        }

        /// <summary>
        /// имя лог файла
        /// </summary>
        static string logFileName = "";

        /// <summary>
        /// Запись ошибк в лог
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(string message)
        {
            var msg = "[ERROR]" + DateTime.Now.ToString("[HH:mm:ss]") + ": " + message;
            Console.WriteLine(msg);
            Write2FileLog(msg);
        }

        /// <summary>
        /// Запись информационного сообщения в лог
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(string message)
        {
            var msg = "[INFO]" + DateTime.Now.ToString("[HH:mm:ss]") + ": " + message;
            Console.WriteLine(msg);
            Write2FileLog(msg);
        }

        /// <summary>
        /// Логирование в файл
        /// </summary>
        /// <param name="msg"></param>
        private static void Write2FileLog(string msg)
        {
            if (logFileName == "")
            {
                if (!string.IsNullOrWhiteSpace(""))
                {
                    logFileName = DateTime.Now.ToString("yyyyMMdd_HHmm") + Path.GetFileNameWithoutExtension("") + ".log";
                }
                else
                {
                    logFileName = DateTime.Now.ToString("yyyyMMdd_HHmm") + ".log";
                }

            }
            File.AppendAllText(logFileName, msg + Environment.NewLine);
        }
    }
}
