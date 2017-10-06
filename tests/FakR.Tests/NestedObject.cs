namespace FakR.Tests {
    public class NestedObject
    {
        public string String { get; set; }
        public int Number { get; set; }
        public bool Bool { get; set; }
        public NestedObject Nested { get; set; }

        public static NestedObject Create()
        {
            return new NestedObject
            {
                Bool = true,
                Number = 1337,
                String = "Elite",
                Nested = new NestedObject
                {
                    String = "Dam!",
                    Bool = false,
                    Number = 1881
                }
            };
        }
    }
}