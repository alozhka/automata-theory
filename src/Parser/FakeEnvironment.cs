using System;
using System.Collections.Generic;

using Execution;

namespace Parser;

/// <summary>
/// Поддельное окружение: работает как настоящее, но не совершает реального ввода/вывода.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly List<double> _results = [];
    private readonly Queue<object> _inputQueue = new Queue<object>();

    public FakeEnvironment(params object[] inputs)
    {
        for (int i = inputs.Length - 1; i >= 0; i--)
        {
            _inputQueue.Enqueue(inputs[i]);
        }
    }

    public IReadOnlyList<double> Results => _results;

    public object ReadInput()
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