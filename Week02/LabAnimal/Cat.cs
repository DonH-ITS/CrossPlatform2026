using System;
using System.Collections.Generic;
using System.Text;

namespace Week2Animal
{
    internal class Cat : Animal
    {
        private string _colour;

        public string Colour
        {
            get { return _colour; }
            set { _colour = value; }
        }

        public Cat(string name, int age, string colour): base(name, age) {
            _colour = colour;
        }

        public override void Sound() {
            Console.WriteLine("Meow!");
        }
    }
}
