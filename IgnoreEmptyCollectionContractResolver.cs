using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CardDesigner;

internal class IgnoreEmptyCollectionContractResolver : DefaultContractResolver
{
    private bool _IgnoreEmptyCollections;
    private readonly Dictionary<Type, object?> _Defaults = [];
    
    internal IgnoreEmptyCollectionContractResolver SetDefaultProperty(Type type, object? defaultProperty)
    {
        _Defaults[type] = defaultProperty;
        return this;
    }

    internal IgnoreEmptyCollectionContractResolver IgnoreEmptyCollections(bool ignore = true)
    {
        _IgnoreEmptyCollections = ignore;
        return this;
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty Property = base.CreateProperty(member, memberSerialization);
        if (Property.PropertyType == null || Property.PropertyName == null)
            return Property;

        // do not serialize if this is an empty collection
        if (_IgnoreEmptyCollections &&
            Property.PropertyType != typeof(string) &&
            typeof(IEnumerable).IsAssignableFrom(Property.PropertyType))
        {
            Property.ShouldSerialize = instance => !IsEmptyCollection(Property, instance);
        }

        if (_Defaults.TryGetValue(Property.PropertyType, out object? DefaultProperty))
            Property.DefaultValue = DefaultProperty;

        return Property;
    }
    
    private static bool IsEmptyCollection(JsonProperty property, object instance)
    {
        object? Value = property.ValueProvider?.GetValue(instance);
        return Value is IEnumerable Enumerable && !Enumerable.Cast<object>().Any();
    }
}