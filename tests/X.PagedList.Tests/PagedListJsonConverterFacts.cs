using System.Collections.Generic;
using System.Text.Json;
using X.PagedList.Serialization.SystemTextJson;
using Xunit;

namespace X.PagedList.Tests;

public class PagedListJsonConverterFacts
{
    private static JsonSerializerOptions CreateOptions(JsonNamingPolicy? namingPolicy = null, bool caseInsensitive = false)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = namingPolicy,
            PropertyNameCaseInsensitive = caseInsensitive
        };

        options.Converters.Add(new PagedListJsonConverterFactory());

        return options;
    }

    private static IPagedList<int> CreatePagedList()
    {
        return new StaticPagedList<int>(new[] { 1, 2, 3 }, 2, 3, 9);
    }

    [Fact]
    public void Write_CamelCase_Produces_CamelCase_Property_Names()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase);
        var pagedList = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(2, root.GetProperty("pageNumber").GetInt32());
        Assert.Equal(3, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(9, root.GetProperty("totalItemCount").GetInt32());
        Assert.Equal(3, root.GetProperty("pageCount").GetInt32());
        Assert.Equal(3, root.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public void Write_CamelCase_Deserializes_Correctly()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase);
        var pagedList = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);

        var readOptions = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);
        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, readOptions)!;

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(9, result.TotalItemCount);
        Assert.Equal(3, result.PageCount);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Write_NullNamingPolicy_Produces_PascalCase_Property_Names()
    {
        var options = CreateOptions(namingPolicy: null);
        var pagedList = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(2, root.GetProperty("PageNumber").GetInt32());
        Assert.Equal(3, root.GetProperty("PageSize").GetInt32());
        Assert.Equal(9, root.GetProperty("TotalItemCount").GetInt32());
        Assert.Equal(3, root.GetProperty("PageCount").GetInt32());
        Assert.Equal(3, root.GetProperty("Items").GetArrayLength());
    }

    [Fact]
    public void Write_NullNamingPolicy_Deserializes_Correctly()
    {
        var options = CreateOptions(namingPolicy: null);
        var pagedList = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);
        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, options)!;

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(9, result.TotalItemCount);
        Assert.Equal(3, result.PageCount);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Write_Serializes_Metadata_Values()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase);
        var pagedList = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(2, root.GetProperty("pageNumber").GetInt32());
        Assert.Equal(3, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(9, root.GetProperty("totalItemCount").GetInt32());
        Assert.Equal(3, root.GetProperty("pageCount").GetInt32());
        Assert.True(root.GetProperty("hasPreviousPage").GetBoolean());
        Assert.True(root.GetProperty("hasNextPage").GetBoolean());
        Assert.False(root.GetProperty("isFirstPage").GetBoolean());
        Assert.False(root.GetProperty("isLastPage").GetBoolean());
        Assert.Equal(4, root.GetProperty("firstItemOnPage").GetInt32());
        Assert.Equal(6, root.GetProperty("lastItemOnPage").GetInt32());
    }

    [Fact]
    public void Write_Metadata_Values_Deserialize_Correctly()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase);
        var pagedList = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);

        var readOptions = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);
        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, readOptions)!;

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(9, result.TotalItemCount);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
        Assert.False(result.IsFirstPage);
        Assert.False(result.IsLastPage);
    }

    [Fact]
    public void Write_Serializes_Items()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase);
        var pagedList = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);
        var doc = JsonDocument.Parse(json);
        var items = doc.RootElement.GetProperty("items");

        Assert.Equal(3, items.GetArrayLength());
        Assert.Equal(1, items[0].GetInt32());
        Assert.Equal(2, items[1].GetInt32());
        Assert.Equal(3, items[2].GetInt32());
    }

    [Fact]
    public void Write_Items_Deserialize_Correctly()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase);
        var pagedList = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);

        var readOptions = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);
        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, readOptions)!;

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Read_CamelCase_Deserializes_Correctly()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);

        var json = """
        {
            "pageNumber": 2,
            "pageSize": 3,
            "totalItemCount": 9,
            "items": [1, 2, 3]
        }
        """;

        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, options)!;

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(9, result.TotalItemCount);
        Assert.Equal(3, result.PageCount);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Read_PascalCase_Deserializes_Correctly()
    {
        var options = CreateOptions(namingPolicy: null);

        var json = """
        {
            "PageNumber": 1,
            "PageSize": 5,
            "TotalItemCount": 20,
            "Items": [10, 20, 30, 40, 50]
        }
        """;

        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, options)!;

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(20, result.TotalItemCount);
        Assert.Equal(4, result.PageCount);
        Assert.Equal(new[] { 10, 20, 30, 40, 50 }, result);
    }

    [Fact]
    public void Read_CaseInsensitive_Matches_Regardless_Of_Casing()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);

        var json = """
        {
            "PAGENUMBER": 1,
            "PAGESIZE": 2,
            "TOTALITEMCOUNT": 10,
            "ITEMS": [1, 2]
        }
        """;

        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, options)!;

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(10, result.TotalItemCount);
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void Read_Missing_Items_Returns_Empty_List()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);

        var json = """
        {
            "pageNumber": 1,
            "pageSize": 10,
            "totalItemCount": 0
        }
        """;

        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, options)!;

        Assert.Empty(result);
        Assert.Equal(0, result.TotalItemCount);
    }

    [Fact]
    public void Roundtrip_CamelCase_Preserves_Data()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);
        var original = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(original, options);
        var deserialized = JsonSerializer.Deserialize<IPagedList<int>>(json, options)!;

        Assert.Equal(original.PageNumber, deserialized.PageNumber);
        Assert.Equal(original.PageSize, deserialized.PageSize);
        Assert.Equal(original.TotalItemCount, deserialized.TotalItemCount);
        Assert.Equal(original.PageCount, deserialized.PageCount);
        Assert.Equal((IEnumerable<int>)original, deserialized);
    }

    [Fact]
    public void Roundtrip_PascalCase_Preserves_Data()
    {
        var options = CreateOptions(namingPolicy: null);
        var original = CreatePagedList();

        var json = JsonSerializer.Serialize<IPagedList<int>>(original, options);
        var deserialized = JsonSerializer.Deserialize<IPagedList<int>>(json, options)!;

        Assert.Equal(original.PageNumber, deserialized.PageNumber);
        Assert.Equal(original.PageSize, deserialized.PageSize);
        Assert.Equal(original.TotalItemCount, deserialized.TotalItemCount);
        Assert.Equal(original.PageCount, deserialized.PageCount);
        Assert.Equal((IEnumerable<int>)original, deserialized);
    }

    [Fact]
    public void Read_Unknown_Properties_Are_Skipped()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);

        var json = """
        {
            "pageNumber": 1,
            "pageSize": 5,
            "totalItemCount": 10,
            "unknownField": "test",
            "items": [1, 2, 3, 4, 5]
        }
        """;

        var result = JsonSerializer.Deserialize<IPagedList<int>>(json, options)!;

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Write_Serializes_Complex_Items()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase);
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };
        var pagedList = new StaticPagedList<TestItem>(items, 1, 10, 2);

        var json = JsonSerializer.Serialize<IPagedList<TestItem>>(pagedList, options);
        var doc = JsonDocument.Parse(json);
        var itemsArray = doc.RootElement.GetProperty("items");

        Assert.Equal(2, itemsArray.GetArrayLength());
        Assert.Equal(1, itemsArray[0].GetProperty("id").GetInt32());
        Assert.Equal("First", itemsArray[0].GetProperty("name").GetString());
    }

    [Fact]
    public void Write_Complex_Items_Deserialize_Correctly()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase);
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };
        var pagedList = new StaticPagedList<TestItem>(items, 1, 10, 2);

        var json = JsonSerializer.Serialize<IPagedList<TestItem>>(pagedList, options);

        var readOptions = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);
        var result = JsonSerializer.Deserialize<IPagedList<TestItem>>(json, readOptions)!;

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.TotalItemCount);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("First", result[0].Name);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Second", result[1].Name);
    }

    [Fact]
    public void Roundtrip_Complex_Items_Preserves_Data()
    {
        var options = CreateOptions(JsonNamingPolicy.CamelCase, caseInsensitive: true);
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };
        var original = new StaticPagedList<TestItem>(items, 1, 10, 2);

        var json = JsonSerializer.Serialize<IPagedList<TestItem>>(original, options);
        var deserialized = JsonSerializer.Deserialize<IPagedList<TestItem>>(json, options)!;

        Assert.Equal(original.PageNumber, deserialized.PageNumber);
        Assert.Equal(original.TotalItemCount, deserialized.TotalItemCount);
        Assert.Equal(2, deserialized.Count);
        Assert.Equal(1, deserialized[0].Id);
        Assert.Equal("Second", deserialized[1].Name);
    }

    public class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
