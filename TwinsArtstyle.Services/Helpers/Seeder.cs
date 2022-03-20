using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Helpers
{
    public class Seeder : ISeeder
    {
        private readonly IRepository repository;

        public Seeder(IRepository repo)
        {
            repository = repo;
        }

        public IEnumerable<RegisterViewModel> SeedUsers()
        {
            var users = new List<RegisterViewModel>();

            for (int i = 0; i < 3000; i++)
            {
                RegisterViewModel registerViewModel = new RegisterViewModel()
                {
                    FirstName = $"Boris{i}",
                    LastName = $"Todorov{i}",
                    Email = $"Boristodorov12{i}@abv.bg",
                    Password = $"9600231{i}",
                    ConfirmPassword = $"9600231{i}",
                };

                users.Add(registerViewModel);
            }

            return users;
        }
    }
}
