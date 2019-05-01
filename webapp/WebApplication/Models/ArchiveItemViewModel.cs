using System.Linq;
using K9.Base.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class ArchiveItemViewModel
    {
        public Base.WebApplication.ViewModels.ArchiveViewModel ArchiveViewModel { get; set; }
        public int SelectedItemId { get; set; }

        public ArchiveItem SelectedArchiveItem =>
            ArchiveViewModel.SelectedArchive.Items.First(e => e.Id == SelectedItemId);
    }
}