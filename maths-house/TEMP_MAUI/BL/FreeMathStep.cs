namespace GCMS.MathHouse.BL;

public class FreeMathStep
{
    public string ImageUrl { get; set; }             // Scene background or visual cue
    public string NarrativeText { get; set; }        // Text displayed to the player
    public MathProblem? Problem { get; set; }        // Null if no problem in this step
    public IngredientType? RewardIngredient { get; set; } // Ingredient rewarded for completing this step (if any)
    public int? NextStepIndex { get; set; }          // Index of the next step in the array
    public string CharacterImageUrl { get; set; }    // Character image to display for this step
}