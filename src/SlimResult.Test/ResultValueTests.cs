﻿using FarzanHajian.SlimResult;
using System.Runtime.CompilerServices;

namespace SlimResult.Test;

[TestClass]
public class ResultValueTests
{
    [TestMethod]
    public void CreateSuccessTest()
    {
        Result<int> iresult = new(20);
        TestSuccess(iresult, 20, -1);

        Result<float> fresult = Success(3.14f);
        TestSuccess(fresult, 3.14f, -1.0f);
    }

    [TestMethod]
    public void CreateFailureTest()
    {
        Result<string> sresult = new(new Exception("Invalid String Data"));
        TestFailure(sresult, "Invalid String Data", "ERROR");

        Result<float> fresult = new(new Error("Invalid Float Data"));
        TestFailure(fresult, "Invalid Float Data", -1.1f);

        Result<int> iresult = Failure<int>(new Error("Invalid Operation"));
        TestFailure(iresult, "Invalid Operation", -1);

        Result<decimal> dresult = Failure<decimal>(new ApplicationException("Invalid Operation"));
        TestFailure(dresult, "Invalid Operation", -1);
    }

    [TestMethod]
    public void DefaultConstructorTest()
    {
        Action act = () => { Result<string> result = new(); };
        act.Should().Throw<InvalidOperationException>().WithMessage("Using the default contructor is not valid.");
    }

    [TestMethod]
    public void MatchTest()
    {
        string buffer = "";
        Result<string> result = new("Valid Data");
        result.Match(str => { buffer = str; }, err => buffer = err.Message);
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        result.Match(str => { buffer = str; }, err => buffer = err.Message);
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchAsyncTest()
    {
        string buffer = "";
        Result<string> result = new("Valid Data");
        await result.Match(async str => { buffer = str; await Task.CompletedTask; }, async err => { buffer = err.Message; await Task.CompletedTask; });
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        await result.Match(async str => { buffer = str; await Task.CompletedTask; }, async err => { buffer = err.Message; await Task.CompletedTask; });
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void MatchValueTest()
    {
        string buffer = "";
        Result<string> result = new("Valid Data");
        buffer = result.Match(str => str, err => err.Message);
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        buffer = result.Match(str => str, err => err.Message);
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public async Task MatchValueAsyncTest()
    {
        string buffer = "";
        Result<string> result = new("Valid Data");
        buffer = await result.Match(async str => { await Task.CompletedTask; return str; }, async err => { await Task.CompletedTask; return err.Message; });
        buffer.Should().Be("Valid Data");

        result = new(new Error("ERROR"));
        buffer = await result.Match(async str => { await Task.CompletedTask; return str; }, async err => { await Task.CompletedTask; return err.Message; });
        buffer.Should().Be("ERROR");
    }

    [TestMethod]
    public void IfSuccessTest()
    {
        var succ = (string val) => new Result<string>(val + " Operation Result");

        Result<string> success = new("Success");
        Result<string> result = success.IfSuccess(succ);
        TestSuccess(result, "Success Operation Result", "");

        Result<string> failure = new(new Error("Failure"));
        result = failure.IfSuccess(succ);
        TestFailure(result, "Failure", "");
    }

    [TestMethod]
    public async Task IfSuccessAsyncTest()
    {
        var succ = async (string val) => { await Task.CompletedTask; return new Result<string>(val + " Operation Result"); };

        Result<string> success = new("Success");
        Result<string> result = await success.IfSuccess(succ);
        TestSuccess(result, "Success Operation Result", "");

        Result<string> failure = new(new Error("Failure"));
        result = await failure.IfSuccess(succ);
        TestFailure(result, "Failure", "");
    }

    [TestMethod]
    public void IfFailureTest()
    {
        var fail = (Error err) => new Result<string>(new Error(err.Message + " Operation Result"));

        Result<string> success = new("Success");
        Result<string> result = success.IfFailure(fail);
        TestSuccess(result, "Success", "");

        Result<string> failure = new(new Error("Failure"));
        result = failure.IfFailure(fail);
        TestFailure(result, "Failure Operation Result", "");
    }

    [TestMethod]
    public async Task IfFailureAsyncTest()
    {
        var fail = async (Error err) => { await Task.CompletedTask; return new Result<string>(new Error(err.Message + " Operation Result")); };

        Result<string> success = new("Success");
        Result<string> result = await success.IfFailure(fail);
        TestSuccess(result, "Success", "");

        Result<string> failure = new(new Error("Failure"));
        result = await failure.IfFailure(fail);
        TestFailure(result, "Failure Operation Result", "");
    }

    [TestMethod]
    public void ImplicitCastFromValueTest()
    {
        Result<int> result = 7;
        TestSuccess(result, 7, -1);
    }

    [TestMethod]
    public void ImplicitCastToResultTest()
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
        result.Value.Should().Be(expectedValue);
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