using SOFTURE.Typesense.Abstractions.Enums;

namespace SOFTURE.Typesense.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FilterOperatorAttribute : Attribute
{
    public FilterOperatorAttribute(FilterOperator @operator)
    {
        Operator = @operator;
    }

    public FilterOperator Operator { get; }
}
