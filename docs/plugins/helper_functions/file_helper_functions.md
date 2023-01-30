# File Helper Functions

```csharp
public void FileWrite(string fileName, string fileContent);
```

```csharp
public async Task FileWriteAsync(string fileName, string fileContent, CancellationToken cancellationToken = default)
```

```csharp
public async Task FileWriteStreamAsync(string fileName, Stream stream,CancellationToken cancellationToken = default)
```

```csharp
public void FileWriteStream(string fileName, Stream stream)
```
