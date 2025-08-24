using Microsoft.Maui.Controls.Shapes;

namespace GCMS.MathHouse.UI.Common
{
    public class FreeMathLayoutManager
    {
        private readonly ResponsiveLayoutService _layoutService;

        public FreeMathLayoutManager(ResponsiveLayoutService layoutService)
        {
            _layoutService = layoutService;
        }

        public async Task InitializeAsync()
        {
            await _layoutService.InitializeAsync();
        }

        public void AdaptLayout(
            Image storyImage,
            Label narrativeLabel,
            Image characterImage,
            Label characterNameLabel,
            Button continueButton,
            Grid mainGrid)
        {
            //AdaptStoryImage(storyImage);
            AdaptNarrativeText(narrativeLabel);
            AdaptCharacterElements(characterImage, characterNameLabel);
            
            // Only adapt continue button if it exists (for backward compatibility)
            if (continueButton != null)
            {
                AdaptContinueButton(continueButton);
            }
        }

        private void AdaptStoryImage(Image storyImage)
        {
            storyImage.MaximumHeightRequest = _layoutService.GetFreeMathImageMaxHeight();
            storyImage.Aspect = Aspect.AspectFit;
        }

        private void AdaptNarrativeText(Label narrativeLabel)
        {
            narrativeLabel.FontSize = _layoutService.GetFontSize("FreeMathNarrative");
        }

        private void AdaptCharacterElements(Image characterImage, Label characterNameLabel)
        {
            var characterSize = _layoutService.GetCharacterImageSize();
            var characterMargin = _layoutService.GetCharacterImageMargin();

            // Make character image bigger than the responsive layout default (110px instead of original size)
            var imageSize = Math.Max(110, characterSize);
            
            characterImage.WidthRequest = imageSize;
            characterImage.HeightRequest = imageSize;
            characterImage.Margin = characterMargin;
            
            // Apply circular clipping to character image with proper sizing
            characterImage.Clip = new EllipseGeometry
            {
                Center = new Point(imageSize / 2, imageSize / 2),
                RadiusX = imageSize / 2,
                RadiusY = imageSize / 2
            };

            characterNameLabel.FontSize = _layoutService.GetFontSize("CharacterName");
        }

        private void AdaptContinueButton(Button continueButton)
        {
            continueButton.FontSize = _layoutService.GetFontSize("ContinueButton");
        }
    }
}