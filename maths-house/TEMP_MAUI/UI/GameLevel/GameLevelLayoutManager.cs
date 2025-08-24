

namespace GCMS.MathHouse.UI.GameLevel;

public class GameLevelLayoutManager
{
    private readonly ResponsiveLayoutService _layoutService;

    public GameLevelLayoutManager(ResponsiveLayoutService layoutService)
    {
        _layoutService = layoutService;
    }

    public void AdaptLayout(Label gameMessageLabel, Label feedbackLabel, Label scoreSummaryLabel)
    {
        var size = _layoutService.GetAnimalSize();

        gameMessageLabel.FontSize = _layoutService.GetFontSize("GameMessage");
        feedbackLabel.FontSize = _layoutService.GetFontSize("Feedback");
        scoreSummaryLabel.FontSize = _layoutService.GetFontSize("ScoreSummary");
    }
}
