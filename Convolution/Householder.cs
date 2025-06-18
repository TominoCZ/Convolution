namespace Convolution;

public class Householder
{
    private readonly float[][] _matrix;
    private readonly int _size;
    private readonly float[] _vector;

    public Householder(int dimensions)
    {
        _size = dimensions;
        _vector = GenerateRandomUnitVector(dimensions);

        float normSquared = 0f;
        for (int i = 0; i < _size; i++)
            normSquared += _vector[i] * _vector[i];

        if (normSquared == 0f)
            throw new ArgumentException("Vector must be non-zero");

        _matrix = new float[_size][];
        for (int i = 0; i < _size; i++)
        {
            _matrix[i] = new float[_size];
            for (int j = 0; j < _size; j++)
            {
                float outer = _vector[i] * _vector[j];
                _matrix[i][j] = (i == j ? 1f : 0f) - 2f * outer / normSquared;
            }
        }
    }

    public void Transform(float[] input, float[] output)
    {
        if (input.Length != _size || output.Length != _size)
            throw new ArgumentException("Input and output must match matrix size");

        for (int i = 0; i < _size; i++)
        {
            float sum = 0f;
            for (int j = 0; j < _size; j++)
                sum += _matrix[i][j] * input[j];
            output[i] = sum;
        }
    }

    public void InPlace(float[] buffer)
    {
        float[] temp = new float[_size];
        Transform(buffer, temp);
        Array.Copy(temp, buffer, _size);
    }
    
    private float[] GenerateRandomUnitVector(int size)
    {
        float[] v = new float[size];
        float sum = 0f;

        Random rnd = new Random();
        for (int i = 0; i < size; i++)
        {
            v[i] = (float)(rnd.NextDouble() * 2 - 1); // [-1, 1]
            sum += v[i] * v[i];
        }

        float norm = MathF.Sqrt(sum);
        for (int i = 0; i < size; i++)
            v[i] /= norm;

        return v;
    }

    public float[] Vector => _vector;

    public float this[int row, int col] => _matrix[row][col];
}