using System;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace CreateLog
{

    class RandomDateTime
    {

        DateTime minValueDate;
        DateTime maxValueDate;
        Random gen;
        int range;

        public RandomDateTime()
        {
            minValueDate = new DateTime(2020, 1, 1, 0, 0, 0);
            maxValueDate = new DateTime(2020, 1, 1, 23, 59, 59);
            gen = new Random();
            range = (int)(maxValueDate-minValueDate).TotalSeconds;
        }

        public DateTime Next()
        {
            return minValueDate.AddSeconds(gen.Next(range)).AddMilliseconds(gen.Next(1000));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DateTime dateTimeStart;
            DateTime dateTimeEnd;

            

            string logPath = null;
            int volumeMax = 300;           //Объем бочки
            int volume = 0;              //Объем бочки текущий

            //Заполнение бочки случайным значением
            Random volumeNow = new Random();
            volume = volumeNow.Next(0,300);

            string pathProgDirectory = System.IO.Directory.GetCurrentDirectory();  //Получение директории приложения       

            try
            {

                Console.WriteLine("Задайте путь к фалу согласно шаблону:\n{путь к файлу}/log.log {начальнае дата}Т{начальное время} {Конечная дата}Т{конечное время}");
                Console.WriteLine("Пример:\nApp/log.log 2020-01-01T12:00:00 2020-01-01T13:00:00");
                string consoleRow = Convert.ToString(Console.ReadLine());

                //Метод разделение строки
                SplitRow(consoleRow, out logPath, out dateTimeStart, out dateTimeEnd);

                //Создание директории по заданому пути
                Directory.CreateDirectory(pathProgDirectory+logPath);

                FullLog(pathProgDirectory, logPath, volumeMax, volume);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally { Console.ReadKey(); }

        }

       static void SplitRow(string consoleRow, out string logPath, out DateTime dateTimeStart, out DateTime dateTimeEnd)
        {
            logPath = null;
            //Получение пути файла
            Regex pathExp = new Regex(@"^(.*[/]) *");
            string rowPath = Convert.ToString(pathExp.Match(consoleRow));

            //Получение пути для создания директории
            Regex pathFolder = new Regex(@"[a-zA-Z]+");
            foreach (Match match in pathFolder.Matches(rowPath))
            {
                logPath = logPath + match + "\\";
            }

            //Получения начальной даты
            Regex dateStartExp = new Regex(@"\d+-\d+-\d+[T]\d+:\d+:\d+");
            ConvertToDateTime(Convert.ToString(dateStartExp.Match(consoleRow)), out dateTimeStart);
            Console.WriteLine("dateTimeStart {0}", dateTimeStart);

            //Получения конечной даты
            Regex dateEndExp = new Regex(@"\d+-\d+-\d+[T]\d+:\d+:\d+$");
            ConvertToDateTime(Convert.ToString(dateEndExp.Match(consoleRow)), out dateTimeEnd);
            Console.WriteLine("dateTimeEnd {0}", dateTimeEnd);

        }

        private static void ConvertToDateTime(string value, out DateTime dateTime)
        {
           try
            {
                dateTime = Convert.ToDateTime(value);
            }
            catch (FormatException)
            {
                Console.WriteLine("'{0}' is not in the proper format.", value);
                dateTime = DateTime.Parse("00/00/0000");
            }
        }

        static void FullLog(string pathProgDirectory, string logPath, int volumeMax, int volume)
        {
            try
            {
                using (FileStream logStream = new FileStream(pathProgDirectory + logPath + "log.log", FileMode.Create))
                using (StreamWriter logWrite = new StreamWriter(logStream))
                {
                    logWrite.WriteLine("META DATA:");
                    logWrite.WriteLine(volumeMax);
                    logWrite.WriteLine(volume);
                    logWrite.Close();
                }
                //Здесь, потому что в цикле показывает одиаковые числа
                RandomDateTime date = new RandomDateTime();         //Случаная дата
                Random user = new Random();                         //Случайный пользователь
                Random active = new Random();                         //Случайное событие
                Random water = new Random();                         //Случайное количество воды

                string row = null;
                string formateDate = null;
                long size;
                int sizeMax = 1048576;
                do
                {
                    FileInfo file = new FileInfo(pathProgDirectory + logPath + "log.log");
                    size = file.Length;
                    row = RowFormat(formateDate, volumeMax, volume, date, user, active, water);
                    File.AppendAllText(pathProgDirectory + logPath + "log.log", row);
                    File.AppendAllText(pathProgDirectory + logPath + "log.log", Environment.NewLine);
                    Thread.Sleep(10);
                }
                while (size <= sizeMax);
                Console.WriteLine("Файл записан");
                Console.ReadKey();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Форматировуем строку на вывод в лог, добавляем юзеров
        static string RowFormat(string formateDate, int volumeMax, int volume, RandomDateTime date, Random water, Random user, Random active)
        {
            string finalRow = null;
            

            formateDate = date.Next().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
            string username = "[username" + user.Next(10)+"]";
            string action = null;
            int waterChange;

            //Генерация случайного значения литража
            waterChange = water.Next(1, 50);
            int switchTumbler = active.Next(2);
            //Генерация строки действия
            switch (switchTumbler)
            {
                case 0:
                    action = $"wanna top up {waterChange}l";
                    break;

                case 1:
                    action = $"wanna scroop {waterChange}l";
                    break;
            }

            //Объединяем строки
            finalRow= $"{formateDate} - {username} - {action}";
            return finalRow;
            
        }
    }


}
