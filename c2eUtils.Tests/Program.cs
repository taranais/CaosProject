using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.DotNet;


namespace c2eUtils.Tests
{
    public class  C2EImagesTest
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }

}
