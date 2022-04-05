using Microsoft.Extensions.DependencyInjection;
using Multicopy.MAUI.Core.Services;
using NUnit.Framework;


namespace Multicopy.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddTransient<ICopyService, CopyService>();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}