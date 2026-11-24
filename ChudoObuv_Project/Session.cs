using ChudoObuv_Project.Models;

namespace ChudoObuv_Project
{
    public static class Session
    {
        public static User? CurrentUser { get; set; }

        public static bool IsAuthorized
        {
            get { return CurrentUser != null; }
        }

        public static bool IsAdmin
        {
            get { return CurrentUser?.IdRoleNavigation.RoleName == "Администратор"; }
        }

        public static bool IsManagerOrAdmin
        {
            get
            {
                string? role = CurrentUser?.IdRoleNavigation.RoleName;
                return role == "Администратор" || role == "Менеджер";
            }
        }

        public static string FullName
        {
            get
            {
                if (CurrentUser == null)
                {
                    return "Гость";
                }

                return $"{CurrentUser.LastName} {CurrentUser.FirstName} {CurrentUser.MiddleName}".Trim();
            }
        }
    }
}
