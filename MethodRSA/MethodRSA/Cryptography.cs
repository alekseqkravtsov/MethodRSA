using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodRSA
{
    internal class Cryptography
    {
        private char[] alphabet = new char[73]
        {
            'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З',
            'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р',
            'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ',
            'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я', 'а', 'б', 'в',
            'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к',
            'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у',
            'ф', 'х', 'ч', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы',
            'ь', 'э', 'ю', 'я', ' ', '.', ':', '!', '?',
            ','
        };

        //Метод генерации всех необходимых чисел для шифрования и дешифрования
        public int[] generateNumbers()
        {
            int[] numbers = new int[6];
            Random r = new Random();

            //генерация p и q
            for (int i = 0; i < 2; i++)
            {
                numbers[i] = r.Next(2, 20);

                while (!IsPrime(numbers[i]))
                {
                    numbers[i] = r.Next(2, 20);
                }
            }

            //статичные p и q
            numbers[0] = 7;
            numbers[1] = 17;

            numbers[2] = numbers[0] * numbers[1];                       //определение числа n;
            numbers[3] = (numbers[0] - 1) * (numbers[1] - 1);           //определение числа fi(n);
            numbers[4] = GenerateE(numbers[3]);                         //определение числа e;    
            numbers[5] = ModInverse(numbers[4], numbers[3]);            //определение числа d;

            return numbers;
        }

        //Метод для генерации числа e
        private int GenerateE(int phiN)
        {
            // Начинаем с 3 и идем до φ(n)
            for (int i = 3; i < phiN; i += 2)      // Используем только нечетные числа
            {
                if (GCD(i, phiN) == 1)
                {
                    return i;                       // Возвращаем первое найденное значение e
                }
            }

            return -1;                              // Если не нашли подходящее значение
        }

        //Метод вычисления обратного значения  e  по модулю  φ(n)
        private int ModInverse(int e, int phiN)
        {
            int m0 = phiN;
            int y = 0, x = 1;

            if (phiN == 1)
                return 0;

            //Расширенный алгоритм Евклида
            while (e > 1)
            {
                //q - коэффициент деления
                int q = e / phiN;
                int t = phiN;

                //phiN - остаток от деления
                phiN = e % phiN;
                e = t;
                t = y;

                //Обновляем y и x
                y = x - q * y;
                x = t;
            }

            //Убедимся, что x положительное
            if (x < 0)
                x += m0;

            return x;
        }

        //Метод для проверки простого числа
        private bool IsPrime(int number)
        {
            if (number <= 1) return false;
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        //Метод для вычисления НОД
        static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static void Main(string[] args)
        {
            Cryptography cryptography = new Cryptography();

            //стартовые числа
            int[]arr = cryptography.generateNumbers();
            Console.Write("Число p = "     + arr[0] +
                        "\nЧисло q = "     + arr[1] +
                        "\nЧисло n = "     + arr[2] +
                        "\nЧисло fi(n) = " + arr[3] +
                        "\nЧисло e = "     + arr[4] +
                        "\nЧисло d = "     + arr[5] +
                        "\n\n");

            Console.WriteLine("Открытым ключом является {e, n} = " + arr[4] + " " + arr[2]);
            Console.WriteLine("Закрытым ключом является {d, n} = " + arr[5] + " " + arr[2]);

        }
    }
}
