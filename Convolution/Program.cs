using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using NAudio.Wave;

public static class Extensions
{
    public static void AddSample(this BufferedWaveProvider provider, float sample)
    {
        provider.AddSamples(BitConverter.GetBytes((short) (sample * short.MaxValue)), 0, 2);
    }
}

class Program
{
    string inputPath = "input.wav";
    string irPath = "ir.wav";
    string outputPath = "output.wav";

    public static void Main(string[] args)
    {
        new Program().Init();
    }

    void Init()
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);
        
        //float[] input = LoadMonoWav(inputPath);

        var format = new WaveFormat(44100, 2);
        var afw = new WaveFileWriter(outputPath, format);

        Process.GetCurrentProcess().Exited += (sender, args) =>
        {
            afw.Close();
        };

        var wi = new WaveInEvent();
        wi.WaveFormat = new WaveFormat(format.SampleRate, 1);

        var wo = new WaveOutEvent();
        var stream = new BufferedWaveProvider(format);

        var reverb = new Reverb(30, 8, 1f, 0.75f);
        reverb.Configure(format.SampleRate);

        //Console.WriteLine(Marshal.SizeOf(reverb));
        
        wo.NumberOfBuffers = 2;
        wo.Init(stream);
        //wo.PlaybackStopped += (s, e) => wo.Play();
        wo.Play();

        wi.BufferMilliseconds = 50;
        wi.NumberOfBuffers = 3;

        wi.DataAvailable += (_, args) =>
        {
            var t = DateTime.Now;
            for (int i = 0; i < args.Buffer.Length; i += 2)
            {
                var value = BitConverter.ToInt16(args.Buffer, i); 
                var sample = -value / (float)short.MinValue;
                
                var output = reverb.Process(sample);

                stream.AddSample(output[0]);
                stream.AddSample(output[1]);
                
                afw.WriteSample(output[0]);
                afw.WriteSample(output[1]);
            }

            var delta = DateTime.Now - t;

            //Console.WriteLine($"{args.Buffer.Length} samples processed in {delta.TotalMilliseconds} milliseconds");
            wo.Play();
        };
        
        wi.RecordingStopped += (_, args) => wi.StartRecording();
        wi.StartRecording();

        while (true)
        {
            Thread.Sleep(100);
        }

        /*

        long written = 0;
        while (written < input.Length)
        {
            if (stream.BufferedDuration > TimeSpan.FromSeconds(1))
                continue;

            for (int i = 0; i < 44100 && (i + written) < input.Length; i++)
            {
                var sample = input[written];

                var output = reverb.Process(sample);

                stream.AddSample(output[0]);
                stream.AddSample(output[1]);

                afw.WriteSample(output[0]);
                afw.WriteSample(output[1]);

                written++;
            }
        }

        afw.Close();*/
    }

    float[] LoadMonoWav(string path)
    {
        using var reader = new AudioFileReader(path);
        var samples = new float[reader.Length / sizeof(float)];
        int read = reader.Read(samples, 0, samples.Length);
        return samples[..read];
    }
}