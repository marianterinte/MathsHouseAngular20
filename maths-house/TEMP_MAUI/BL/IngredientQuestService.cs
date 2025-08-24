using System.Collections.Generic;

namespace GCMS.MathHouse.BL
{
    public class IngredientQuestService
    {
        // Represents a quest for an animal/ingredient
        public class IngredientQuest
        {
            public string AnimalId { get; set; }
            public string IngredientId { get; set; }
            public string NarrativeText { get; set; }
            public string AnimalImage { get; set; }
            public List<Question> MathProblems { get; set; }
        }

        // Stores quests for each animal/ingredient
        private readonly Dictionary<string, IngredientQuest> _quests = new();

        public IngredientQuestService()
        {
            // Initialize with sample data (should be loaded from resources or localization)
            _quests["cat"] = new IngredientQuest
            {
                AnimalId = "cat",
                IngredientId = "magic_wool",
                NarrativeText = "Pisica are nevoie s? numere ?oriceii...",
                AnimalImage = "cat.png",
                MathProblems = new List<Question> { /* Add questions here */ }
            };
            // Add other animals/ingredients similarly
        }

        public IngredientQuest GetQuestForAnimal(string animalId)
        {
            return _quests.TryGetValue(animalId, out var quest) ? quest : null;
        }

        public int GetMagicNumberForIngredient(string ingredientId, int playerPerformanceData)
        {
            // Example: magic number could be the sum of correct answers or a fixed value
            // This should be customized per ingredient/quest
            return playerPerformanceData;
        }
    }
}