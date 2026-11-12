using FluentAssertions;
using SOFTURE.Typesense.Extensions;
using SOFTURE.Typesense.ValueObjects;
using Typesense;
using Xunit;

namespace SOFTURE.Typesense.Tests.Unit;

public sealed class CollectionConfigurationTests
{
    [Fact]
    public void Create_WhenDocumentHasEveryRequiredField_Succeeds()
    {
        var result = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, ProductCollection.Fields(), "quantity");

        result.IsSuccess.Should().BeTrue();
        result.Value.GetSchema().Name.Should().Be(ProductCollection.Name);
        result.Value.GetSchema().DefaultSortingField.Should().Be("quantity");
    }

    [Fact]
    public void Create_WhenSchemaHasFieldMissingInDocument_FailsWithFieldName()
    {
        var fields = ProductCollection.Fields();
        fields.Add(new Field("price", FieldType.Float));

        var result = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, fields, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("'price'");
    }

    [Fact]
    public void Create_WhenMissingFieldIsOptional_Succeeds()
    {
        var fields = ProductCollection.Fields();
        fields.Add(new Field("price", FieldType.Float) { Optional = true });

        CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, fields, null).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WhenDefaultSortingFieldIsUnknown_Fails()
    {
        var result = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, ProductCollection.Fields(), "price");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("'price'");
    }

    [Fact]
    public void ConfigureCollection_WhenConfigurationIsInvalid_Throws()
    {
        var configurations = new List<CollectionConfiguration>();

        var act = () => configurations.ConfigureCollection<ProductDocument, ProductQuery, ProductFilters>(ProductCollection.Name, [new Field("price", FieldType.Float)]);

        act.Should().Throw<InvalidOperationException>().WithMessage("*'price'*");
    }

    [Fact]
    public void Equality_DependsOnlyOnCollectionName()
    {
        var first = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, ProductCollection.Fields(), null).Value;
        var second = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, ProductCollection.Fields(), "quantity").Value;
        var other = CollectionConfiguration.Create<ProductDocument>("other_products", ProductCollection.Fields(), null).Value;

        first.Should().Be(second);
        first.Should().NotBe(other);
    }

    [Fact]
    public void ISchemaModified_WhenFieldsMatchWithDefaults_ReturnsFalse()
    {
        var configuration = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, ProductCollection.Fields(), null).Value;

        var existing = new List<Field>
        {
            new("name", FieldType.String) { Sort = true, Facet = false, Index = true, Optional = false, Infix = false, Locale = "" },
            new("city", FieldType.String) { Facet = true, Sort = false, Index = true, Optional = false, Infix = false, Locale = "" },
            new("is_active", FieldType.Bool) { Facet = false, Sort = true, Index = true, Optional = false, Infix = false, Locale = "" },
            new("quantity", FieldType.Int32) { Facet = false, Sort = true, Index = true, Optional = false, Infix = false, Locale = "" },
            new("timestamp", FieldType.Int64) { Facet = false, Sort = true, Index = true, Optional = false, Infix = false, Locale = "" },
            new("tags", FieldType.StringArray) { Facet = true, Sort = false, Index = true, Optional = false, Infix = false, Locale = "" }
        };

        configuration.ISchemaModified(existing).Should().BeFalse();
    }

    [Fact]
    public void ISchemaModified_WhenServerReportsDefaultSortOnFloatField_ReturnsFalse()
    {
        var fields = ProductCollection.Fields();
        fields.Add(new Field("price", FieldType.Float) { Optional = true });
        var configuration = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, fields, null).Value;

        var existing = ProductCollection.Fields();
        existing.Add(new Field("price", FieldType.Float) { Optional = true, Sort = true });

        configuration.ISchemaModified(existing).Should().BeFalse();
    }

    [Fact]
    public void ISchemaModified_WhenFieldTypeChanged_ReturnsTrue()
    {
        var configuration = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, ProductCollection.Fields(), null).Value;
        var existing = ProductCollection.Fields();
        existing[3] = new Field("quantity", FieldType.Int64);

        configuration.ISchemaModified(existing).Should().BeTrue();
    }

    [Fact]
    public void ISchemaModified_WhenFieldCountDiffers_ReturnsTrue()
    {
        var configuration = CollectionConfiguration.Create<ProductDocument>(ProductCollection.Name, ProductCollection.Fields(), null).Value;

        configuration.ISchemaModified(ProductCollection.Fields().Take(5).ToList()).Should().BeTrue();
        configuration.ISchemaModified(null).Should().BeTrue();
    }
}
