namespace Number_Pyramid
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int num=int.Parse(Console.ReadLine());

            int currentNumber = 1;
            for (int rows = 1; rows <= num; rows++)
            {
                bool isFinis=false;

                for (int cols = 1; cols <= rows; cols++)
                {
                    Console.Write($"{currentNumber} ");
                    currentNumber++;    
                    if(currentNumber>num)
                    {
                        isFinis = true;
                        break;
                    }
                    
                }
                Console.WriteLine();
                if (isFinis)
                {
                    break;
                }
            }
        }
    }
}