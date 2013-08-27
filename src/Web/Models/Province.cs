namespace Web.Models
{
    using System.Collections.Generic;

    public class LocationRef
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public static implicit operator LocationRef(NamedEntity ent)
        {
            return new LocationRef { Id = ent.Id, Name = ent.Name };
        }
    }

    public abstract class NamedEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class Location : NamedEntity
    {
        public bool Safe { get; set; }

        public string Kind { get; set; }

        public int ReportedOn { get; set; }

        public LocationRef Host { get; set; }
    }

    public enum RouteDirection
    {
        North,
        South,
        East,
        West,
        Out,
        In
    }

    public class Route
    {
        public LocationRef Target { get; set; }

        public RouteDirection Direction { get; set; }

        public int Time { get; set; }
    }

    public class Province : NamedEntity
    {
        public int X { get; set; }

        public string Y { get; set; }

        public string Region { get; set; }

        public string Terrain { get; set; }

        public int Civ { get; set; }

        public bool Safe { get; set; }

        public int ReportedOn { get; set; }

        public List<Route> Routes { get; set; }

        public Province()
        {
            Routes = new List<Route>();
        }
    }
}