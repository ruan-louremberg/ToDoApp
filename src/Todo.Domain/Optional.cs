namespace ToDoApp.Domain;

public readonly struct Optional<T>
{
    public bool IsSet { get; }

    public T? Value { get; }

    private Optional(bool isSet, T? value)
    {
        IsSet = isSet;
        Value = value;
    }

    public static Optional<T> Unset() => new(false, default);

    public static Optional<T> Of(T? value) => new(true, value);
}
