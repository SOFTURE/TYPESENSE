Lista funkcjonalności do zaimplementowania w SOFTURE.Typesense

1. Range Operators (przedziały wartości)

Format: {from}..{to}

Używane w:
- CompanyStartDateRange: string? → "{fromTimestamp}..{toTimestamp}" (int64)
- LastSurveyDateRange: string? → "{fromTimestamp}..{toTimestamp}" (int64)
- ActiveReservationExpirationRange: string? → "{now}..{in30days}" (int64)
- ReservationCountRange: string? → "{from}..{to}" (int32)
- CreatedDateRange: string? → "{from}..{to}" (int64)

Wymagane:
- Parsowanie formatu "{min}..{max}" dla pól typu string
- Generowanie Typesense query: field:[min..max]
- Wsparcie dla int32 i int64 timestamp

  ---
2. Comparison Operators (operatory porównania)

Format: >={value}, <={value}, >{value}, <{value}

Używane w:
- ScoringRange: string? → ">={value}" (int32)

Wymagane:
- Parsowanie operatorów: >=, <=, >, <
- Generowanie Typesense query: field:>=value, field:>value, etc.

  ---
3. Negation Operator (operator != dla tablic)

Format: !={value} w string[]

Używane w:
- HistoricalSellerIds: string[]? → new[] { "!={userId}" }

Wymagane:
- Parsowanie formatu "!={value}" w elementach tablicy
- Generowanie Typesense query: field:!=[value1,value2,...]
- Wsparcie dla negacji w array filters

  ---
4. Multiple Values (array filters) - może już działa

Format: string[] z wartościami

Używane w:
- HistoricalSellerIds: string[]? → new[] { "value1", "value2" }

Wymagane (do weryfikacji):
- Generowanie Typesense query: field:=[value1,value2,value3]

  ---
5. Attribute Mapping (snake_case konwersja)

Wymagane:
- Auto-konwersja nazw pól C# PascalCase → Typesense snake_case
- Przykład: IsActive → is_active, CompanyStartDateRange → company_start_date_timestamp
- Wsparcie dla atrybutów [JsonPropertyName] lub custom attribute

  ---
6. Null Handling

Wymagane:
- Ignorowanie filtrów z wartością null (nie dodawać do query)
- Dotyczy wszystkich typów: bool?, int?, string?, string[]?

  ---
Priorytety implementacji

Priority 1 (Critical) - blokują obecną funkcjonalność:

1. ✅ Range operators (.. syntax) - używane w 5 miejscach
2. ✅ Comparison operators (>=, etc.) - używane dla scoring
3. ✅ Negation in arrays (!= syntax) - używane dla HideHistoricalClients

Priority 2 (High) - potrzebne dla prawidłowego działania:

4. ✅ Attribute name mapping (PascalCase → snake_case)
5. ✅ Null handling

Priority 3 (Medium) - nice-to-have:

6. Array filters (może już działa, do weryfikacji)

  ---
Przykłady oczekiwanego mappingu

// Input (C# ClientFilters):
ScoringRange = ">=50"
CompanyStartDateRange = "1609459200..1640995200"
HistoricalSellerIds = new[] { "!=user123", "!=user456" }

// Expected Typesense query:
filter_by: scoring:>=50 && company_start_date_timestamp:[1609459200..1640995200] && active_seller_id:!=[user123,user456]

  ---
Dodatkowe requirements

Type Safety

- Walidacja operatorów dla odpowiednich typów pól
- Range operators tylko dla int32/int64
- Comparison operators tylko dla numerów

Error Handling

- Błąd jeśli nieprawidłowy format range "{from}..{to}"
- Błąd jeśli nieznany operator

Performance

- Reflection caching dla attribute mapping
- Lazy evaluation dla filter building
