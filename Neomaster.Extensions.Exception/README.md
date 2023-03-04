# Neomaster.Extensions.Exception
Retrieves all internal exceptions of the exception tree, including `AggregateException`.

![.NET Standard 2.1](https://img.shields.io/badge/.NET_Standard-v2.1-informational)

---

## Tests

### [GetAllInnerExceptions() Tests](https://github.com/neomasterhub/Neomaster.Nugets/blob/master/Tests/GetAllInnerExceptionsTests.cs)

### Conventions
* `e` - `Exception` instance
* `ae` - `AggregateException` instance
* `[e,ae]` - list

### Examples
#### Don't include `AggregateException` *(default)*
```c#
[Fact(DisplayName = "ae1[e[ae2]] -> [e]")]
public void ShouldReturnSingle_8()
{
    var tree =
        new AggregateException(
            new Exception(null,
                new AggregateException()));

    var actual = tree.GetAllInnerExceptions();

    Assert.Single(actual);
    Assert.Equal(nameof(Exception), actual.Single().GetType().Name);
}
```
#### Include `AggregateException`
```c#
[Fact(DisplayName = "ae[e] -> [ae,e]")]
public void ShouldReturnAllExceptions_1()
{
    var e = new Exception();
    var ae = new AggregateException(e);
    var expected = new Exception[]
    {
        ae,
        e,
    };

    var actual = ae.GetAllInnerExceptions(addAggregate: true);

    Assert.Equal(expected.Length, actual.Count);
    Assert.Equal(expected, actual);
    Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
}
```
