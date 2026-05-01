using System.Collections;
using System.Reflection;
using DxWorks.ScriptBee.Plugin.Api.Model;
using ScriptBee.Domain.Model.Context;
using ScriptBee.UseCases.Analysis;

namespace ScriptBee.Service.Analysis;

public class GetContextGraphService(IProjectManager projectManager) : IGetContextGraphUseCase
{
    public ContextGraphResult SearchNodes(string query, int offset, int limit)
    {
        var project = projectManager.GetProject();

        var filteredModels = project
            .Context.Models.SelectMany(entry =>
                entry.Value.Select(inner =>
                    (id: $"{entry.Key.Item1}_{entry.Key.Item2}_{inner.Key}", model: inner.Value)
                )
            )
            .Where(x => Matches(x.id, x.model, query))
            .Skip(offset)
            .Take(limit)
            .ToList();

        var nodes = filteredModels.Select(x => MapToNode(x.id, x.model)).ToList();

        var edges = new List<ContextGraphEdge>();
        var objectToNodeIdMap = BuildObjectToNodeIdMap(project);
        var filteredNodeIds = nodes.Select(n => n.Id).ToHashSet();

        foreach (var x in filteredModels)
        {
            foreach (var (key, value) in GetAllProperties(x.model))
            {
                if (value == null)
                {
                    continue;
                }

                if (objectToNodeIdMap.TryGetValue(value, out var targetId))
                {
                    if (filteredNodeIds.Contains(targetId))
                    {
                        edges.Add(new ContextGraphEdge(x.id, targetId, key));
                    }
                }
                else if (value is IEnumerable enumerable and not string)
                {
                    foreach (var item in enumerable)
                    {
                        if (
                            item == null
                            || !objectToNodeIdMap.TryGetValue(item, out var listTargetId)
                        )
                        {
                            continue;
                        }
                        if (filteredNodeIds.Contains(listTargetId))
                        {
                            edges.Add(new ContextGraphEdge(x.id, listTargetId, key));
                        }
                    }
                }
            }
        }

        return new ContextGraphResult(nodes, edges);
    }

    public ContextGraphResult GetNeighbors(string nodeId)
    {
        var project = projectManager.GetProject();
        var objectToNodeIdMap = BuildObjectToNodeIdMap(project);
        var nodeIdToObjectMap = BuildNodeIdToObjectMap(project);

        if (!nodeIdToObjectMap.TryGetValue(nodeId, out var model))
        {
            return new ContextGraphResult([], Enumerable.Empty<ContextGraphEdge>());
        }

        var nodes = new List<ContextGraphNode>();
        var edges = new List<ContextGraphEdge>();

        foreach (var property in GetAllProperties(model))
        {
            ProcessProperty(nodeId, property.Key, property.Value, nodes, edges, objectToNodeIdMap);
        }

        return new ContextGraphResult(nodes.DistinctBy(n => n.Id), edges);
    }

    private bool Matches(string id, object model, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return true;

        if (id.Contains(query, StringComparison.OrdinalIgnoreCase))
            return true;

        return GetAllProperties(model)
            .Any(p =>
                p.Value?.ToString()?.Contains(query, StringComparison.OrdinalIgnoreCase) == true
            );
    }

    private void ProcessProperty(
        string sourceId,
        string propertyName,
        object? value,
        List<ContextGraphNode> nodes,
        List<ContextGraphEdge> edges,
        Dictionary<object, string> objectToNodeIdMap
    )
    {
        if (value == null)
            return;

        if (objectToNodeIdMap.TryGetValue(value, out var targetId))
        {
            AddNeighbor(sourceId, propertyName, value, targetId, nodes, edges);
        }
        else if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var item in enumerable)
            {
                if (item != null && objectToNodeIdMap.TryGetValue(item, out var listTargetId))
                {
                    AddNeighbor(sourceId, propertyName, item, listTargetId, nodes, edges);
                }
            }
        }
    }

    private void AddNeighbor(
        string sourceId,
        string propertyName,
        object targetModel,
        string targetId,
        List<ContextGraphNode> nodes,
        List<ContextGraphEdge> edges
    )
    {
        edges.Add(new ContextGraphEdge(sourceId, targetId, propertyName));
        nodes.Add(MapToNode(targetId, targetModel));
    }

    private static Dictionary<object, string> BuildObjectToNodeIdMap(IProject project)
    {
        var map = new Dictionary<object, string>(ReferenceEqualityComparer.Instance);
        foreach (var entry in project.Context.Models)
        {
            var (modelName, pluginId) = entry.Key;
            foreach (var (id, model) in entry.Value)
            {
                map[model] = $"{modelName}_{pluginId}_{id}";
            }
        }

        return map;
    }

    private static Dictionary<string, object> BuildNodeIdToObjectMap(IProject project)
    {
        var map = new Dictionary<string, object>();
        foreach (var entry in project.Context.Models)
        {
            var (modelName, pluginId) = entry.Key;
            foreach (var (id, model) in entry.Value)
            {
                map[$"{modelName}_{pluginId}_{id}"] = model;
            }
        }

        return map;
    }

    private IEnumerable<KeyValuePair<string, object?>> GetAllProperties(object model)
    {
        var properties = new Dictionary<string, object?>();

        AddDynamicProperties(model, properties);
        AddStaticProperties(model, properties);

        return properties;
    }

    private static void AddDynamicProperties(object model, Dictionary<string, object?> properties)
    {
        if (model is IDictionary dynamicDict)
        {
            foreach (DictionaryEntry entry in dynamicDict)
            {
                if (entry.Key is string key)
                {
                    properties[key] = entry.Value;
                }
            }
        }
    }

    private static void AddStaticProperties(object model, Dictionary<string, object?> properties)
    {
        var type = model.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (IsSimpleProperty(prop) && !properties.ContainsKey(prop.Name))
            {
                AddPropertyValue(model, prop, properties);
            }
        }
    }

    private static bool IsSimpleProperty(PropertyInfo prop) =>
        prop.GetIndexParameters().Length == 0;

    private static void AddPropertyValue(
        object model,
        PropertyInfo prop,
        Dictionary<string, object?> properties
    )
    {
        try
        {
            properties[prop.Name] = prop.GetValue(model);
        }
        catch { }
    }

    private ContextGraphNode MapToNode(string id, object model)
    {
        var allProps = GetAllProperties(model).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var label = allProps.TryGetValue("Name", out var name) ? name?.ToString() : null;
        label ??= allProps.TryGetValue("Id", out var idProp) ? idProp?.ToString() : id;

        var type = id.Split('_')[0];
        var loader = id.Split('_').Length > 1 ? id.Split('_')[1] : null;

        var primitiveProperties = allProps
            .Where(kvp => IsPrimitive(kvp.Value))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value ?? "null");

        return new ContextGraphNode(id, label ?? id, type, loader, primitiveProperties);
    }

    private static bool IsPrimitive(object? value)
    {
        switch (value)
        {
            case null:
            case string:
                return true;
            default:
            {
                var type = value.GetType();
                return type.IsPrimitive || type.IsValueType || type == typeof(string);
            }
        }
    }
}
