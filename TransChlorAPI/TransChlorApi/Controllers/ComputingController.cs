using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SysFile = System.IO;
using System.Threading.Tasks;
using TransChlorApi.Services;

namespace TransChlorApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ComputingController : ControllerBase
    {
        private readonly ComputingStrategyFactory _factory;
        private readonly ComputationTaskManager _taskManager;
        
        public ComputingController(ComputingStrategyFactory factory, ComputationTaskManager taskManager)
        {
            _factory = factory;
            _taskManager = taskManager;
        }

        [HttpPost("run")]
        public IActionResult Run([FromBody] JsonElement body)
        {
            if (!body.TryGetProperty("computationId", out var idElement) || !idElement.TryGetInt32(out var computationId))
                return BadRequest("Missing or invalid computationId");
            
            string mode = "1D";       // default
            string outfile = "";      // default
            string data = "";         // default

            if (body.TryGetProperty("mode", out var modeElement))
            {
                mode = modeElement.GetString() ?? mode;
            }

            if (body.TryGetProperty("outfile", out var outfileElement))
            {
                outfile = outfileElement.GetString() ?? outfile;
            }

            if (body.TryGetProperty("data", out var dataElement))
            {
                data = dataElement.GetString() ?? data;
            }


            // instantiate a new cancellation token for each tasks
            var cts = new CancellationTokenSource();
            
            //Here I am trying to instanciate a computation in the manager (does not launch anything, only stored in a list so we can retrieve the task)
            if (!_taskManager.TryStart(computationId, cts))
                return StatusCode(500, "Error while running computation");
            
            try
            {
                //starting the task in async
                Task.Run(async () =>
                {
                    var strategy = _factory.GetStrategy(mode, computationId);
                    await strategy.ExecuteAsync(computationId, cts.Token, outfile, data);
                });
                
                return Ok(new { status = $"Computation {computationId} has started" });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Computation {computationId} cancelled.");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost("cancel")]
        public IActionResult Cancel([FromQuery] int computationId)
        {
            if (_taskManager.TryStop(computationId))
            {
                return Ok(new { status = $"Computation {computationId} has stopped" });
            }
            
            return NotFound("Computation wasn't found");
        }
        
    }
}