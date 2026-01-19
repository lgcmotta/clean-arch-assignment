using System.Diagnostics.CodeAnalysis;

namespace OrderManagement.Domain.Core;

public readonly struct Money : IEquatable<Money>
{
    private const int Scale = 2;

    private readonly decimal _value;

    public Money(decimal value)
    {
        _value = value;
    }

    public Money() : this(decimal.Zero) { }

    public static Money Zero => new();

    public decimal Value => TruncateValue(_value);

    public long ToCents() => decimal.ToInt64(Value * 100m);

    public static Money FromCents(long cents) => new(cents * 100m);

    public static implicit operator decimal(Money money) => money._value;

    public static implicit operator Money(decimal value) => new(value);

    public static Money operator +(Money left, Money right) => left._value + right._value;

    public static Money operator -(Money left, Money right) => left._value - right._value;

    public static Money operator *(Money left, Money right) => left._value * right._value;

    public static Money operator /(Money left, Money right) => left._value / right._value;

    public static bool operator !=(Money left, Money right) => !(left == right);

    public static bool operator ==(Money left, Money right)
    {
        var compare = decimal.Compare(left._value, right.Value);

        return compare is 0;
    }

    public static bool operator <(Money left, Money right)
    {
        return decimal.Compare(left._value, right._value) switch
        {
            < 0 => true,
            _ => false,
        };
    }

    public static bool operator >(Money left, Money right)
    {
        return decimal.Compare(left._value, right._value) switch
        {
            > 0 => true,
            _ => false,
        };
    }

    public static bool operator <=(Money left, Money right)
    {
        return decimal.Compare(left._value, right._value) switch
        {
            <= 0 => true,
            _ => false,
        };
    }

    public static bool operator >=(Money left, Money right)
    {
        return decimal.Compare(left._value, right._value) switch
        {
            >= 0 => true,
            _ => false,
        };
    }

    public bool Equals(Money other) => _value == other._value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Money money && Equals(money);

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => $"{Value:2F}";

    private decimal TruncateValue(decimal value)
    {
        var rounded = decimal.Round(value, Scale);

        return _value switch
        {
            > 0 when rounded > value => rounded - new decimal(lo: 1, mid: 0, hi: 0, isNegative: false, scale: Scale),
            < 0 when rounded < value => rounded + new decimal(lo: 1, mid: 0, hi: 0, isNegative: false, scale: Scale),
            _ => rounded
        };
    }
}