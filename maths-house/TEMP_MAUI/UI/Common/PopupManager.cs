using GCMS.MathHouse.Localization;

namespace GCMS.MathHouse.UI.Common
{
    public class PopupManager
    {
        public async Task ShowFinalPopup()
        {
            var popup = CreateFinalPopup();
            var container = new ContentPage
            {
                BackgroundColor = Colors.Transparent,
                Content = new Frame
                {
                    BackgroundColor = Colors.White,
                    Padding = 16,
                    CornerRadius = 20,
                    HasShadow = true,
                    Content = popup,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            };

            await Application.Current.MainPage.Navigation.PushModalAsync(container);
        }

        private Grid CreateFinalPopup()
        {
            var popup = new Grid
            {
                BackgroundColor = Color.FromArgb("#CCFFFFFF"),
                Padding = 24,
                WidthRequest = 300,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            var titleLabel = new Label
            {
                Text = LocalizationService.Translate("main_page_popup_title"),
                FontSize = 18,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 0, 16),
                FontAttributes = FontAttributes.Bold
            };

            var aboutButton = new Button
            {
                Text = LocalizationService.Translate("main_page_popup_about"),
                BackgroundColor = Color.FromArgb("#E0E0FF"),
                TextColor = Colors.DarkBlue,
                CornerRadius = 12,
                Margin = 8,
                Command = new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                    await Shell.Current.GoToAsync("//AboutPage");
                })
            };

            var closeButton = new Button
            {
                Text = LocalizationService.Translate("main_page_popup_close"),
                BackgroundColor = Color.FromArgb("#FFE0E0"),
                TextColor = Colors.DarkRed,
                CornerRadius = 12,
                Margin = 8,
                Command = new Command(async () =>
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                })
            };

            popup.Children.Add(titleLabel);
            popup.Children.Add(aboutButton);
            popup.Children.Add(closeButton);

            Grid.SetRow(titleLabel, 0);
            Grid.SetColumnSpan(titleLabel, 2);
            Grid.SetRow(aboutButton, 1);
            Grid.SetColumn(aboutButton, 0);
            Grid.SetRow(closeButton, 1);
            Grid.SetColumn(closeButton, 1);

            return popup;
        }
    }
}
