using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Source_Code_Viewer.Models;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer;
using GroupDocs.Viewer.Results;
using System.Text.Json;

namespace Source_Code_Viewer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private string projectRootPath;
        private string outputPath;
        private string storagePath;
        List<string> lstFiles; 

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            projectRootPath = _hostingEnvironment.ContentRootPath;
            outputPath = Path.Combine(projectRootPath, "wwwroot/Content");
            storagePath = Path.Combine(projectRootPath, "storage");
            lstFiles = new List<string>(); 
        }

        public IActionResult Index()
        {
            var files = Directory.GetFiles(storagePath);
            foreach (var file in files)
            {
                lstFiles.Add(Path.GetFileName(file));
            }
            ViewBag.lstFiles = lstFiles; 
            return View();
        }
        [HttpPost]
        public IActionResult OnPost(string FileName)
        {
            int pageCount = 0;
            string imageFilesFolder = Path.Combine(outputPath, Path.GetFileName(FileName).Replace(".", "_"));
            if (!Directory.Exists(imageFilesFolder))
            {
                Directory.CreateDirectory(imageFilesFolder);
            }
            string imageFilesPath = Path.Combine(imageFilesFolder, "page-{0}.png");
            using (Viewer viewer = new Viewer(Path.Combine(storagePath, FileName)))
            {
                ViewInfo info = viewer.GetViewInfo(ViewInfoOptions.ForPngView(false));
                pageCount = info.Pages.Count;

                PngViewOptions options = new PngViewOptions(imageFilesPath);
                viewer.View(options);
            } 
            return new JsonResult(pageCount);
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
