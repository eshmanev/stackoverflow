namespace StackOverflow.Answers.MethodsReturningTask;

/// <summary>
/// <see href="https://stackoverflow.com/questions/75982824/correct-way-of-using-functaskt/75983068#75983068"/>
/// </summary>
internal class Example1
{
    public static void Run()
    {
        Func<Task<int>> getter = async () => await Get();

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
