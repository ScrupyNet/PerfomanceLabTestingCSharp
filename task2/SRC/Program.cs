using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace task2
{
    class Triangle 
    {
        //Координаты точек первого треугольника
        int[] firstDot, secondDot, thirdDot = new int[3];


        public Triangle(int[] firstDot, int[] secondDot, int[] thirdDot)
        {
            this.firstDot = firstDot;
            this.secondDot = secondDot;
            this.thirdDot = thirdDot;
            
        }
        //Данный метод считает 3 стороны треугольника и углы, возврат массива углов.
        public double[] Angle()
        {
            double lineX = 0;
            double lineY = 0;
            double lineZ = 0;
            double lineAC, lineAB, lineBC;
            double alfa=0;
            double beta=0;
            double gamma=0;

            //Находим АB
            for (int i = 0; i < firstDot.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        lineX = firstDot[i] - secondDot[i];
                        break;
                    case 1:
                        lineY = firstDot[i] - secondDot[i];
                        break;
                    case 2:
                        lineZ = firstDot[i] - secondDot[i];
                        break;
                }
                
            }
            lineAB = Math.Sqrt(Math.Pow(lineX, 2) + Math.Pow(lineY, 2) + Math.Pow(lineZ, 2));

            //Находим АС
            for (int i = 0; i < firstDot.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        lineX = firstDot[i] - thirdDot[i];
                        break;
                    case 1:
                        lineY = firstDot[i] - thirdDot[i];
                        break;
                    case 2:
                        lineZ = firstDot[i] - thirdDot[i];
                        break;
                }

            }
            lineAC = Math.Sqrt(Math.Pow(lineX, 2) + Math.Pow(lineY, 2) + Math.Pow(lineZ, 2));

            //Находим BС
            for (int i = 0; i < firstDot.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        lineX = secondDot[i] - thirdDot[i];
                        break;
                    case 1:
                        lineY = secondDot[i] - thirdDot[i];
                        break;
                    case 2:
                        lineZ = secondDot[i] - thirdDot[i];
                        break;
                }
            }
            lineBC = Math.Sqrt(Math.Pow(lineX, 2) + Math.Pow(lineY, 2) + Math.Pow(lineZ, 2));
            //Теорема косинусов
            alfa = Math.Acos((Math.Pow(lineAB, 2) + Math.Pow(lineAC, 2) - Math.Pow(lineBC, 2)) / (2 * lineAB * lineAC));
            beta = Math.Acos((Math.Pow(lineAB, 2) + Math.Pow(lineBC, 2) - Math.Pow(lineAC, 2)) / (2 * lineAB * lineBC));
            gamma = Math.Acos((Math.Pow(lineBC, 2) + Math.Pow(lineAC, 2) - Math.Pow(lineAB, 2)) / (2 * lineAC * lineBC));
           // Console.WriteLine($"{alfa}, {beta}, {gamma}");
           // Console.ReadKey();
            double[] angle = { alfa, beta, gamma};
            return angle;

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string pathProgDirectory = System.IO.Directory.GetCurrentDirectory();  //Получение директории приложения 

            //CreateMass();  //Создание первого массива (при необходимости)
            Console.WriteLine("Текстовый файл должен находиться в директории приложения и называться Triangle.txt!");
            Console.WriteLine($"Путь к файлу должен быть следующий: {pathProgDirectory}");
            Console.WriteLine("Нажмите любую клавишу, если условие выше выполнено...");
            Console.ReadKey();

            try
            {
                int[] coordinateDots = new int[0];
                int[] firstDots = new int[3];
                int[] secondDots = new int[3];
                int[] thirdDots = new int[3];
                double[] angleFirst = new double[3];
                double[] angleSecond = new double[3];
                bool answer;

                using (FileStream fs = new FileStream(pathProgDirectory + @"\Triangle.txt", FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string row = null;
                   
                    while (!string.IsNullOrEmpty(row = sr.ReadLine()))
                    {
                        GetCoordinat(row, out coordinateDots);          //В этот метод мы подаем строку, а получаем набор координат в массиве

                        
                        //начинаем запись координат в классы
                        for (int i = 0; i < coordinateDots.Length; i++)
                        {
                            for (int j = 0; j < coordinateDots.Length/6; j++)
                            {
                                firstDots[j] = coordinateDots[i];
                                i++;
                            }
                            for (int j = 0; j < coordinateDots.Length / 6; j++)
                            {
                                secondDots[j] = coordinateDots[i];
                                i++;
                            }
                            for (int j = 0; j < coordinateDots.Length / 6; j++)
                            {
                                thirdDots[j] = coordinateDots[i];
                                i++;
                            }
                            //Запись первого объекта
                            Triangle triangleOne = new Triangle(firstDots, secondDots, thirdDots);
                            //Заполнение массива со сторонами первого треугольника
                            angleFirst = triangleOne.Angle();

                            for (int j = 0; j < coordinateDots.Length / 6; j++)
                            {
                                firstDots[j] = coordinateDots[i];
                                i++;
                            }
                            for (int j = 0; j < coordinateDots.Length / 6; j++)
                            {
                                secondDots[j] = coordinateDots[i];
                                i++;
                            }
                            for (int j = 0; j < coordinateDots.Length / 6; j++)
                            {
                                thirdDots[j] = coordinateDots[i];
                                i++;
                            }
                            //Запись второго объекта
                            Triangle triangleTwo = new Triangle(firstDots, secondDots, thirdDots);
                            //Заполнение массива с углами второго треугольника
                            angleSecond = triangleTwo.Angle();                           
                        }
                        //Теперь есть 2 объекта как треугольники, подобие определяем по двум углам
                        //Если два угла одного треугольника соответственно равны двум углам другого, то такие треугольники подобны.

                        answer = FindAnswer(angleFirst, angleSecond);
                        if (answer == true)
                        {
                            Console.WriteLine("Треугольники подобны");
                        }
                        else
                        {
                            Console.WriteLine("Треугольники не подобны");
                        }
                    }
                    Console.ReadKey();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally { Console.ReadKey(); }
        }

        //{triangle1: {A: [0, 0, 0], B: [0, 5, 0], C: [5, 5, 0]}, triangle2: {A: [0, 0, 0], C: [0, 0, 5], B: [5, 0, 3]}}
        public static void GetCoordinat(string row, out int[] coordinateDots)
        {

            string[] coordinateRow = new string[0];
            coordinateDots = new int[0];
           

            int iTriangleRow = 0;      //счетчик i для цикла foreach triangleRow
            int iCoordinateRow = 0;    //счетчик i для цикла foreach coordinaterow


            Regex coordTriangle = new Regex(@"(\d+.\s\d+.\s\d+)|(\S?\d+.\s\S?\d+.\s\S?\d+)");  // 1, 1, 1 | -1, +1, 1
            
            

           //ищем и записываем строчки координат треугольников
           foreach (Match match in coordTriangle.Matches(row))
           {
               {
                    Array.Resize<string>(ref coordinateRow, ++iTriangleRow);
                    coordinateRow[iTriangleRow-1] = match.Value;
               }
           }

            Regex coordValue = new Regex(@"[0-9]+|\S?[0-9]+");                          //[1 | -1

            //вытаскиваем координаты каждой точки для каждого треугольника [x,y,z]
            for (int i = 0; i < coordinateRow.Length; i++)
            {
                
                foreach (Match match in coordValue.Matches(coordinateRow[i]))
                {
                    Array.Resize<int>(ref coordinateDots, ++iCoordinateRow);
                    var temp = 0;

                    //Для считывания отрицательных координат, пропускам отрицательные значения
                    bool ok = int.TryParse(Convert.ToString(match), out temp);
                    if (ok == false)
                    {
                        //Если [1 и подобное, оставляем только число
                        Regex cutValue = new Regex(@"[0-9]+");
                        foreach (Match matchSecond in cutValue.Matches(Convert.ToString(match)))
                        {
                            coordinateDots[iCoordinateRow - 1] = Convert.ToInt32(matchSecond.Value);
                        }    
                    }
                    else
                    {
                        coordinateDots[iCoordinateRow - 1] = Convert.ToInt32(match.Value);
                    }
                }
            }

            if (coordinateDots.Length % 3 != 0 | coordinateDots.Length / 9 != 2)
            {
                    Console.WriteLine("\nЧто-то не так с данными внутри файла");
                    Console.WriteLine("Запишите данные в виде:\n{triangle1: {A: [x, y, z], B: [x, y, z], C: [x, y, z]}, triangle2: {A: [x, y, z], C: [x, y, z], B: [x, y, z]}}");
                    Console.WriteLine("В случае необходимости большего числа сравнений фигур, поместите следующие треугольники на новую строчку");
                    Console.WriteLine("Вернитесь, когда исправите. Пока!");
                    Thread.Sleep(10000);
                    Process.GetCurrentProcess().Kill();
            }

            
        }

        static bool FindAnswer(double[] angleFirst, double[] angleSecond)
        {
            bool answer = false;
            byte tikAnswer = 0;                             //счетчик совпадений, min2
            int[] iPosExp= { 5, 5, 5 };                      //массив исключения уже проверенных, изначально заполнен пятерками, как недостижимыми значениями
            int[] jPosExp = { 5, 5, 5 };

            for (int i = 0; i < angleFirst.Length; i++)
            {
                
                for (int j = 0; j < angleSecond.Length; j++)
                {
                        
                    if (angleFirst[j] - angleSecond[i] == 0 & iPosExp[i] != i & jPosExp[j] != j)
                    {
                        //Запрещает обрабатывать одинаковые углы дважды
                        iPosExp[i] = i;
                        jPosExp[j] = j;
                            
                        tikAnswer++;
                        break;
                    }
                }

            }

            if (tikAnswer >= 2) answer = true;

            return answer;
        }

    }


}


