namespace Convolution;

class CutFilter
{
    private LowPassFilter _low;
    private HighPassFilter _high;

    public CutFilter(float lowFreq, float highFreq)
    {
        _low = new LowPassFilter(highFreq);
        _high = new HighPassFilter(lowFreq);
    }

    public void Configure(int sampleRate)
    {
        _low.Configure(sampleRate);
        _high.Configure(sampleRate);
    }

    public float Process(float input)
    {
        return _high.Process(_low.Process(input));
    }
}