using Neomaster.Extensions.Exception;

namespace Tests;

public class GetAllInnerExceptionsTests
{
    #region Should return empty
    [Fact(DisplayName = "null -> []")]
    public void ShouldReturnEmpty_1()
    {
        var actual = (null as Exception).GetAllInnerExceptions();

        Assert.Empty(actual);
    }

    [Fact(DisplayName = "ae -> []")]
    public void ShouldReturnEmpty_2()
    {
        var actual = new AggregateException().GetAllInnerExceptions();

        Assert.Empty(actual);
    }

    [Fact(DisplayName = "ae1[ae2[ae3]] -> []")]
    public void ShouldReturnEmpty_3()
    {
        var tree =
            new AggregateException(
                new AggregateException(
                    new AggregateException()));

        var actual = tree.GetAllInnerExceptions();

        Assert.Empty(actual);
    }
    #endregion

    #region Should return single
    [Fact(DisplayName = "e -> [e]")]
    public void ShouldReturnSingle_1()
    {
        var expectedSingle = new Exception();

        var actual = expectedSingle.GetAllInnerExceptions();

        Assert.Single(actual);
        Assert.Equal(expectedSingle, actual.Single());
    }

    [Fact(DisplayName = "ae -> [ae]")]
    public void ShouldReturnSingle_2()
    {
        var expectedSingle = new AggregateException();

        var actual = expectedSingle.GetAllInnerExceptions(addAggregate: true);
        var actualSingle = actual.Single();

        Assert.Single(actual);
        Assert.IsType<AggregateException>(actualSingle);
        Assert.Equal(expectedSingle, actualSingle);
    }

    [Fact(DisplayName = "ae -> [e]")]
    public void ShouldReturnSingle_3()
    {
        var expectedSingleMessage = "error";

        var actual = new AggregateException(expectedSingleMessage).GetAllInnerExceptions(
            addAggregate: true,
            aggregateToSingle: true);
        var actualSingle = actual.Single();

        Assert.Single(actual);
        Assert.IsNotType<AggregateException>(actualSingle);
        Assert.Equal(nameof(Exception), actualSingle.GetType().Name);
        Assert.Equal(expectedSingleMessage, actualSingle.Message);
    }

    [Fact(DisplayName = "e[ae] -> [e]")]
    public void ShouldReturnSingle_4()
    {
        var actual = new Exception(null, new AggregateException()).GetAllInnerExceptions();

        Assert.Single(actual);
        Assert.Equal(nameof(Exception), actual.Single().GetType().Name);
    }

    [Fact(DisplayName = "ae[e] -> [e]")]
    public void ShouldReturnSingle_5()
    {
        var actual = new AggregateException(new Exception()).GetAllInnerExceptions();

        Assert.Single(actual);
        Assert.Equal(nameof(Exception), actual.Single().GetType().Name);
    }

    [Fact(DisplayName = "e[ae1[ae2]] -> [e]")]
    public void ShouldReturnSingle_6()
    {
        var tree =
            new Exception(null,
                new AggregateException(
                    new AggregateException()));

        var actual = tree.GetAllInnerExceptions();

        Assert.Single(actual);
        Assert.Equal(nameof(Exception), actual.Single().GetType().Name);
    }

    [Fact(DisplayName = "ae1[ae2[e]] -> [e]")]
    public void ShouldReturnSingle_7()
    {
        var tree =
            new AggregateException(
                new AggregateException(
                    new Exception()));

        var actual = tree.GetAllInnerExceptions();

        Assert.Single(actual);
        Assert.Equal(nameof(Exception), actual.Single().GetType().Name);
    }

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
    #endregion

    #region Should return only non-aggregate exceptions
    [Fact(DisplayName = "ae[e1,e2] -> [e1,e2]")]
    public void ShouldReturnOnlyNonAggregateExceptions_1()
    {
        var expected = new Exception[]
        {
            new Exception("1"),
            new Exception("2"),
        };
        var tree = new AggregateException(expected);

        var actual = tree.GetAllInnerExceptions();

        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
    }

    [Fact(DisplayName = "e1[ae[e2]] -> [e1,e2]")]
    public void ShouldReturnOnlyNonAggregateExceptions_2()
    {
        var e2 = new Exception("2");
        var ae = new AggregateException(e2);
        var e1 = new Exception("1", ae);
        var expected = new Exception[]
        {
            e1,
            e2,
        };

        var actual = e1.GetAllInnerExceptions();

        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
    }

    [Fact(DisplayName = "ae1[e1,ae2[e2]] -> [e1,e2]")]
    public void ShouldReturnOnlyNonAggregateExceptions_3()
    {
        var e1 = new Exception("1");
        var e2 = new Exception("2");
        var expected = new Exception[]
        {
            e1,
            e2,
        };
        var tree =
            new AggregateException(
                e1,
                new AggregateException(
                    e2));

        var actual = tree.GetAllInnerExceptions();

        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
    }

    [Fact(DisplayName = "ae1[e1,ae2,e2] -> [e1,e2]")]
    public void ShouldReturnOnlyNonAggregateExceptions_4()
    {
        var e1 = new Exception("1");
        var e2 = new Exception("2");
        var expected = new Exception[]
        {
            e1,
            e2,
        };
        var tree =
            new AggregateException(
                e1,
                new AggregateException(),
                e2);

        var actual = tree.GetAllInnerExceptions();

        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
    }

    [Fact(DisplayName = "ae1[ae2[e1,e2],ae3[e3,e4]] -> [e1,e2,e3,e4]")]
    public void ShouldReturnOnlyNonAggregateExceptions_5()
    {
        var e1 = new Exception("1");
        var e2 = new Exception("2");
        var e3 = new Exception("3");
        var e4 = new Exception("4");
        var expected = new Exception[]
        {
            e1,
            e2,
            e3,
            e4,
        };
        var tree =
            new AggregateException(
                new AggregateException(
                    e1,
                    e2),
                new AggregateException(
                    e3,
                    e4));

        var actual = tree.GetAllInnerExceptions();

        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
    }

    [Fact(DisplayName = "ae1[e1[e2],ae2[e3[e4]]] -> [e1,e2,e3,e4]")]
    public void ShouldReturnOnlyNonAggregateExceptions_6()
    {
        var e4 = new Exception("4");
        var e3 = new Exception("3", e4);
        var e2 = new Exception("2");
        var e1 = new Exception("1", e2);
        var expected = new Exception[]
        {
            e1,
            e2,
            e3,
            e4,
        };
        var tree =
            new AggregateException(
                e1,
                new AggregateException(
                    e3));

        var actual = tree.GetAllInnerExceptions();

        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
    }
    #endregion

    #region Should return all exceptions
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

    [Fact(DisplayName = "e[ae] -> [e,ae]")]
    public void ShouldReturnAllExceptions_2()
    {
        var ae = new AggregateException();
        var e = new Exception(null, ae);
        var expected = new Exception[]
        {
            e,
            ae,
        };

        var actual = e.GetAllInnerExceptions(addAggregate: true);

        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
    }

    [Fact(DisplayName = "ae1[ae2[ae3]] -> [ae1,ae2,ae3]")]
    public void ShouldReturnAllExceptions_3()
    {
        var ae3 = new AggregateException();
        var ae2 = new AggregateException(ae3);
        var ae1 = new AggregateException(ae2);
        var expected = new Exception[]
        {
            ae1,
            ae2,
            ae3,
        };

        var actual = ae1.GetAllInnerExceptions(addAggregate: true);

        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Select(e => e.Message), actual.Select(e => e.Message));
    }

    [Fact(DisplayName = "ae1[ae2[ae3]] -> [e1,e2,e3]")]
    public void ShouldReturnAllExceptions_4()
    {
        var ae3 = new AggregateException();
        var ae2 = new AggregateException(ae3);
        var ae1 = new AggregateException(ae2);
        var expectedMessages = new string[]
        {
            ae1.Message,
            ae2.Message,
            ae3.Message,
        };

        var actual = ae1.GetAllInnerExceptions(addAggregate: true, aggregateToSingle: true);

        Assert.Equal(expectedMessages.Length, actual.Count);
        Assert.All(actual, (e) => Assert.Equal(nameof(Exception), e.GetType().Name));
        Assert.Equal(expectedMessages, actual.Select(e => e.Message));
    }
    #endregion
}