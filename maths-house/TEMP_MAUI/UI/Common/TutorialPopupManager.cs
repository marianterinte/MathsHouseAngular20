using GCMS.MathHouse.Localization;
using Microsoft.Maui.Controls.Shapes;

namespace GCMS.MathHouse.UI.Common
{
    public class TutorialPopupManager
    {
        public async Task ShowCharacterInteractionTutorial()
        {
            var tcs = new TaskCompletionSource<bool>();
            var popup = CreateCharacterInteractionTutorialPopup(tcs);
            var container = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#80000000"), // Semi-transparent dark background
                Content = popup
            };

            await Application.Current.MainPage.Navigation.PushModalAsync(container);
            
            // Wait for the user to close the popup
            await tcs.Task;
        }

        public async Task<bool> ShowGameCompletionPopup()
        {
            var tcs = new TaskCompletionSource<bool>();
            var popup = CreateGameCompletionPopup(tcs);
            var container = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#80000000"), // Semi-transparent dark background
                Content = popup
            };

            await Application.Current.MainPage.Navigation.PushModalAsync(container);
            
            // Wait for the user to close the popup and return their choice
            return await tcs.Task;
        }

        private Grid CreateGameCompletionPopup(TaskCompletionSource<bool> completionSource)
        {
            var mainGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            // Completion content container
            var contentFrame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 20,
                HasShadow = true,
                Padding = 30,
                Margin = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var contentGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }, // Title
                    new RowDefinition { Height = GridLength.Auto }, // Success icon
                    new RowDefinition { Height = GridLength.Auto }, // Message
                    new RowDefinition { Height = GridLength.Auto }  // Button
                },
                RowSpacing = 20
            };

            // Title
            var titleLabel = new Label
            {
                Text = LocalizationService.Translate("free_math_quest_complete_title") ?? "🎉 Quest Complete!",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2E7D32"),
                HorizontalTextAlignment = TextAlignment.Center
            };

            // Success icon/image
            var successImage = new Image
            {
                Source = "puf_succeded.png",
                WidthRequest = 80,
                HeightRequest = 80,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // Message
            var messageLabel = new Label
            {
                Text = LocalizationService.Translate("free_math_quest_complete_message") ?? 
                       "Congratulations! You have successfully completed the wizard rescue quest and saved the magical world!",
                FontSize = 16,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap,
                MaxLines = 5
            };

            // Button
            var completeButton = new Button
            {
                Text = LocalizationService.Translate("free_math_quest_complete_button") ?? "Complete Game 🏆",
                BackgroundColor = Color.FromArgb("#4CAF50"),
                TextColor = Colors.White,
                CornerRadius = 15,
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
                Padding = new Thickness(30, 15)
            };

            // Button event handler - user decides when to complete the game
            completeButton.Clicked += async (sender, e) =>
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
                completionSource.SetResult(true); // Signal that user wants to complete the game
            };

            // Add all elements to content grid
            contentGrid.Children.Add(titleLabel);
            contentGrid.Children.Add(successImage);
            contentGrid.Children.Add(messageLabel);
            contentGrid.Children.Add(completeButton);

            Grid.SetRow(titleLabel, 0);
            Grid.SetRow(successImage, 1);
            Grid.SetRow(messageLabel, 2);
            Grid.SetRow(completeButton, 3);

            contentFrame.Content = contentGrid;
            mainGrid.Children.Add(contentFrame);
            Grid.SetRow(contentFrame, 1);

            return mainGrid;
        }

        private Grid CreateCharacterInteractionTutorialPopup(TaskCompletionSource<bool> completionSource)
        {
            var mainGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            // Tutorial content container
            var contentFrame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 20,
                HasShadow = true,
                Padding = 20,
                Margin = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var contentGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }, // Title
                    new RowDefinition { Height = GridLength.Auto }, // Character example
                    new RowDefinition { Height = GridLength.Auto }, // Arrow
                    new RowDefinition { Height = GridLength.Auto }, // Instructions
                    new RowDefinition { Height = GridLength.Auto }, // Checkbox area
                    new RowDefinition { Height = GridLength.Auto }  // Buttons
                },
                RowSpacing = 15
            };

            // Title
            var titleLabel = new Label
            {
                Text = LocalizationService.Translate("tutorial_character_interaction_title") ?? "⬆️ How to Continue the Story",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2E7D32"),
                HorizontalTextAlignment = TextAlignment.Center
            };

            // Character example with circle
            var characterExampleGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var exampleCircle = new Ellipse
            {
                Fill = Colors.Transparent,
                Stroke = Color.FromArgb("#1E90FF"),
                StrokeThickness = 8,
                Opacity = 0.8,
                WidthRequest = 80,
                HeightRequest = 80,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var exampleImage = new Image
            {
                Source = "puf_wondering.png",
                WidthRequest = 70,
                HeightRequest = 70,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFill
            };
            exampleImage.Clip = new EllipseGeometry(new Point(35, 35), 35, 35);

            characterExampleGrid.Children.Add(exampleCircle);
            characterExampleGrid.Children.Add(exampleImage);

            // Animated arrow pointing down at the character
            var arrow = new Label
            {
                Text = "⬆️",
                FontSize = 30,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            // Start arrow animation
            StartArrowAnimation(arrow);

            // Instructions
            var instructionsLabel = new Label
            {
                Text = LocalizationService.Translate("tutorial_character_interaction_instructions") ?? 
                       "When the character finishes speaking and the blue circle appears, tap on the character image to continue the story!",
                FontSize = 16,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap,
                MaxLines = 4
            };

            // Checkbox area
            var checkboxGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                ColumnSpacing = 10,
                HorizontalOptions = LayoutOptions.Center
            };

            var checkbox = new CheckBox
            {
                VerticalOptions = LayoutOptions.Center
            };

            var checkboxLabel = new Label
            {
                Text = LocalizationService.Translate("tutorial_dont_show_again") ?? "Don't show this tutorial again",
                FontSize = 14,
                TextColor = Colors.Gray,
                VerticalOptions = LayoutOptions.Center
            };

            checkboxGrid.Children.Add(checkbox);
            checkboxGrid.Children.Add(checkboxLabel);
            Grid.SetColumn(checkbox, 0);
            Grid.SetColumn(checkboxLabel, 1);

            // Buttons
            var buttonGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                ColumnSpacing = 10
            };

            var tryItButton = new Button
            {
                Text = LocalizationService.Translate("tutorial_got_it_button") ?? "Got it! ??",
                BackgroundColor = Color.FromArgb("#4CAF50"),
                TextColor = Colors.White,
                CornerRadius = 10,
                FontAttributes = FontAttributes.Bold
            };

            var laterButton = new Button
            {
                Text = LocalizationService.Translate("tutorial_remind_later_button") ?? "Remind me later",
                BackgroundColor = Color.FromArgb("#E0E0E0"),
                TextColor = Colors.Black,
                CornerRadius = 10
            };

            // Button event handlers
            tryItButton.Clicked += async (sender, e) =>
            {
                if (checkbox.IsChecked)
                {
                    TutorialService.MarkCharacterInteractionTutorialAsShown();
                }
                await Application.Current.MainPage.Navigation.PopModalAsync();
                completionSource.SetResult(true); // Signal that tutorial is closed
            };

            laterButton.Clicked += async (sender, e) =>
            {
                // Don't mark as shown, will appear next time
                await Application.Current.MainPage.Navigation.PopModalAsync();
                completionSource.SetResult(false); // Signal that tutorial is closed
            };

            buttonGrid.Children.Add(laterButton);
            buttonGrid.Children.Add(tryItButton);
            Grid.SetColumn(laterButton, 0);
            Grid.SetColumn(tryItButton, 1);

            // Add all elements to content grid
            contentGrid.Children.Add(titleLabel);
            contentGrid.Children.Add(characterExampleGrid);
            contentGrid.Children.Add(arrow);
            contentGrid.Children.Add(instructionsLabel);
            contentGrid.Children.Add(checkboxGrid);
            contentGrid.Children.Add(buttonGrid);

            Grid.SetRow(titleLabel, 0);
            Grid.SetRow(characterExampleGrid, 1);
            Grid.SetRow(arrow, 2);
            Grid.SetRow(instructionsLabel, 3);
            Grid.SetRow(checkboxGrid, 4);
            Grid.SetRow(buttonGrid, 5);

            contentFrame.Content = contentGrid;
            mainGrid.Children.Add(contentFrame);
            Grid.SetRow(contentFrame, 1);

            return mainGrid;
        }

        private async void StartArrowAnimation(Label arrow)
        {
            // Continuous bouncing animation
            while (arrow != null)
            {
                try
                {
                    await arrow.TranslateTo(0, -10, 500, Easing.SinInOut);
                    await arrow.TranslateTo(0, 0, 500, Easing.SinInOut);
                    await Task.Delay(200);
                }
                catch
                {
                    // Animation stopped (probably because popup was closed)
                    break;
                }
            }
        }
    }
}