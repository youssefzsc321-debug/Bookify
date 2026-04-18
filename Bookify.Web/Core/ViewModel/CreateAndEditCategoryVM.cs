namespace Bookify.Web.Core.ViewModel
{
    public class CreateAndEditCategoryVM
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage ="Max Length Is 100 Charcter")]
        public string Name { get; set; } = null!;
    }
}
