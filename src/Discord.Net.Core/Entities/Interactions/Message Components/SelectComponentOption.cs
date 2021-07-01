namespace Discord
{
	/// <summary>
	///		Represents the options for a <see cref="SelectComponent"/>.
	/// </summary>
	public class SelectComponentOption
	{
		/// <summary>
		///     The label of the option, this is the text that is shown, max 25 characters
		/// </summary>
		public string Label { get; }

		/// <summary>
		///     A unique id that will be sent with a <see cref="IDiscordInteraction"/>. This is how you know what option was selected.
		/// </summary>
		public string Value { get; }
	    
		/// <summary>
		///     The description of the option, max 50 characters
		/// </summary>
		public string Description { get; }
	    
		/// <summary>
		///     A <see cref="IEmote"/> that will be displayed with this button.
		/// </summary>
		public IEmote Emote { get; }
	    
		/// <summary>
		///     The option to be displayed by default
		/// </summary>
		public bool Default { get; }
	}
}