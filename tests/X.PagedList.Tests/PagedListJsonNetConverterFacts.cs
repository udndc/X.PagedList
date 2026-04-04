using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using X.PagedList.Serialization.JsonNet;
using Xunit;

namespace X.PagedList.Tests;

public class PagedListJsonNetConverterFacts
{
    private static JsonSerializerSettings CreateSettings(NamingStrategy? namingStrategy = null)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = namingStrategy
            },
            Converters = { new PagedListJsonNetConverter() }
        };

        return settings;
    }

    private static IPagedList<int> CreatePagedList()
    {
        return new StaticPagedList<int>(new[] { 1, 2, 3 }, 2, 3, 9);
    }

    [Fact]
    public void Write_CamelCase_Produces_CamelCase_Property_Names()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var pagedList = CreatePagedList();

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var jo = JObject.Parse(json);

        Assert.Equal(2, jo.Value<int>("pageNumber"));
        Assert.Equal(3, jo.Value<int>("pageSize"));
        Assert.Equal(9, jo.Value<int>("totalItemCount"));
        Assert.Equal(3, jo.Value<int>("pageCount"));
        Assert.Equal(new JArray(1, 2, 3), jo["items"]);
    }

    [Fact]
    public void Write_CamelCase_Deserializes_Correctly()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var pagedList = CreatePagedList();

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(9, result.TotalItemCount);
        Assert.Equal(3, result.PageCount);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Write_NoNamingStrategy_Produces_PascalCase_Property_Names()
    {
        var settings = CreateSettings(namingStrategy: null);
        var pagedList = CreatePagedList();

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var jo = JObject.Parse(json);

        Assert.Equal(2, jo.Value<int>("PageNumber"));
        Assert.Equal(3, jo.Value<int>("PageSize"));
        Assert.Equal(9, jo.Value<int>("TotalItemCount"));
        Assert.Equal(3, jo.Value<int>("PageCount"));
        Assert.Equal(new JArray(1, 2, 3), jo["Items"]);
    }

    [Fact]
    public void Write_NoNamingStrategy_Deserializes_Correctly()
    {
        var settings = CreateSettings(namingStrategy: null);
        var pagedList = CreatePagedList();

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(9, result.TotalItemCount);
        Assert.Equal(3, result.PageCount);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Write_Serializes_Metadata_Values()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var pagedList = CreatePagedList();

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var jo = JObject.Parse(json);

        Assert.Equal(2, jo.Value<int>("pageNumber"));
        Assert.Equal(3, jo.Value<int>("pageSize"));
        Assert.Equal(9, jo.Value<int>("totalItemCount"));
        Assert.Equal(3, jo.Value<int>("pageCount"));
        Assert.True(jo.Value<bool>("hasPreviousPage"));
        Assert.True(jo.Value<bool>("hasNextPage"));
        Assert.False(jo.Value<bool>("isFirstPage"));
        Assert.False(jo.Value<bool>("isLastPage"));
        Assert.Equal(4, jo.Value<int>("firstItemOnPage"));
        Assert.Equal(6, jo.Value<int>("lastItemOnPage"));
    }

    [Fact]
    public void Write_Metadata_Values_Deserialize_Correctly()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var pagedList = CreatePagedList();

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

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
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var pagedList = CreatePagedList();

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var jo = JObject.Parse(json);
        var items = (JArray)jo["items"]!;

        Assert.Equal(3, items.Count);
        Assert.Equal(1, items[0].Value<int>());
        Assert.Equal(2, items[1].Value<int>());
        Assert.Equal(3, items[2].Value<int>());
    }

    [Fact]
    public void Write_Items_Deserialize_Correctly()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var pagedList = CreatePagedList();

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Read_CamelCase_Deserializes_Correctly()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());

        var json = """
        {
            "pageNumber": 2,
            "pageSize": 3,
            "totalItemCount": 9,
            "items": [1, 2, 3]
        }
        """;

        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(9, result.TotalItemCount);
        Assert.Equal(3, result.PageCount);
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Read_PascalCase_Deserializes_Correctly()
    {
        var settings = CreateSettings(namingStrategy: null);

        var json = """
        {
            "PageNumber": 1,
            "PageSize": 5,
            "TotalItemCount": 20,
            "Items": [10, 20, 30, 40, 50]
        }
        """;

        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(20, result.TotalItemCount);
        Assert.Equal(4, result.PageCount);
        Assert.Equal(new[] { 10, 20, 30, 40, 50 }, result);
    }

    [Fact]
    public void Read_CaseInsensitive_Matches_Regardless_Of_Casing()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());

        var json = """
        {
            "PAGENUMBER": 1,
            "PAGESIZE": 2,
            "TOTALITEMCOUNT": 10,
            "ITEMS": [1, 2]
        }
        """;

        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(10, result.TotalItemCount);
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void Read_Missing_Items_Returns_Empty_List()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());

        var json = """
        {
            "pageNumber": 1,
            "pageSize": 10,
            "totalItemCount": 0
        }
        """;

        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Empty(result);
        Assert.Equal(0, result.TotalItemCount);
    }

    [Fact]
    public void Roundtrip_CamelCase_Preserves_Data()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var original = CreatePagedList();

        var json = JsonConvert.SerializeObject(original, settings);
        var deserialized = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(original.PageNumber, deserialized.PageNumber);
        Assert.Equal(original.PageSize, deserialized.PageSize);
        Assert.Equal(original.TotalItemCount, deserialized.TotalItemCount);
        Assert.Equal(original.PageCount, deserialized.PageCount);
        Assert.Equal((IEnumerable<int>)original, deserialized);
    }

    [Fact]
    public void Roundtrip_PascalCase_Preserves_Data()
    {
        var settings = CreateSettings(namingStrategy: null);
        var original = CreatePagedList();

        var json = JsonConvert.SerializeObject(original, settings);
        var deserialized = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(original.PageNumber, deserialized.PageNumber);
        Assert.Equal(original.PageSize, deserialized.PageSize);
        Assert.Equal(original.TotalItemCount, deserialized.TotalItemCount);
        Assert.Equal(original.PageCount, deserialized.PageCount);
        Assert.Equal((IEnumerable<int>)original, deserialized);
    }

    [Fact]
    public void Read_Unknown_Properties_Are_Skipped()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());

        var json = """
        {
            "pageNumber": 1,
            "pageSize": 5,
            "totalItemCount": 10,
            "unknownField": "test",
            "items": [1, 2, 3, 4, 5]
        }
        """;

        var result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings)!;

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Write_Null_Value_Writes_Null()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());

        var json = JsonConvert.SerializeObject(null, typeof(IPagedList<int>), settings);

        Assert.Equal("null", json);
    }

    [Fact]
    public void CanConvert_Returns_True_For_IPagedList()
    {
        var converter = new PagedListJsonNetConverter();

        Assert.True(converter.CanConvert(typeof(IPagedList<int>)));
        Assert.True(converter.CanConvert(typeof(StaticPagedList<int>)));
        Assert.True(converter.CanConvert(typeof(PagedList<int>)));
    }

    [Fact]
    public void CanConvert_Returns_False_For_NonPagedList()
    {
        var converter = new PagedListJsonNetConverter();

        Assert.False(converter.CanConvert(typeof(List<int>)));
        Assert.False(converter.CanConvert(typeof(string)));
        Assert.False(converter.CanConvert(typeof(int)));
    }

    [Fact]
    public void Write_Serializes_Complex_Items()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };
        var pagedList = new StaticPagedList<TestItem>(items, 1, 10, 2);

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var jo = JObject.Parse(json);
        var itemsArray = (JArray)jo["items"]!;

        Assert.Equal(2, itemsArray.Count);
        Assert.Equal(1, itemsArray[0].Value<int>("id"));
        Assert.Equal("First", itemsArray[0].Value<string>("name"));
    }

    [Fact]
    public void Write_Complex_Items_Deserialize_Correctly()
    {
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };
        var pagedList = new StaticPagedList<TestItem>(items, 1, 10, 2);

        var json = JsonConvert.SerializeObject(pagedList, settings);
        var result = JsonConvert.DeserializeObject<IPagedList<TestItem>>(json, settings)!;

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
        var settings = CreateSettings(new CamelCaseNamingStrategy());
        var items = new List<TestItem>
        {
            new() { Id = 1, Name = "First" },
            new() { Id = 2, Name = "Second" }
        };
        var original = new StaticPagedList<TestItem>(items, 1, 10, 2);

        var json = JsonConvert.SerializeObject(original, settings);
        var deserialized = JsonConvert.DeserializeObject<IPagedList<TestItem>>(json, settings)!;

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
