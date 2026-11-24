using System.Windows;

namespace ChudoObuv_Project
{
    public static class Messages
    {
        public static void Error(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void Warning(string text)
        {
            MessageBox.Show(text, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void Info(string text)
        {
            MessageBox.Show(text, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool Confirm(string text)
        {
            var result = MessageBox.Show(text, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
