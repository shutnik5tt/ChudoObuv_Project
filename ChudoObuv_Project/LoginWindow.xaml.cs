using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.Any(char.IsWhiteSpace);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();

            if (login == "")
            {
                Messages.Warning("Введите логин. Если логина нет, нажмите «Войти как гость».");
                return;
            }

            try
            {
                using var db = Db.Create();
                var user = db.Users
                    .Include(u => u.IdRoleNavigation)
                    .FirstOrDefault(u => u.Login == login);

                if (user == null)
                {
                    Messages.Warning("Пользователь с таким логином не найден. Проверьте логин и попробуйте ещё раз.");
                    return;
                }

                OpenCatalog(user);
            }
            catch (Exception ex)
            {
                Messages.Error("Не удалось выполнить вход. Проверьте подключение к базе данных.\n\n" + ex.Message);
            }
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            OpenCatalog(null);
        }

        private void OpenCatalog(Models.User? user)
        {
            Session.CurrentUser = user;
            OrderDraft.Lines.Clear();
            new MainWindow().Show();
            Close();
        }
    }
}
