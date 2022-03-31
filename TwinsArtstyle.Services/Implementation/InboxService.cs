using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
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

        public async Task AddContactLetter(ContactUsViewModel contactUsViewModel, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            
            if(user != null)
            {
                Message message = new Message()
                {
                    User = user,
                    Title = contactUsViewModel.Title,
                    Body = contactUsViewModel.Content
                };

                await _repository.Add(message);
                await _repository.SaveChanges();
            }
        }
    }
}
