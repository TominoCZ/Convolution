namespace Convolution;

class MultiChannelMixedFeedback
{
    private Householder _matrix;
    private Delay[] _delays;
    private int _channels;

    private float _delayMs = 150;
    
    public float DecayGain = 0.85f;

    public MultiChannelMixedFeedback(int channels, float roomSize)
    {
        _matrix = new Householder(channels);
        _delays = new Delay[channels];
        _channels = channels;
        _delayMs = roomSize;

        for (int c = 0; c < _channels; ++c)
        {
            _delays[c] = new Delay();
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
        }
    }

    public float[] Process(float[] input)
    {
        var delayed = new float[_channels];
        for (int c = 0; c < _channels; c++)
        {
            delayed[c] = _delays[c].Read();
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