using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace X.PagedList.Serialization.JsonNet;

/// <summary>
/// A <see cref="JsonConverter"/> for <see cref="IPagedList{T}"/> that serializes paging metadata
/// along with the items, and deserializes into a <see cref="StaticPagedList{T}"/>.
/// </summary>
public class PagedListJsonNetConverter : JsonConverter
{
    private const string ItemsPropertyName = "Items";

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        if (!objectType.IsGenericType)
        {
            return false;
        }

        var genericDef = objectType.GetGenericTypeDefinition();

        return genericDef == typeof(IPagedList<>)
               || genericDef == typeof(StaticPagedList<>)
               || genericDef == typeof(PagedList<>)
               || genericDef == typeof(BasePagedList<>);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var itemType = objectType.GetGenericArguments()[0];
        var jo = JObject.Load(reader);

        var namingStrategy = (serializer.ContractResolver as DefaultContractResolver)?.NamingStrategy;
        string ConvertName(string name) => namingStrategy?.GetPropertyName(name, false) ?? name;

        var pageNumber = jo.GetValue(ConvertName(nameof(IPagedList.PageNumber)), StringComparison.OrdinalIgnoreCase)?.Value<int>() ?? 1;
        var pageSize = jo.GetValue(ConvertName(nameof(IPagedList.PageSize)), StringComparison.OrdinalIgnoreCase)?.Value<int>() ?? BasePagedList<object>.DefaultPageSize;
        var totalItemCount = jo.GetValue(ConvertName(nameof(IPagedList.TotalItemCount)), StringComparison.OrdinalIgnoreCase)?.Value<int>() ?? 0;

        var itemsToken = jo.GetValue(ConvertName(ItemsPropertyName), StringComparison.OrdinalIgnoreCase);
        var listType = typeof(List<>).MakeGenericType(itemType);

        var items = itemsToken != null
            ? itemsToken.ToObject(listType, serializer)!
            : Activator.CreateInstance(listType)!;

        var staticPagedListType = typeof(StaticPagedList<>).MakeGenericType(itemType);

        return Activator.CreateInstance(staticPagedListType, items, pageNumber, pageSize, totalItemCount)!;
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if(value is null)
        {
            writer.WriteNull();
            return;
        }

        var pagedList = (IPagedList)value;

        var namingStrategy = (serializer.ContractResolver as DefaultContractResolver)?.NamingStrategy;
        string ConvertName(string name) => namingStrategy?.GetPropertyName(name, false) ?? name;

        writer.WriteStartObject();

        writer.WritePropertyName(ConvertName(nameof(pagedList.PageNumber)));
        writer.WriteValue(pagedList.PageNumber);

        writer.WritePropertyName(ConvertName(nameof(pagedList.PageSize)));
        writer.WriteValue(pagedList.PageSize);

        writer.WritePropertyName(ConvertName(nameof(pagedList.TotalItemCount)));
        writer.WriteValue(pagedList.TotalItemCount);

        writer.WritePropertyName(ConvertName(nameof(pagedList.PageCount)));
        writer.WriteValue(pagedList.PageCount);

        writer.WritePropertyName(ConvertName(nameof(pagedList.HasPreviousPage)));
        writer.WriteValue(pagedList.HasPreviousPage);

        writer.WritePropertyName(ConvertName(nameof(pagedList.HasNextPage)));
        writer.WriteValue(pagedList.HasNextPage);

        writer.WritePropertyName(ConvertName(nameof(pagedList.IsFirstPage)));
        writer.WriteValue(pagedList.IsFirstPage);

        writer.WritePropertyName(ConvertName(nameof(pagedList.IsLastPage)));
        writer.WriteValue(pagedList.IsLastPage);

        writer.WritePropertyName(ConvertName(nameof(pagedList.FirstItemOnPage)));
        writer.WriteValue(pagedList.FirstItemOnPage);

        writer.WritePropertyName(ConvertName(nameof(pagedList.LastItemOnPage)));
        writer.WriteValue(pagedList.LastItemOnPage);

        writer.WritePropertyName(ConvertName(ItemsPropertyName));

        writer.WriteStartArray();

        foreach (var item in (IEnumerable)value)
        {
            serializer.Serialize(writer, item);
        }

        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}
