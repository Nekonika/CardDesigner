// https://github.com/Burtsev-Alexey/net-object-deep-copy

using System.Reflection;

namespace CardDesigner;

internal static class ObjectExtensions
{
    private static readonly MethodInfo _CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static bool IsPrimitive(this Type type)
    {
        if (type == typeof(string)) 
            return true;
        
        return type.IsValueType & type.IsPrimitive;
    }

    public static object? Copy(this object? originalObject)
        => InternalCopy(originalObject, new Dictionary<object, object?>(new ReferenceEqualityComparer()));
    
    private static object? InternalCopy(object? originalObject, IDictionary<object, object?> visited)
    {
        if (originalObject == null) 
            return null;
        
        Type TypeToReflect = originalObject.GetType();
        if (IsPrimitive(TypeToReflect)) 
            return originalObject;
        
        if (visited.TryGetValue(originalObject, out object? Value)) 
            return Value;
        
        if (typeof(Delegate).IsAssignableFrom(TypeToReflect)) 
            return null;
        
        object? CloneObject = _CloneMethod.Invoke(originalObject, null);
        if (TypeToReflect.IsArray)
        {
            Type? ArrayType = TypeToReflect.GetElementType();
            if (IsPrimitive(ArrayType!) == false)
            {
                Array ClonedArray = (Array)CloneObject!;
                ClonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(ClonedArray.GetValue(indices), visited), indices));
            }

        }
        visited.Add(originalObject, CloneObject);
        CopyFields(originalObject, visited, CloneObject!, TypeToReflect);
        RecursiveCopyBaseTypePrivateFields(originalObject, visited, CloneObject!, TypeToReflect);
        return CloneObject;
    }

    private static void RecursiveCopyBaseTypePrivateFields(object? originalObject, IDictionary<object, object?> visited, object cloneObject, Type typeToReflect)
    {
        if (typeToReflect.BaseType == null) 
            return;
        
        RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
        CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
    }

    private static void CopyFields(object? originalObject, IDictionary<object, object?> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool>? filter = null)
    {
        foreach (FieldInfo FieldInfo in typeToReflect.GetFields(bindingFlags))
        {
            if (filter != null && filter(FieldInfo) == false) continue;
            if (IsPrimitive(FieldInfo.FieldType)) continue;
            object? OriginalFieldValue = FieldInfo.GetValue(originalObject);
            object? ClonedFieldValue = InternalCopy(OriginalFieldValue, visited);
            FieldInfo.SetValue(cloneObject, ClonedFieldValue);
        }
    }
    public static T Copy<T>(this T original)
    {
        return (T)Copy((object)original!)!;
    }
}

public class ReferenceEqualityComparer : EqualityComparer<object>
{
    public override bool Equals(object? x, object? y)
        => ReferenceEquals(x, y);
    
    public override int GetHashCode(object? obj)
        => obj == null ? 0 : obj.GetHashCode();
}

public static class ArrayExtensions
{
    public static void ForEach(this Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0) 
            return;
        
        ArrayTraverse Walker = new ArrayTraverse(array);
        do action(array, Walker.Position);
        while (Walker.Step());
    }
}

internal class ArrayTraverse
{
    public readonly int[] Position;
    private readonly int[] _MaxLengths;

    public ArrayTraverse(Array array)
    {
        _MaxLengths = new int[array.Rank];
        for (int I = 0; I < array.Rank; ++I)
            _MaxLengths[I] = array.GetLength(I) - 1;
        
        Position = new int[array.Rank];
    }

    public bool Step()
    {
        for (int I = 0; I < Position.Length; ++I)
        {
            if (Position[I] >= _MaxLengths[I]) 
                continue;
            
            Position[I]++;
            for (int J = 0; J < I; J++)
            {
                Position[J] = 0;
            }
            
            return true;
        }
        
        return false;
    }
}