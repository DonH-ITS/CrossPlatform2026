namespace Week2Animal
{
    internal class Dog : Animal
    {
        private string _breed;

        public string Breed
        {
            get { return _breed; }
            set { _breed = value; }
        }

        public Dog(string name, int age, string breed) : base(name, age) {
            _breed = breed;
        }

        public override void Sound() {
            Console.WriteLine("Woof!");
        }

        public override string ToString() {
            return $"{Name} the dog is {Age} years old";
        }
    }
}
