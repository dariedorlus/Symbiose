using AutoFixture;
using NUnit.Framework;

namespace Symbiose.Test.Unit
{
    [TestFixture]
    public class TestBase 
    {
        protected Fixture fixture;

        public TestBase()
        {
            fixture = new Fixture();
        }

        
    }
}