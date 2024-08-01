namespace Company.Function
{
    public class ToDoListItemDocument
    {
        public string Id { get; set; }

        public string PartitionKey { get; set; }

        public string Description { get; set; }

        public int Number { get; set; }

        public bool Boolean { get; set; }
    }
}