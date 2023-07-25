using System.Reflection;

namespace StackOverflow.Answers.CSharp.CallGenericMethodDynamically;

public static class CallViaReflection
{
    private readonly static Cache<MethodInfo> cache = new();

    public static void CallGenericMethod(Sample sample, Type genericType)
    {
        var callDelegate = GetDelegate(nameof(Sample.GenericMethod), BindingFlags.Instance | BindingFlags.Public, genericType);
        callDelegate.Invoke(sample, null);
    }

    public static void CallGenericMethod(Sample sample, Type genericType, int someValue)
    {
        var callDelegate = GetDelegate(nameof(Sample.GenericMethodWithArg), BindingFlags.Instance | BindingFlags.Public, genericType, typeof(int));
        callDelegate.Invoke(sample, new object[] { someValue });
    }

    public static void CallStaticMethod(Type genericType)
    {
        var callDelegate = GetDelegate(nameof(Sample.StaticMethod), BindingFlags.Static | BindingFlags.Public, genericType);
        callDelegate.Invoke(null, null);
    }

    public static void CallStaticMethod(Type genericType, int someValue)
    {
        var callDelegate = GetDelegate(nameof(Sample.StaticMethodWithArg), BindingFlags.Static | BindingFlags.Public, genericType, typeof(int));
        callDelegate.Invoke(null, new object[] { someValue });
    }

    private static MethodInfo GetDelegate(string methodName, BindingFlags bindingFlags, Type genericType, params Type[] arguments)
    {
        if (cache.TryGet(methodName, genericType, out var concreteMethodInfo))
            return concreteMethodInfo;

        var sampleType = typeof(Sample);
        MethodInfo genericMethodInfo = sampleType.GetMethod(methodName, bindingFlags)!;
        concreteMethodInfo = genericMethodInfo.MakeGenericMethod(genericType);
        cache.Add(methodName, genericType, concreteMethodInfo);
        return concreteMethodInfo;
    }
}
