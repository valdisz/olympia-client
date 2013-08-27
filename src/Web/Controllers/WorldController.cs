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
        public IEnumerable<Province> Get()
        {
            using (var session = store.OpenSession())
            {
                return session.Query<Province>()
                    .ToList();
            }
        }
    }
}