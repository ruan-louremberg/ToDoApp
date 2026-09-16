using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToDoApp.Application.Common;

// 1. A Struct wrapper
public readonly struct Option<T>
{
    public bool IsDefined { get; }
    public T? Value { get; }

    public Option(T? value)
    {
        IsDefined = true;
        Value = value;
    }

    public static Option<T> Undefined => default;
}

// 2. O Converter do System.Text.Json
public class OptionJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionJsonConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class OptionJsonConverter<T> : JsonConverter<Option<T>>
{
    public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return new Option<T>(default);
        }

        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return new Option<T>(value);
    }

    public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options)
    {
        if (value.IsDefined)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}