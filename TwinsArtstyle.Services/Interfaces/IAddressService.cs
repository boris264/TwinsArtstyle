using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.ViewModels;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface IAddressService
    {
        public Task<IEnumerable<AddressViewModel>> GetAddressesForUser(string userId);

        public Task<OperationResult> AddNewAddress(AddressViewModel addressViewModel, string userId);

        public Task<AddressViewModel> AddressExistsForUser(string addressName, string userId);
    }
}
