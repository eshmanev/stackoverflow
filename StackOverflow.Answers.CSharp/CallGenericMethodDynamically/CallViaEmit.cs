using System.Reflection;
using System.Reflection.Emit;

namespace StackOverflow.Answers.CSharp.CallGenericMethodDynamically;

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
