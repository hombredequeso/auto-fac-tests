using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using FluentAssertions;
using Xunit;

namespace AutofacTests
{
    public interface IMessage
    {
    }

    public class MessageA : IMessage
    {
    }

    public class MessageB : IMessage
    {
    }

    public interface IMessageHandler<in T> where T : IMessage
    {
        void Handle(T Message);
    }

    public class MessageAHandler : IMessageHandler<MessageA>
    {
        public void Handle(MessageA Message)
        {
        }
    }

    public class MessageBHandler : IMessageHandler<MessageB>
    {
        public void Handle(MessageB Message)
        {
        }
    }
    
    public class MessageHandlerTests
    {
        public IContainer CreateContainer()
        {
            var dataAccess = Assembly.GetExecutingAssembly();

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(dataAccess)
                .AsClosedTypesOf(typeof(IMessageHandler<>))
                .AsImplementedInterfaces();
            return builder.Build();
        }

        [Fact]
        public void CreateHandlerATest()
        {

            IContainer container = CreateContainer();

            var handler = container.Resolve<IMessageHandler<MessageA>>();
            
            handler.Should().BeOfType<MessageAHandler>();
        }

        public IMessageHandler<T> GetHandlerFor<T>(IContainer container, T m) where T : IMessage
        {
            Type t = m.GetType();
            Type closedHandlerType = typeof(IMessageHandler<>).MakeGenericType(t);
            object handler = container.Resolve(closedHandlerType);
            return (IMessageHandler<T>)handler;
        }

        [Fact]
        public void CreateHandlerAUsingClosingTest()
        {
            IContainer container = CreateContainer();

            MessageA messageA = new MessageA();
            var handler = GetHandlerFor(container, messageA);
            
            handler.Should().BeOfType<MessageAHandler>();
            
            handler.Handle(messageA);
        }

        [Fact]
        public void VarianceTest()
        {
            IEnumerable<string> ss = new List<string>(){"aa", "bb"};
            IEnumerable<object> so = ss;
        }
        
        
        public void DispatchMessage<T>(IContainer c, T m) where T : IMessage
        {
            Type closedHandlerType = typeof(IMessageHandler<>).MakeGenericType(m.GetType());
            object handler = c.Resolve(closedHandlerType);
            MethodInfo handleMethod = closedHandlerType.GetMethod("Handle");
            handleMethod.Invoke(handler, new object[] {m});
        }

        [Fact]
        public void DispatchTest()
        {
            IContainer container = CreateContainer();

            IMessage messageA = new MessageA();
            DispatchMessage(container, messageA);
        }
    }
}