# X.PagedList.Serialization.JsonNet

[![NuGet Version](http://img.shields.io/nuget/v/X.PagedList.Serialization.JsonNet.svg?style=flat)](https://www.nuget.org/packages/X.PagedList.Serialization.JsonNet/)
[![Twitter URL](https://img.shields.io/twitter/url/https/x.com/andrew_gubskiy.svg?style=social&label=Follow%20me!)](https://x.com/intent/user?screen_name=andrew_gubskiy)


## What is this?
The X.PagedList.Serialization.JsonNet library provides Json.NET (Newtonsoft.Json) serialization support for the X.PagedList library, enabling JSON serialization and deserialization of IPagedList<T> instances with full support for naming strategies and case-insensitive property matching.

## Installation

```
dotnet add package X.PagedList.Serialization.JsonNet
```

## How to use

Register the converter in `JsonSerializerSettings`:

```csharp
var settings = new JsonSerializerSettings
{
    ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    },
    Converters = { new PagedListJsonNetConverter() }
};
```

Serialize:

```csharp
IPagedList<int> pagedList = new StaticPagedList<int>(new[] { 1, 2, 3 }, 1, 10, 100);
string json = JsonConvert.SerializeObject(pagedList, settings);
```

Deserialize:

```csharp
IPagedList<int> result = JsonConvert.DeserializeObject<IPagedList<int>>(json, settings);
```

## Get a digital subscription for project news
[Subscribe](https://x.com/intent/user?screen_name=andrew_gubskiy) to my X to keep up-to-date with project news and receive announcements.
