namespace GCMS.MathHouse.BL;

public class MathProblem
{
    public string Statement { get; set; }            // The math question
    public int CorrectAnswer { get; set; }           // Expected result
    public string ImageUrl { get; set; }             // Illustration for the problem
    public IngredientType Ingredient { get; set; }   // Enum representing the collected ingredient
}