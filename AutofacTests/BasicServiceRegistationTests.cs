using System;
using System.Reflection;
using Autofac;
using FluentAssertions;
using Xunit;

namespace AutofacTests
{
    public interface IService
    {
    }

    public class MyService : IService
    {
    }


    public class BasicServiceRegistrationTests
    {
        public IContainer CreateExplicitContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyService>().As<IService>();
            return builder.Build();
        }

        [Fact]
        public void ExplicitRegistration()
        {
            var container = CreateExplicitContainer();

            var service = container.Resolve<IService>();
            service.Should().BeOfType<MyService>();
        }

        public IContainer CreateScannedContainer()
        {
            var dataAccess = Assembly.GetExecutingAssembly();

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();
            return builder.Build();
        }

        [Fact]
        public void ScannedRegistration()
        {
            var container = CreateScannedContainer();

            var service = container.Resolve<IService>();
            service.Should().BeOfType<MyService>();
        }
    }
}