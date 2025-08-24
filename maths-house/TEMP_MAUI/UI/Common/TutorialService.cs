using GCMS.MathHouse.BL;

namespace GCMS.MathHouse.UI.Common
{
    public class TutorialService
    {
        private const string CHARACTER_INTERACTION_TUTORIAL_KEY = "CharacterInteractionTutorialShown";

        public static bool ShouldShowCharacterInteractionTutorial()
        {
            return !PreferencesManager.IsCharacterInteractionTutorialShown();
        }

        public static void MarkCharacterInteractionTutorialAsShown()
        {
            PreferencesManager.SetCharacterInteractionTutorialShown(true);
        }

        public static void ResetAllTutorials()
        {
            PreferencesManager.RemoveCharacterInteractionTutorial();
        }

        /// <summary>
        /// Reset just the character interaction tutorial (useful for testing)
        /// </summary>
        public static void ResetCharacterInteractionTutorial()
        {
            PreferencesManager.RemoveCharacterInteractionTutorial();
        }

        /// <summary>
        /// Check if any tutorial has been shown (for analytics or UI purposes)
        /// </summary>
        public static bool HasAnyTutorialBeenShown()
        {
            return PreferencesManager.IsCharacterInteractionTutorialShown();
        }
    }
}