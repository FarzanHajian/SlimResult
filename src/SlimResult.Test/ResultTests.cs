using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlimResult.Test;

[TestClass]
public class ResultTests
{
    [TestMethod]
    public void CreateSuccessTest()
    {
        Result result = new();
        TestSuccess(result);

        result = Success();
        TestSuccess(result);
    }

    [TestMethod]
    public void CreateFailureTest()
    {
        Result result = new(new Exception("Invalid Data"));
        TestFailure(result, "Invalid Data");

        result = new(new Error("Invalid Value"));
        TestFailure(result, "Invalid Value");

        result = Failure(new Error("Invalid Operation"));
        TestFailure(result, "Invalid Operation");

        result = Failure(new ApplicationException("Invalid Operation"));
        TestFailure(result, "Invalid Operation");
    }

    [TestMethod]
    public void MatchTest()
    {
        string buffer = "";
        Result result = new();
        result.Match(() => { buffer = "Valid Data"; }, err => buffer = err.Message);
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        result.Match(() => { buffer = "Valid Data"; }, err => buffer = err.Message);
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchAsyncTest()
    {
        string buffer = "";
        Result result = new();
        await result.Match(async () => { buffer = "Valid Data"; await Task.CompletedTask; }, async err => { buffer = err.Message; await Task.CompletedTask; });
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        await result.Match(async () => { buffer = "Valid Data"; await Task.CompletedTask; }, async err => { buffer = err.Message; await Task.CompletedTask; });
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void MatchReturnTest()
    {
        string buffer = "";
        Result result = new();
        buffer = result.Match(() => "Valid Data", err => err.Message);
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        buffer = result.Match(() => "Valid Data", err => err.Message);
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchReturnAsyncTest()
    {
        string buffer = "";
        Result result = new();
        buffer = await result.Match(async () => { await Task.CompletedTask; return "Valid Data"; }, async err => { await Task.CompletedTask; return err.Message; });
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        buffer = await result.Match(async () => { await Task.CompletedTask; return "Valid Data"; }, async err => { await Task.CompletedTask; return err.Message; });
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void TryTest()
    {
        Action act = () => { };
        Action actFailure = () => { throw new Exception("Bad Data"); };

        Result result = Try(act);
        result.IsSuccess.Should().BeTrue();

        result = Try(act, err => new Result($"Error: {err.Message}"));
        result.IsSuccess.Should().BeTrue();

        result = Try(actFailure);
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Bad Data");

        result = Try(actFailure, err => new Result($"Error: {err.Message}"));
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Error: Bad Data");
    }

    [TestMethod]
    public async Task TryAsyncTest()
    {
        Func<Task> act = async () => { await Task.CompletedTask; };
        Func<Task> actFailure = async () => { await Task.CompletedTask; throw new Exception("Bad Data"); };

        Result result = await Try(act);
        result.IsSuccess.Should().BeTrue();

        result = await Try(act, err => new Result($"Error: {err.Message}"));
        result.IsSuccess.Should().BeTrue();

        result = await Try(actFailure);
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Bad Data");

        result = await Try(actFailure, err => new Result($"Error: {err.Message}"));
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Error: Bad Data");
    }

    [TestMethod]
    public void IfSuccessTest()
    {
        Result success = new();
        Result result = success.IfSuccess(() => new Result("Invalid Operation"));
        TestFailure(result, "Invalid Operation");

        Result failure = new(new Error("Failure"));
        result = failure.IfSuccess(() => new Result());
        TestFailure(result, "Failure");
    }

    [TestMethod]
    public async Task IfSuccessAsyncTest()
    {
        Result success = new();
        Result result = await success.IfSuccess(async () => { await Task.CompletedTask; return new Result("Invalid Operation"); });
        TestFailure(result, "Invalid Operation");

        Result failure = new(new Error("Failure"));
        result = await failure.IfSuccess(async () => { await Task.CompletedTask; return new Result(); });
        TestFailure(result, "Failure");
    }

    [TestMethod]
    public void IfFailureTest()
    {
        var fail = (Error err) => new Result("Failure, " + err.Message);

        Result success = new();
        Result result = success.IfFailure(fail);
        TestSuccess(result);

        Result failure = new(new Error("Invalid Operation"));
        result = failure.IfFailure(fail);
        TestFailure(result, "Failure, Invalid Operation");
    }

    [TestMethod]
    public async Task IfFailureAsyncTest()
    {
        var fail = async (Error err) => { await Task.CompletedTask; return new Result("Failure, " + err.Message); };

        Result success = new();
        Result result = await success.IfFailure(fail);
        TestSuccess(result);

        Result failure = new(new Error("Invalid Operation"));
        result = await failure.IfFailure(fail);
        TestFailure(result, "Failure, Invalid Operation");
    }

    private static void TestSuccess(Result result)
    {
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Invoking(r => r.Error).Should().Throw<InvalidOperationException>().WithMessage($"The current {nameof(Option<Error>)} instance is empty.");
    }

    private static void TestFailure(Result result, string expectedError)
    {
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedError);
    }
}