using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers
{
    using Raven.Client;

    using Web.Models;

    public class World
    {
        public Noble[] Nobles { get; set; }

        public Province[] Provinces { get; set; }

        public Location[] Locations { get; set; }
    }

    public class WorldController : ApiController
    {
        private readonly IDocumentStore store;

        public WorldController(IDocumentStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            this.store = store;
        }

        // GET api/<controller>
        public World Get()
        {
            using (var session = store.OpenSession())
            {
                return new World
                {
                    Provinces = session.Query<Province>().ToArray(),
                    Nobles = session.Query<Noble>().ToArray(),
                    Locations = session.Query<Location>().ToArray(),
                };
            }
        }
    }
}