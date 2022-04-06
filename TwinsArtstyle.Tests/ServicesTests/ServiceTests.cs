using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NUnit.Framework;
using TwinsArtstyle.Infrastructure.Data;
using TwinsArtstyle.Infrastructure.Interfaces;
using TwinsArtstyle.Infrastructure.Models;
using TwinsArtstyle.Tests.Helpers;

namespace TwinsArtstyle.Tests.ServicesTests
{
    public class ServiceTests
    {
        public ApplicationDbContext dbContext;
        public IRepository repository;
        public UserManager<User> userManager;

        [SetUp]
        public virtual void Setup()
        {
            dbContext = InMemoryDbConfig.CreateDbContext("Test_Database");
            repository = new Repository(dbContext);
            userManager = new UserManager<User>(new UserStore<User>(dbContext), null, null, null, null, null, null, null, null);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }
    }
}
