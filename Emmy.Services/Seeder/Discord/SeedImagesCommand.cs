using System.Threading;
using System.Threading.Tasks;
using Emmy.Data.Enums;
using Emmy.Data.Util;
using Emmy.Services.Discord.Image.Commands;
using MediatR;

namespace Emmy.Services.Seeder.Discord
{
    public record SeedImagesCommand : IRequest<TotalAndAffectedCountDto>;
    
    public class SeedImagesHandler : IRequestHandler<SeedImagesCommand, TotalAndAffectedCountDto>
    {
        private readonly IMediator _mediator;

        public SeedImagesHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TotalAndAffectedCountDto> Handle(SeedImagesCommand request, CancellationToken ct)
        {
            var result = new TotalAndAffectedCountDto();
            var commands = new CreateImageCommand[]
            {
                new(Image.Placeholder, "https://cdn.discordapp.com/attachments/929693044054294578/929693137075597402/unknown.png"),
                new(Image.Welcome, "https://cdn.discordapp.com/attachments/931165008262492160/931165174616961084/unknown.png"),
                new(Image.WelcomeScheduledEvents, "https://cdn.discordapp.com/attachments/931165008262492160/931166150384042004/unknown.png"),
                new(Image.WelcomeGameWorld, "https://cdn.discordapp.com/attachments/931165008262492160/931166194122256414/unknown.png"),
                new(Image.WelcomeVoiceChannels, "https://cdn.discordapp.com/attachments/931165008262492160/931166124173824060/unknown.png"),
                new(Image.RequestGenderRole, "https://cdn.discordapp.com/attachments/931165008262492160/931166233519349830/unknown.png"),
                new(Image.GetGameRoles, "https://cdn.discordapp.com/attachments/931165008262492160/931167023763980339/unknown.png"),
                new(Image.Fishing, "https://cdn.discordapp.com/attachments/929693044054294578/935233643318738974/Fishing.png"),
                new(Image.Farm, "https://cdn.discordapp.com/attachments/929693044054294578/935233643612368986/Harvesting.png"),
                new(Image.Container, "https://cdn.discordapp.com/attachments/929693044054294578/935233643851436122/OpenBox.png"),
                new(Image.PrivateRoom, "https://cdn.discordapp.com/attachments/929693044054294578/935233644266668122/PrivateRoom.png"),
                new(Image.Relationship, "https://cdn.discordapp.com/attachments/929693044054294578/935233644690284634/Relationship.png"),
                new(Image.DailyRide, "https://cdn.discordapp.com/attachments/929693044054294578/935233644996472852/Ride.png"),
                new(Image.ShopBanner, "https://cdn.discordapp.com/attachments/929693044054294578/935233645269114900/ShopBanner.png"),
                new(Image.Casino, "https://cdn.discordapp.com/attachments/929693044054294578/935233645717880852/Casino.png"),
                new(Image.Contract, "https://cdn.discordapp.com/attachments/929693044054294578/935233645998907402/Contract.png"),
                new(Image.UserRoles, "https://cdn.discordapp.com/attachments/929693044054294578/935233710226280448/UserRoles.png"),
                new(Image.Vendor, "https://cdn.discordapp.com/attachments/929693044054294578/940120669105045555/Vendor.png"),
                new(Image.ShopKey, "https://cdn.discordapp.com/attachments/929693044054294578/935233710670888981/ShopKey.png"),
                new(Image.ShopSeed, "https://cdn.discordapp.com/attachments/929693044054294578/935233711090307113/ShopSeed.png"),
                new(Image.Transit, "https://cdn.discordapp.com/attachments/929693044054294578/935233711350362213/Transit.png"),
                new(Image.UserAchievements, "https://cdn.discordapp.com/attachments/929693044054294578/935233711602016256/UserAchievements.png"),
                new(Image.UserBanners, "https://cdn.discordapp.com/attachments/929693044054294578/935233711832694884/UserBanners.png"),
                new(Image.UserCollection, "https://cdn.discordapp.com/attachments/929693044054294578/935233712075984896/UserCollection.png"),
                new(Image.UserInventory, "https://cdn.discordapp.com/attachments/929693044054294578/935233712356986900/UserInventory.png"),
                new(Image.WorldInfo, "https://cdn.discordapp.com/attachments/929693044054294578/935233736319062066/WorldInfo.png"),
                new(Image.UserTitles, "https://cdn.discordapp.com/attachments/929693044054294578/935233736650391633/UserTitles.png"),
                new(Image.Wardrobe, "https://cdn.discordapp.com/attachments/929693044054294578/935234030171992195/Wardrobe.png"),
                new(Image.ShopRole, "https://cdn.discordapp.com/attachments/929693044054294578/935233710884782150/ShopRole.png"),
                new(Image.GetPremium, "https://cdn.discordapp.com/attachments/931165008262492160/935632605750128650/GetPremium.png"),
                new(Image.PremiumInfoRole, "https://cdn.discordapp.com/attachments/931165008262492160/935632606232453171/PremiumInfoRole.png"),
                new(Image.PremiumInfoWardrobe, "https://cdn.discordapp.com/attachments/931165008262492160/935632606446354483/PremiumInfoWardrobe.png"),
                new(Image.PremiumInfoCommandColor, "https://cdn.discordapp.com/attachments/931165008262492160/935632606005973002/PremiumInfoCommandColor.png"),
                new(Image.Rating, "https://cdn.discordapp.com/attachments/929693044054294578/935676150636748830/Rating.png"),
                new(Image.DonateInfo, "https://cdn.discordapp.com/attachments/931165008262492160/935671070017613834/DonateInfo.png"),
                new(Image.DailyReward, "https://cdn.discordapp.com/attachments/931165008262492160/940068963134631997/DailyReward.png"),
                new(Image.DailyRewardPremium, "https://cdn.discordapp.com/attachments/931165008262492160/940068963348537364/DailyRewardPremium.png"),
                new(Image.ReferralRewards, "https://cdn.discordapp.com/attachments/931165008262492160/940939305516408842/ReferralRewards.png"),
                new(Image.Referral, "https://cdn.discordapp.com/attachments/929693044054294578/940939388387467325/Referral.png"),
                new(Image.NotExpectedException, "https://cdn.discordapp.com/attachments/929693044054294578/942788475466428476/NotExpectedException.gif"),
                new(Image.ExpectedException, "https://cdn.discordapp.com/attachments/929693044054294578/942794923432878160/Error.png")
            };
            
            foreach (var createImageCommand in commands)
            {
                result.Total++;

                try
                {
                    await _mediator.Send(createImageCommand);

                    result.Affected++;
                }
                catch
                {
                    // ignored
                }
            }

            return result;
        }
    }
}