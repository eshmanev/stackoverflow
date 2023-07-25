using System.Linq.Expressions;
using System.Reflection;

namespace StackOverflow.Answers.CallGenericMethodDynamically;

public static class CallViaExpression
{
    private static readonly Cache<Delegate> cache = new();

    public static void CallGenericMethod(Sample sample, Type genericType)
    {
        var callDelegate = GetDelegate(nameof(Sample.GenericMethod), BindingFlags.Instance | BindingFlags.Public, genericType);
        ((Action<Sample>)callDelegate).Invoke(sample);
    }

    public static void CallGenericMethod(Sample sample, Type genericType, int someValue)
    {
        var callDelegate = GetDelegate(nameof(Sample.GenericMethodWithArg), BindingFlags.Instance | BindingFlags.Public, genericType, typeof(int));
        ((Action<Sample, int>)callDelegate).Invoke(sample, someValue);
    }

    public static void CallStaticMethod(Type genericType)
    {
        var callDelegate = GetDelegate(nameof(Sample.StaticMethod), BindingFlags.Static | BindingFlags.Public, genericType);
        ((Action)callDelegate).Invoke();
    }

    public static void CallStaticMethod(Type genericType, int someValue)
    {
        var callDelegate = GetDelegate(nameof(Sample.StaticMethodWithArg), BindingFlags.Static | BindingFlags.Public, genericType, typeof(int));
        ((Action<int>)callDelegate).Invoke(someValue);
    }

    private static Delegate GetDelegate(string methodName, BindingFlags bindingFlags, Type genericType, params Type[] arguments)
    {
        if (cache.TryGet(methodName, genericType, out var callDelegate))
            return callDelegate;

        var sampleType = typeof(Sample);
        MethodInfo genericMethodInfo = sampleType.GetMethod(methodName, bindingFlags)!;
        var concreteMethodInfo = genericMethodInfo.MakeGenericMethod(genericType);

        var argumentExpr = arguments.Select((type, i) => Expression.Parameter(type, "arg" + i)).ToArray();
        if (concreteMethodInfo.IsStatic)
        {
            var callExpr = Expression.Call(concreteMethodInfo, argumentExpr);
            callDelegate = Expression.Lambda(callExpr, argumentExpr).Compile();
        }
        else
        {
            var parameterExpr = Expression.Parameter(sampleType, "sample");
            var callExpr = Expression.Call(parameterExpr, concreteMethodInfo, argumentExpr);
            callDelegate = Expression.Lambda(callExpr, new[] { parameterExpr }.Union(argumentExpr).ToArray()).Compile();
        }

        cache.Add(methodName, genericType, callDelegate);
        return callDelegate;
    }
}
