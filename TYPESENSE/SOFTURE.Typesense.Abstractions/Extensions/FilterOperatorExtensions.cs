using SOFTURE.Typesense.Abstractions.Enums;

namespace SOFTURE.Typesense.Abstractions.Extensions;

public static class FilterOperatorExtensions
{
    public static string ToOperatorString(this FilterOperator @operator)
    {
        return @operator switch
        {
            FilterOperator.Equals => ":",
            FilterOperator.NotEquals => "!=",
            FilterOperator.LessThan => "<",
            FilterOperator.GreaterThan => ">",
            FilterOperator.LessThanOrEquals => "<=",
            FilterOperator.GreaterThanOrEquals => ">=",
            FilterOperator.Range => "[..]",
            _ => ":"
        };
    }
}
