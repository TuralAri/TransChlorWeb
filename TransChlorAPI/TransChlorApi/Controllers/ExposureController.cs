using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SysFile = System.IO;
using System.Threading.Tasks;

namespace TransChlorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExposureController : ControllerBase
    {
        private readonly Meteo meteo;

        public ExposureController()
        {
            meteo = new Meteo();
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export()
        {
            var files = Request.Form.Files;
            
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files received");
            }
            
            var paths = new List<string>();
            var uploadedPaths = new List<string>(); //will be the exact clone of paths until it'll be used by MeteoTreatmentExport(paths)

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    var savePath = Path.Combine(Path.GetTempPath(),$"{originalFileNameWithoutExtension}.dat");
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    paths.Add(savePath);
                    uploadedPaths.Add(savePath);
                }    
            }
            
            var zipPath = meteo.MeteoTreatmentExport(paths).ToString();

            if (SysFile.File.Exists(zipPath))
            {
                var fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                //cleaning temp files after sending file
                HttpContext.Response.OnCompleted(() =>
                {
                    fileStream.Dispose();
                    SysFile.File.Delete(zipPath);
                    Console.WriteLine("Temp file cleaned: " + zipPath);

                    foreach (var filePath in uploadedPaths)
                    {
                        if (SysFile.File.Exists(filePath))
                        {
                            SysFile.File.Delete(filePath);
                            Console.WriteLine("Uploaded temp file cleaned: " + filePath);
                        }
                    }
                    
                    return Task.CompletedTask;
                });
                
                return File(fileStream, "application/octet-stream");
            }
            
            return StatusCode(500,"An error occured while exporting files");
        }

    }
}