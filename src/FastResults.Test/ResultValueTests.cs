using FarzanHajian.FastResults;

namespace FastResults.Test;

[TestClass]
public class ResultValueTests
{
    [TestMethod]
    public void CreateSuccess()
    {
        Result<int> iresult = new(20);
        TestSuccess(iresult, 20, -1);

        Result<float> fresult = Result<float>.Success(3.14f);
        TestSuccess(fresult, 3.14f, -1.0f);
    }

    [TestMethod]
    public void CreateFailure()
    {
        Result<string> sresult = new(new Exception("Invalid String Data"));
        TestFailure(sresult, "Invalid String Data", "ERROR");

        Result<float> fresult = new(new Error("Invalid Float Data"));
        TestFailure(fresult, "Invalid Float Data", -1.1f);

        Result<int> iresult = Result<int>.Failure(new Error("Invalid Operation"));
        TestFailure(iresult, "Invalid Operation", -1);
    }

    [TestMethod]
    public void DefaultConstructor()
    {
        Action act = () => { Result<string> result = new(); };
        act.Should().Throw<InvalidOperationException>().WithMessage("Using the default contructor is not valid.");
    }

    [TestMethod]
    public void Match()
    {
        string buffer = "";
        Result<string> result = new("Valid Data");
        result.Match(str => buffer = str, err => buffer = err.Message);
        buffer.Should().Be("Valid Data");
        result.Match();
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        result.Match(str => buffer = str, err => buffer = err.Message);
        buffer.Should().Be("ERROR");
        result.Match();
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchAsync()
    {
        string buffer = "";
        Result<string> result = new("Valid Data");
        await result.MatchAsync(async str => { buffer = str; await Task.CompletedTask; }, async err => { buffer = err.Message; await Task.CompletedTask; });
        buffer.Should().Be("Valid Data");
        await result.MatchAsync();
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        await result.MatchAsync(async str => { buffer = str; await Task.CompletedTask; }, async err => { buffer = err.Message; await Task.CompletedTask; });
        buffer.Should().Be("ERROR");
        await result.MatchAsync();
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void MatchValue()
    {
        string buffer = "";
        Result<string> result = new("Valid Data");
        buffer = result.Match(str => str, err => err.Message);
        buffer.Should().Be("Valid Data");
        result.Match();
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        buffer = result.Match(str => str, err => err.Message);
        buffer.Should().Be("ERROR");
        result.Match();
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchValueAsync()
    {
        string buffer = "";
        Result<string> result = new("Valid Data");
        buffer = await result.MatchAsync(async str => { await Task.CompletedTask; return str; }, async err => { await Task.CompletedTask; return err.Message; });
        buffer.Should().Be("Valid Data");
        await result.MatchAsync();
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        buffer = await result.MatchAsync(async str => { await Task.CompletedTask; return str; }, async err => { await Task.CompletedTask; return err.Message; });
        buffer.Should().Be("ERROR");
        await result.MatchAsync();
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void ImplicitCastFromValue()
    {
        Result<int> result = 7;
        TestSuccess(result, 7, -1);
    }

    [TestMethod]
    public void ImplicitCastToResult()
    {
        Result<int> src = new(7);
        Result dest = src;
        dest.IsSuccess.Should().BeTrue();
        dest.Invoking(r => r.Error).Should().Throw<InvalidOperationException>().WithMessage($"The current {nameof(Option<int>)} instance is empty.");

        src = new(new Error("Invalid Data"));
        dest = src;
        dest.IsFailure.Should().BeTrue();
        dest.Error.Message.Should().Be("Invalid Data");
    }

    private static void TestSuccess<T>(Result<T> result, T expectedValue, T defaultValue)
    {
        result.ValueOrDefault(defaultValue).Should().Be(expectedValue);
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Invoking(r => r.Error).Should().Throw<InvalidOperationException>().WithMessage($"The current {nameof(Option<Error>)} instance is empty.");
    }

    private static void TestFailure<T>(Result<T> result, string expectedError, T defaultValue)
    {
        result.Invoking(r => r.Value).Should().Throw<InvalidOperationException>().WithMessage($"The current {nameof(Option<T>)} instance is empty.");
        result.ValueOrDefault(defaultValue).Should().Be(defaultValue);
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedError);
    }
}