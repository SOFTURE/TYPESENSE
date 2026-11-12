using FluentAssertions;
using SOFTURE.Typesense.Abstractions;
using SOFTURE.Typesense.Abstractions.Enums;
using SOFTURE.Typesense.Abstractions.Extensions;
using SOFTURE.Typesense.Abstractions.Models;
using Xunit;

namespace SOFTURE.Typesense.Tests.Unit;

public sealed class SearchBuildersTests
{
    [Fact]
    public void FilterBy_WithEveryKindOfFilter_JoinsThemInPropertyOrder()
    {
        var filters = new ProductFilters
        {
            City = "Warszawa",
            IsActive = true,
            QuantityAbove = 5,
            Timestamp = (100, 200),
            Tags = ["Red", "Blue"],
            ExcludedNames = ["Old"]
        };

        filters.FilterBy().Should().Be(
            "city:warszawa && is_active:true && quantity:>5 && timestamp:[100..200] && tags:=[red,blue] && name:![old]");
    }

    [Fact]
    public void FilterBy_WhenEveryFilterIsEmpty_ReturnsEmptyString()
    {
        var filters = new ProductFilters { Tags = [] };

        filters.FilterBy().Should().BeEmpty();
    }

    [Fact]
    public void FilterBy_WithOnlyBooleanFalse_KeepsTheFilter()
    {
        var filters = new ProductFilters { IsActive = false };

        filters.FilterBy().Should().Be("is_active:false");
    }

    [Theory]
    [InlineData(FilterOperator.Equals, ":")]
    [InlineData(FilterOperator.ExactEquals, "=")]
    [InlineData(FilterOperator.NotEquals, "!=")]
    [InlineData(FilterOperator.LessThan, "<")]
    [InlineData(FilterOperator.GreaterThan, ">")]
    [InlineData(FilterOperator.LessThanOrEquals, "<=")]
    [InlineData(FilterOperator.GreaterThanOrEquals, ">=")]
    [InlineData(FilterOperator.Range, "[..]")]
    [InlineData(FilterOperator.In, "[]")]
    [InlineData(FilterOperator.NotIn, "![]")]
    public void ToOperatorString_MapsEveryOperator(FilterOperator @operator, string expected)
    {
        @operator.ToOperatorString().Should().Be(expected);
    }

    [Fact]
    public void QueryBase_WithValues_ReturnsTextAndQueryByInPropertyOrder()
    {
        var query = new ProductQuery { Name = "Laptop", City = "Krakow" };

        query.Text().Should().Be("Laptop, Krakow");
        query.QueryBy().Should().Be("name, city");
        query.CanSearch().Should().BeTrue();
    }

    [Fact]
    public void QueryBase_WithoutValues_SearchesEverything()
    {
        var query = new ProductQuery();

        query.Text().Should().Be("*");
        query.QueryBy().Should().BeEmpty();
        query.CanSearch().Should().BeFalse();
    }

    [Fact]
    public void SortBy_KeepsInsertionOrderAndOverridesRepeatedField()
    {
        var sort = new ProductSort();

        sort.Add("quantity", SortDirection.Desc);
        sort.Add("name", SortDirection.Asc);
        sort.Add("quantity", SortDirection.Asc);

        sort.SortBy().Should().Be("quantity:asc,name:asc");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Collection_Create_WithBlankName_Fails(string name)
    {
        Collection.Create(name).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Collection_ConvertsFromAndToString()
    {
        Collection collection = "products";
        string name = collection;

        name.Should().Be("products");
        collection.Should().Be(Collection.Create("products").Value);
    }
}
