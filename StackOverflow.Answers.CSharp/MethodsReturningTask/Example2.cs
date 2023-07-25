namespace StackOverflow.Answers.CSharp.MethodsReturningTask;


internal class Example2
{
    public static void Run()
    {
        Func<Task<int>> getter = () => Get();

        int x = getter().Result;

        Console.WriteLine("hello : " + x);
    }

    static async Task<int> Get()
    {
        throw new Exception("test");
        await Task.Delay(1000);
        return 1;
    }
}
