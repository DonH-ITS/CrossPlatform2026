namespace Week2Animal
{
    internal class Animal
    {
        private string _name;
        private int _age;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Age
        {
            get { return _age; }
            set {
                // Put some check, age cannot be negative
                if(value >=0)
                    _age = value; 
            }
        }

        public Animal(string name, int age) {
            _name = name;
            _age = age;
        }

        public virtual void Sound() {
            Console.WriteLine("This animal makes a sound");
        }

        public override string ToString() {
            return $"{Name} is {Age} years old";
        }
    }
}
