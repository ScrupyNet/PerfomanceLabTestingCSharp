using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace task1
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathProgDirectory = System.IO.Directory.GetCurrentDirectory();  //Получение директории приложения 

            //CreateMass();  //Создание первого массива (при необходимости)
            Console.WriteLine("Текстовый файл должен находиться в директории приложения и называться Mass.txt!");
            Console.WriteLine($"Путь к файлу должен быть следующий: {pathProgDirectory}");
            Console.WriteLine("Нажмите любую клавишу, если условие выше выполнено...");
            Console.ReadKey();
            try
            {
                //Загрузка чисел из файла в динамический массив (с обходом инородного типа данных)
                int[] mass = GetNumber(pathProgDirectory + @"\Mass.txt").ToArray();


                SortMass(mass, 0, mass.Length - 1);

                /*
                for (int i = 0; i < mass.Length; i++)
                {
                    Console.Write(mass[i] + ", ");
                }
                Console.WriteLine();
                */

                var indexMiddle = FindMiddleIndex(mass);            //Порядковый номер среднего значения
                var indexPercentile = FindPercentileIndex(mass);    //Порядковый номер значения перцентиля

                //Console.ReadKey();

                var ansverSum = Answer(mass, indexMiddle, indexPercentile);

                Console.WriteLine(ansverSum);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally { Console.ReadKey(); }   
        }

        //Заполнение массива случайными числами
        static void CreateMass()
        {
            Random rand = new Random();
            try
            {
                using (FileStream massTxt = new FileStream("Mass.txt", FileMode.Create))
                using (StreamWriter mt = new StreamWriter(massTxt))
                {
                    int maxRow = 0;
                    start:
                    Console.WriteLine("Введите необходимое количество элементов");
                    bool okMaxRow = int.TryParse(Console.ReadLine(), out maxRow);
                    if (okMaxRow == false)
                    {
                        Console.WriteLine("Некорректно введены данные");
                        Thread.Sleep(2000);
                        Console.Clear();
                        goto start;
                    }

                    int[] mass = new int[maxRow]; 
                    for (int i = 0; i < maxRow; i++)
                        mass[i] = rand.Next(10000);
                    for (int i = 0; i < maxRow; i++)
                        mt.WriteLine(mass[i]);

                    Console.WriteLine("Заполнение массива завершено");

                    mt.Close();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Получение чисел из файла в массив
        static IEnumerable<int> GetNumber(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                int numberFromMass;
                string row = null;
                while (!string.IsNullOrEmpty(row = sr.ReadLine()))
                {
                    bool okMaxRow = int.TryParse(row, out numberFromMass);
                    if (okMaxRow == true)
                        yield return numberFromMass;
                }
            }
        }

        //быстрая сортировка

        static int[] SortMass(int[] arrayForSort, int start, int end)
        {
            if (start >= end)
            {
                return arrayForSort;
            }
            int pivot = Border(arrayForSort, start, end);
            SortMass(arrayForSort, start, pivot - 1);
            SortMass(arrayForSort, pivot + 1, end);
            return arrayForSort;
        }

        static int Border(int[] arrayForSort, int start, int end)
        {
            int temp;
            int marker = start;
            for (int i = start; i < end; i++)
            {
                if (arrayForSort[i] < arrayForSort[end]) 
                {
                    temp = arrayForSort[marker]; 
                    arrayForSort[marker] = arrayForSort[i];
                    arrayForSort[i] = temp;
                    marker += 1;
                }
            }

            temp = arrayForSort[marker];
            arrayForSort[marker] = arrayForSort[end];
            arrayForSort[end] = temp;
            return marker;
        }

        //Нахождение индекса среднего значения (первый элемент)
        static int FindMiddleIndex(int[] massForMiddle)
        {
            double sum = 0;
            int middleIndex = 0;
            for (int i = 0; i < massForMiddle.Length; i++)
            {
                sum += massForMiddle[i];
            }
            if (sum == 0)
            {
                Console.WriteLine("Массив пустой или с нулевыми значениями");
                return 0;
            }

            var middleValue = sum / massForMiddle.Length;

            for (int i = 0; middleValue > Convert.ToDouble(massForMiddle[i]); i++)
            {
                middleIndex = i;
            }

            middleIndex++;

            return middleIndex;
        }

        //Нахождение индекса перцентиля (последний элемент)
        static int FindPercentileIndex(int[] massForPercentile)
        {
            var percentileIndex = Convert.ToInt32(Math.Truncate(massForPercentile.Length * 0.9));
            //Console.WriteLine("Индекс массива, равный 90%: {0}", percentileIndex);
            //Console.ReadKey();
            return percentileIndex;

        }

        //Нахождение конечного ответа
        static int Answer(int[] massForAnswer, int middleIndex, int percentileIndex)
        {
            int answer = 0;
            for (int i = middleIndex; i < percentileIndex; i++)
            {
                answer += massForAnswer[i];
            }

            return answer;
        }
    }
}
