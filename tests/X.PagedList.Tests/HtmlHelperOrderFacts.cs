using System.Linq;
using X.PagedList.Mvc.Core;
using Xunit;
using X.PagedList;

namespace X.PagedList.Tests;

public class HtmlHelperOrderFacts
{
    private static string RenderPager(bool placeFirstAfterPrevious)
    {
        var items = Enumerable.Range(1, 20).ToList();
        var paged = new StaticPagedList<int>(items, 3, 4, items.Count);

        var helper = new X.PagedList.Mvc.Core.HtmlHelper(new TestTagBuilderFactory());

        var options = new PagedListRenderOptions
        {
            DisplayLinkToFirstPage = PagedListDisplayMode.Always,
            DisplayLinkToLastPage = PagedListDisplayMode.Always,
            DisplayLinkToPreviousPage = PagedListDisplayMode.Always,
            DisplayLinkToNextPage = PagedListDisplayMode.Always,
            PlaceFirstPageAfterPreviousAndLastPageBeforeNext = placeFirstAfterPrevious
        };

        return helper.PagedListPager(paged, i => $"/page/{i}", options)!;
    }

    [Fact]
    public void PlacesFirstAfterPreviousWhenConfigured()
    {
        var html = System.Net.WebUtility.HtmlDecode(RenderPager(true));

        AssertOrdering(html, "PagedList-skipToPrevious", "PagedList-skipToFirst");
        AssertOrdering(html, "PagedList-skipToLast", "PagedList-skipToNext");
    }

    [Fact]
    public void KeepsDefaultOrderWhenNotConfigured()
    {
        var html = System.Net.WebUtility.HtmlDecode(RenderPager(false));

        AssertOrdering(html, "PagedList-skipToFirst", "PagedList-skipToPrevious");
        AssertOrdering(html, "PagedList-skipToNext", "PagedList-skipToLast");
    }

    private static void AssertOrdering(string html, string first, string second)
    {
        int firstIndex = html.IndexOf(first);
        int secondIndex = html.IndexOf(second);

        Assert.True(firstIndex >= 0 && secondIndex >= 0, $"Expected both '{first}' and '{second}' in HTML.");
        Assert.True(firstIndex < secondIndex, $"Expected '{first}' to appear before '{second}'.");
    }

    private sealed class TestTagBuilderFactory : ITagBuilderFactory
    {
        public Microsoft.AspNetCore.Mvc.Rendering.TagBuilder Create(string tagName) =>
            new(tagName);
    }
}
