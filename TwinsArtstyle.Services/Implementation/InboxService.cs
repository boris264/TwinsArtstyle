using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Services.Constants;
using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Implementation
{
    public class InboxService : IInboxService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository _repository;

        public InboxService(UserManager<User> userManager,
            IRepository repository)
        {
            _userManager = userManager;
            _repository = repository;
        }

        public async Task<OperationResult> AddContactLetter(ContactUsViewModel contactUsViewModel, string userId)
        {
            var result = new OperationResult();
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                try
                {
                    Message message = new Message()
                    {
                        User = user,
                        Title = contactUsViewModel.Title,
                        Body = contactUsViewModel.Content
                    };

                    await _repository.Add(message);
                    await _repository.SaveChanges();
                    result.Success = true;
                }
                catch (DbUpdateException)
                {
                    result.ErrorMessage = ErrorMessages.DbUpdateFailedMessage;
                }
            }

            result.ErrorMessage = "User should not be null!";
            return result;
        }

        public async Task<IEnumerable<MessageViewModel>> GetAllMessages()
        {
            return await _repository.All<Message>()
                .Select(m => new MessageViewModel()
                {
                    User = new UserViewModel()
                    {
                        FirstName = m.User.FirstName,
                        LastName = m.User.LastName,
                        Email = m.User.Email,
                        PhoneNumber = m.User.PhoneNumber,
                    },
                    Title = m.Title,
                    Content = m.Body
                }).ToListAsync();
        }
    }
}
