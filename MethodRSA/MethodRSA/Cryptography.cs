using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
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

        public void startProgram()
        {
            string message;

            printAlphabet();

            //стартовые числа
            int[] arr = generateNumbers();
            Console.Write("\n\nЧисло p = " + arr[0] +
                          "\nЧисло q = " + arr[1] +
                          "\nЧисло n = " + arr[2] +
                          "\nЧисло fi(n) = " + arr[3] +
                          "\nЧисло e = " + arr[4] +
                          "\nЧисло d = " + arr[5] +
                          "\n\n");

            Console.WriteLine("Открытым ключом является {e, n} = " + arr[4] + " " + arr[2]);
            Console.WriteLine("Закрытым ключом является {d, n} = " + arr[5] + " " + arr[2]);

            while (true)
            {
                Console.Write("\nВведите сообщение: "); message = Console.ReadLine();
                BigInteger[] encryptMessage = EcnryptByIndex(message);

                if (encryptMessage != null)
                {
                    Console.Write("Шифр по индексам: "); printArray(encryptMessage);


                    encryptMessage = Encrypt(encryptMessage, arr[4], arr[2]);
                    Console.Write("\nШифр открытым ключем шифрования: "); printArray(encryptMessage);

                    message = Decrypt(encryptMessage, arr[5], arr[2]);
                    Console.Write("\nРасшифрованное сообщение закрытым ключом: "); Console.WriteLine(message + "\n");
                }
                else
                    Console.WriteLine("Сообщение или ключ содержит недопустимый символ для заданного алфавита.");
            }
        }

        public BigInteger[] Encrypt(BigInteger[] message, int e, int n)
        {
            BigInteger[] lockedMessage = new BigInteger[message.Length];
            for(int i = 0; i < message.Length; i++)
            {
                lockedMessage[i] = (BigInteger.Pow(message[i], e)) % n;
            }
            return lockedMessage;
        }

        public string Decrypt(BigInteger[] lockedMessage, int d, int n)
        {
            string message = "";
            BigInteger alphabetIndex;

            for (int i = 0; i < lockedMessage.Length; i++)
            {
                alphabetIndex = ((BigInteger.Pow(lockedMessage[i], d)) % n) - 1;
                message += GetSymbol((int)alphabetIndex);
            }

            return message;
            
        }

        public BigInteger[] EcnryptByIndex(string message)
        {
            char[] characters = message.ToCharArray();
            BigInteger[] lockedMessage = new BigInteger[characters.Length];

            for (int i = 0; i < characters.Length; i++)
            {
                if (GetIndex(characters[i]) == -1)
                    return null;
                else
                    lockedMessage[i] = GetIndex(characters[i]) + 1;
            }

            return lockedMessage;
        }

        private int GetIndex(char character)
        {
            int index = -1;
            for (int i = 0; i < alphabet.Length; i++)
            {
                if (alphabet[i] == character)
                {
                    index = i; break;
                }
            }

            return index;
        }

        private char GetSymbol(int index)
        {
            if ((index >= alphabet.Length) && (index < 0))
                return ' ';

            for (int i = 0; i < alphabet.Length; i++)
            {
                if (i == index)
                    return alphabet[i];
            }

            return ' ';
        }

        public void printArray(BigInteger[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write(array[i] + " ");
            }
        }

        public void printAlphabet()
        {
            Console.WriteLine("Алфавит символов:");

            int i;
            for (i = 0; i < alphabet.Length; i++)
            {
                Console.Write(alphabet[i] + " ");
                if ((i + 1) % 8 == 0)
                    Console.WriteLine("");
            }
        }

        //Метод генерации всех необходимых чисел для шифрования и дешифрования
        public int[] generateNumbers()
        {
            int[] numbers = new int[6];


            Random r = new Random();

            //генерация числа p в диапазоне от 2 до 20
            numbers[0] = r.Next(2, 20);
            while (!IsPrime(numbers[0]))
                numbers[0] = r.Next(2, 20);

            //генерация числа q в диапазоне от p до 30
            numbers[1] = r.Next(numbers[0] + 1, 30);
            while (!IsPrime(numbers[1]))
            {
                numbers[1] = r.Next(numbers[0] + 1, 30);
            }
                

            //статичные p и q
            //numbers[0] = 7;
            //numbers[1] = 17;

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
                if (GCD(phiN, i) == 1)
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
            cryptography.startProgram();
        }
    }
}
