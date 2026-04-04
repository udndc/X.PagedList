using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace X.PagedList.Serialization.SystemTextJson;

/// <summary>
/// A <see cref="JsonConverterFactory"/> that creates <see cref="PagedListJsonConverter{T}"/> instances
/// for <see cref="IPagedList{T}"/>, <see cref="BasePagedList{T}"/>, <see cref="PagedList{T}"/>,
/// and <see cref="StaticPagedList{T}"/> types.
/// </summary>
public class PagedListJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        var genericDef = typeToConvert.GetGenericTypeDefinition();

        return genericDef == typeof(IPagedList<>)
               || genericDef == typeof(StaticPagedList<>)
               || genericDef == typeof(PagedList<>)
               || genericDef == typeof(BasePagedList<>);
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        return BuildFactory(itemType)();
    }

    private static Func<JsonConverter> BuildFactory(Type itemType)
    {
        var converterType = typeof(PagedListJsonConverter<>).MakeGenericType(itemType);
        var ctor = converterType.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException(
                $"Type '{converterType}' must have a parameterless constructor.");

        var newExpr = Expression.New(ctor);
        var castExpr = Expression.Convert(newExpr, typeof(JsonConverter));

        return Expression.Lambda<Func<JsonConverter>>(castExpr).Compile();
    }
}
