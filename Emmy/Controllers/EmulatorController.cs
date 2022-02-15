using System.Threading.Tasks;
using Emmy.Services.Emulator;
using Emmy.Services.Emulator.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Emmy.Controllers
{
    [ApiController, Route("emulator")]
    public class EmulatorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmulatorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("casino")]
        public async Task<ActionResult<EmulateCasinoResult>> EmulateCasino(EmulateCasino request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet("fishing")]
        public async Task<ActionResult<CycleEmulateFishingResult>> EmulateFishing(EmulateFishing request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}