using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers
{
    using System.IO;

    public class Province
    {
        public string Terrain { get; set; }

        public int X { get; set; }

        public string Y { get; set; }
    }

    public class TestController : ApiController
    {
        // GET api/<controller>
        public Province[] Get()
        {
            Dictionary<string, Dictionary<int, Province>> provinces = new Dictionary<string, Dictionary<int, Province>>();
            string[] reports = new []
            {
                @"C:\Local\Apps\Olympia G3 MapViewer\Games\G3-Test\report-5.txt",
                @"C:\Local\Apps\Olympia G3 MapViewer\Games\G3-Test\report-6.txt",
                @"C:\Local\Apps\Olympia G3 MapViewer\Games\G3-Test\report-7.txt",
                @"C:\Local\Apps\Olympia G3 MapViewer\Games\G3-Test\report-8.txt",
                @"C:\Local\Apps\Olympia G3 MapViewer\Games\G3-Test\report-9.txt",
                @"C:\Local\Apps\Olympia G3 MapViewer\Games\G3-Test\report-10.txt",
                @"C:\Local\Apps\Olympia G3 MapViewer\Games\G3-Test\report-11.txt"
            };

            foreach (var f in reports)
            {
                var lines = File.ReadAllLines(f);
                var report = Parser.parse(lines);

                foreach (var province in report)
                {
                    AddProvince(provinces, province.terrain, province.coords);

                    foreach (var route in province.routes)
                    {
                        this.AddProvince(provinces, route.province, route.coords);
                    }
                } 
            }

            return provinces.Values.SelectMany(x => x.Values).ToArray();
        }

        private void AddProvince(Dictionary<string, Dictionary<int, Province>> provinces, string terrain, Tuple<string, int> coords)
        {
            if (!provinces.ContainsKey(coords.Item1))
            {
                provinces.Add(coords.Item1, new Dictionary<int, Province>());
            }

            if (!provinces[coords.Item1].ContainsKey(coords.Item2))
            {
                terrain = terrain[0].ToString().ToUpperInvariant() + terrain.Substring(1);
                provinces[coords.Item1].Add(coords.Item2, new Province { Terrain = terrain, X = coords.Item2, Y = coords.Item1 });
            }
        }
    }
}