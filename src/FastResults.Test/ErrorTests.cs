namespace FastResults.Test
{
    [TestClass]
    public class ErrorTests
    {
        [TestMethod]
        public void CreateByMessage()
        {
            Error error = new("Bad Data");
            error.Message.Should().Be("Bad Data");
            error.Exception.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public void CreateByException()
        {
            Exception exception = new("Failed Operation");
            Error error = new(exception);
            error.Message.Should().Be("Failed Operation");
            error.Exception.Value.Should().BeOfType<Exception>();
            error.Exception.Value.Message.Should().Be("Failed Operation");
        }

        [TestMethod]
        public void ImplicitCasts()
        {
            Error error = "Bad Data";
            error.Message.Should().Be("Bad Data");
            error.Exception.IsNone.Should().BeTrue();

            Exception exception = new("Failed Operation");
            error = exception;
            error.Message.Should().Be("Failed Operation");
            error.Exception.Value.Should().BeOfType<Exception>();
            error.Exception.Value.Message.Should().Be("Failed Operation");
        }
    }
}