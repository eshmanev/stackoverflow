using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace CallGenericMethodDynamically;

public class Sample
{
    public void TestDirectCall(Type type)
    {
        GenericMethod<string>();
        GenericMethodWithArg<string>(42);
        StaticMethod<string>();
        StaticMethodWithArg<string>(6);
    }

    public void TestReflection(Type type)
    {
        CallViaReflection.CallGenericMethod(this, type);
        CallViaReflection.CallGenericMethod(this, type, 42);
        CallViaReflection.CallStaticMethod(type);
        CallViaReflection.CallStaticMethod(type, 6);
    }

    public void TestExpression(Type type)
    {
        CallViaExpression.CallGenericMethod(this, type);
        CallViaExpression.CallGenericMethod(this, type, 42);
        CallViaExpression.CallStaticMethod(type);
        CallViaExpression.CallStaticMethod(type, 6);
    }

    public void TestEmit(Type type)
    {
        this.CallGenericMethod(type);
        this.CallGenericMethod(type, 42);
        CallViaEmit.CallStaticMethod(type);
        CallViaEmit.CallStaticMethod(type, 6);
    }

    public void T()
    {
        StaticMethod<string>();
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

public static class CallViaEmit
{
    private static readonly Cache<Delegate> cache = new();

    public static void CallGenericMethod(this Sample sample, Type genericType)
    {
        var callDelegate = GetDynamicMethod(nameof(Sample.GenericMethod), BindingFlags.Instance | BindingFlags.Public, genericType);
        ((Action<Sample>)callDelegate).Invoke(sample);
    }

    public static void CallGenericMethod(this Sample sample, Type genericType, int someValue)
    {
        var callDelegate = GetDynamicMethod(nameof(Sample.GenericMethodWithArg), BindingFlags.Instance | BindingFlags.Public, genericType);
        ((Action<Sample, int>)callDelegate).Invoke(sample, someValue);
    }

    public static void CallStaticMethod(Type genericType)
    {
        var callDelegate = GetDynamicMethod(nameof(Sample.StaticMethod), BindingFlags.Static | BindingFlags.Public, genericType);
        ((Action)callDelegate).Invoke();
    }

    public static void CallStaticMethod(Type genericType, int someValue)
    {
        var callDelegate = GetDynamicMethod(nameof(Sample.StaticMethodWithArg), BindingFlags.Static | BindingFlags.Public, genericType);
        ((Action<int>)callDelegate).Invoke(someValue);
    }

    private static Delegate GetDynamicMethod(string methodName, BindingFlags bindingFlags, Type genericType)
    {
        if (cache.TryGet(methodName, genericType, out var callDelegate))
            return callDelegate;

        var genericMethodInfo = typeof(Sample).GetMethod(methodName, bindingFlags)!;
        var concreteMethodInfo = genericMethodInfo.MakeGenericMethod(genericType);
        var argumentTypes = concreteMethodInfo.GetParameters().Select(x => x.ParameterType).ToArray(); ;
        var dynamicMethodArgs = concreteMethodInfo.IsStatic
            ? argumentTypes
            : new[] { typeof(Sample) }.Union(argumentTypes).ToArray();

        var dynamicMethod = new DynamicMethod("DynamicCall", null, dynamicMethodArgs);
        var il = dynamicMethod.GetILGenerator();
        il.Emit(OpCodes.Nop);

        switch (dynamicMethodArgs.Length)
        {
            case 0:
                break;
            case 1:
                il.Emit(OpCodes.Ldarg_0);
                break;
            case 2:
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                break;
            case 3:
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                break;
            default:
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldarg_3);
                for (int i = 4; i < argumentTypes.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg, argumentTypes[i]);
                }
                break;
        }

        il.EmitCall(concreteMethodInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, concreteMethodInfo, null);
        il.Emit(OpCodes.Nop);
        il.Emit(OpCodes.Ret);

        callDelegate = dynamicMethod.CreateDelegate(GetActionType(dynamicMethodArgs));
        cache.Add(methodName, genericType, callDelegate);
        return callDelegate;
    }

    private static Type GetActionType(Type[] argumentTypes)
    {
        switch (argumentTypes.Length)
        {
            case 0:
                return typeof(Action);
            case 1:
                return typeof(Action<>).MakeGenericType(argumentTypes);
            case 2:
                return typeof(Action<,>).MakeGenericType(argumentTypes);
            case 3:
                return typeof(Action<,,>).MakeGenericType(argumentTypes);
            case 4:
                return typeof(Action<,,,>).MakeGenericType(argumentTypes);
            case 5:
                return typeof(Action<,,,,>).MakeGenericType(argumentTypes);
            case 6:
                return typeof(Action<,,,,,>).MakeGenericType(argumentTypes);
            case 7:
                return typeof(Action<,,,,,,>).MakeGenericType(argumentTypes);
            case 8:
                return typeof(Action<,,,,,,,>).MakeGenericType(argumentTypes);
            default:
                throw new NotSupportedException("Action with more than 8 arguments is not supported");
        }
    }
}

public class Cache<T> where T : class
{
    private Dictionary<string, Dictionary<Type, T>> genericMethodCache = new();

    public bool TryGet(string methodName, Type genericType, [MaybeNullWhen(false)] out T callDelegate)
    {
        if (!genericMethodCache.TryGetValue(methodName, out var typeMap))
        {
            callDelegate = null;
            return false;
        }

        if (!typeMap.TryGetValue(genericType, out callDelegate))
        {
            callDelegate = null;
            return false;
        }

        return true;
    }

    public void Add(string methodName, Type genericType, T callDelegate)
    {
        if (!genericMethodCache.TryGetValue(methodName, out var typeMap))
            typeMap = genericMethodCache[methodName] = new();

        typeMap[genericType] = callDelegate;
    }
}