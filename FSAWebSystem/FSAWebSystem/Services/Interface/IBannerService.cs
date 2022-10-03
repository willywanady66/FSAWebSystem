using FSAWebSystem.Models;
using FSAWebSystem.Models.ViewModels;

namespace FSAWebSystem.Services.Interface
{
    public interface IBannerService
    {
        public Task<BannerPagingData> GetBannersPagination(DataTableParam param);
        public Task<PlantPagingData> GetPlantsPagination(DataTableParam param);
    }
}
