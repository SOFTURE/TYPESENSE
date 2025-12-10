# Filtering Guide

This document provides comprehensive guidance on using filtering capabilities in SOFTURE.Typesense.

## Overview

SOFTURE.Typesense supports all 10 Typesense filtering operators through a type-safe, attribute-based approach. Instead of building filter strings manually, you define filter properties with attributes that specify which operator to use.

## How Filtering Works

### 1. Define Filter Models

Create a class inheriting from `FilterBase` and decorate properties with `FilterOperatorAttribute`:

```csharp
public class ProductFilters : FilterBase
{
    public ProductFilters() : base("products") { }
    
    // Property without attribute uses default operator (:)
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    // Property with specific operator
    [FilterOperator(FilterOperator.ExactEquals)]
    [JsonPropertyName("category")]
    public string? Category { get; set; }
}
```

### 2. Operator Mapping

Each `FilterOperator` enum value maps to a Typesense operator:

```csharp
FilterOperator.Equals           → ":"     // Partial match
FilterOperator.ExactEquals      → "="     // Exact match  
FilterOperator.NotEquals        → "!="    // Not equal
FilterOperator.LessThan         → "<"     // Less than
FilterOperator.GreaterThan      → ">"     // Greater than
FilterOperator.LessThanOrEquals → "<="    // Less than or equal
FilterOperator.GreaterThanOrEquals → ">=" // Greater than or equal
FilterOperator.In               → "[]"    // Is one of
FilterOperator.NotIn            → "![]"   // Is not any of
FilterOperator.Range            → "[..]"  // Range
```

## Complete Operator Reference

### String Operators

#### Partial Match (Default)
```csharp
[JsonPropertyName("name")]
public string? Name { get; set; }
```
**Usage:** `Name = "john"` → `name:john`  
**Matches:** "john", "johnny", "johnson"

#### Exact Match
```csharp
[FilterOperator(FilterOperator.ExactEquals)]
[JsonPropertyName("country")]
public string? Country { get; set; }
```
**Usage:** `Country = "USA"` → `country:=USA`  
**Matches:** Only exactly "USA"

#### Not Equal
```csharp
[FilterOperator(FilterOperator.NotEquals)]
[JsonPropertyName("status")]
public string? Status { get; set; }
```
**Usage:** `Status = "inactive"` → `status:!=inactive`  
**Matches:** Everything except "inactive"

### Numeric Operators

#### Greater Than
```csharp
[FilterOperator(FilterOperator.GreaterThan)]
[JsonPropertyName("price")]
public decimal? MinPrice { get; set; }
```
**Usage:** `MinPrice = 100` → `price:>100`

#### Less Than
```csharp
[FilterOperator(FilterOperator.LessThan)]
[JsonPropertyName("price")]
public decimal? MaxPrice { get; set; }
```
**Usage:** `MaxPrice = 500` → `price:<500`

#### Greater Than or Equal
```csharp
[FilterOperator(FilterOperator.GreaterThanOrEquals)]
[JsonPropertyName("rating")]
public float? MinRating { get; set; }
```
**Usage:** `MinRating = 4.5f` → `rating:>=4.5`

#### Less Than or Equal
```csharp
[FilterOperator(FilterOperator.LessThanOrEquals)]
[JsonPropertyName("max_quantity")]
public int? MaxQuantity { get; set; }
```
**Usage:** `MaxQuantity = 10` → `max_quantity:<=10`

#### Range
```csharp
[FilterOperator(FilterOperator.Range)]
[JsonPropertyName("price")]
public (decimal From, decimal To)? PriceRange { get; set; }
```
**Usage:** `PriceRange = (100, 500)` → `price:[100..500]`

### Array Operators

#### Is One Of
```csharp
[FilterOperator(FilterOperator.In)]
[JsonPropertyName("category")]
public string[]? Categories { get; set; }
```
**Usage:** `Categories = ["electronics", "books"]` → `category:[electronics,books]`

#### Is Not Any Of
```csharp
[FilterOperator(FilterOperator.NotIn)]
[JsonPropertyName("tag")]
public string[]? ExcludedTags { get; set; }
```
**Usage:** `ExcludedTags = ["spam", "adult"]` → `tag:![spam,adult]`

### Boolean Operators

```csharp
[JsonPropertyName("is_active")]
public bool? IsActive { get; set; }

[FilterOperator(FilterOperator.NotEquals)]
[JsonPropertyName("is_featured")]
public bool? IsNotFeatured { get; set; }
```

## Best Practices

### 1. Property Naming

Use clear, descriptive names that indicate the operation:

```csharp
// Good
public decimal? MinPrice { get; set; }
public decimal? MaxPrice { get; set; }
public string[]? ExcludedCategories { get; set; }

// Avoid
public decimal? Price1 { get; set; }
public decimal? Price2 { get; set; }
public string[]? Categories2 { get; set; }
```

### 2. Multiple Constraints on Same Field

You can apply multiple operators to the same field by creating separate properties:

```csharp
public class ProductFilters : FilterBase
{
    // Minimum price constraint
    [FilterOperator(FilterOperator.GreaterThanOrEquals)]
    [JsonPropertyName("price")]
    public decimal? MinPrice { get; set; }
    
    // Maximum price constraint  
    [FilterOperator(FilterOperator.LessThanOrEquals)]
    [JsonPropertyName("price")]
    public decimal? MaxPrice { get; set; }
    
    // Or use range for the same effect
    [FilterOperator(FilterOperator.Range)]
    [JsonPropertyName("price")]
    public (decimal From, decimal To)? PriceRange { get; set; }
}
```

### 3. Faceting Requirements

Ensure fields you want to filter on are configured with `facet: true` in your collection configuration:

```csharp
new Field("category", FieldType.String, facet: true),  // Can filter
new Field("description", FieldType.String, facet: false) // Cannot filter
```

### 4. Type Safety

Use appropriate .NET types that match your Typesense field types:

```csharp
// String fields
public string? Name { get; set; }
public string[]? Tags { get; set; }

// Numeric fields
public int? Quantity { get; set; }
public decimal? Price { get; set; }
public float? Rating { get; set; }
public long? Timestamp { get; set; }

// Boolean fields
public bool? IsActive { get; set; }
```

### 5. Nullable Properties

Always use nullable types for filter properties to distinguish between "not set" and actual values:

```csharp
// Correct
public int? MinAge { get; set; }        // null = no filter, 0 = filter by 0
public bool? IsActive { get; set; }     // null = no filter, false = filter by false

// Incorrect
public int MinAge { get; set; }         // Can't distinguish unset from 0
public bool IsActive { get; set; }      // Can't distinguish unset from false
```

### 6. Performance Considerations

- Use exact match (`=`) instead of partial match (`:`) when you need exact values
- Prefer `In` operator over multiple `Equals` filters for the same field
- Use ranges instead of separate greater/less than filters when possible

```csharp
// Better performance
[FilterOperator(FilterOperator.In)]
[JsonPropertyName("status")]
public string[]? Statuses { get; set; }  // status:[active,pending]

// Less efficient
public string? Status1 { get; set; }  // status:active
public string? Status2 { get; set; }  // status:pending
```

## Common Patterns

### Search with Price Range
```csharp
var filters = new ProductFilters
{
    Categories = ["electronics"],        // category:[electronics]
    PriceRange = (100, 1000),           // price:[100..1000]
    MinRating = 4.0f,                   // rating:>=4.0
    ExcludedTags = ["discontinued"]     // tag:![discontinued]
};
```

### User Content Filtering
```csharp
var filters = new PostFilters
{
    IsActive = true,                    // is_active:true
    ExcludedStatuses = ["spam", "deleted"], // status:![spam,deleted]
    MinScore = 0,                       // score:>=0
    AuthorName = "john"                 // author_name:john (partial)
};
```

### Date Range Filtering
```csharp
var filters = new EventFilters
{
    StartDate = DateTimeOffset.Now.ToUnixTimeSeconds(),  // start_date:>=1640995200
    EndDate = DateTimeOffset.Now.AddDays(30).ToUnixTimeSeconds(), // end_date:<=1643673600
    Categories = ["conference", "workshop"]             // category:[conference,workshop]
};
```