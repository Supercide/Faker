namespace FakR.Tests {
    public class NestedObject
    {
        public string String { get; set; }
        public string[] Array { get; set; }
        public int Number { get; set; }
        public bool Bool { get; set; }
        public NestedObject Nested { get; set; }

        public static NestedObject Create()
        {
            return new NestedObject
            {
                Bool = true,
                Number = 1337,
                Array = new[] { "1", "2", "3" },
                String = "Elite",
                Nested = new NestedObject
                {
                    String = "Dam!",
                    Bool = false,
                    Array = new[] { "4", "5", "6" },
                    Number = 1881
                }
            };
        }
    }
}