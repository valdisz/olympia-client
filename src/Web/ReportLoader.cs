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
            using(var session = store.OpenSession())
            {
                foreach (var p in report.provinces)
                {
                    var id = CoordsToId(p.coords);

                    Province province = GetProvince(session, id);
                    province.Civ = p.civ;
                    province.Name = p.name;
                    province.Region = p.region;
                    province.Terrain = p.terrain;
                    province.Safe = p.safe;
                    province.ReportedOn = report.turn;

                    foreach (var innerLocation in p.locations)
                    {
                        Location loc = session.Load<Location>(innerLocation.id) ?? new Location();
                        loc.Id = innerLocation.id;
                        loc.Host = province;
                        loc.Kind = innerLocation.kind;
                        loc.Safe = innerLocation.safe;
                        session.Store(loc);

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

                        Province routedProvince = GetProvince(session, routeId);
                        routedProvince.Name = r.province;
                        routedProvince.Region = r.region;
                        routedProvince.Id = routeId;
                        session.Store(routedProvince);

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
                    }

                    session.Store(province);
                }

                session.SaveChanges();
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
    }
}