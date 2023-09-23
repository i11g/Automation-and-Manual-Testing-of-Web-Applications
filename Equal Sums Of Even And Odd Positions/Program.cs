namespace Equal_Sums_Of_Even_And_Odd_Positions
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int startNumber=int.Parse(Console.ReadLine());
            int endNumber=int.Parse(Console.ReadLine());

            for (int i = startNumber; i <=endNumber; i++)
            {
                
                string currentNumber=i.ToString();

                int sumEven = 0;
                int sumOdd = 0;
                for (int j = 0; j < currentNumber.Length ; j++)
                {
                    int number = int.Parse(currentNumber[j].ToString()); 
                    if(j%2==0) 
                    {
                        sumEven += number;  
                    }
                    else
                    {
                        sumOdd += number;
                    }
                    if(sumEven==sumOdd)
                    {
                        Console.WriteLine(currentNumber);
                    }
                }
            }

        }
    }
}