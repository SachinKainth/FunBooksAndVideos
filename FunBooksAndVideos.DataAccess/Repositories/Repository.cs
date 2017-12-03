namespace FunBooksAndVideos.DataAccess.Repositories
{
    public abstract class Repository<T>
    {
        public abstract T Get(long id);
    }
}