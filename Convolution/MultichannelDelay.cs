using Convolution;

class MultichannelDelay
{
    private Delay[] _channels;
    private int[] _polarity;
    private Hadamard _matrix;

    public MultichannelDelay(int sampleRate, int channels = 4)
    {
        _channels = new Delay[channels];
        _polarity = new int[channels];
        _matrix = new Hadamard(channels, 2);

        var random = new Random();
        var range = 80 * sampleRate * 0.001;

        for (int i = 0; i < channels; i++)
        {
            var min = range * i / channels;
            var max = range * (i + 1) / channels;
            var samples = min + (max - min) * random.NextSingle();

            var c = new Delay();
            c.Configure((int) samples + 1); //48 * (int)Math.Pow(2, i));

            _channels[i] = c;
            _polarity[i] = random.NextSingle() < 0.5f ? 1 : -1;
        }
    }

    public float[] Process(float[] samples)
    {
        for (int i = 0; i < _channels.Length; i++)
        {
            samples[i] = _channels[i].Process(samples[i]);
        }
        
        return samples;
    }
}