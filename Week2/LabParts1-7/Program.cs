namespace Week2LabParts1_7
{
    internal class Program
    {
        static void Main(string[] args) {
            Console.WriteLine("Hello, World!");
            // Part 3
            int a = 10, b = 20;
            Console.WriteLine(a + " + " + b + " = " + (a + b));
            // OR
            Console.WriteLine($"{a} + {b} = {a + b}");

            // Part 4
            string name = "Mary";
            int age = 21;
            int mark = 73;
            Console.WriteLine($"{name} is {age} years old and has an average mark of {mark}");

            double doubleNumber = 3.14159265359;

            Console.WriteLine($"Pi rounded is : {doubleNumber:F2}");
            Console.WriteLine($"Pi rounded is : {doubleNumber:F4}");
            Console.WriteLine("Pi rounded is : " + doubleNumber.ToString("F3"));

            // Part 5
            PrintNumbers(7);

            // Part 6
            Console.WriteLine(IsEven(7));
            Console.WriteLine(IsEven(19));
            Console.WriteLine(IsEven(10));

            string result = age >=18 ? "Adult" : "Under 18";
            Console.WriteLine(result);

            int[] arr = { 235, 234, 12, 5, 7, 123, 124, 9, 12, 4 };
            int total = 0;
            for (int i = 0; i < arr.Length; i++) {
                total += arr[i];
            }
            Console.WriteLine($"Total: {total}");
            total = 0; // reset the total
            foreach (int num in arr) {
                total += num;
            }
            Console.WriteLine($"Total (foreach): {total}");
        }

        // Also part 5
        static void PrintNumbers(int n) {
            for (int i = 1; i <= n; i++) {
                Console.WriteLine(i);
            }
        }

        static bool IsEven(int number) {
            if(number % 2 == 0) {
                return true;
            }
            else {
                return false;
            }

            // OR You can do simply
            // return number % 2 == 0;
        }
    }
}
