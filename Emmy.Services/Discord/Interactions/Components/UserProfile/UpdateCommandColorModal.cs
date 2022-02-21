using Discord;
using Discord.Interactions;

namespace Emmy.Services.Discord.Interactions.Components.UserProfile
{
    public class UpdateCommandColorModal : IModal
    {
        public string Title => "Изменение цвета команд";

        [InputLabel("Цвет команд")]
        [ModalTextInput("user-commandcolor", TextInputStyle.Short, "Цвет в формате #HEX", maxLength: 6)]
        public string CommandColor { get; set; }
    }
}