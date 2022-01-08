public partial class FoodCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("food", "Tell us about your favorite food.")]
    public async Task Command()
        => await RespondWithModalAsync(new FoodModal());
        
    [ModalInteraction("food_menu")]
    public async Task ModalResponce(FoodModal modal)
    {
        // The Interaction Service will automatically fill in 
        // properties that have a ModalTextInput attribute with the 
        // user provided value.
        string message = "hey @everyone, I just learned " +
            $"{Context.User.Mention}'s favorite food is " +
            $"{modal.Food} because {modal.Reason}.";

        // Specify the AllowedMentions so we don't actually ping 
        // everyone.
        AllowedMentions mentions = new();
        mentions.AllowedTypes = AllowedMentionTypes.Users;

        // Respond to the modal.
        await RespondAsync(message, allowedMentions: mentions);
    }
    
    public class FoodModal : IModal
    {
        // Title and CrustomId are both required.
        public string Title => "Fav Food"; 
        public string CustomId => "food_menu";
        
        // Strings with the ModalTextInput attribute will automatically 
        [ModalTextInput("What??", "food_name", placeholder:"Pizza", maxLength:20)]
        public string Food { get; set; }
        
        // Additional paremeters can be specified to further customize
        // the input.
        [ModalTextInput("Why??", "food_reason", TextInputStyle.Paragraph, "Kuz it's tasty", maxLength:500)]
        public string Reason { get; set; } 
    }
}