namespace Convolution;

class Delay
{
    private float[] _buffer;
    private int _index;
    
    public int Length { get; private set; }
    
    public void Configure(int samples)
    {
        _buffer = new float[samples];
        _index = 0;
        
        Length = samples;
    }

    public float Process(float sample)
    {
        _buffer[_index++ % Length] = sample;

        return Read();
    }

    public float Read(int offset = 0)
    {
        return _buffer[(_index + Length + offset) % Length];
    }
}