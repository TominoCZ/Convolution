namespace Convolution;

class Feedback
{
    private Delay _delay = new();

    private LowPassFilter _lowPass;
    private HighPassFilter _highPass;

    private float _volume = 0.4f;
    private float _gainLoss = 0.45f;

    public Feedback(int samples)
    {
        _lowPass = new LowPassFilter(4000);
        _highPass = new HighPassFilter(100);
        
        _delay.Configure(samples);
    }

    public void Configure(int sampleRate)
    {
        _lowPass.Configure(sampleRate);
        _highPass.Configure(sampleRate);
    }

    public void Process(float sample)
    {
        var feedback = _delay.Read();
        feedback = _lowPass.Process(feedback * (1 - _gainLoss));
        sample = _highPass.Process(sample * _volume + feedback);

        _delay.Process(sample);
    }
    
    public float Read() => _delay.Read();
}