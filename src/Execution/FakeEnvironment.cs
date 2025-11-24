namespace Execution;

/// <summary>
/// ���������� ���������: �������� ��� ���������, �� �� ��������� ��������� �����/������.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly List<double> _results = [];
    private readonly Queue<double> _inputQueue = new();

    public FakeEnvironment(params double[] inputs)
    {
        for (int i = inputs.Length - 1; i >= 0; i--)
        {
            _inputQueue.Enqueue(inputs[i]);
        }
    }

    public IReadOnlyList<double> Results => _results;

    public double ReadInput()
    {
        if (_inputQueue.Count == 0)
        {
            throw new InvalidOperationException("No input available in test environment");
        }

        return _inputQueue.Dequeue();
    }

    public void AddResult(double result)
    {
        _results.Add(result);
    }
}