namespace Convolution;

class DetunedDelay
{
    private float[] _buffer;
    private int _index;
    private float _readPhase;

    private float _detuneRate = 1f;
    private bool _detuneEnabled = false;

    public int Length { get; private set; }

    public void Configure(int samples)
    {
        Length = samples;
        _buffer = new float[samples];
        _index = 0;
        _readPhase = 0f;
    }

    public void EnableDetune(float cents)
    {
        _detuneRate = MathF.Pow(2f, cents / 1200f); // e.g. 3 cents
        _detuneEnabled = true;
        _readPhase = (_index - Length + _buffer.Length) % _buffer.Length;
    }

    public void DisableDetune()
    {
        _detuneEnabled = false;
    }

    public float Process(float sample)
    {
        // Store sample
        _buffer[_index % Length] = sample;

        float result;
        if (_detuneEnabled)
        {
            result = ReadInterpolated(_readPhase);
            _readPhase += _detuneRate;
            if (_readPhase >= Length)
                _readPhase -= Length;
        }
        else
        {
            result = Read(); // default non-detuned read
        }

        _index++;
        return result;
    }

    public float Read(int offset = 0)
    {
        return _buffer[(_index + Length + offset) % Length];
    }

    private float ReadInterpolated(float phase)
    {
        int i0 = (int)phase % Length;
        int i1 = (i0 + 1) % Length;
        float frac = phase - i0;

        float a = _buffer[i0];
        float b = _buffer[i1];

        return a * (1 - frac) + b * frac;
    }
}