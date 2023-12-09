namespace FastResults.Test;

[TestClass]
public class OptionTests
{
    [TestMethod]
    public void Some()
    {
        Option<int> option = Option<int>.Some(20);
        TestSome(option, 20);

        option = new Option<int>(1361);
        TestSome(option, 1361);
    }

    [TestMethod]
    public void None()
    {
        Option<int> option = Option<int>.None();
        TestNone(option);

        option = new Option<int>();
        TestNone(option);
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
        TestSome(option, 2.71f);

        option.Unset();
        TestNone(option);
    }

    [TestMethod]
    public void Match()
    {
        string buffer = "";
        Option<string> option = new("Valid data");
        option.Match(str => buffer = str, () => buffer = "ERROR");
        buffer.Should().Be("Valid data");

        option = new();
        option.Match(str => buffer = str, () => buffer = "ERROR");
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
    public void MatchReturn()
    {
        string buffer = "";
        Option<string> option = new("Valid data");
        buffer = option.MatchReturn(str => str, () => "ERROR");
        buffer.Should().Be("Valid data");

        option = new();
        buffer = option.MatchReturn(str => str, () => "ERROR");
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchReturnAsync()
    {
        string buffer = "";
        Option<string> option = new("Valid data");
        buffer = await option.MatchReturn(async str => { await Task.CompletedTask; return str; }, async () => { await Task.CompletedTask; return "ERROR"; });
        buffer.Should().Be("Valid data");

        option = new();
        buffer = await option.MatchReturn(async str => { await Task.CompletedTask; return str; }, async () => { await Task.CompletedTask; return "ERROR"; });
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void ImplicitCast()
    {
        Option<int> option = 7;
        TestSome(option, 7);
    }

    private static void TestSome<T>(Option<T> option, T expectedValue)
    {
        option.IsSome.Should().BeTrue();
        option.IsNone.Should().BeFalse();
        option.Value.Should().Be(expectedValue);
    }

    private static void TestNone<T>(Option<T> option)
    {
        option.IsNone.Should().BeTrue();
        option.IsSome.Should().BeFalse();
        option.Invoking(o => o.Value).Should().Throw<InvalidOperationException>().WithMessage($"The current {nameof(Option<T>)} instance is empty.");
    }
}