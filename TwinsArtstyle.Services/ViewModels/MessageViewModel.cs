namespace TwinsArtstyle.Services.ViewModels
{
    public class MessageViewModel
    {
        public Guid Id { get; set; }    
        public UserViewModel User { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
    }
}
