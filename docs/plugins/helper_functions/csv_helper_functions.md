# Csv Helper Functions

Csv Helper Functions use `CsvHelper` to convert objects to csv and export them to a file.

```csharp
 public async Task ExportCsvAsync<T>(string fileName, IEnumerable<T> records, CancellationToken cancellationToken = default);
```

```csharp
public void ExportCsv(string fileName, List<object> records);
```
