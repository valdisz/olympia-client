using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers
{
    using System.IO;

    public class TestController : ApiController
    {
        // GET api/<controller>
        public AST.Province[] Get()
        {
            var lines = File.ReadAllLines(@"C:\Local\Apps\Olympia G3 MapViewer\Games\G3-Test\report-10.txt");
            var provinces = Parser.parse(lines);

            return provinces;
        }
    }
}