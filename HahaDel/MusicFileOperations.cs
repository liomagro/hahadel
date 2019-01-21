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
    class MusicFileOperations
    {

        static WaveOut waveOut;
        static readonly int SampleRate = 48000;
        static readonly int Channels = 2;

        public static void PlaySound(string testWavFile)
        {
            string testFile = testWavFile;
            if (!File.Exists(testFile)) { Program.LogError("No file " + testFile); return; }
            using (var reader = new AudioFileReader(testFile))
            {
                waveOut = new WaveOut();
                waveOut.Init(reader);
                waveOut.Play();
            }
        }

        /// <summary>
        /// Saving part of sound file
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outFile"></param>
        /// <param name="startSec"></param>
        /// <param name="endSec"></param>
        public static void ReadAndSaveSoundPart(string inFile, string outFile, int startSec, int endSec)
        {
            if (!File.Exists(inFile)) { Program.LogError("No file " + inFile); return; }
            using (var reader = new AudioFileReader(inFile))
            {
                Program.LogInfo("TotalTime:" + reader.TotalTime);
                Program.LogInfo("Length:" + reader.Length);
                Program.LogInfo("Volume:" + reader.Volume);
                var waveFormat = reader.WaveFormat;
                Program.LogInfo("SampleRate:" + waveFormat.SampleRate);
                Program.LogInfo("AverageBytesPerSecond:" + waveFormat.AverageBytesPerSecond);
                Program.LogInfo("BitsPerSample:" + waveFormat.BitsPerSample);
                Program.LogInfo("BlockAlign:" + waveFormat.BlockAlign);
                Program.LogInfo("Channels:" + waveFormat.Channels);
                Program.LogInfo("Encoding:" + waveFormat.Encoding);

                int valuesPerSecond = (int)(reader.Length / reader.TotalTime.TotalSeconds);
                Program.LogInfo("Values per second:" + valuesPerSecond);

                var resampler = new WdlResamplingSampleProvider(reader, 44100);
                var wp = new SampleToWaveProvider(reader);

                var rdr = wp.ToSampleProvider();


                var wf2 = rdr.WaveFormat;
                Program.LogInfo("SampleRate:" + wf2.SampleRate);
                Program.LogInfo("AverageBytesPerSecond:" + wf2.AverageBytesPerSecond);
                Program.LogInfo("BitsPerSample:" + wf2.BitsPerSample);
                Program.LogInfo("BlockAlign:" + wf2.BlockAlign);
                Program.LogInfo("Channels:" + wf2.Channels);
                Program.LogInfo("Encoding:" + wf2.Encoding);


                var wf = WaveFormat.CreateIeeeFloatWaveFormat(rdr.WaveFormat.SampleRate, rdr.WaveFormat.Channels);
                using (WaveFileWriter writer = new WaveFileWriter(outFile, wf))
                {
                    // 1 sec length buffer
                    var buf = new float[rdr.WaveFormat.SampleRate * rdr.WaveFormat.Channels];
                    int currentSec = 0;
                    while (true)
                    {
                        if (currentSec > endSec) break;

                        int read = rdr.Read(buf, 0, buf.Length);
                        if (read > 0)
                        {
                            if (currentSec > startSec)
                            {
                                writer.WriteSamples(buf, 0, read);
                            }
                        }
                        else
                            break;
                        currentSec++;
                    }
                }
            }
        }

        /// <summary>
        /// saving sound file from array of seconds
        /// </summary>
        /// <param name="file"></param>
        /// <param name="res"></param>
        internal static void SaveArraysToFile(string file, List<float[]> res)
        {
            // only one channel
            var wf = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 1);
            using (WaveFileWriter writer = new WaveFileWriter(file, wf))
            {
                // 1 sec length buffer
                var buf = new float[wf.SampleRate * wf.Channels];
                int currentSec = 0;
                for(int i=0; i < res.Count; i++)
                {
                    var data = res[i];
                    writer.WriteSamples(data, 0, data.Length);
                }                
            }
        }

        /// <summary>
        /// Converts sound file to List of arrays. One array - data for 1 second
        /// </summary>
        /// <param name="v"></param>
        internal static List<float[]> GetArraysFromFile(string inFile)
        {
            var res = new List<float[]>();

            if (!File.Exists(inFile)) { Program.LogError("No file " + inFile); return null; }
            using (var reader = new AudioFileReader(inFile))
            {
                Program.LogInfo("TotalTime:" + reader.TotalTime);
                Program.LogInfo("Length:" + reader.Length);
                Program.LogInfo("Volume:" + reader.Volume);
                var waveFormat = reader.WaveFormat;
                Program.LogInfo("SampleRate:" + waveFormat.SampleRate);
                Program.LogInfo("AverageBytesPerSecond:" + waveFormat.AverageBytesPerSecond);
                Program.LogInfo("BitsPerSample:" + waveFormat.BitsPerSample);
                Program.LogInfo("BlockAlign:" + waveFormat.BlockAlign);
                Program.LogInfo("Channels:" + waveFormat.Channels);
                Program.LogInfo("Encoding:" + waveFormat.Encoding);

                int valuesPerSecond = (int)(reader.Length / reader.TotalTime.TotalSeconds);
                Program.LogInfo("Values per second:" + valuesPerSecond);

                var resampler = new WdlResamplingSampleProvider(reader, 44100);
                var wp = new SampleToWaveProvider(reader);

                var rdr = wp.ToSampleProvider();


                var wf2 = rdr.WaveFormat;
                Program.LogInfo("SampleRate:" + wf2.SampleRate);
                Program.LogInfo("AverageBytesPerSecond:" + wf2.AverageBytesPerSecond);
                Program.LogInfo("BitsPerSample:" + wf2.BitsPerSample);
                Program.LogInfo("BlockAlign:" + wf2.BlockAlign);
                Program.LogInfo("Channels:" + wf2.Channels);
                Program.LogInfo("Encoding:" + wf2.Encoding);



                // 1 sec length buffer
                var buf = new float[rdr.WaveFormat.SampleRate * rdr.WaveFormat.Channels];
                while (true)
                {

                    int read = rdr.Read(buf, 0, buf.Length);
                    if (read > 0)
                    {
                        // only 1 channel
                        res.Add(buf.Where((x, i) => i % 2 == 0).ToArray());
                    }
                    else
                        break;
                }
                return res;
            }
        }

        /// <summary>
        /// demo function for reading files
        /// </summary>
        /// <param name="testWavFile"></param>
        /// <param name="testOutWavFile"></param>
        /// <param name="testOutWavFile2"></param>
        public static void ReadSound(string testWavFile, string testOutWavFile, string testOutWavFile2)
        {
            string testFile = testWavFile;
            if (!File.Exists(testFile)) { Program.LogError("No file " + testFile); return; }
            string outFile = testOutWavFile2;
            using (var reader = new AudioFileReader(testFile))
            {
                Program.LogInfo("TotalTime:" + reader.TotalTime);
                Program.LogInfo("Length:" + reader.Length);
                Program.LogInfo("Volume:" + reader.Volume);
                var waveFormat = reader.WaveFormat;
                Program.LogInfo("SampleRate:" + waveFormat.SampleRate);
                Program.LogInfo("AverageBytesPerSecond:" + waveFormat.AverageBytesPerSecond);
                Program.LogInfo("BitsPerSample:" + waveFormat.BitsPerSample);
                Program.LogInfo("BlockAlign:" + waveFormat.BlockAlign);
                Program.LogInfo("Channels:" + waveFormat.Channels);
                Program.LogInfo("Encoding:" + waveFormat.Encoding);
                Program.LogInfo("Values per second:" + reader.Length / reader.TotalTime.TotalSeconds);

                var resampler = new WdlResamplingSampleProvider(reader, 44100);
                var wp = new SampleToWaveProvider(reader);

                var rdr = wp.ToSampleProvider();


                var wf2 = rdr.WaveFormat;
                Program.LogInfo("SampleRate:" + wf2.SampleRate);
                Program.LogInfo("AverageBytesPerSecond:" + wf2.AverageBytesPerSecond);
                Program.LogInfo("BitsPerSample:" + wf2.BitsPerSample);
                Program.LogInfo("BlockAlign:" + wf2.BlockAlign);
                Program.LogInfo("Channels:" + wf2.Channels);
                Program.LogInfo("Encoding:" + wf2.Encoding);


                //var buf = new float[rdr.WaveFormat.SampleRate* rdr.WaveFormat.BitsPerSample/4*32];
                //  var buf = new float[1024*32*32];

                /*
                var wf = WaveFormat.CreateIeeeFloatWaveFormat(rdr.WaveFormat.SampleRate, rdr.WaveFormat.Channels);

                Console.WriteLine(buf[1024 * 32 * 2].ToString());

                int read = reader.Read(buf, 0, buf.Length);

                using (WaveFileWriter writer = new WaveFileWriter("out.wav", wf))
                {
                    writer.WriteSamples(buf, 0, buf.Length);
                }
                */

                // var wf = WaveFormat.CreateIeeeFloatWaveFormat(rdr.WaveFormat.SampleRate, rdr.WaveFormat.Channels);
                var wf = WaveFormat.CreateIeeeFloatWaveFormat(rdr.WaveFormat.SampleRate, 1);
                var buf = new float[rdr.WaveFormat.SampleRate];
                using (WaveFileWriter writer = new WaveFileWriter(testOutWavFile, wf))
                {
                    while (true)
                    {
                        int read = rdr.Read(buf, 0, buf.Length);
                        var buf2 = (new List<float>(buf)).Where((value, index) => index % rdr.WaveFormat.Channels == 0).ToArray();
                        if (read > 0)
                            writer.WriteSamples(buf2, 0, read / 2);
                        else
                            break;
                    }
                }

                /*
                // downsample to 22kHz
                var resampler = new WdlResamplingSampleProvider(reader, 44100);
                var wp = new SampleToWaveProvider(reader);
                using (var writer = new WaveFileWriter(outFile, wp.WaveFormat))
                {
                    byte[] b = new byte[wp.WaveFormat.AverageBytesPerSecond];
                    while (true)
                    {
                        int read = wp.Read(b, 0, b.Length);
                        if (read > 0)
                            writer.Write(b, 0, read);
                        else
                            break;
                    }
                }*/
                //WaveFileWriter.CreateWaveFile(outFile, );
            }
        }
                     
        /// <summary>
        /// demo function fo reading and saving sound files
        /// </summary>
        /// <param name="testWavFile"></param>
        /// <param name="testOutWavFile"></param>
        public static void ReadAndWriteSound(string testWavFile, string testOutWavFile)
        {
            string testFile = testWavFile;
            if (!File.Exists(testFile)) { Program.LogError("No file " + testFile); return; }
            using (var reader = new AudioFileReader(testFile))
            {
                Program.LogInfo("TotalTime:" + reader.TotalTime);
                Program.LogInfo("Length:" + reader.Length);
                Program.LogInfo("Volume:" + reader.Volume);
                var waveFormat = reader.WaveFormat;
                Program.LogInfo("SampleRate:" + waveFormat.SampleRate);
                Program.LogInfo("AverageBytesPerSecond:" + waveFormat.AverageBytesPerSecond);
                Program.LogInfo("BitsPerSample:" + waveFormat.BitsPerSample);
                Program.LogInfo("BlockAlign:" + waveFormat.BlockAlign);
                Program.LogInfo("Channels:" + waveFormat.Channels);
                Program.LogInfo("Encoding:" + waveFormat.Encoding);
                Program.LogInfo("Values per second:" + reader.Length / reader.TotalTime.TotalSeconds);

                var resampler = new WdlResamplingSampleProvider(reader, 44100);
                var wp = new SampleToWaveProvider(reader);

                var rdr = wp.ToSampleProvider();


                var wf2 = rdr.WaveFormat;
                Program.LogInfo("SampleRate:" + wf2.SampleRate);
                Program.LogInfo("AverageBytesPerSecond:" + wf2.AverageBytesPerSecond);
                Program.LogInfo("BitsPerSample:" + wf2.BitsPerSample);
                Program.LogInfo("BlockAlign:" + wf2.BlockAlign);
                Program.LogInfo("Channels:" + wf2.Channels);
                Program.LogInfo("Encoding:" + wf2.Encoding);


                var wf = WaveFormat.CreateIeeeFloatWaveFormat(rdr.WaveFormat.SampleRate, rdr.WaveFormat.Channels);
                using (WaveFileWriter writer = new WaveFileWriter(testOutWavFile, wf))
                {
                    var buf = new float[rdr.WaveFormat.SampleRate];
                    while (true)
                    {
                        int read = rdr.Read(buf, 0, buf.Length);
                        if (read > 0)
                            writer.WriteSamples(buf, 0, read);
                        else
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// extractig sound from video and savgion as a file
        /// </summary>
        /// <param name="testAviFile"></param>
        public static void ExtractSound(string testAviFile)
        {
            AviManager aviManager = new AviManager(testAviFile, true);
            try
            {
                AudioStream audioStream = aviManager.GetWaveStream();
                audioStream.ExportStream(@"sound.wav");
                aviManager.Close();
                Program.LogInfo("...finished.");
            }
            catch (Exception ex)
            {
                Program.LogError("The file does not contain a wave audio stream, or it cannot be opened.\r\n" + ex.ToString());
            }
        }


        /// <summary>
        /// Mixing 2 files starting from time @secDelay
        /// </summary>
        public static void OverlapFiles(string file1, string file2, int secDelay, string outFile)
        {
            if (secDelay < 0)
            {
                var tmp = file1;
                file1 = file2;
                file2 = tmp;
                secDelay = -secDelay;
            }


            if (!File.Exists(file1)) { Program.LogError("No file " + file1); return; }
            if (!File.Exists(file2)) { Program.LogError("No file " + file2); return; }

            using (var reader1 = new AudioFileReader(file1))
            {
                //var resampler1 = new WdlResamplingSampleProvider(reader1, SampleRate);
                var outFormat = new WaveFormat(SampleRate, Channels);
                using (var resampler1 =  new MediaFoundationResampler(reader1, outFormat))
                {
                    using (var reader2 = new AudioFileReader(file2))
                    {
                        using (var resampler2 = new MediaFoundationResampler(reader2, outFormat))
                        {
                            // standard sample rate and channels
                            var res1 = WaveExtensionMethods.ToSampleProvider(resampler1);
                            var res2 = WaveExtensionMethods.ToSampleProvider(resampler2);

                            PrintFileInfo(reader1, "Info of file: " + file1);
                            PrintFileInfo(reader2, "Info of file: " + file2);


                            var wf = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);
                            using (WaveFileWriter writer = new WaveFileWriter(outFile, wf))
                            {
                                // 1 second buffer
                                var buf1 = new float[SampleRate* Channels];
                                var buf2 = new float[SampleRate* Channels];
                                int currentSec = 0;
                                var file2finished = false;
                                while (true)
                                {
                                    int read1 = res1.Read(buf1, 0, buf1.Length);
                                    if (read1 > 0)
                                    {
                                        if (currentSec < secDelay || file2finished)
                                        {
                                            writer.WriteSamples(buf1, 0, read1);
                                        }
                                        else
                                        {
                                            int read2 = res2.Read(buf2, 0, buf2.Length);
                                            WriteMix(read1, buf1, read2, buf2, writer);
                                        }
                                    }
                                    else
                                        break;
                                    currentSec++;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// writing sum of 2 sounds to stream
        /// </summary>
        private static void WriteMix(int read1, float[] buf1, int read2, float[] buf2, WaveFileWriter writer)
        {
            var bufMix = new float[Math.Max(read1, read2)];
            for(int i=0;i< Math.Max(read1, read2); i++)
            {
                var v = 0f;
                if (i < read1) v += buf1[i];
                if (i < read2) v += buf2[i];
                bufMix[i] = v;
            }
            writer.WriteSamples(bufMix, 0, Math.Max(read1, read2));
        }

        /// <summary>
        /// printing some info about sound file
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="v"></param>
        private static void PrintFileInfo(AudioFileReader reader, string v)
        {
            Program.LogInfo("TotalTime:" + reader.TotalTime);
            Program.LogInfo("Length:" + reader.Length);
            Program.LogInfo("Volume:" + reader.Volume);
            var waveFormat = reader.WaveFormat;
            Program.LogInfo("SampleRate:" + waveFormat.SampleRate);
            Program.LogInfo("AverageBytesPerSecond:" + waveFormat.AverageBytesPerSecond);
            Program.LogInfo("BitsPerSample:" + waveFormat.BitsPerSample);
            Program.LogInfo("BlockAlign:" + waveFormat.BlockAlign);
            Program.LogInfo("Channels:" + waveFormat.Channels);
            Program.LogInfo("Encoding:" + waveFormat.Encoding);

            int valuesPerSecond = (int)(reader.Length / reader.TotalTime.TotalSeconds);
            Program.LogInfo("Values per second:" + valuesPerSecond);
        }
    }
}
