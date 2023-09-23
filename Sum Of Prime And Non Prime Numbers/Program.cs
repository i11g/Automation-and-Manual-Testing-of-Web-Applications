namespace Sum_Of_Prime_And_Non_Prime_Numbers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string command=Console.ReadLine();
            int sumPrime = 0;
            int sumNonPrime = 0;
            while (command != "stop")
            {
                int num=int.Parse(command);
                bool isPrime = true;
                for (int i = 2; i <=Math.Sqrt(num); i++)
                {
                    if(num% i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (!isPrime)
                {
                    sumNonPrime += num;  
                }
                else 
                {
                    sumPrime += num;

                }
                command=Console.ReadLine();
            }
            Console.WriteLine($"Sum of all prime numbers is: {sumPrime}");
            Console.WriteLine($"Sum of all non prime numbers is: {sumNonPrime}");
        }
    }
}