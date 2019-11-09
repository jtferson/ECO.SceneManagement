using Autofac;
using System;

namespace ECO.SceneManagement
{
    public class SceneKernel
    {
        public ContainerBuilder Builder { get; private set; }
        public IContainer Container { get; private set; }

        public void CreateBuilder()
        {
            Builder = new ContainerBuilder();
        }

        public void Build()
        {
            if (Builder == null) throw new Exception("ContainerBuilder is null");

            Container = Builder.Build();
        }
    }
}
