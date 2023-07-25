namespace StackOverflow.Answers.CSharp.CallGenericMethodDynamically;

/// <summary>
/// What's the best way to call a generic method when the type parameter isn't known at compile time, but instead is obtained dynamically at runtime?
/// </summary>
/// <see href="https://stackoverflow.com/questions/232535/how-do-i-use-reflection-to-call-a-generic-method/76756161#76756161"/>
public class Sample
{
    /// <summary>
    /// Calls methods directly.
    /// </summary>
    public void TestDirectCall(Type type)
    {
        GenericMethod<string>();
        GenericMethodWithArg<string>(42);
        StaticMethod<string>();
        StaticMethodWithArg<string>(6);
    }

    /// <summary>
    /// Calls methods via reflection.
    /// </summary>
    public void TestReflection(Type type)
    {
        CallViaReflection.CallGenericMethod(this, type);
        CallViaReflection.CallGenericMethod(this, type, 42);
        CallViaReflection.CallStaticMethod(type);
        CallViaReflection.CallStaticMethod(type, 6);
    }

    /// <summary>
    /// Calls methods via expression tree.
    /// </summary>
    public void TestExpression(Type type)
    {
        CallViaExpression.CallGenericMethod(this, type);
        CallViaExpression.CallGenericMethod(this, type, 42);
        CallViaExpression.CallStaticMethod(type);
        CallViaExpression.CallStaticMethod(type, 6);
    }

    /// <summary>
    /// Emits calling generic methods.
    /// </summary>
    /// <param name="type"></param>
    public void TestEmit(Type type)
    {
        this.CallGenericMethod(type);
        this.CallGenericMethod(type, 42);
        CallViaEmit.CallStaticMethod(type);
        CallViaEmit.CallStaticMethod(type, 6);
    }

    public void GenericMethod<T>()
    {
    }

    public void GenericMethodWithArg<T>(int someValue)
    {
    }

    public static void StaticMethod<T>()
    {
    }

    public static void StaticMethodWithArg<T>(int someValue)
    {
    }
}
