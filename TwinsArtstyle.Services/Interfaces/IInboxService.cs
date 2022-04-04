using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IInboxService
    {
        public Task<OperationResult> AddContactLetter(ContactUsViewModel contactUsViewModel, string userId);

        public Task<IEnumerable<MessageViewModel>> GetAllMessages();
    }
}
