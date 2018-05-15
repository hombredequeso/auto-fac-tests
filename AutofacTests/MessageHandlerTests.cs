using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Autofac;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
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

    public interface MessageHandler<T> where T : IMessage
    {
        void Handle(T Message);
    }

    public class MessageAHandler : MessageHandler<MessageA>
    {
        public void Handle(MessageA Message)
        {
        }
    }

    public class MessageBHandler : MessageHandler<MessageB>
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
                .AsClosedTypesOf(typeof(MessageHandler<>))
                .AsImplementedInterfaces();
            return builder.Build();
        }

        [Fact]
        public void CreateHandlerATest()
        {

            IContainer container = CreateContainer();

            var handler = container.Resolve<MessageHandler<MessageA>>();
            
            handler.Should().BeOfType<MessageAHandler>();
        }

        public MessageHandler<T> GetHandlerFor<T>(IContainer container, T m) where T : IMessage
        {
            Type t = m.GetType();
            Type closedHandlerType = typeof(MessageHandler<>).MakeGenericType(t);
            object handler = container.Resolve(closedHandlerType);
            return (MessageHandler<T>)handler;
        }
        
        [Fact]
        public void CreateHandlerAUsingClosingTest()
        {
            IContainer container = CreateContainer();

            MessageA messageA = new MessageA();
            MessageHandler<MessageA> handler = GetHandlerFor(container, messageA);
            
            handler.Should().BeOfType<MessageAHandler>();
            
            handler.Handle(messageA);
        }
    }
}