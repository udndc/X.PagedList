# X.PagedList.Serialization.SystemTextJson

[![NuGet Version](http://img.shields.io/nuget/v/X.PagedList.Serialization.SystemTextJson.svg?style=flat)](https://www.nuget.org/packages/X.PagedList.Serialization.SystemTextJson/)
[![Twitter URL](https://img.shields.io/twitter/url/https/x.com/andrew_gubskiy.svg?style=social&label=Follow%20me!)](https://x.com/intent/user?screen_name=andrew_gubskiy)


## What is this?
The X.PagedList.Serialization.SystemTextJson library provides System.Text.Json serialization support for the X.PagedList library, enabling JSON serialization and deserialization of IPagedList<T> instances with full support for naming policies and case-insensitive property matching.

## Installation

```
dotnet add package X.PagedList.Serialization.SystemTextJson
```

## How to use

Register the converter factory in `JsonSerializerOptions`:

```csharp
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

options.Converters.Add(new PagedListJsonConverterFactory());
```

Serialize:

```csharp
IPagedList<int> pagedList = new StaticPagedList<int>(new[] { 1, 2, 3 }, 1, 10, 100);
string json = JsonSerializer.Serialize<IPagedList<int>>(pagedList, options);
```

Deserialize:

```csharp
IPagedList<int> result = JsonSerializer.Deserialize<IPagedList<int>>(json, options);
```

## Get a digital subscription for project news
[Subscribe](https://x.com/intent/user?screen_name=andrew_gubskiy) to my X to keep up-to-date with project news and receive announcements.
