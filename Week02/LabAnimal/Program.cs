namespace Week2Animal
{
    internal class Program
    {
        static void Main(string[] args) {
            Dog dog1 = new Dog("Rex", 5, "German Shepherd");
            Dog dog2 = new Dog("Buddy", 3, "Labrador");

            Cat cat1 = new Cat("Mittens", 4, "Black");
            Cat cat2 = new Cat("Luna", 2, "White");

            // Properties
            dog1.Age = 6;
            dog1.Age = -4; // This change will not happen as the setter will not allow it

            Console.WriteLine(dog1.Name);
            Console.WriteLine(dog1.Age);
            Console.WriteLine(dog1.Breed);

            Console.WriteLine(cat1.Name);
            Console.WriteLine(cat1.Age);
            Console.WriteLine(cat1.Colour);

            // Sounds
            dog1.Sound();
            dog2.Sound();
            cat1.Sound();
            cat2.Sound();

            // ToString()
            Console.WriteLine(dog1);
            Console.WriteLine(cat1);

            // Polymorphism
            Animal[] animals = { dog1, cat1, dog2, cat2 };

            foreach (Animal animal in animals) {
                animal.Sound();
            }

            Animal[] moreAnimals = new Animal[4];
            moreAnimals[0] = new Dog("Max", 4, "Bulldog");
            moreAnimals[1] = new Cat("Whiskers", 3, "Gray");
            moreAnimals[2] = new Dog("Charlie", 2, "Beagle");
            moreAnimals[3] = new Cat("Simba", 1, "Orange");
        }
    }
}
