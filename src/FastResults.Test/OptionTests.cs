namespace FastResults.Test;

[TestClass]
public class OptionTests
{
    [TestMethod]
    public void Some()
    {
        Option<int> option = Option<int>.Some(20);
        TestSome(option, 20, -99);

        option = new Option<int>(1361);
        TestSome(option, 1361, -99);
    }

    [TestMethod]
    public void None()
    {
        Option<int> option = Option<int>.None();
        TestNone(option, -99);

        option = new Option<int>();
        TestNone(option, -99);
    }

    [TestMethod]
    public void Null()
    {
        Action act = () => { Option<string> option = new(null!); };
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'value')");

        act = () => { Option<string> option = Option<string>.Some(null!); };
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'value')");
    }

    [TestMethod]
    public void SetUnset()
    {
        Option<float> option = new Option<float>(3.14f);
        option.Set(2.71f);
        TestSome(option, 2.71f, -99);

        option.Unset();
        TestNone(option, -99);

        Action act = () => { var option = Option<string>.Some("Hello"); option.Set(null!); };
        act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Match()
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
    public async Task MatchAsync()
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
    public void MatchWithReturn()
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
    public async Task MatchWithReturnAsync()
    {
        string buffer = "";
        Option<string> option = new("Valid data");
        buffer = await option.Match(async str => { await Task.CompletedTask; return str; }, async () => { await Task.CompletedTask; return "ERROR"; });
        buffer.Should().Be("Valid data");

        option = new();
        buffer = await option.Match(async str => { await Task.CompletedTask; return str; }, async () => { await Task.CompletedTask; return "ERROR"; });
        buffer.Should().Be("ERROR");
    }

    //[TestMethod]
    //public void Equality()
    //{
    //    Option<float> opt1 = 2;
    //    Option<float> opt2 = 3.14f;
    //    Option<float> opt3 = 3.14f;
    //    Option<int> opt4 = 2;
    //    Option<int> opt5 = Option<int>.None();

    //    opt1.Equals(opt2).Should().BeFalse();
    //    opt1.Equals((object)opt2).Should().BeFalse();
    //    (opt1 == opt2).Should().BeFalse();
    //    (opt1 != opt2).Should().BeTrue();

    //    opt3.Equals(opt2).Should().BeTrue();
    //    opt3.Equals((object)opt2).Should().BeTrue();
    //    (opt3 == opt2).Should().BeTrue();
    //    (opt3 != opt2).Should().BeFalse();

    //    opt1.Equals(opt4).Should().BeTrue();
    //    (opt1 == opt4).Should().BeFalse();
    //    (opt1 != opt4).Should().BeTrue();

    //    var r = 4.Equals(opt4);
    //    r = opt4.Equals(4);
    //    r = opt4 == 4;
    //    r = 4 == opt4;
    //    r = opt4 != 4;
    //    r = 4 != opt4;

    //    r = opt3.Equals(opt4);
    //    r = opt3 == opt4;
    //    r = opt3 != opt4;

    //    opt1.Equals(null).Should().BeFalse();

    //    opt4.Equals(opt5).Should().BeFalse();
    //    opt4.Equals((object)opt5).Should().BeFalse();
    //    (opt4 == opt5).Should().BeFalse();
    //    (opt4 != opt5).Should().BeTrue();
    //}

    //[TestMethod]
    //public void HashCode()
    //{
    //    Option<float> opt = 3.14f;
    //    opt.GetHashCode().Should().Be(3.14f.GetHashCode());

    //    opt.Unset();
    //    opt.GetHashCode().Should().Be(0);
    //}

    [TestMethod]
    public void StringRepresentation()
    {
        Option<float> opt = 123.45f;
        opt.ToString().Should().Be("123.45");

        opt.Unset();
        opt.ToString().Should().Be("");
    }

    [TestMethod]
    public void ImplicitCasts()
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
}