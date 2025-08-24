namespace GCMS.MathHouse.BL
{
    public class GameProgress
    {
        public Dictionary<string, FloorStatus> FloorStates { get; set; } = new()
        {
            { "GroundFloor", FloorStatus.Unlocked },
            { "FirstFloorLeft", FloorStatus.Locked },
            { "FirstFloorRight", FloorStatus.Locked },
            { "SecondFloorLeft", FloorStatus.Locked },
            { "SecondFloorRight", FloorStatus.Locked },
            { "ThirdFloorLeft", FloorStatus.Locked },
            { "ThirdFloorRight", FloorStatus.Locked },
            { "TopFloor", FloorStatus.Locked }
        };

        public Dictionary<string, string> ResolvedAnimalImages { get; set; } = new()
        {
            { "GroundFloor", "bunny.png" },
            { "FirstFloorLeft", "cat.png" },
            { "FirstFloorRight", "dog.png" },
            { "SecondFloorLeft", "parrot.png" },
            { "SecondFloorRight", "koala.png" },
            { "ThirdFloorLeft", "parrot.png" },
            { "ThirdFloorRight", "panda.png" },
            { "TopFloor", "owl.png" }
        };
    }
}
