namespace Web.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;

    public class UploadController : ApiController
    {
        private readonly IReportLoader reportLoader;

        public UploadController(IReportLoader reportLoader)
        {
            if (reportLoader == null)
            {
                throw new ArgumentNullException("reportLoader");
            }

            this.reportLoader = reportLoader;
        }

        public async Task PostMultipartStream()
        {
            // Verify that this is an HTML Form file upload request
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string tempDir = HttpContext.Current.Server.MapPath("~/App_Data/temp");
            var provider = new MultipartFileStreamProvider(tempDir);
            var multipart = await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var fileData in multipart.FileData)
            {
                try
                {
                    var lines = File.ReadAllLines(fileData.LocalFileName);
                    var report = Parser.parse(lines);
                    this.reportLoader.LoadReport(report);
                }
                finally
                {
                    File.Delete(fileData.LocalFileName);
                }
            }
        }
    }
}