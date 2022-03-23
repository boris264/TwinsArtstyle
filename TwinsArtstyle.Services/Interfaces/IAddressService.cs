using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IAddressService
    {
        public Task<IEnumerable<AddressViewModel>> GetAddressesForUser(string userId);

        public Task<bool> AddNewAddress(AddressViewModel addressViewModel, string userId);
    }
}
