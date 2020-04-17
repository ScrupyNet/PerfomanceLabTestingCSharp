using System;
using System.Threading;

namespace task4
{
    class Program
    {
        static void Main(string[] args)
        {
            string rowFirst;
            string rowSecond;
            char[] massRowSecond;
            bool ok;

            //Получаем 2 массива из строк
            InputRows(out rowFirst, out massRowSecond, out rowSecond);

            ok = CompareMassives(rowFirst, massRowSecond, rowSecond);

            //Выводим ответ
            if (ok == true) Console.WriteLine("OK");
            else Console.WriteLine("KO");

            Console.ReadKey();
        }

        static void InputRows(out string rowFirst, out char[] massRowSecond, out string rowSecond)
        {

            
            bool rowRightFirst;
            bool rowRightSecond;
            do
            {
                Console.Clear();
                Console.WriteLine("Введите первую строку");
                rowFirst = Convert.ToString(Console.ReadLine());
                if (rowFirst == "" | rowFirst == null)
                {
                    Console.WriteLine("Строка пустая");
                    rowRightFirst = false;
                }
                else rowRightFirst = true;


                Console.WriteLine("Введите вторую строку");
                rowSecond = Convert.ToString(Console.ReadLine());
                if (rowSecond == "" | rowSecond == null)
                {
                    Console.WriteLine("Строка пустая");
                    rowRightSecond = false;
                }

                else rowRightSecond = true;

                if (rowRightFirst == false | rowRightSecond == false)
                {
                    Console.WriteLine("Данные введены некоректно");
                    Thread.Sleep(1000);
                }
            }
            while (rowRightFirst == false | rowRightSecond == false);


            massRowSecond = rowSecond.ToCharArray();

        }

        static bool CompareMassives(string rowFirst, char[] massRowSecond, string rowSecond)
        {
            string row = null;
            char[] massRowFirst = rowFirst.ToCharArray();

            int n = 0; //Размер массива

            string[] massWithoutStar = new string[n];

            //Делаем массив без звезд для обеспечения поиска
            for (int i = 0; i < massRowSecond.Length;)
            {
                
                if (massRowSecond[i] != '*')
                {
                row = row + Convert.ToString(massRowSecond[i]);
                i++;
                }
                else
                {
                    if (row != "" & row != null)
                    {
                        Array.Resize<string>(ref massWithoutStar, ++n);
                        massWithoutStar[n - 1] = row;
                    }
                    i++;
                    row = null;
                }
            }

            if (row != "" & row != null)
            {
                Array.Resize<string>(ref massWithoutStar, ++n);
                massWithoutStar[n - 1] = row;
            }

            //Если несовпадение в первом символе
            if (massRowFirst[0] != massRowSecond[0] & massRowSecond[0] != '*')
                return false;

            //Если несовпадение в последнем символе
            if (massRowFirst[massRowFirst.Length-1] != massRowSecond[massRowSecond.Length-1] & massRowSecond[massRowSecond.Length-1] != '*')
                return false;

            //Если во второй строке одни звезды
            if (massWithoutStar.Length == 0)
                return true;

            int index = 0;
            int searchPosition;
            int indexTemp = 0;
            for (int i = 0; i < massWithoutStar.Length;)
            {
                searchPosition = rowFirst.IndexOf(massWithoutStar[i], index);

                if (searchPosition < 0) return false; //Если больше нет нахождений внутри первой строки, а массив второй еще не закончен, значит строка не совпадает
                
                //Если возможно контролируем предыдущий символ, если не совпадают и не равен *, то фалс
                if (index - 1 >= 0)
                {
                    if ((massRowSecond[rowSecond.IndexOf(massWithoutStar[i], indexTemp)-1]) != '*' & (massRowSecond[rowSecond.IndexOf(massWithoutStar[i], indexTemp) - 1]) != massRowFirst[index - 1])
                        return false;

                }
                //Если возможно контролируем следующий символ, если не совпадают и не равен *, то фалс
                if (index + massWithoutStar[i].Length < rowFirst.Length)
                {
                    if ((massRowSecond[rowSecond.IndexOf(massWithoutStar[i], indexTemp) + massWithoutStar[i].Length]) != '*' & massRowSecond[rowSecond.IndexOf(massWithoutStar[i], indexTemp) + massWithoutStar[i].Length] != massRowFirst[index + massWithoutStar[i].Length])
                        return false;
                }

                index = rowFirst.IndexOf(massWithoutStar[i], searchPosition) + massWithoutStar[i].Length + 1;
                indexTemp = rowSecond.IndexOf(massWithoutStar[i], indexTemp) + massWithoutStar[i].Length + 1;
                i++;
            }

            return true;
        }

            #region notwork
            /*
            static void CompareMassives(char[] massRowFirst, char[] massRowSecond, out bool ok)
            {
                int rage = 0;
                for (int i = 0; i < massRowFirst.Length;)
                {
                    if (massRowFirst[i] == massRowSecond[i + rage])
                    {
                        i++;
                        continue;
                    }
                    if (massRowSecond[i+rage] == '*')
                    {
                        int j = i + rage;
                        while (massRowSecond[j] == '*')
                        {
                            j++;
                        }

                        if (j<massRowSecond.Length)
                        {
                            while (massRowFirst[i] != massRowSecond[j])
                            {
                                i++;
                            }
                            rage = j - i;
                        }

                    }
                }


                ok = true;
                Console.WriteLine(ok);
                Console.ReadKey();
            }
            */
            #endregion
    }
}


    