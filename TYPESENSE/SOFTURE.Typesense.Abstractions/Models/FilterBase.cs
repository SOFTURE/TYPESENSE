using System.Reflection;
using System.Text.Json.Serialization;
using SOFTURE.Typesense.Abstractions.Attributes;
using SOFTURE.Typesense.Abstractions.Enums;
using SOFTURE.Typesense.Abstractions.Extensions;

namespace SOFTURE.Typesense.Abstractions.Models;

public abstract class FilterBase : SearchBase
{
    protected FilterBase(Collection collection) : base(collection)
    {
    }

    public string FilterBy()
    {
        var filterBy = new List<string>();

        foreach (var property in GetProperties())
        {
            var value = property.GetValue(this);
            var name = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name.ToLower();
            var operatorAttr = property.GetCustomAttribute<FilterOperatorAttribute>();

            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                continue;
            }

            if (property.PropertyType.IsArray || (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(property.PropertyType)?.IsArray == true))
            {
                var arrayType = property.PropertyType.IsArray
                    ? property.PropertyType
                    : Nullable.GetUnderlyingType(property.PropertyType);

                var arrayOperator = operatorAttr?.Operator ?? FilterOperator.Equals;
                var formattedArrayValue = FormatArrayValue(value, arrayType!, arrayOperator);
                if (!string.IsNullOrEmpty(formattedArrayValue))
                {
                    filterBy.Add($"{name}:{formattedArrayValue}");
                }
                continue;
            }

            var @operator = operatorAttr?.Operator ?? FilterOperator.Equals;
            var operatorString = @operator.ToOperatorString();
            var formattedValue = FormatValue(value, @operator, property.PropertyType);

            var filter = @operator is FilterOperator.Equals or FilterOperator.Range
                ? $"{name}:{formattedValue}"
                : $"{name}:{operatorString}{formattedValue}";

            filterBy.Add(filter);
        }

        return string.Join(" && ", filterBy);
    }

    private static string FormatValue(object value, FilterOperator @operator, Type propertyType)
    {
        if (propertyType == typeof(bool) || propertyType == typeof(bool?))
        {
            return value.ToString()!.ToLower();
        }

        if (@operator == FilterOperator.Range)
        {
            if (propertyType.IsGenericType &&
                propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                propertyType.GetGenericArguments()[0].IsGenericType)
            {
                var underlyingType = Nullable.GetUnderlyingType(propertyType);
                if (underlyingType != null && underlyingType.Name.StartsWith("ValueTuple"))
                {
                    var tuple = Convert.ChangeType(value, underlyingType);
                    var fields = underlyingType.GetFields();
                    if (fields.Length == 2)
                    {
                        var from = fields[0].GetValue(tuple);
                        var to = fields[1].GetValue(tuple);
                        return $"[{from}..{to}]";
                    }
                }
            }
        }

        var isNumericOperator = @operator is FilterOperator.LessThan or FilterOperator.GreaterThan or FilterOperator.LessThanOrEquals or FilterOperator.GreaterThanOrEquals or FilterOperator.NotEquals;
        var isStringType = propertyType == typeof(string) ||
                          (propertyType.IsGenericType && Nullable.GetUnderlyingType(propertyType) == typeof(string));

        if (isStringType && !isNumericOperator)
            return value.ToString()!.ToLower();

        return value.ToString()!;
    }

    private static string FormatArrayValue(object value, Type arrayType, FilterOperator? @operator)
    {
        if (value is not Array array || array.Length == 0)
            return string.Empty;

        var elementType = arrayType.GetElementType();
        if (elementType == null)
        {
            return string.Empty;
        }

        var values = new List<string>();

        foreach (var item in array)
        {
            if (item == null)
            {
                continue;
            }

            var stringValue = item.ToString()!;

            values.Add(elementType == typeof(string) ? stringValue.ToLower() : stringValue);
        }

        if (values.Count == 0)
            return string.Empty;

        var formattedValues = string.Join(",", values);

        return @operator switch
        {
            FilterOperator.In => $"[{formattedValues}]",
            FilterOperator.NotIn => $"![{formattedValues}]",
            FilterOperator.NotEquals => $"!=[{formattedValues}]",
            _ => $"=[{formattedValues}]"
        };
    }
}