using Convolution;

class DiffusionStep
{
    private Hadamard _matrix;
    private DetunedDelay[] _delays;
    private CutFilter[] _filters;
    private int[] _polarity;
    private int _channels;
    private float _msRange;

    public DiffusionStep(int channels, float msRange)
    {
        _matrix = new Hadamard(channels, channels);
        _delays = new DetunedDelay[channels];
        _filters = new CutFilter[channels];
        _polarity = new int[channels];
        _channels = channels;
        _msRange = msRange;
    }
    
    public void Configure(int sampleRate)
    {
        var random = new Random();
        var range = _msRange * 0.001 * sampleRate;

        for (int c = 0; c < _channels; c++)
        {
            var min = range * c / _channels;
            var max = range * (c + 1) / _channels;
            var samples = min + (max - min) * random.NextSingle();

            var cents = 8;
            var delay = new DetunedDelay();
            delay.Configure((int) samples + 1);
            delay.EnableDetune(cents * (0.5f - random.NextSingle()));

            _delays[c] = delay;
            _polarity[c] = c % 2;
            _filters[c] = new CutFilter(100, 25000);
            _filters[c].Configure(sampleRate);
        }
    }

    public float[] Process(float[] input)
    {
        //var delayed = new float[_channels];
        for (int c = 0; c < _channels; c++)
        {
            input[c] = _filters[c].Process(_delays[c].Process(input[c]));
            
            if (_polarity[c] == 1)
                input[c] *= -1;
        }

        _matrix.InPlace(input);

        return input;
    }
}