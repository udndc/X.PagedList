# X.PagedList.Serialization.JsonNet

[![Sponsor on GitHub](https://img.shields.io/badge/Sponsor_on_GitHub-ff7f00?logo=github&logoColor=white&style=for-the-badge)](https://github.com/sponsors/a-gubskiy)
[![Subscribe on X](https://img.shields.io/badge/Subscribe_on_X-000000?logo=x&logoColor=white&style=for-the-badge)](https://x.com/andrew_gubskiy)
[![NuGet Downloads](https://img.shields.io/nuget/dt/X.PagedList.Serialization.JsonNet?style=for-the-badge&label=NuGet%20Downloads&color=004880&logo=nuget&logoColor=white)](https://www.nuget.org/packages/X.PagedList.Serialization.JsonNet)

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

## Support this project

X.PagedList is used in over 15 million installs and maintained by one
person in his spare time. If it saves your team time, consider
[sponsoring](https://github.com/sponsors/a-gubskiy) — it directly funds
issue triage, .NET version support, and documentation.

Companies using X.PagedList in production: the Corporate tier includes
priority triage and advance notice of breaking changes.
