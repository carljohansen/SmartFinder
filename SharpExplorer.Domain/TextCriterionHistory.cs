using System.Text.Json;

namespace SharpExplorer.Domain;

public class TextCriterionHistory
{
    public IReadOnlyCollection<string> Items => _items;

    private readonly List<string> _items = [];

    public int MaxItems { get; init; }

    public TextCriterionHistory(int maxItems)
    {
        MaxItems = maxItems;
    }

    public void AddItem(string text)
    {
        _items.Remove(text);
        _items.Insert(0, text);
        if (_items.Count > MaxItems)
        {
            _items.RemoveAt(Items.Count - 1);
        }
    }

    public string ToJson(JsonSerializerOptions? options = null)
    {
        var model = new Dto
        {
            MaxItems = MaxItems,
            Items = [.. _items]
        };

        return JsonSerializer.Serialize(model, options);
    }

    public static TextCriterionHistory FromJson(string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON content cannot be null or empty.", nameof(json));
        }

        var model = JsonSerializer.Deserialize<Dto>(json, options)
            ?? throw new InvalidOperationException("Unable to deserialize TextCriterionHistory.");

        var history = new TextCriterionHistory(Math.Max(1, model.MaxItems));

        if (model.Items is not null)
        {
            foreach (var item in model.Items)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    history.AddItem(item);
                }
            }
        }

        return history;
    }

    private sealed class Dto
    {
        public int MaxItems { get; set; }
        public List<string>? Items { get; set; }
    }
}