using Microsoft.Maui.Controls.Shapes;

namespace GCMS.MathHouse.UI.Common
{
    public class LayoutManager
    {
        private readonly ResponsiveLayoutService _layoutService;

        public LayoutManager(ResponsiveLayoutService layoutService)
        {
            _layoutService = layoutService;
        }

        public async Task InitializeAsync()
        {
            await _layoutService.InitializeAsync();
        }

        public void AdaptLayout(
            Image[] animalImages,
            ImageButton[] animalButtons,
            ImageButton topFloorButton,
            Grid houseGrid,
            Label gameMessageLabel,
            Image houseImage)
        {
            var size = _layoutService.GetAnimalSize();

            AdaptAnimalImages(animalImages, size);
            AdaptAnimalButtons(animalButtons, size);
            AdaptTopFloorButton(topFloorButton, size);
            AdaptHouseGrid(houseGrid);
            AdaptOtherElements(gameMessageLabel, houseImage);
        }

        private void AdaptAnimalImages(Image[] animalImages, double size)
        {
            foreach (var image in animalImages)
            {
                image.WidthRequest = size;
                image.HeightRequest = size;
                image.Margin = _layoutService.GetAnimalMargin(image.ClassId);
                image.Clip = new RoundRectangleGeometry
                {
                    CornerRadius = new CornerRadius(size * 0.25),
                    Rect = new Rect(0, 0, size, size)
                };
            }
        }

        private void AdaptAnimalButtons(ImageButton[] animalButtons, double size)
        {
            foreach (var button in animalButtons)
            {
                button.WidthRequest = size;
                button.HeightRequest = size;
                button.CornerRadius = (int)(size * 0.50);
                button.Clip = new RoundRectangleGeometry
                {
                    CornerRadius = new CornerRadius(size * 0.50),
                    Rect = new Rect(0, 0, size, size)
                };
            }
        }

        private void AdaptTopFloorButton(ImageButton topFloorButton, double size)
        {
            var marginTopFloor = _layoutService.GetAnimalMargin("TopFloor");
            topFloorButton.WidthRequest = size;
            topFloorButton.HeightRequest = size;
            topFloorButton.Margin = marginTopFloor;
        }

        private void AdaptHouseGrid(Grid houseGrid)
        {
            houseGrid.RowDefinitions.Clear();

            for (int i = 0; i < 5; i++)
            {
                houseGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = _layoutService.GetRowHeight(i)
                });
            }
        }

        private void AdaptOtherElements(Label gameMessageLabel, Image houseImage)
        {
            gameMessageLabel.FontSize = _layoutService.GetFontSize("GameMessage");
            houseImage.MaximumHeightRequest = _layoutService.GetHouseImageMaxHeight();
        }
    }
}
