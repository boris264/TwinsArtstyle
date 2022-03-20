using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Helpers
{
    public interface ISeeder
    {
        public IEnumerable<RegisterViewModel> SeedUsers();
    }
}
