using System.Text.Json;

namespace RangeFinder.Domain.History;

public class TextCriterionHistory
{
    public readonly string? _fileName;

    private DateTime? _lastRefreshTimestamp;

    public IReadOnlyCollection<string> Items => _items;

    private readonly List<string> _items = [];

    public int MaxItems { get; init; }

    public TextCriterionHistory(int maxItems, string? fileName)
    {
        MaxItems = maxItems;
        _fileName = fileName;
    }

    public void Initialize()
    {
        if (HasBackingFile())
        {
            EnsureRefreshed();
        }
    }

    public void Clear()
    {
        _items.Clear();
    }

    private bool HasBackingFile() => _fileName != null && File.Exists(_fileName);

    private void EnsureRefreshed()
    {
        if (_fileName == null)
            return;

        var lastSaved = new FileInfo(_fileName).LastWriteTimeUtc;

        if (_lastRefreshTimestamp == null
            || _lastRefreshTimestamp < lastSaved)
        {
            var json = File.ReadAllText(_fileName);
            FromJson(json);
            _lastRefreshTimestamp = lastSaved;
        }
    }

    public void AddItem(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (HasBackingFile())
        {
            EnsureRefreshed();
        }

        _items.Remove(text);
        _items.Insert(0, text);
        if (_items.Count > MaxItems)
        {
            _items.RemoveAt(Items.Count - 1);
        }

        if (_fileName == null)
        {
            return;
        }

        var json = ToJson();
        File.WriteAllText(_fileName, json);
        _lastRefreshTimestamp = DateTime.UtcNow;
    }

    public virtual string[] GetPossibleContinuations(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return [];
        }
        if (HasBackingFile())
        {
            EnsureRefreshed();
        }
        var bestMatches = _items
            .Where(item => item.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var otherMatches = _items
            .Where(item => item.Contains(userInput, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return [.. bestMatches, .. otherMatches.Except(bestMatches, StringComparer.OrdinalIgnoreCase)];
    }

    public string ToJson(JsonSerializerOptions? options = null)
    {
        var model = new Dto
        {
            Items = [.. _items]
        };

        return JsonSerializer.Serialize(model, options);
    }

    private void FromJson(string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON content cannot be null or empty.", nameof(json));
        }

        var model = JsonSerializer.Deserialize<Dto>(json, options)
            ?? throw new InvalidOperationException("Unable to deserialize TextCriterionHistory.");

        if (model.Items is not null)
        {
            this._items.Clear();
            this._items.AddRange(model.Items);
        }
    }

    private sealed class Dto
    {
        public List<string>? Items { get; set; }
    }
}