namespace SlimResult.Test
{
    [TestClass]
    public class ErrorTests
    {
        [TestMethod]
        public void CreateByMessageTest()
        {
            Error error = new("Bad Data");
            error.Message.Should().Be("Bad Data");
            error.Exception.IsNone.Should().BeTrue();
        }

        [TestMethod]
        public void CreateByExceptionTest()
        {
            Exception exception = new("Failed Operation");
            Error error = new(exception);
            error.Message.Should().Be("Failed Operation");
            error.Exception.Value.Should().BeOfType<Exception>();
            error.Exception.Value.Message.Should().Be("Failed Operation");
        }

        [TestMethod]
        public void ImplicitCastsTest()
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