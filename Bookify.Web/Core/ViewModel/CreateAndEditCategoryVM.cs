namespace Bookify.Web.Core.ViewModel
{
    public class CreateAndEditCategoryVM
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage ="Max Length Is 100 Charcter"),MinLength(3,ErrorMessage ="Enter at least 3 charcters")]
        public string Name { get; set; } = null!;
    }
}
