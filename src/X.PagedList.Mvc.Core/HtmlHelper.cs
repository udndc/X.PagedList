using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace X.PagedList.Mvc.Core;

/// <summary>
/// HTML helper responsible for building pagination markup and a "go to page" form
/// using <see cref="TagBuilder"/> instances produced by <see cref="ITagBuilderFactory"/>.
/// </summary>
public class HtmlHelper
{
    private readonly ITagBuilderFactory _tagBuilderFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlHelper"/> class.
    /// </summary>
    /// <param name="tagBuilderFactory">Factory used to create <see cref="TagBuilder"/> instances.</param>
    public HtmlHelper(ITagBuilderFactory tagBuilderFactory)
    {
        _tagBuilderFactory = tagBuilderFactory;
    }

    /// <summary>
    /// Sets the inner text content of a tag (HTML-encoded).
    /// </summary>
    /// <param name="tagBuilder">The tag to mutate.</param>
    /// <param name="innerText">The text to set as content.</param>
    private static void SetInnerText(TagBuilder tagBuilder, string innerText)
    {
        tagBuilder.SetInnerText(innerText);
    }

    /// <summary>
    /// Appends raw HTML to the tag's inner content (not encoded).
    /// </summary>
    /// <param name="tagBuilder">The tag to mutate.</param>
    /// <param name="innerHtml">The HTML fragment to append.</param>
    private static void AppendHtml(TagBuilder tagBuilder, string innerHtml)
    {
        tagBuilder.AppendHtml(innerHtml);
    }

    /// <summary>
    /// Renders a <see cref="TagBuilder"/> to its HTML string using normal render mode.
    /// </summary>
    /// <param name="tagBuilder">The tag to render.</param>
    /// <returns>The rendered HTML.</returns>
    private static string TagBuilderToString(TagBuilder tagBuilder)
    {
        return tagBuilder.ToString(TagRenderMode.Normal);
    }

    /// <summary>
    /// Renders a <see cref="TagBuilder"/> to its HTML string using the specified render mode.
    /// </summary>
    /// <param name="tagBuilder">The tag to render.</param>
    /// <param name="renderMode">The render mode to use.</param>
    /// <returns>The rendered HTML.</returns>
    private static string TagBuilderToString(TagBuilder tagBuilder, TagRenderMode renderMode)
    {
        return tagBuilder.ToString(renderMode);
    }

    /// <summary>
    /// Wraps the provided text in an &lt;li&gt; element.
    /// </summary>
    /// <param name="text">The text to place inside the list item.</param>
    /// <returns>An &lt;li&gt; <see cref="TagBuilder"/>.</returns>
    private TagBuilder WrapInListItem(string text)
    {
        var li = _tagBuilderFactory.Create("li");

        SetInnerText(li, text);

        return li;
    }

    /// <summary>
    /// Wraps the provided inner tag in an &lt;li&gt; element and applies optional classes and transformation.
    /// </summary>
    /// <param name="inner">The inner tag to place inside the list item.</param>
    /// <param name="options">Rendering options that may transform the link.</param>
    /// <param name="classes">Optional CSS classes for the &lt;li&gt; element.</param>
    /// <returns>An &lt;li&gt; <see cref="TagBuilder"/> containing the inner tag.</returns>
    private TagBuilder WrapInListItem(TagBuilder inner, PagedListRenderOptions? options, params string[] classes)
    {
        var li = _tagBuilderFactory.Create("li");

        foreach (var @class in classes)
        {
            li.AddCssClass(@class);
        }

        if (options?.FunctionToTransformEachPageLink != null)
        {
            return options.FunctionToTransformEachPageLink(li, inner);
        }

        AppendHtml(li, TagBuilderToString(inner));

        return li;
    }

    /// <summary>
    /// Builds the "first page" link item.
    /// </summary>
    /// <param name="list">The paged list.</param>
    /// <param name="generatePageUrl">Function that generates a URL for a given page number.</param>
    /// <param name="options">Render options.</param>
    /// <returns>An &lt;li&gt; with an anchor to the first page, or a disabled item if already on first page.</returns>
    private TagBuilder First(IPagedList list, Func<int, string?> generatePageUrl, PagedListRenderOptions options)
    {
        const int targetPageNumber = 1;

        var first = _tagBuilderFactory.Create("a");

        AppendHtml(first, string.Format(options.LinkToFirstPageFormat, targetPageNumber));

        foreach (var c in options.PageClasses ?? Enumerable.Empty<string>())
        {
            first.AddCssClass(c);
        }

        if (list.IsFirstPage)
        {
            return WrapInListItem(first, options, "PagedList-skipToFirst", "disabled");
        }

        first.Attributes.Add("href", generatePageUrl(targetPageNumber));

        return WrapInListItem(first, options, "PagedList-skipToFirst");
    }

    /// <summary>
    /// Builds the "previous page" link item.
    /// </summary>
    /// <param name="list">The paged list.</param>
    /// <param name="generatePageUrl">Function that generates a URL for a given page number.</param>
    /// <param name="options">Render options.</param>
    /// <returns>An &lt;li&gt; with an anchor to the previous page, or a disabled item if not available.</returns>
    private TagBuilder Previous(IPagedList list, Func<int, string?> generatePageUrl, PagedListRenderOptions options)
    {
        var targetPageNumber = list.PageNumber - 1;

        var previous = _tagBuilderFactory.Create("a");

        AppendHtml(previous, string.Format(options.LinkToPreviousPageFormat, targetPageNumber));

        previous.Attributes.Add("rel", "prev");

        foreach (var c in options.PageClasses ?? Enumerable.Empty<string>())
        {
            previous.AddCssClass(c);
        }

        if (!list.HasPreviousPage)
        {
            return WrapInListItem(previous, options, options.PreviousElementClass, "disabled");
        }

        previous.Attributes.Add("href", generatePageUrl(targetPageNumber));

        return WrapInListItem(previous, options, options.PreviousElementClass);
    }

    /// <summary>
    /// Builds an individual page number item, using &lt;span&gt; for the active page and &lt;a&gt; for others.
    /// </summary>
    /// <param name="i">The page number to render.</param>
    /// <param name="list">The paged list.</param>
    /// <param name="generatePageUrl">Function that generates a URL for a given page number.</param>
    /// <param name="options">Render options.</param>
    /// <returns>An &lt;li&gt; containing the page number element.</returns>
    private TagBuilder Page(int i, IPagedList list, Func<int, string?> generatePageUrl, PagedListRenderOptions options)
    {
        var format = options.FunctionToDisplayEachPageNumber
                     ?? (pageNumber => string.Format(options.LinkToIndividualPageFormat, pageNumber));

        var targetPageNumber = i;

        var page = i == list.PageNumber
            ? _tagBuilderFactory.Create("span")
            : _tagBuilderFactory.Create("a");

        SetInnerText(page, format(targetPageNumber));

        foreach (var c in options.PageClasses ?? Enumerable.Empty<string>())
        {
            page.AddCssClass(c);
        }

        if (i == list.PageNumber)
        {
            return WrapInListItem(page, options, options.ActiveLiElementClass);
        }

        page.Attributes.Add("href", generatePageUrl(targetPageNumber));

        return WrapInListItem(page, options);
    }

    /// <summary>
    /// Builds the "next page" link item.
    /// </summary>
    /// <param name="list">The paged list.</param>
    /// <param name="generatePageUrl">Function that generates a URL for a given page number.</param>
    /// <param name="options">Render options.</param>
    /// <returns>An &lt;li&gt; with an anchor to the next page, or a disabled item if not available.</returns>
    private TagBuilder Next(IPagedList list, Func<int, string?> generatePageUrl, PagedListRenderOptions options)
    {
        var targetPageNumber = list.PageNumber + 1;
        var next = _tagBuilderFactory.Create("a");

        AppendHtml(next, string.Format(options.LinkToNextPageFormat, targetPageNumber));

        next.Attributes.Add("rel", "next");

        foreach (var c in options.PageClasses ?? Enumerable.Empty<string>())
        {
            next.AddCssClass(c);
        }

        if (!list.HasNextPage)
        {
            return WrapInListItem(next, options, options.NextElementClass, "disabled");
        }

        next.Attributes.Add("href", generatePageUrl(targetPageNumber));

        return WrapInListItem(next, options, options.NextElementClass);
    }

    /// <summary>
    /// Builds the "last page" link item.
    /// </summary>
    /// <param name="list">The paged list.</param>
    /// <param name="generatePageUrl">Function that generates a URL for a given page number.</param>
    /// <param name="options">Render options.</param>
    /// <returns>An &lt;li&gt; with an anchor to the last page, or a disabled item if already on last page.</returns>
    private TagBuilder Last(IPagedList list, Func<int, string?> generatePageUrl, PagedListRenderOptions options)
    {
        var targetPageNumber = list.PageCount;
        var last = _tagBuilderFactory.Create("a");

        AppendHtml(last, string.Format(options.LinkToLastPageFormat, targetPageNumber));

        foreach (var c in options.PageClasses ?? Enumerable.Empty<string>())
        {
            last.AddCssClass(c);
        }

        if (list.IsLastPage)
        {
            return WrapInListItem(last, options, "PagedList-skipToLast", "disabled");
        }

        last.Attributes.Add("href", generatePageUrl(targetPageNumber));

        return WrapInListItem(last, options, "PagedList-skipToLast");
    }

    /// <summary>
    /// Builds a disabled item showing the current page number and total page count.
    /// </summary>
    /// <param name="list">The paged list.</param>
    /// <param name="options">Render options.</param>
    /// <returns>An &lt;li&gt; containing the page count/location text.</returns>
    private TagBuilder PageCountAndLocationText(IPagedList list, PagedListRenderOptions options)
    {
        var text = _tagBuilderFactory.Create("a");

        SetInnerText(text, string.Format(options.PageCountAndCurrentLocationFormat, list.PageNumber, list.PageCount));

        return WrapInListItem(text, options, "PagedList-pageCountAndLocation", "disabled");
    }

    /// <summary>
    /// Builds a disabled item showing the current item slice and total item count.
    /// </summary>
    /// <param name="list">The paged list.</param>
    /// <param name="options">Render options.</param>
    /// <returns>An &lt;li&gt; containing the item slice/total text.</returns>
    private TagBuilder ItemSliceAndTotalText(IPagedList list, PagedListRenderOptions options)
    {
        var text = _tagBuilderFactory.Create("a");

        SetInnerText(text, string.Format(options.ItemSliceAndTotalFormat, list.FirstItemOnPage, list.LastItemOnPage, list.TotalItemCount));

        return WrapInListItem(text, options, "PagedList-pageCountAndLocation", "disabled");
    }

    /// <summary>
    /// Builds a "previous ellipsis" item that links to the page before the first displayed page.
    /// </summary>
    /// <param name="list">The paged list.</param>
    /// <param name="generatePageUrl">Function that generates a URL for a given page number.</param>
    /// <param name="options">Render options.</param>
    /// <param name="firstPageToDisplay">The first page number currently displayed.</param>
    /// <returns>An &lt;li&gt; containing the ellipsis link or a disabled item.</returns>
    private TagBuilder PreviousEllipsis(IPagedList list, Func<int, string?> generatePageUrl, PagedListRenderOptions options, int firstPageToDisplay)
    {
        var previous = _tagBuilderFactory.Create("a");

        AppendHtml(previous, options.EllipsesFormat);

        previous.Attributes.Add("rel", "prev");
        previous.AddCssClass("PagedList-skipToPrevious");

        foreach (var c in options.PageClasses ?? Enumerable.Empty<string>())
        {
            previous.AddCssClass(c);
        }

        if (!list.HasPreviousPage)
        {
            return WrapInListItem(previous, options, options.EllipsesElementClass, "disabled");
        }

        int targetPageNumber = firstPageToDisplay - 1;

        previous.Attributes.Add("href", generatePageUrl(targetPageNumber));

        return WrapInListItem(previous, options, options.EllipsesElementClass);
    }

    /// <summary>
    /// Builds a "next ellipsis" item that links to the page after the last displayed page.
    /// </summary>
    /// <param name="list">The paged list.</param>
    /// <param name="generatePageUrl">Function that generates a URL for a given page number.</param>
    /// <param name="options">Render options.</param>
    /// <param name="lastPageToDisplay">The last page number currently displayed.</param>
    /// <returns>An &lt;li&gt; containing the ellipsis link or a disabled item.</returns>
    private TagBuilder NextEllipsis(IPagedList list, Func<int, string?> generatePageUrl, PagedListRenderOptions options, int lastPageToDisplay)
    {
        var next = _tagBuilderFactory.Create("a");

        AppendHtml(next, options.EllipsesFormat);

        next.Attributes.Add("rel", "next");
        next.AddCssClass("PagedList-skipToNext");

        foreach (var c in options.PageClasses ?? Enumerable.Empty<string>())
        {
            next.AddCssClass(c);
        }

        if (!list.HasNextPage)
        {
            return WrapInListItem(next, options, options.EllipsesElementClass, "disabled");
        }

        var targetPageNumber = lastPageToDisplay + 1;

        next.Attributes.Add("href", generatePageUrl(targetPageNumber));

        return WrapInListItem(next, options, options.EllipsesElementClass);
    }

    /// <summary>
    /// Builds a complete pager as an outer &lt;div&gt; containing a &lt;ul&gt; with list items for first/prev/next/last,
    /// page number links, ellipses, and optional informational items.
    /// </summary>
    /// <param name="pagedList">The paged list to render. If null, an empty list is assumed.</param>
    /// <param name="generatePageUrl">Function that generates a URL for a given page number.</param>
    /// <param name="options">Render options controlling appearance and behavior.</param>
    /// <returns>
    /// The HTML for the pager, or <c>null</c> if rendering is suppressed by <see cref="PagedListRenderOptions.Display"/>.
    /// </returns>
    public string? PagedListPager(IPagedList? pagedList, Func<int, string?> generatePageUrl, PagedListRenderOptions options)
    {
        var list = pagedList ?? new StaticPagedList<int>(ImmutableList<int>.Empty, 1, 10, 0);

        if (options.Display == PagedListDisplayMode.Never || (options.Display == PagedListDisplayMode.IfNeeded && list.PageCount <= 1))
        {
            return null;
        }

        var listItemLinks = new List<TagBuilder>();

        //calculate start and end of range of page numbers
        int firstPageToDisplay = 1;
        int lastPageToDisplay = list.PageCount;
        int pageNumbersToDisplay = lastPageToDisplay;

        if (options.MaximumPageNumbersToDisplay.HasValue && list.PageCount > options.MaximumPageNumbersToDisplay)
        {
            // cannot fit all pages into pager
            int maxPageNumbersToDisplay = options.MaximumPageNumbersToDisplay.Value;

            firstPageToDisplay = list.PageNumber - maxPageNumbersToDisplay / 2;

            if (firstPageToDisplay < 1)
            {
                firstPageToDisplay = 1;
            }

            pageNumbersToDisplay = maxPageNumbersToDisplay;
            lastPageToDisplay = firstPageToDisplay + pageNumbersToDisplay - 1;

            if (lastPageToDisplay > list.PageCount)
            {
                firstPageToDisplay = list.PageCount - maxPageNumbersToDisplay + 1;
            }
        }

        //first / previous
        var firstLink = (options.DisplayLinkToFirstPage == PagedListDisplayMode.Always ||
                         (options.DisplayLinkToFirstPage == PagedListDisplayMode.IfNeeded && firstPageToDisplay > 1))
            ? First(list, generatePageUrl, options)
            : null;

        var previousLink = (options.DisplayLinkToPreviousPage == PagedListDisplayMode.Always ||
                            (options.DisplayLinkToPreviousPage == PagedListDisplayMode.IfNeeded && !list.IsFirstPage))
            ? Previous(list, generatePageUrl, options)
            : null;

        if (options.PlaceFirstPageAfterPreviousAndLastPageBeforeNext)
        {
            if (previousLink != null) listItemLinks.Add(previousLink);
            if (firstLink != null) listItemLinks.Add(firstLink);
        }
        else
        {
            if (firstLink != null) listItemLinks.Add(firstLink);
            if (previousLink != null) listItemLinks.Add(previousLink);
        }

        //text
        if (options.DisplayPageCountAndCurrentLocation)
        {
            listItemLinks.Add(PageCountAndLocationText(list, options));
        }

        //text
        if (options.DisplayItemSliceAndTotal && options.ItemSliceAndTotalPosition == ItemSliceAndTotalPosition.Start)
        {
            listItemLinks.Add(ItemSliceAndTotalText(list, options));
        }

        //page
        if (options.DisplayLinkToIndividualPages)
        {
            //if there are previous page numbers not displayed, show an ellipsis
            if (options.DisplayEllipsesWhenNotShowingAllPageNumbers && firstPageToDisplay > 1)
            {
                listItemLinks.Add(PreviousEllipsis(list, generatePageUrl, options, firstPageToDisplay));
            }

            foreach (int i in Enumerable.Range(firstPageToDisplay, pageNumbersToDisplay))
            {
                //show delimiter between page numbers
                if (i > firstPageToDisplay && !string.IsNullOrWhiteSpace(options.DelimiterBetweenPageNumbers))
                {
                    listItemLinks.Add(WrapInListItem(options.DelimiterBetweenPageNumbers));
                }

                //show page number link
                listItemLinks.Add(Page(i, list, generatePageUrl, options));
            }

            //if there are subsequent page numbers not displayed, show an ellipsis
            if (options.DisplayEllipsesWhenNotShowingAllPageNumbers &&
                (firstPageToDisplay + pageNumbersToDisplay - 1) < list.PageCount)
            {
                listItemLinks.Add(NextEllipsis(list, generatePageUrl, options, lastPageToDisplay));
            }
        }

        //next / last
        var nextLink = (options.DisplayLinkToNextPage == PagedListDisplayMode.Always ||
                        (options.DisplayLinkToNextPage == PagedListDisplayMode.IfNeeded && !list.IsLastPage))
            ? Next(list, generatePageUrl, options)
            : null;

        var lastLink = (options.DisplayLinkToLastPage == PagedListDisplayMode.Always ||
                        (options.DisplayLinkToLastPage == PagedListDisplayMode.IfNeeded && lastPageToDisplay < list.PageCount))
            ? Last(list, generatePageUrl, options)
            : null;

        if (options.PlaceFirstPageAfterPreviousAndLastPageBeforeNext)
        {
            if (lastLink != null) listItemLinks.Add(lastLink);
            if (nextLink != null) listItemLinks.Add(nextLink);
        }
        else
        {
            if (nextLink != null) listItemLinks.Add(nextLink);
            if (lastLink != null) listItemLinks.Add(lastLink);
        }

        //text
        if (options.DisplayItemSliceAndTotal && options.ItemSliceAndTotalPosition == ItemSliceAndTotalPosition.End)
        {
            listItemLinks.Add(ItemSliceAndTotalText(list, options));
        }

        if (listItemLinks.Any())
        {
            //append class to first item in list?
            if (!string.IsNullOrWhiteSpace(options.ClassToApplyToFirstListItemInPager))
            {
                listItemLinks.First().AddCssClass(options.ClassToApplyToFirstListItemInPager);
            }

            //append class to last item in list?
            if (!string.IsNullOrWhiteSpace(options.ClassToApplyToLastListItemInPager))
            {
                listItemLinks.Last().AddCssClass(options.ClassToApplyToLastListItemInPager);
            }

            //append classes to all list item links
            foreach (var li in listItemLinks)
            {
                foreach (var c in options.LiElementClasses ?? Enumerable.Empty<string>())
                {
                    li.AddCssClass(c);
                }
            }
        }

        //Collapse all the list items into one big string
        var listItemLinksString = listItemLinks.Aggregate(
            new StringBuilder(),
            (sb, listItem) => sb.Append(TagBuilderToString(listItem)),
            sb => sb.ToString());

        var ul = _tagBuilderFactory.Create("ul");

        AppendHtml(ul, listItemLinksString);

        foreach (var c in options.UlElementClasses ?? Enumerable.Empty<string>())
        {
            ul.AddCssClass(c);
        }

        if (options.UlElementattributes != null)
        {
            foreach (var c in options.UlElementattributes)
            {
                ul.MergeAttribute(c.Key, c.Value);
            }
        }

        var outerDiv = _tagBuilderFactory
            .Create("div");

        foreach (var c in options.ContainerDivClasses ?? Enumerable.Empty<string>())
        {
            outerDiv.AddCssClass(c);
        }

        AppendHtml(outerDiv, TagBuilderToString(ul));

        return TagBuilderToString(outerDiv);
    }

    /// <summary>
    /// Builds a "go to page" form that submits a GET request with the desired page number.
    /// </summary>
    /// <param name="list">The current paged list used to initialize the input with the current page number.</param>
    /// <param name="formAction">The form action URL.</param>
    /// <param name="options">Options controlling the fields, classes, and sizes.</param>
    /// <returns>The HTML for the form.</returns>
    public string PagedListGoToPageForm(IPagedList list, string formAction, GoToFormRenderOptions options)
    {
        var form = _tagBuilderFactory.Create("form");

        form.AddCssClass("PagedList-goToPage");
        form.Attributes.Add("action", formAction);
        form.Attributes.Add("method", "get");

        var fieldset = _tagBuilderFactory.Create("fieldset");

        var label = _tagBuilderFactory.Create("label");

        label.Attributes.Add("for", options.InputFieldName);

        SetInnerText(label, options.LabelFormat);

        var input = _tagBuilderFactory.Create("input");

        input.Attributes.Add("type", options.InputFieldType);
        input.Attributes.Add("name", options.InputFieldName);
        input.Attributes.Add("value", list.PageNumber.ToString());
        input.Attributes.Add("class", options.InputFieldClass);
        input.Attributes.Add("Style", $"width: {options.InputWidth}px");

        var submit = _tagBuilderFactory.Create("input");

        submit.Attributes.Add("type", "submit");
        submit.Attributes.Add("value", options.SubmitButtonFormat);
        submit.Attributes.Add("class", options.SubmitButtonClass);
        submit.Attributes.Add("Style", $"width: {options.SubmitButtonWidth}px");

        AppendHtml(fieldset, TagBuilderToString(label));
        AppendHtml(fieldset, TagBuilderToString(input, TagRenderMode.SelfClosing));
        AppendHtml(fieldset, TagBuilderToString(submit, TagRenderMode.SelfClosing));
        AppendHtml(form, TagBuilderToString(fieldset));

        return TagBuilderToString(form);
    }
}
