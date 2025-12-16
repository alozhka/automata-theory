using Runtime;

namespace Execution;

public class FakeEnvironment : IEnvironment
{
    private readonly List<Value> _results = [];
    private readonly Queue<Value> _inputQueue = new();

    public FakeEnvironment(params Value[] inputs)
    {
        for (int i = inputs.Length - 1; i >= 0; i--)
        {
            _inputQueue.Enqueue(inputs[i]);
        }
    }

    public IReadOnlyList<Value> Results => _results;

    public Value ReadInput()
    {
        if (_inputQueue.Count == 0)
        {
            throw new InvalidOperationException("No input available in test environment");
        }

        return _inputQueue.Dequeue();
    }

    public void AddResult(Value result)
    {
        _results.Add(result);
    }
}