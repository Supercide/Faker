namespace Faker.Core {
    public sealed class Property
    {
        private Property(int index)
        {
            IsNumber = true;
            Index = index;
        }

        private Property(string property)
        {
            IsNumber = false;
            Path = property;
        }

        public string Path { get; }

        public int Index { get; }

        public bool IsNumber { get; }

        public static implicit operator Property(string property)
        {
            var isANumber = int.TryParse(property, out var index);

            return isANumber ? new Property(index) : new Property(property);
        }

        public static implicit operator string(Property property)
        {
            return property.IsNumber
                       ? $"{property.Index}"
                       : property.Path;
        }
    }
}