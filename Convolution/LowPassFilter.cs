namespace Convolution;

class LowPassFilter
{
    private float _y;
    private float _alpha;
    private float _cutoff;

    public LowPassFilter(float cutoffFreq)
    {
        _cutoff = cutoffFreq;
    }

    public void Configure(int sampleRate)
    {
        float dt = 1f / sampleRate;
        float rc = 1f / (2f * MathF.PI * _cutoff);

        _alpha = dt / (rc + dt);
        _y = 0f;
    }

    public float Process(float x)
    {
        _y += _alpha * (x - _y);
        return _y;
    }
}