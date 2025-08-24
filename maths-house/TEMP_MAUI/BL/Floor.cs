
namespace GCMS.MathHouse.BL
{
    public class Floor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public FloorStatus Status { get; set; }
        public QuestAnimal Animal { get; set; }
        public MathOperationType OperationType { get; set; }
        public int QuestionsCount { get; set; }
        public List<string> NextFloorIds { get; set; } = new();
        public Point Position { get; set; }
    }
}