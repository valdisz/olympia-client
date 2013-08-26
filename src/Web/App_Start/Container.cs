using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.App_Start
{
    using Autofac;

    using Raven.Client;
    using Raven.Client.Embedded;

    public class Container : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(ctx =>
            {
                var store = new EmbeddableDocumentStore
                {
                    DataDirectory = "~/App_Data"
                };
                store.Initialize();
            
                return store;
            }).As<IDocumentStore>().SingleInstance();
        }
    }
}