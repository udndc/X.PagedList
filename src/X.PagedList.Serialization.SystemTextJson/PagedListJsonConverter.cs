using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace X.PagedList.Serialization.SystemTextJson;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="IPagedList{T}"/> that serializes paging metadata
/// along with the items, and deserializes into a <see cref="StaticPagedList{T}"/>.
/// </summary>
/// <typeparam name="T">The type of objects in the paged list.</typeparam>
public class PagedListJsonConverter<T> : JsonConverter<IPagedList<T>>
{
    private const string ItemsPropertyName = "Items";

    /// <inheritdoc />
    public override IPagedList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object.");
        }

        string ConvertName(string propertyName)
        {
            if(options.PropertyNamingPolicy is null)
            {
                return propertyName;
            }
            return options.PropertyNamingPolicy.ConvertName(propertyName);
        }

        var comparison = options.PropertyNameCaseInsensitive
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        var pageNumberName = ConvertName(nameof(IPagedList<T>.PageNumber));
        var pageSizeName = ConvertName(nameof(IPagedList<T>.PageSize));
        var totalItemCountName = ConvertName(nameof(IPagedList<T>.TotalItemCount));
        var itemsName = ConvertName(ItemsPropertyName);

        var items = new List<T>();
        int pageNumber = 1;
        int pageSize = BasePagedList<T>.DefaultPageSize;
        int totalItemCount = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name.");
            }

            var propertyName = reader.GetString() ?? throw new JsonException("Expected non-null property name.");
            reader.Read();

            if (string.Equals(propertyName, pageNumberName, comparison))
            {
                pageNumber = reader.GetInt32();
            }
            else if (string.Equals(propertyName, pageSizeName, comparison))
            {
                pageSize = reader.GetInt32();
            }
            else if (string.Equals(propertyName, totalItemCountName, comparison))
            {
                totalItemCount = reader.GetInt32();
            }
            else if (string.Equals(propertyName, itemsName, comparison))
            {
                items = JsonSerializer.Deserialize<List<T>>(ref reader, options) ?? new List<T>();
            }
            else
            {
                reader.Skip();
            }
        }

        return new StaticPagedList<T>(items, pageNumber, pageSize, totalItemCount);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IPagedList<T> value, JsonSerializerOptions options)
    {
        string ConvertName(string propertyName)
        {
            if (options.PropertyNamingPolicy is null)
            {
                return propertyName;
            }
            return options.PropertyNamingPolicy.ConvertName(propertyName);
        }

        writer.WriteStartObject();

        writer.WriteNumber(ConvertName(nameof(IPagedList<T>.PageNumber)), value.PageNumber);
        writer.WriteNumber(ConvertName(nameof(value.PageSize)), value.PageSize);
        writer.WriteNumber(ConvertName(nameof(value.TotalItemCount)), value.TotalItemCount);
        writer.WriteNumber(ConvertName(nameof(value.PageCount)), value.PageCount);
        writer.WriteBoolean(ConvertName(nameof(value.HasPreviousPage)), value.HasPreviousPage);
        writer.WriteBoolean(ConvertName(nameof(value.HasNextPage)), value.HasNextPage);
        writer.WriteBoolean(ConvertName(nameof(value.IsFirstPage)), value.IsFirstPage);
        writer.WriteBoolean(ConvertName(nameof(value.IsLastPage)), value.IsLastPage);
        writer.WriteNumber(ConvertName(nameof(value.FirstItemOnPage)), value.FirstItemOnPage);
        writer.WriteNumber(ConvertName(nameof(value.LastItemOnPage)), value.LastItemOnPage);

        writer.WritePropertyName(ConvertName(ItemsPropertyName));
        JsonSerializer.Serialize(writer, (IEnumerable<T>)value, options);

        writer.WriteEndObject();
    }
}
