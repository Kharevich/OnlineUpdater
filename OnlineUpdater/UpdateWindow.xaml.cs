using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlineUpdater
{
    /// <summary>
    /// Логика взаимодействия для UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
            
            Title = Updater.UpdateDialogTitle;
            textBlock.Text = string.Empty;
            Bold startstring = new Bold(new Run(string.Format("Доступна новая версия программы {0}!", Updater.AppTitle)));
            startstring.FontSize = 15;
            string middlestring = string.Format("Доступна версия {0}. У Вас установлена версия {1}. Хотите установить обновление прямо сейчас?", Updater.LatestVersion, Updater.InstalledVersion);
            Bold endstring = new Bold(new Run("Что нового:"));
            endstring.FontSize = 13;

            textBlock.Inlines.Add(startstring);
            textBlock.Inlines.Add(new LineBreak());
            textBlock.Inlines.Add(new LineBreak());
            textBlock.Inlines.Add(middlestring);
            textBlock.Inlines.Add(new LineBreak());
            textBlock.Inlines.Add(new LineBreak());
            textBlock.Inlines.Add(endstring);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Navigate(Updater.ChangeLogURL);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Updater.DownloadUpdate();
            this.Close();
        }

    }
}
