using System.Diagnostics.CodeAnalysis;

namespace StackOverflow.Answers.CallGenericMethodDynamically;

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