using System.Threading.Tasks;
using Emmy.Data.Util;
using Emmy.Services.Seeder.Game;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Emmy.Controllers.Seeder
{
    [ApiController, Route("seeder/game")]
    public class SeederGameController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeederGameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("banners")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedBanners()
        {
            return Ok(await _mediator.Send(new SeedBannersCommand()));
        }

        [HttpPost, Route("crops")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedCrops()
        {
            return Ok(await _mediator.Send(new SeedCropsCommand()));
        }

        [HttpPost, Route("fishes")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedFishes()
        {
            return Ok(await _mediator.Send(new SeedFishesCommand()));
        }

        [HttpPost, Route("keys")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedKeys()
        {
            return Ok(await _mediator.Send(new SeedKeysCommand()));
        }

        [HttpPost, Route("localizations")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedLocalizations()
        {
            return Ok(await _mediator.Send(new SeedLocalizationsCommand()));
        }

        [HttpPost, Route("seeds")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedSeeds()
        {
            return Ok(await _mediator.Send(new SeedSeedsCommand()));
        }

        [HttpPost, Route("transits")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedTransits()
        {
            return Ok(await _mediator.Send(new SeedTransitsCommand()));
        }

        [HttpPost, Route("world-properties")]
        public async Task<ActionResult<TotalAndAffectedCountDto>> SeedWorldProperties()
        {
            return Ok(await _mediator.Send(new SeedWorldPropertiesCommand()));
        }
    }
}