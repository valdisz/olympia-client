namespace Web
{
    using System;
    using System.Linq;

    using Raven.Client;

    using Web.Models;

    public class ReportLoader : IReportLoader
    {
        private readonly IDocumentStore store;

        public ReportLoader(IDocumentStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            this.store = store;
        }

        public void LoadReport(AST.Report report)
        {
            foreach (var p in report.provinces)
            {
                var id = CoordsToId(p.coords);

            Province province;
            using (var session = store.OpenSession())
            {
                province = GetProvince(session, id);
            }

            province.X = p.coords.Item2;
                province.Y = p.coords.Item1;
                province.Civ = p.civ;
                province.Name = p.name;
                province.Region = p.region;
                province.Terrain = FToUper(p.terrain);
                province.Safe = p.safe;
                province.ReportedOn = report.turn;

                foreach (var innerLocation in p.locations)
                {
                    Location loc;
                    using (var session = store.OpenSession())
                    {
                        loc = session.Load<Location>(innerLocation.id) ?? new Location();
                        loc.Id = innerLocation.id;
                        loc.Host = province;
                        loc.Kind = innerLocation.kind;
                        loc.Safe = innerLocation.safe;

                        session.Store(loc);
                        session.SaveChanges();
                    }

                    if (RouteExists(province, loc.Id))
                    {
                        continue;
                    }

                    Route route = new Route
                    {
                        Target = loc,
                        Time = innerLocation.time,
                        Direction = RouteDirection.In
                    };
                    province.Routes.Add(route);
                }

                foreach (var r in p.routes)
                {
                    string routeId = CoordsToId(r.coords);
                    if (RouteExists(province, routeId))
                    {
                        continue;
                    }

                    Province routedProvince;
                    using (var session = store.OpenSession())
                    {
                        routedProvince = GetProvince(session, routeId);
                        routedProvince.X = r.coords.Item2;
                        routedProvince.Y = r.coords.Item1;
                        routedProvince.Name = r.province;
                        routedProvince.Terrain = FToUper(r.province);
                        routedProvince.Region = r.region;
                        routedProvince.Id = routeId;

                        session.Store(routedProvince);
                        session.SaveChanges();
                    }

                    Route route = new Route
                    {
                        Target = routedProvince,
                        Time = r.time
                    };

                    RouteDirection direction;
                    if (RouteDirection.TryParse(r.direction, out direction))
                    {
                        route.Direction = direction;
                    }

                    province.Routes.Add(route);
                    using (var session = store.OpenSession())
                    {
                        session.Store(province);
                        session.SaveChanges();
                    }
                }
            }

            foreach (var n in report.nobles)
            {
                Noble noble = new Noble
                {
                    Id = n.id,
                    Name = n.name,
                    X = n.coords.Item2,
                    Y = n.coords.Item1,
                };
                
                using (var session = store.OpenSession())
                {
                    session.Store(noble);
                    session.SaveChanges();
                }
            }
        }

        private static void Store<T>(T entity, IDocumentSession session)
        {
            session.Store(entity);
        }

        private static bool RouteExists(Province province, string routeId)
        {
            return province.Routes.Any(x => x.Target.Id == routeId);
        }

        private static Province GetProvince(IDocumentSession session, string id)
        {
            Province province = session.Load<Province>(id) ?? new Province();
            return province;
        }

        private static string CoordsToId(Tuple<string, int> coords)
        {
            return string.Format("{0}{1}", coords.Item1, coords.Item2);
        }

        private static string FToUper(string s)
        {
            return s.Substring(0, 1)
                .ToUpperInvariant() + s.Substring(1);
        }
    }
}