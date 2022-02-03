using System;

namespace Emmy.Data.Enums.Discord
{
    public enum Channel : byte
    {
        Welcome,
        Chat,
        Commands,
        GetRoles,
        Announcements,

        GameParent,
        GameInfo,
        GameLore,
        GameEvents,
        GameUpdates,

        SearchParent,
        SearchGenshinImpact,
        SearchLeagueOfLegends,
        SearchTeamfightTactics,
        SearchValorant,
        SearchTarkov,
        SearchDeadByDaylight,
        SearchApexLegends,
        SearchDota,
        SearchMinecraft,
        SearchOsu,
        SearchAmongUs,
        SearchRust,
        SearchCsGo,
        SearchMobileGaming,

        EventParent,
        EventLobby,

        CommunityDescParent,
        CommunityDescHowItWork,
        Photos,
        Screenshots,
        Memes,
        Arts,
        Music,
        Erotic,
        Nsfw,

        LibraryParent,
        Rules,
        Giveaways,
        Suggestions,

        TavernParent,
        TavernOne,
        TavernTwo,
        TavernMale,
        TavernFemale,

        CreateRoomParent,
        NoMic,
        CreateRoom,

        PrivateRoomParent,

        LoveRoomParent,

        AfkParent,
        Afk,

        AdministrationParent,
        Administration,
        Moderation,
        EventManager,
        Staff,
        Meeting
    }

    public static class ChannelHelper
    {
        private const string Emote = "・";

        public static string Name(this Channel channel)
        {
            return channel switch
            {
                Channel.Welcome => Emote + "приветствие",
                Channel.Chat => Emote + "общение",
                Channel.Commands => Emote + "команды",
                Channel.GetRoles => Emote + "получение-ролей",
                Channel.Announcements => Emote + "объявления",

                Channel.GameParent => "игровая вселенная",
                Channel.GameInfo => Emote + "информация",
                Channel.GameLore => Emote + "история-мира",
                Channel.GameEvents => Emote + "игровые-события",
                Channel.GameUpdates => Emote + "обновления",

                Channel.SearchParent => "поиск игроков",
                Channel.SearchGenshinImpact => Emote + "genshin-impact",
                Channel.SearchLeagueOfLegends => Emote + "league-of-legends",
                Channel.SearchTeamfightTactics => Emote + "teamfight-tactics",
                Channel.SearchValorant => Emote + "valorant",
                Channel.SearchTarkov => Emote + "tarkov",
                Channel.SearchDeadByDaylight => Emote + "dead-by-daylight",
                Channel.SearchApexLegends => Emote + "apex-legends",
                Channel.SearchDota => Emote + "dota",
                Channel.SearchMinecraft => Emote + "minecraft",
                Channel.SearchAmongUs => Emote + "among-us",
                Channel.SearchOsu => Emote + "osu",
                Channel.SearchRust => Emote + "rust",
                Channel.SearchCsGo => Emote + "cs-go",
                Channel.SearchMobileGaming => Emote + "mobile-gaming",

                Channel.EventParent => "мероприятия",
                Channel.EventLobby => "Лобби мероприятия",

                Channel.CommunityDescParent => "доска сообщества",
                Channel.CommunityDescHowItWork => Emote + "как-работает",
                Channel.Photos => Emote + "фотографии",
                Channel.Screenshots => Emote + "скриншоты",
                Channel.Memes => Emote + "мемесы",
                Channel.Arts => Emote + "арты",
                Channel.Music => Emote + "музыка",
                Channel.Erotic => Emote + "эротика",
                Channel.Nsfw => Emote + "nsfw",

                Channel.LibraryParent => "великая «тосёкан»",
                Channel.Rules => Emote + "правила",
                Channel.Giveaways => Emote + "розыгрыши",
                Channel.Suggestions => Emote + "предложения",

                Channel.TavernParent => "Таверны",
                Channel.TavernOne => "Таверна «Идзакая»",
                Channel.TavernTwo => "Таверна «Каябукия»",
                Channel.TavernMale => "Таверна «Оками»",
                Channel.TavernFemale => "Таверна «Китсунэ»",

                Channel.CreateRoomParent => "созданные каналы",
                Channel.NoMic => Emote + "без-микрофона",
                Channel.CreateRoom => "Создать канал",

                Channel.PrivateRoomParent => "приватные секторы",

                Channel.LoveRoomParent => "любовные гнезда",

                Channel.AfkParent => "zzz",
                Channel.Afk => "Афк, жду подарки",

                Channel.AdministrationParent => "скрытый раздел",
                Channel.Administration => Emote + "администраторы",
                Channel.Moderation => Emote + "модераторы",
                Channel.EventManager => Emote + "организаторы",
                Channel.Staff => Emote + "стафф",
                Channel.Meeting => "Собрание",
                _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
            };
        }

        public static ChannelType Type(this Channel channel)
        {
            return channel switch
            {
                Channel.Welcome => ChannelType.Text,
                Channel.Chat => ChannelType.Text,
                Channel.Commands => ChannelType.Text,
                Channel.GetRoles => ChannelType.Text,
                Channel.Announcements => ChannelType.Text,

                Channel.GameParent => ChannelType.Category,
                Channel.GameInfo => ChannelType.Text,
                Channel.GameLore => ChannelType.Text,
                Channel.GameEvents => ChannelType.Text,
                Channel.GameUpdates => ChannelType.Text,

                Channel.SearchParent => ChannelType.Category,
                Channel.SearchGenshinImpact => ChannelType.Text,
                Channel.SearchLeagueOfLegends => ChannelType.Text,
                Channel.SearchTeamfightTactics => ChannelType.Text,
                Channel.SearchValorant => ChannelType.Text,
                Channel.SearchTarkov => ChannelType.Text,
                Channel.SearchDeadByDaylight => ChannelType.Text,
                Channel.SearchApexLegends => ChannelType.Text,
                Channel.SearchDota => ChannelType.Text,
                Channel.SearchMinecraft => ChannelType.Text,
                Channel.SearchAmongUs => ChannelType.Text,
                Channel.SearchOsu => ChannelType.Text,
                Channel.SearchRust => ChannelType.Text,
                Channel.SearchCsGo => ChannelType.Text,
                Channel.SearchMobileGaming => ChannelType.Text,

                Channel.EventParent => ChannelType.Category,
                Channel.EventLobby => ChannelType.Voice,

                Channel.CommunityDescParent => ChannelType.Category,
                Channel.CommunityDescHowItWork => ChannelType.Text,
                Channel.Photos => ChannelType.Text,
                Channel.Screenshots => ChannelType.Text,
                Channel.Memes => ChannelType.Text,
                Channel.Arts => ChannelType.Text,
                Channel.Music => ChannelType.Text,
                Channel.Erotic => ChannelType.Text,
                Channel.Nsfw => ChannelType.Text,

                Channel.LibraryParent => ChannelType.Category,
                Channel.Rules => ChannelType.Text,
                Channel.Giveaways => ChannelType.Text,
                Channel.Suggestions => ChannelType.Text,

                Channel.TavernParent => ChannelType.Category,
                Channel.TavernOne => ChannelType.Voice,
                Channel.TavernTwo => ChannelType.Voice,
                Channel.TavernMale => ChannelType.Voice,
                Channel.TavernFemale => ChannelType.Voice,

                Channel.CreateRoomParent => ChannelType.Category,
                Channel.NoMic => ChannelType.Text,
                Channel.CreateRoom => ChannelType.Voice,

                Channel.PrivateRoomParent => ChannelType.Category,

                Channel.LoveRoomParent => ChannelType.Category,

                Channel.AfkParent => ChannelType.Category,
                Channel.Afk => ChannelType.Voice,

                Channel.AdministrationParent => ChannelType.Category,
                Channel.Administration => ChannelType.Text,
                Channel.Moderation => ChannelType.Text,
                Channel.EventManager => ChannelType.Text,
                Channel.Staff => ChannelType.Text,
                Channel.Meeting => ChannelType.Voice,

                _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
            };
        }

        public static Channel Parent(this Channel channel)
        {
            return channel switch
            {
                Channel.GameInfo => Channel.GameParent,
                Channel.GameLore => Channel.GameParent,
                Channel.GameEvents => Channel.GameParent,
                Channel.GameUpdates => Channel.GameParent,

                Channel.SearchGenshinImpact => Channel.SearchParent,
                Channel.SearchLeagueOfLegends => Channel.SearchParent,
                Channel.SearchTeamfightTactics => Channel.SearchParent,
                Channel.SearchValorant => Channel.SearchParent,
                Channel.SearchTarkov => Channel.SearchParent,
                Channel.SearchDeadByDaylight => Channel.SearchParent,
                Channel.SearchApexLegends => Channel.SearchParent,
                Channel.SearchDota => Channel.SearchParent,
                Channel.SearchMinecraft => Channel.SearchParent,
                Channel.SearchOsu => Channel.SearchParent,
                Channel.SearchAmongUs => Channel.SearchParent,
                Channel.SearchRust => Channel.SearchParent,
                Channel.SearchMobileGaming => Channel.SearchParent,

                Channel.EventLobby => Channel.EventParent,

                Channel.CommunityDescHowItWork => Channel.CommunityDescParent,
                Channel.Photos => Channel.CommunityDescParent,
                Channel.Screenshots => Channel.CommunityDescParent,
                Channel.Memes => Channel.CommunityDescParent,
                Channel.Arts => Channel.CommunityDescParent,
                Channel.Music => Channel.CommunityDescParent,
                Channel.Erotic => Channel.CommunityDescParent,
                Channel.Nsfw => Channel.CommunityDescParent,

                Channel.Rules => Channel.LibraryParent,
                Channel.Giveaways => Channel.LibraryParent,
                Channel.Suggestions => Channel.LibraryParent,

                Channel.TavernOne => Channel.TavernParent,
                Channel.TavernTwo => Channel.TavernParent,
                Channel.TavernMale => Channel.TavernParent,
                Channel.TavernFemale => Channel.TavernParent,

                Channel.NoMic => Channel.CreateRoomParent,
                Channel.CreateRoom => Channel.CreateRoomParent,

                Channel.Afk => Channel.AfkParent,

                Channel.Administration => Channel.AdministrationParent,
                Channel.Moderation => Channel.AdministrationParent,
                Channel.EventManager => Channel.AdministrationParent,
                Channel.Staff => Channel.AdministrationParent,
                Channel.Meeting => Channel.AdministrationParent,

                _ => channel
            };
        }
    }
}