namespace Convolution;

public class Hadamard
{
    private readonly int[][] _matrix;
    private readonly int _inputs;
    private readonly int _outputs;
    private readonly float _scale;

    public Hadamard(int inputs, int outputs)
    {
        if (!IsPowerOfTwo(inputs) || !IsPowerOfTwo(outputs))
            throw new ArgumentException("Inputs and outputs must be powers of 2");

        _inputs = inputs;
        _outputs = outputs;
        _scale = 1f / MathF.Sqrt(outputs);

        _matrix = new int[outputs][];
        for (int i = 0; i < outputs; i++)
        {
            _matrix[i] = new int[inputs];
            for (int j = 0; j < inputs; j++)
                _matrix[i][j] = HadamardElement(i, j);
        }
    }

    public void Mix(float[] input, float[] output)
    {
        if (input.Length != _inputs)
            throw new ArgumentException("Input length mismatch");
        if (output.Length != _outputs)
            throw new ArgumentException("Output length mismatch");

        for (int o = 0; o < _outputs; o++)
        {
            float sum = 0f;
            int[] row = _matrix[o];
            for (int i = 0; i < _inputs; i++)
                sum += row[i] * input[i];
            output[o] = sum * _scale;
        }
    }

    public void InPlace(float[] buffer)
    {
        if (_inputs != _outputs)
            throw new InvalidOperationException("In-place only valid when inputs == outputs");

        float[] temp = new float[_inputs];
        Array.Copy(buffer, temp, _inputs);
        Mix(temp, buffer);
    }

    static int HadamardElement(int i, int j)
    {
        return (CountBits(i & j) % 2 == 0) ? 1 : -1;
    }

    static int CountBits(int x)
    {
        int count = 0;
        while (x != 0)
        {
            count += x & 1;
            x >>= 1;
        }
        return count;
    }

    static bool IsPowerOfTwo(int n) => (n & (n - 1)) == 0 && n > 0;
}