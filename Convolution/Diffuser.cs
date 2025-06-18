using Convolution;

class Diffuser
{
    private DiffusionStep[] _steps;

    private int _stepCount;
    private int _channels;

    public Diffuser(int channels, int stepCount, float diffusionMs)
    {
        _steps = new DiffusionStep[stepCount];
        _stepCount = stepCount;
        _channels = channels;

        var ms = diffusionMs;
        for (int i = 0; i < stepCount; i++)
        {
            ms /= 2;
            _steps[i] = new DiffusionStep(_channels, ms);
        }
    }

    public void Configure(int sampleRate)
    {
        for (int i = 0; i < _stepCount; i++)
        {
            _steps[i].Configure(sampleRate);
        }
    }

    public float[] Process(float[] samples)
    {
        for (int i = 0; i < _stepCount; i++)
        {
            samples = _steps[i].Process(samples);
        }

        return samples;
    }
}