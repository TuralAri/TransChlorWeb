using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SysFile = System.IO;
using System.Threading.Tasks;

namespace TransChlorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly Meteo meteo;

        public DataController()
        {
            meteo = new Meteo();
        }

        [HttpPost("precalcul")]
        public async Task<IActionResult> Precalcul(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file received");
            }

            //saving in a temp file that will be deleted
            var savePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".dat");

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var fileToSendPath = meteo.MeteoTreatmentPrecalcul(savePath).ToString();

            if (SysFile.File.Exists(fileToSendPath))
            {
                var fileStream = new FileStream(fileToSendPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                //cleaning temp files after sending file
                HttpContext.Response.OnCompleted(() =>
                {
                    fileStream.Dispose();
                    SysFile.File.Delete(fileToSendPath);
                    Console.WriteLine("Temp file cleaned: " + fileToSendPath);
                    
                    SysFile.File.Delete(savePath);
                    Console.WriteLine("Uploaded temp file cleaned: " + savePath);
                    return Task.CompletedTask;
                });
                
                Console.WriteLine("File sent in response: " + fileToSendPath);

                return File(fileStream, "application/octet-stream", "response_fichier.dat");
                
            }
            
            return StatusCode(500, "An error occurred while processing the file.");
        }

        [HttpPost("calcul")]
        public async Task<IActionResult> Calcul()
        {
            var files = Request.Form.Files;
            
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files received");
            }
            
            var paths = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    //Each file is saved in the temp directory with a unique name
                    var savePath = Path.Combine(Path.GetTempPath(),Guid.NewGuid().ToString() + ".dat");
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    paths.Add(savePath);
                }    
            }
            
            var fileToSendPath = meteo.MeteoTreatmentCalcul(paths).ToString();
            if (SysFile.File.Exists(fileToSendPath))
            {
                var fileStream = new FileStream(fileToSendPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                //cleaning temp files after sending file
                HttpContext.Response.OnCompleted(() =>
                {
                    fileStream.Dispose();
                    SysFile.File.Delete(fileToSendPath);
                    Console.WriteLine("Temp file cleaned: " + fileToSendPath);

                    foreach (var filePath in paths)
                    {
                        SysFile.File.Delete(filePath);
                        Console.WriteLine("Uploaded temp file cleaned: " + filePath);
                    }
                    
                    return Task.CompletedTask;
                });
                
                Console.WriteLine("File sent in response: " + fileToSendPath);

                return File(fileStream, "application/octet-stream", "response_fichier.dat");
                
            }
            
            return StatusCode(500, "An error occurred while processing the file.");
        }
    }
}