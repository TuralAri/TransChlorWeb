using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TransChlorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TroubleshootController : ControllerBase
    {
        private readonly Meteo _meteo;
        public TroubleshootController()
        {
            _meteo = new Meteo();
        }
        
        [HttpPost("troubleshoot1")]
        public async Task<IActionResult> Troubleshoot1(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file received");
            }
            
            var savePath = Path.Combine(Path.GetTempPath(), "reçu_fichier1.dat");
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var result = _meteo.MeteoTreatmentTroubleshootingPart1(savePath);
            
            //Adding delete temp files logic once we made all files unique
            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);
            }

            return Ok(result);
        }
        
        [HttpPost("troubleshoot2")]
        public async Task<IActionResult> Troubleshoot2(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file received");
            }
            
            var savePath = Path.Combine(Path.GetTempPath(), "reçu_fichier2.dat");
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var result = _meteo.MeteoTreatmentTroubleshootingPart2(savePath);
            
            //Adding delete temp files logic once we made all files unique
            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);
            }

            return Ok(result);
        }
        
        
    } 
}