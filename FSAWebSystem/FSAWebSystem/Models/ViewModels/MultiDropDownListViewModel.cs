using Microsoft.AspNetCore.Mvc.Rendering;

namespace FSAWebSystem.Models.ViewModels
{
    public class MultiDropDownListViewModel
    {
        public MultiDropDownListViewModel()
        {
            ItemList = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> ItemList { get; set; }
    }
}
