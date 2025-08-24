using System.Text.Json;
using GCMS.MathHouse.Localization;

namespace GCMS.MathHouse.BL;

public class FreeMathGameFlowController
{
    public List<FreeMathStep> AllSteps { get; set; }
    public int CurrentStepIndex { get; private set; }

    public FreeMathGameFlowController()
    {
        InitializeSteps();
        LoadProgress();

        var ingredientCount = GetCollectedIngredientsCount();
    }

    // Add method to refresh steps when language changes
    public void RefreshStepsWithCurrentLocalization()
    {
        InitializeSteps();
    }

    public FreeMathStep GetCurrentStep() => AllSteps[CurrentStepIndex];

    public void AdvanceToNextStep()
    {
        var next = AllSteps[CurrentStepIndex].NextStepIndex;
        if (next.HasValue && next.Value < AllSteps.Count)
        {
            CurrentStepIndex = next.Value;
            SaveProgress();
        }
    }

    public void LoadProgress()
    {
        CurrentStepIndex = PreferencesManager.GetFreeMathProgress(0);
    }

    public void SaveProgress()
    {
        PreferencesManager.SetFreeMathProgress(CurrentStepIndex);
    }

    public void ResetProgress()
    {
        CurrentStepIndex = 0;
        SaveProgress();
        ClearCollectedIngredients();
    }

    public void CollectIngredient(IngredientType ingredient)
    {
        var collectedIngredients = GetCollectedIngredients();

        if (!collectedIngredients.Contains(ingredient))
        {
            collectedIngredients.Add(ingredient);
            SaveCollectedIngredients(collectedIngredients);

            var verifyCount = GetCollectedIngredientsCount();
        }
    }

    public List<IngredientType> GetCollectedIngredients()
    {
        return PreferencesManager.GetCollectedIngredients();
    }

    public bool HasIngredient(IngredientType ingredient)
    {
        return GetCollectedIngredients().Contains(ingredient);
    }

    public int GetCollectedIngredientsCount()
    {
        return GetCollectedIngredients().Count;
    }

    public List<string> GetCollectedIngredientsDisplay()
    {
        var collectedIngredients = GetCollectedIngredients();
        var displayList = new List<string>();

        foreach (var ingredient in collectedIngredients)
        {
            var displayName = ingredient switch
            {
                IngredientType.CarrotDust => LocalizationService.Translate("ingredient_carrot_dust"),
                IngredientType.MouseTail => LocalizationService.Translate("ingredient_mouse_tail"),
                IngredientType.FishBones => LocalizationService.Translate("ingredient_fish_bones"),
                IngredientType.MilletSeeds => LocalizationService.Translate("ingredient_millet_seeds"),
                IngredientType.BambooElixir => LocalizationService.Translate("ingredient_bamboo_elixir"),
                IngredientType.MagicWand => LocalizationService.Translate("ingredient_magic_wand"),
                IngredientType.PufPufPowder => LocalizationService.Translate("ingredient_puf_puf_powder"),
                IngredientType.WizardHat => LocalizationService.Translate("ingredient_wizard_hat"),
                _ => ingredient.ToString()
            };
            displayList.Add(displayName);
        }

        return displayList;
    }

    public string GetProgressMapImageName()
    {
        var ingredientCount = GetCollectedIngredientsCount();
        return ingredientCount switch
        {
            >= 7 => "freemath_quest7_completed.png",
            >= 6 => "freemath_quest6_completed.png",
            >= 5 => "freemath_quest5_completed.png",
            >= 4 => "freemath_quest4_completed.png",
            >= 3 => "freemath_quest3_completed.png",
            >= 2 => "freemath_quest2_completed.png",
            >= 1 => "freemath_quest1_completed.png",
            _ => "freemath_home.png"
        };
    }

    public void ClearCollectedIngredients()
    {
        PreferencesManager.ClearFreeMathCollectedIngredients();
    }

    private void SaveCollectedIngredients(List<IngredientType> ingredients)
    {
        try
        {
            var stringIngredients = ingredients.ConvertAll(i => i.ToString());
            var jsonString = JsonSerializer.Serialize(stringIngredients);
            PreferencesManager.SetFreeMathCollectedIngredients(jsonString);

            var verifyJson = PreferencesManager.GetFreeMathCollectedIngredients("[]");
        }
        catch (Exception ex)
        {
        }
    }

    // Helper method to get animal name from localization
    private static string GetAnimalName(string animalKey)
    {
        return LocalizationService.Translate(animalKey);
    }

    private void InitializeSteps()
    {
        AllSteps = new List<FreeMathStep>
        {
            // Step 0: Introduction
            new FreeMathStep
            {
                ImageUrl = "freemath_home.png",
                NarrativeText = LocalizationService.Translate("freemath_step0_narrative"),
                CharacterImageUrl = "puf_wondering.png",
                NextStepIndex = 1
            },

            // Step 1: Bunny – Carrot Dust
            new FreeMathStep
            {
                ImageUrl = "freemath_home.png",
                NarrativeText = string.Format(LocalizationService.Translate("freemath_step1_narrative"), GetAnimalName("animal_names_bunny")),
                CharacterImageUrl = "bunny.png",
                Problem = new MathProblem
                {
                    Statement = string.Format(LocalizationService.Translate("freemath_step1_problem"), GetAnimalName("animal_names_bunny")),
                    CorrectAnswer = 7,
                    ImageUrl = "bunny.png",
                    Ingredient = IngredientType.CarrotDust
                },
                RewardIngredient = IngredientType.CarrotDust,
                NextStepIndex = 2
            },

            // Step 2: Bunny celebration
            new FreeMathStep
            {
                ImageUrl = "freemath_quest1_completed.png",
                NarrativeText = string.Format(LocalizationService.Translate("freemath_step2_narrative"), GetAnimalName("animal_names_bunny")),
                CharacterImageUrl = "bunny.png",
                NextStepIndex = 3
            },

            // Step 3: Cat – Mouse Tail
            new FreeMathStep
            {
                ImageUrl = "freemath_quest1_completed.png",
                NarrativeText = string.Format(LocalizationService.Translate("freemath_step3_narrative"), GetAnimalName("animal_names_cat")),
                CharacterImageUrl = "cat.png",
                Problem = new MathProblem
                {
                    Statement = string.Format(LocalizationService.Translate("freemath_step3_problem"), GetAnimalName("animal_names_cat")),
                    CorrectAnswer = 7,
                    ImageUrl = "cat.png",
                    Ingredient = IngredientType.MouseTail
                },
                RewardIngredient = IngredientType.MouseTail,
                NextStepIndex = 4
            },
             
            // Step 4: Parrot – Millet Seeds
            new FreeMathStep
            {
                ImageUrl = "freemath_quest2_completed.png",
                NarrativeText = string.Format(LocalizationService.Translate("freemath_step5_narrative"), GetAnimalName("animal_names_parrot")),
                CharacterImageUrl = "parrot.png",
                Problem = new MathProblem
                {
                    Statement = string.Format(LocalizationService.Translate("freemath_step5_problem"), GetAnimalName("animal_names_parrot")),
                    CorrectAnswer = 6,
                    ImageUrl = "parrot.png",
                    Ingredient = IngredientType.MilletSeeds
                },
                RewardIngredient = IngredientType.MilletSeeds,
                NextStepIndex = 5
            },

            // Step 5: Panda – Bamboo Elixir
            new FreeMathStep
            {
                ImageUrl = "freemath_quest3_completed.png",
                NarrativeText = string.Format(LocalizationService.Translate("freemath_step6_narrative"), GetAnimalName("animal_names_panda")),
                CharacterImageUrl = "panda.png",
                Problem = new MathProblem
                {
                    Statement = string.Format(LocalizationService.Translate("freemath_step6_problem"), GetAnimalName("animal_names_panda")),
                    CorrectAnswer = 8,
                    ImageUrl = "panda.png",
                    Ingredient = IngredientType.BambooElixir
                },
                RewardIngredient = IngredientType.BambooElixir,
                NextStepIndex = 6
            },

            // Step 6: Koala – Magic Wand
            new FreeMathStep
            {
                ImageUrl = "freemath_quest4_completed.png",
                NarrativeText = string.Format(LocalizationService.Translate("freemath_step7_narrative"), GetAnimalName("animal_names_koala")),
                CharacterImageUrl = "koala.png",
                Problem = new MathProblem
                {
                    Statement = LocalizationService.Translate("freemath_step7_problem"),
                    CorrectAnswer = 9,
                    ImageUrl = "koala.png",
                    Ingredient = IngredientType.MagicWand
                },
                RewardIngredient = IngredientType.MagicWand,
                NextStepIndex = 7
            },

            // Step 7: Owl – Wizard's Hat
            new FreeMathStep
            {
                ImageUrl = "freemath_quest5_completed.png",
                NarrativeText = string.Format(LocalizationService.Translate("freemath_step8_narrative"), GetAnimalName("animal_names_owl")),
                CharacterImageUrl = "owl.png",
                Problem = new MathProblem
                {
                    Statement = LocalizationService.Translate("freemath_step8_problem"),
                    CorrectAnswer = 13,
                    ImageUrl = "owl.png",
                    Ingredient = IngredientType.WizardHat
                },
                RewardIngredient = IngredientType.WizardHat,
                NextStepIndex = 8
            },

            // Step 8: Puf-Puf – Puf-Puf Powder (FINAL INGREDIENT)
            new FreeMathStep
            {
                ImageUrl = "freemath_quest6_completed.png",
                NarrativeText = LocalizationService.Translate("freemath_step9_narrative"),
                CharacterImageUrl = "puf_wondering.png",
                Problem = new MathProblem
                {
                    Statement = LocalizationService.Translate("freemath_step9_problem"),
                    CorrectAnswer = 12,
                    ImageUrl = "pufpuf.png",
                    Ingredient = IngredientType.PufPufPowder
                },
                RewardIngredient = IngredientType.PufPufPowder,
                NextStepIndex = 9
            },

            // Step 9: Final celebration
            new FreeMathStep
            {
                ImageUrl = "freemath_quest7_completed.png",
                NarrativeText = LocalizationService.Translate("freemath_step10_narrative"),
                CharacterImageUrl = "puf_succeded.png",
                NextStepIndex = null
            }
        };
    }
}