using TFT_Stats.Data;
using TFT_Stats.Models;

namespace TFT_Stats.Services
{
    public interface ITFT_DataService
    {

        void GetAdditionalCompanionInfo();

        void GetApiData();

        void UpdateCompanionVmDb();

    }
}