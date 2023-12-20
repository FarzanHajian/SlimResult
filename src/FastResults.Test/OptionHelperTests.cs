namespace FastResults.Test;

//using static LanguageExt.Prelude;

using static OptionHelper;

[TestClass]
public class OptionHelperTests
{
    [TestMethod]
    public void SomeHelper()
    {
        Option<int> option = Some(20);
        TestSome(option, 20);
    }

    [TestMethod]
    public void NoneHelper()
    {
        Option<int> option = None<int>();
        TestNone(option);
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