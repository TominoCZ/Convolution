namespace Convolution;

class MultiChannelMixedFeedback
{
    private Householder _matrix;
    private Delay[] _delays;
    private CutFilter[] _filters;
    private int _channels;

    private float _delayMs = 150;
    
    public float DecayGain = 0.85f;

    public MultiChannelMixedFeedback(int channels, float roomSize)
    {
        _matrix = new Householder(channels);
        _delays = new Delay[channels];
        _filters = new CutFilter[channels];
        _channels = channels;
        _delayMs = roomSize;

        var random = new Random();
        
        for (int c = 0; c < _channels; ++c)
        {
            _delays[c] = new Delay();
            _filters[c] = new CutFilter(50/*60 works well*/, random.Next(12000, 25000));
        }
    }
    
    public void Configure(int sampleRate)
    {
        float range = _delayMs * 0.001f * sampleRate;
        for (int c = 0; c < _channels; ++c)
        {
            var scale = c * 1.0f / _channels;
            var samples = (int) (Math.Pow(2, scale) * range);
            
            _delays[c].Configure(samples);
            _filters[c].Configure(sampleRate);
        }
    }

    public float[] Process(float[] input)
    {
        var delayed = new float[_channels];
        for (int c = 0; c < _channels; c++)
        {
            delayed[c] = _filters[c].Process(_delays[c].Read());
        }

        _matrix.InPlace(delayed);

        for (int c = 0; c < _channels; ++c)
        {
            float sum = input[c] + delayed[c] * DecayGain;
            _delays[c].Process(sum);
        }

        return delayed;
    }
}