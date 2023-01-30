# Json Helper Functions

Json Helper Functions use `Newtonsoft.Json` to convert objects to json and export them to a file.

```csharp
public async Task ExportJsonAsync<T>(string fileName, T obj, JsonSerializerSettings? settings = default,CancellationToken cancellationToken = default)
```

```csharp
public void ExportJson(string fileName, object obj)
```

```csharp
public string ConvertJson(object obj);
```
