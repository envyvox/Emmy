using Discord;
using Discord.Interactions;

namespace Emmy.Services.Discord.Interactions.Components.UserProfile
{
    public class UpdateAboutModal : IModal
    {
        public string Title => "Обновление информации профиля";

        [InputLabel("Информация")]
        [ModalTextInput("user-about", TextInputStyle.Paragraph,
            "Информация, которая будет отображатся в твоем профиле", maxLength: 1024)]
        public string About { get; set; }
    }
}