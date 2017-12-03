namespace FunBooksAndVideos.BusinessLogic.Models
{
    public abstract class PhysicalProduct : Product
    {
        public string Title { get; }

        protected PhysicalProduct(string title)
        {
            Title = title;
        }
    }
}