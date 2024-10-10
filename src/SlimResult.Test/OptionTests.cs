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
    public void StringRepresentatioTestn()
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