using NUnit.Framework;

namespace StackOverflow.Answers.CSharp.MethodsReturningTask
{
    [TestFixture]
    public class TestFixture
    {
        [Test]
        public void RunExample1()
        {
            Example1.Run();
        }

        [Test]
        public void RunExample2()
        {
            Example2.Run();
        }
    }
}
