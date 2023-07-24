using NUnit.Framework;
using System.Diagnostics;

namespace CallGenericMethodDynamically;

[TestFixture]
public class SampleTests
{
    private const int Iterations = 10000000;

    [Test]
    public void TestDirectCall()
    {
        var sample = new Sample();
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (var i = 0; i < Iterations; i++)
            sample.TestDirectCall(typeof(string));

        stopwatch.Stop();
        Assert.Pass($"Calling methods directly took {stopwatch.ElapsedMilliseconds} milliseconds.");
    }

    [Test]
    public void TestReflection()
    {
        var sample = new Sample();
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (var i = 0; i < Iterations; i++)
            sample.TestReflection(typeof(string));

        stopwatch.Stop();
        Assert.Pass($"Calling methods dynamically via reflection took {stopwatch.ElapsedMilliseconds} milliseconds.");
    }

    [Test]
    public void TestExpressionTree()
    {
        var sample = new Sample();
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (var i = 0; i < Iterations; i++)
            sample.TestExpression(typeof(string));

        stopwatch.Stop();
        Assert.Pass($"Calling methods dynamically via expression tree took {stopwatch.ElapsedMilliseconds} milliseconds.");
    }

    [Test]
    public void TestEmit()
    {
        var sample = new Sample();
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (var i = 0; i < Iterations; i++)
            sample.TestEmit(typeof(string));

        stopwatch.Stop();
        Assert.Pass($"Calling methods dynamically via emit took {stopwatch.ElapsedMilliseconds} milliseconds.");
    }
}
