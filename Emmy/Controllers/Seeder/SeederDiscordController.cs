using System.Threading.Tasks;
using Emmy.Data.Util;
using Emmy.Services.Seeder.Discord;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Emmy.Controllers.Seeder
{
    [ApiController, Route("seeder/discord")]
    public class SeederDiscordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeederDiscordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("images")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedImages()
        {
            return Ok(await _mediator.Send(new SeedImagesCommand()));
        }
    }
}