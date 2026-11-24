using ChudoObuv_Project.Models;

namespace ChudoObuv_Project
{
    public static class Db
    {
        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
    }
}
