using Convolution;

class Reverb
{
    private MultiChannelMixedFeedback _feedback;
    private Diffuser _diffuser;
    private CutFilter _filter;

    private int _channels;
    private int _steps;

    private float _wet;
    private float _roomSize;

    public Reverb(float roomSizeMs, float rt60, float wet = 1)
    {
        //_channels = 32;
        //_steps = 16;
        
        //_channels = 32;
        //_steps = 4;
        
        _channels = 64;
        _steps = 4;
        
        _wet = wet;
        _roomSize = roomSizeMs;

        _feedback = new MultiChannelMixedFeedback(_channels, _roomSize);
        _diffuser = new Diffuser(_channels, _steps, _roomSize);
        _filter = new CutFilter(100, 5500);

        double typicalLoopMs = roomSizeMs * 1.5;
        double loopsPerRt60 = rt60 / (typicalLoopMs * 0.001);
        double dbPerCycle = -60 / loopsPerRt60;

        _feedback.DecayGain = (float) Math.Pow(10, dbPerCycle * 0.05);
    }

    public void Configure(int sampleRate)
    {
        _filter.Configure(sampleRate);
        _feedback.Configure(sampleRate);
        _diffuser.Configure(sampleRate);
    }

    public float[] Process(float sample)
    {
        float[] input = new float[_channels];
        for (int i = 0; i < _channels; i++)
            input[i] = sample;

        float[] diffuse = _diffuser.Process(input);
        float[] last = _feedback.Process(diffuse);
        float[] mix = new float[2];
        float dry = 1 - _wet;
        for (int c = 0; c < _channels; ++c)
        {
            last[c] = dry * sample + _wet * last[c];
            mix[c % 2] += last[c];
        }
        
        mix[0] *= 2f / _channels;
        mix[1] *= 2f / _channels;

        return mix;
    }
}