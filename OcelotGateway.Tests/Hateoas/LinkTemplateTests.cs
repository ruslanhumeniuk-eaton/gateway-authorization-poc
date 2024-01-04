using OcelotGateway.Hateoas;

namespace OcelotGateway.Tests.Hateoas;

/// <summary>
///     Test link templating.
/// </summary>
public class LinkTemplateTests
{
    /// <summary>
    ///     Assert that a templated link formatted without place holders returns the same template.
    /// </summary>
    [Fact]
    public void TemplateWithPlaceHolders_FormattedWithNoPlaceHolders_ReturnsSameTemplate()
    {
        // Arrange
        var pagePlaceHolder = "page";
        var pageSizePlaceHolder = "pageSize";
        var sortByPlaceHolder = "sortBy";
        var sortOrderPlaceHolder = "sortOrder";
        var filterByPlaceHolder = "filterBy";
        var filterCriteriaPlaceHolder = "filterCriteria";
        string template =
            $"/contracts?page={{{pagePlaceHolder}}}&pageSize={{{pageSizePlaceHolder}}}&sortBy={{{sortByPlaceHolder}}}&sortOrder={{{sortOrderPlaceHolder}}}&filterBy={{{filterByPlaceHolder}}}&filterCriteria={{{filterCriteriaPlaceHolder}}}";
        var linkTemplate = new LinkTemplate(template);

        // Act
        string formattedLinkTemplate = linkTemplate.Format(null);

        // Assert
        Assert.Equal(template, formattedLinkTemplate);
    }

    /// <summary>
    ///     Assert that a link not templated formatted without place holders returns the same link.
    /// </summary>
    [Fact]
    public void TemplateWithNoPlaceHolders_FormattedWithNoPlaceHolders_ReturnsSameTemplate()
    {
        // Arrange
        var template = "/contracts";
        var linkTemplate = new LinkTemplate(template);

        // Act
        string formattedLinkTemplate = linkTemplate.Format(null);

        // Assert
        Assert.Equal(template, formattedLinkTemplate);
    }

    /// <summary>
    ///     Assert that a templated link formatted with all place holders return a link without template.
    /// </summary>
    [Fact]
    public void TemplatedLinkWithPlaceHolders_FormattedWithAllPlaceHolders_ReturnsLinkWithoutTemplate()
    {
        // Arrange
        var pagePlaceHolder = "page";
        var pageSizePlaceHolder = "pageSize";
        var sortByPlaceHolder = "sortBy";
        var sortOrderPlaceHolder = "sortOrder";
        var filterByPlaceHolder = "filterBy";
        var filterCriteriaPlaceHolder = "filterCriteria";
        string template =
            $"/contracts?page={{{pagePlaceHolder}}}&pageSize={{{pageSizePlaceHolder}}}&sortBy={{{sortByPlaceHolder}}}&sortOrder={{{sortOrderPlaceHolder}}}&filterBy={{{filterByPlaceHolder}}}&filterCriteria={{{filterCriteriaPlaceHolder}}}";
        var linkTemplate = new LinkTemplate(template);

        // Act
        string templatedLink = linkTemplate.Format(new List<KeyValuePair<string, string>>
        {
            new (pagePlaceHolder, "1"),
            new (pageSizePlaceHolder, "%20"),
            new (sortByPlaceHolder, "     "),
            new (sortOrderPlaceHolder, "  \t"),
            new (filterByPlaceHolder,
                "4EZwzyJTEdLAGXfOAKajw4Mtn_Fn-ckbksFTK4aA3rV0MYsvM8TMszv7eDaAg1xN"),
            new (filterCriteriaPlaceHolder, null)
        });

        // Assert
        Assert.Equal(
            "/contracts?page=1&pageSize=%2520&sortBy=+++++&sortOrder=++%09&filterBy=4EZwzyJTEdLAGXfOAKajw4Mtn_Fn-ckbksFTK4aA3rV0MYsvM8TMszv7eDaAg1xN&filterCriteria={filterCriteria}",
            templatedLink);
    }

    /// <summary>
    ///     Assert that a templated link formatted with only a part of the place holders return a templated link.
    /// </summary>
    [Fact]
    public void TemplatedLinkWithPlaceHolders_FormattedWithPartPlaceHolders_ReturnsLinkWithTemplate()
    {
        // Arrange
        var pagePlaceHolder = "page";
        var pageSizePlaceHolder = "pageSize";
        var sortByPlaceHolder = "sortBy";
        var sortOrderPlaceHolder = "sortOrder";
        var filterByPlaceHolder = "filterBy";
        var filterCriteriaPlaceHolder = "filterCriteria";
        string template =
            $"/contracts?page={{{pagePlaceHolder}}}&pageSize={{{pageSizePlaceHolder}}}&sortBy={{{sortByPlaceHolder}}}&sortOrder={{{sortOrderPlaceHolder}}}&filterBy={{{filterByPlaceHolder}}}&filterCriteria={{{filterCriteriaPlaceHolder}}}";
        var linkTemplate = new LinkTemplate(template);

        // Act
        string templatedLink = linkTemplate.Format(new List<KeyValuePair<string, string>>
        {
            new (pagePlaceHolder, "1"),
            new (pageSizePlaceHolder, "lorem"),
            new (sortByPlaceHolder, "     "),
            new (sortOrderPlaceHolder, "  \t"),
            new (filterByPlaceHolder, "<>")
        });

        // Assert
        Assert.Equal(
            "/contracts?page=1&pageSize=lorem&sortBy=+++++&sortOrder=++%09&filterBy=%3c%3e&filterCriteria={filterCriteria}",
            templatedLink);
    }

    /// <summary>
    ///     Assert that a templated link with two same place holders formatted with only one of the place holder returns a
    ///     templated link only with the first occurrence replaced.
    /// </summary>
    [Fact]
    public void TemplateWithSamePlaceHolders_FormattedWithOnePlaceHolder_ReturnsOnlyFirstOccurrenceReplaced()
    {
        // Arrange
        var pagePlaceHolder = "page";
        string template =
            $"/contracts?page={{{pagePlaceHolder}}}&page={{{pagePlaceHolder}}}";
        var linkTemplate = new LinkTemplate(template);

        // Act
        string formattedLinkTemplate = linkTemplate.Format(new List<KeyValuePair<string, string>>
            { new (pagePlaceHolder, "10") });

        // Assert
        Assert.Equal($"/contracts?page=10&page={{{pagePlaceHolder}}}", formattedLinkTemplate);
    }
}