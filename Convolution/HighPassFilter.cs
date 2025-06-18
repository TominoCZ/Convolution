class HighPassFilter
{
    private float _y;      // last output
    private float _xPrev;  // last input
    private float _alpha;
    private float _cutoff;

    public HighPassFilter(float cutoffFreq)
    {
        _cutoff = cutoffFreq;
    }
    
    public void Configure(int sampleRate)
    {
        float dt = 1f / sampleRate;
        float rc = 1f / (2f * MathF.PI * _cutoff);
        _alpha = rc / (rc + dt);

        _y = 0f;
        _xPrev = 0f;
    }

    public float Process(float x)
    {
        float y = _alpha * (_y + x - _xPrev);
        _xPrev = x;
        _y = y;
        return y;
    }
}