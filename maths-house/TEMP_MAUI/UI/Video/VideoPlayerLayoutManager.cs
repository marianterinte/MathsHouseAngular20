namespace GCMS.MathHouse.UI.Video
{
    public class VideoPlayerLayoutManager
    {
        private readonly ResponsiveLayoutService _layoutService;

        public VideoPlayerLayoutManager(ResponsiveLayoutService layoutService)
        {
            _layoutService = layoutService;
        }

        public void AdaptLayout(ImageButton closeButton)
        {
            //if (closeButton != null)
            //{
            //    closeButton.FontSize = _layoutService.GetFontSize("CloseButton");
            //}
        }
    }
}