namespace SlimResult.Test;

[TestClass]
public class OptionTests
{
    [TestMethod]
    public void SomeTest()
    {
        Option<int> option = Some(20);
        TestSome(option, 20, -99);

        option = new Option<int>(1361);
        TestSome(option, 1361, -99);
    }

    [TestMethod]
    public void NoneTest()
    {
        Option<int> option = None<int>();
        TestNone(option, -99);

        option = new Option<int>();
        TestNone(option, -99);
    }

    [TestMethod]
    public void NullTest()
    {
        Action act = () => { Option<string> option = new(null!); };
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'value')");

        act = () => { Option<string> option = Some<string>(null!); };
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'value')");
    }

    [TestMethod]
    public void SetUnsetTest()
    {
        Option<float> option = new(3.14f);
        option.Set(2.71f);
        TestSome(option, 2.71f, -99);

        option.Unset();
        TestNone(option, -99);

        Action act = () => { var option = Some("Hello"); option.Set(null!); };
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void MatchTest()
    {
        string buffer = "";
        Option<string> option = new("Valid data");
        option.Match(str => { buffer = str; }, () => buffer = "ERROR");
        buffer.Should().Be("Valid data");

        option = new();
        option.Match(str => { buffer = str; }, () => buffer = "ERROR");
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchAsyncTest()
    {
        string buffer = "";
        Option<string> option = new("Valid data");
        await option.Match(async str => { buffer = str; await Task.CompletedTask; }, async () => { buffer = "ERROR"; await Task.CompletedTask; });
        buffer.Should().Be("Valid data");

        option = new();
        await option.Match(async str => { buffer = str; await Task.CompletedTask; }, async () => { buffer = "ERROR"; await Task.CompletedTask; });
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void MatchWithReturnTest()
    {
        string buffer = "";
        Option<string> option = new("Valid data");
        buffer = option.Match(str => str, () => "ERROR");
        buffer.Should().Be("Valid data");

        option = new();
        buffer = option.Match(str => str, () => "ERROR");
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchWithReturnAsyncTest()
    {
        string buffer = "";
        Option<string> option = new("Valid data");
        buffer = await option.Match(async str => { await Task.CompletedTask; return str; }, async () => { await Task.CompletedTask; return "ERROR"; });
        buffer.Should().Be("Valid data");

        option = new();
        buffer = await option.Match(async str => { await Task.CompletedTask; return str; }, async () => { await Task.CompletedTask; return "ERROR"; });
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void StringRepresentatioTest()
    {
        Option<float> opt = 123.45f;
        opt.ToString().Should().Be("123.45");

        opt.Unset();
        opt.ToString().Should().Be("");
    }

    [TestMethod]
    public void ImplicitCastsTest()
    {
        Option<int> iOption = 7;
        TestSome(iOption, 7, -99);

        Option<float> fOption = new(3.14f); ;
        float fValue = fOption;
        fValue.Should().Be(3.14f);

        Option<string> empty = new();
        var action = () => { string sValue = empty; };
        action.Should().Throw<InvalidOperationException>().WithMessage($"The current {nameof(Option<string>)} instance is empty.");
    }

    [TestMethod]
    public void EqualityGenericTest()
    {
        Option<float> f1 = 12.45f;
        Option<float> f2 = 12.45f;
        Option<float> f3 = 12f;
        Option<float> f4 = None<float>();

        f1.Equals(f2).Should().Be(true);
        f2.Equals(f1).Should().Be(true);

        f1.Equals(f3).Should().Be(false);
        f3.Equals(f1).Should().Be(false);

        f1.Equals(f4).Should().Be(false);
        f4.Equals(f1).Should().Be(false);

        f4.Equals(None<float>()).Should().Be(true);
    }

    [TestMethod]
    public void EqualityTest()
    {
        Option<float> f1 = 12.45f;
        object f2 = Some(12.45f);
        object f3 = Some(12f);
        Option<float> f4 = None<float>();

        f1.Equals(f2).Should().Be(true);
        f1.Equals(f3).Should().Be(false);
        f1.Equals(null).Should().Be(false);
        f1.Equals("").Should().Be(false);
        f1.Equals(Some("")).Should().Be(false);
        f4.Equals(null).Should().Be(false);
    }

    [TestMethod]
    public void CompareToGenereicTest()
    {
        Option<float> f1 = 45.12f;
        Option<float> f2 = 45.12f;
        Option<float> f3 = 45f;
        Option<float> f4 = None<float>();
        Option<float> f5 = None<float>();

        f1.CompareTo(f2).Should().Be(0);
        f1.CompareTo(f3).Should().Be(1);
        f3.CompareTo(f1).Should().Be(-1);

        f1.CompareTo(f4).Should().Be(1);
        f4.CompareTo(f1).Should().Be(-1);

        f4.CompareTo(f5).Should().Be(0);

        f1.CompareTo(f1).Should().Be(0);
        f5.CompareTo(f5).Should().Be(0);

        Some(new ComparableOnly()).CompareTo(new ComparableOnly()).Should().Be(0);

        Action act = () => Some(new { I1 = 12, I2 = 14 }).CompareTo(new { I1 = 12, I2 = 14 });
        act.Should().Throw<ArgumentException>().WithMessage("TValue must impelement either IComparable or IComparable<T>.");
    }

    [TestMethod]
    public void CompareTest()
    {
        Option<float> f1 = 45.12f;
        object f2 = Some(45.12f);
        object f3 = Some(45f);

        f1.CompareTo(f2).Should().Be(0);
        f1.CompareTo(f3).Should().Be(1);

        Action act = () => f1.CompareTo(null);
        act.Should().Throw<ArgumentException>().WithMessage("obj is not the same type as this instance.");

        act = () => f1.CompareTo("My message");
        act.Should().Throw<ArgumentException>().WithMessage("obj is not the same type as this instance.");

        act = () => f1.CompareTo(Some("My message #2"));
        act.Should().Throw<ArgumentException>().WithMessage("obj is not the same type as this instance.");
    }

    [TestMethod]
    public void GetHashCodeTest()
    {
        double v = 12.45;
        Some(v).GetHashCode().Should().Be(v.GetHashCode());

        None<decimal>().GetHashCode().Should().Be(0);
    }

    [TestMethod]
    public void EqalityOperatorTest()
    {
        Option<float> f1 = 12.45f;
        Option<float> f2 = 12.45f;
        Option<float> f3 = 12.0f;
        Option<float> f4 = None<float>();
        Option<float> f5 = None<float>();

        (f1 == f2).Should().Be(true);
        (f2 == f1).Should().Be(true);
        (f1 == f3).Should().Be(false);
        (f3 == f1).Should().Be(false);
        (f1 == f4).Should().Be(false);
        (f4 == f1).Should().Be(false);
        (f4 == f5).Should().Be(true);

        (f1 != f2).Should().Be(false);
        (f2 != f1).Should().Be(false);
        (f1 != f3).Should().Be(true);
        (f3 != f1).Should().Be(true);
        (f1 != f4).Should().Be(true);
        (f4 != f1).Should().Be(true);
        (f4 != f5).Should().Be(false);
    }

    [TestMethod]
    public void CompareOperatorTest()
    {
        Option<float> f1 = 12.45f;
        Option<float> f2 = 12.45f;
        Option<float> f3 = 12.0f;
        Option<float> f4 = None<float>();
        Option<float> f5 = None<float>();

        (f1 < f2).Should().Be(false);
        (f1 < f3).Should().Be(false);
        (f3 < f1).Should().Be(true);
        (f1 < f4).Should().Be(false);
        (f4 < f3).Should().Be(false);
        (f4 < f5).Should().Be(false);

        (f1 <= f2).Should().Be(true);
        (f1 <= f3).Should().Be(false);
        (f3 <= f1).Should().Be(true);
        (f1 <= f4).Should().Be(false);
        (f4 <= f3).Should().Be(false);
        (f4 <= f5).Should().Be(true);

        (f1 > f2).Should().Be(false);
        (f1 > f3).Should().Be(true);
        (f3 > f1).Should().Be(false);
        (f1 > f4).Should().Be(false);
        (f4 > f3).Should().Be(false);
        (f4 > f5).Should().Be(false);

        (f1 >= f2).Should().Be(true);
        (f1 >= f3).Should().Be(true);
        (f3 >= f1).Should().Be(false);
        (f1 >= f4).Should().Be(false);
        (f4 >= f3).Should().Be(false);
        (f4 >= f5).Should().Be(true);
    }

    private static void TestSome<T>(Option<T> option, T expectedValue, T defaultValue)
    {
        option.IsSome.Should().BeTrue();
        option.IsNone.Should().BeFalse();
        option.Value.Should().Be(expectedValue);
        option.ValueOrDefault(defaultValue).Should().Be(expectedValue);
    }

    private static void TestNone<T>(Option<T> option, T defaultValue)
    {
        option.IsNone.Should().BeTrue();
        option.IsSome.Should().BeFalse();
        option.Invoking(o => o.Value).Should().Throw<InvalidOperationException>().WithMessage($"The current {nameof(Option<T>)} instance is empty.");
        option.ValueOrDefault(defaultValue).Should().Be(defaultValue);
    }

    private class ComparableOnly : IComparable
    {
        public int CompareTo(object? obj) => 0;
    }
}