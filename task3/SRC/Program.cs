using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace task3
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dateTimeStart;
            DateTime dateTimeEnd;
            bool work;                      //Флаг контроля директории
            bool workSplit;                 //Флаг контроля ввода
            bool csvCreate;                 //флаг контроля промежутка
            string logPath = null;
            int volumeMax;                  //Объем бочки
            int volumeBegin;               //Объем бочки в начале
            string[] rowsSort;


            string pathProgDirectory = Environment.CurrentDirectory;  //Получение директории приложения       
            
            try
            {
                //Делаем до тех пор, пока не пройдут все условия для входных данных
                do
                {
                    work = true;
                    Console.Clear();
                    Console.WriteLine("Задайте путь к фалу согласно шаблону:\n{путь к файлу}/log.log {начальнае дата}Т{начальное время} {Конечная дата}Т{конечное время}");
                    Console.WriteLine("Пример:\nApp/log.log 2020-01-01T10:00:00 2020-01-01T13:00:00\n");

                    string consoleRow = Convert.ToString(Console.ReadLine());

                    //Метод разделение строки
                    SplitRow(consoleRow, out logPath, out dateTimeStart, out dateTimeEnd, out workSplit);


                    if (!Directory.Exists(pathProgDirectory + logPath))
                    {
                        Console.WriteLine("Директории не существует!");
                        work = false;
                    }

                    if (!work | !workSplit) Thread.Sleep(2000);

                }
                while ((work == false ) | (workSplit == false));
             
                Console.WriteLine("Данные поданы верно");
                string path = pathProgDirectory + logPath;          //общий путь к логу

                using (FileStream logStream = new FileStream(path + "log.log", FileMode.Open))
                using (StreamReader logRead = new StreamReader(logStream))
                {
                    logRead.ReadLine();
                    bool ok = int.TryParse(logRead.ReadLine(), out volumeMax);
                    ok = int.TryParse(logRead.ReadLine(), out volumeBegin);
                    logRead.Close();
                }

                //Считываем лог и получаем из него отсортированный массив
                rowsSort = ReadLog(path, volumeMax, volumeBegin);

                //Расчеты и создание файла csv
                FindAnswer(path, rowsSort, volumeMax, volumeBegin, dateTimeStart, dateTimeEnd, out csvCreate);

                if (csvCreate == true) Console.WriteLine("Работа завершена, файл csv находится по адресу \n{0}", path);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally { Console.ReadKey(); }

        }

        static void SplitRow(string consoleRow, out string logPath, out DateTime dateTimeStart, out DateTime dateTimeEnd, out bool workSplit)
        {
            workSplit = true;
            logPath = null;
            string rowPath = null;
            string dateStart = null;
            string dateEnd = null;

            //Получение пути файла
            Regex pathExp = new Regex(@"^(.*[/]) *");
            rowPath = Convert.ToString(pathExp.Match(consoleRow));
            if (rowPath == null)
            {
                Console.WriteLine("Директории не существует");
                workSplit = false;
            }
            else
            {
                //Получение пути для создания директории
                byte flag = 0;
                Regex pathFolder = new Regex(@"[a-zA-Z]+");
                foreach (Match match in pathFolder.Matches(rowPath))
                {
                    logPath = logPath + match + "\\";
                    flag++;
                }

                if (flag == 0) logPath = logPath + "\\";

            }
                

            //Получения начальной даты
            Regex dateStartExp = new Regex(@"\d\d\d\d-\d\d-\d\d[T]\d\d:\d\d:\d\d\s");    //1111-11-11T11:11:11
            dateStart = Convert.ToString(dateStartExp.Match(consoleRow));
            if (dateStart == "")
            {
                Console.WriteLine("Неверно задана первая дата");
                workSplit = false;
                dateTimeStart = DateTime.Now;
            }
            else
            {
                ConvertToDateTime(Convert.ToString(dateStartExp.Match(consoleRow)), out dateTimeStart);
            }

            //Получения конечной даты
            Regex dateEndExp = new Regex(@"\d\d\d\d-\d\d-\d\d[T]\d\d:\d\d:\d\d$");    //1111-11-11T11:11:11 в конце строки
            dateEnd = Convert.ToString(dateEndExp.Match(consoleRow));
            if (dateEnd == "")
            {
                Console.WriteLine("Неверно задана вторая дата");
                workSplit = false;
                dateTimeEnd = DateTime.Now;
            }
            else
            {
                ConvertToDateTime(Convert.ToString(dateEndExp.Match(consoleRow)), out dateTimeEnd);
            }

            if (dateTimeStart>dateTimeEnd)
            {
                Console.WriteLine("Начальная дата больше конечной");
                workSplit = false;
            }
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

        static string[] ReadLog(string path, int volumeMax, int volumeBegin)
        {
            string[] rows = GetRows(path).ToArray();
            DateTime[] rowsDate = GetDate(path).ToArray();
            //Переменные для сортировки
            int start = 0;
            int end = rows.Length-1;
            SortingMass(rows, rowsDate, start, end);
            return rows;
        }

        static IEnumerable<string> GetRows(string path)
        {
            using (FileStream fs = new FileStream(path + "log.log", FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                Regex dateExp = new Regex(@"^\d\d\d\d-\d\d-\d\d[T]\d\d:\d\d:\d\d.\d\d\dZ");
                string row = null;
                while (!string.IsNullOrEmpty(row = sr.ReadLine()))
                {
                    Match rowMatch = dateExp.Match(row);
                    if (Convert.ToString(rowMatch) != "")
                    {
                        yield return row;
                    }
                }
            }
        }
            
        static IEnumerable<DateTime> GetDate(string pathDate)
        {
            using (FileStream fs = new FileStream(pathDate + "log.log", FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                Regex dateExp = new Regex(@"^\d\d\d\d-\d\d-\d\d[T]\d\d:\d\d:\d\d.\d\d\dZ");
                string row = null;
                while (!string.IsNullOrEmpty(row = sr.ReadLine()))
                {
                    Match rowMatch = dateExp.Match(row);
                    string rowDateTemp = Convert.ToString(rowMatch);
                    if (rowDateTemp != "")
                    {
                        yield return Convert.ToDateTime(rowDateTemp);
                    }
                }
            }
        }

        //Сортировка для сравнения со временем
        static string[] SortingMass(string[] rows, DateTime[] rowsDate, int start, int end)
        {
            if (start >= end)
            {
                return rows;
            }
            int pivot = Border(rows, rowsDate, start, end);
            SortingMass(rows, rowsDate, start, pivot - 1);
            SortingMass(rows, rowsDate, pivot + 1, end);
            return rows;
        }

        static int Border(string[] rows,  DateTime[] rowsDate, int start, int end)
        {
            string tempString;
            DateTime tempDate;
            int marker = start;
            for (int i = start; i < end; i++)
            {
                //DoubleSwap

                if (rowsDate[i] < rowsDate[end])
                {
                    tempString = rows[marker];
                    rows[marker] = rows[i];
                    rows[i] = tempString;
                    tempDate = rowsDate[marker];
                    rowsDate[marker] = rowsDate[i];
                    rowsDate[i] = tempDate;
                    marker += 1;
                }
            }

            tempString = rows[marker];
            rows[marker] = rows[end];
            rows[end] = tempString;
            tempDate = rowsDate[marker];
            rowsDate[marker] = rowsDate[end];
            rowsDate[end] = tempDate;
            return marker;
        }

        static void FindAnswer (string path, string []rowsSort, int volumeMax, int volumeBegin, DateTime dateTimeStart, DateTime dateTimeEnd, out bool csvCreate)
        {
            int volumeNow = volumeBegin;                  //Переменная, в которую записывается объем в настоящее время
            int volumeChange;               //Переменная, в которую записывается изменение объема
            int volumeTemp;                 //переменная для проверки

            byte flagStart = 0;                 //Флаг первого значения
            int volumeStart = 0 ;               //Объем бочки на старте времени
            byte flagFinish = 0;                //Флаг последнего значения
            int volumeFinish = 0;               //Объем бочки на в конце времени
            int tryTop = 0;                     //попытки налить
            int tryTopError = 0;                //неудачные попытки налить
            int tryScoop = 0;                   //попытки слить
            int tryScoopError = 0;              //неудачные попытки слить
            int volumeSumTop = 0;               //налитый объем
            int volumeSumTopError = 0;          //не налитый объем
            int volumeSumScoop = 0;             //слитый объем
            int volumeSumScoopError = 0;        //не слитый объем
            double percentTopError;             //процент ошибок
            double percentScoopError;           //процент ошибок


            for (int i = 0; i < rowsSort.Length; i++)
            {
                Regex dateExp = new Regex(@"^\d\d\d\d-\d\d-\d\d[T]\d\d:\d\d:\d\d.\d\d\dZ");     //Дата
                Regex activeExp = new Regex(@"\d+[l]$");                                        //xxl
                Regex volumeChangeExp = new Regex(@"\d+");                          
                string row = rowsSort[i];
                //Получение даты из строки
                Match rowMatchDate = dateExp.Match(row);
                //Получение изменения объема из строки
                Match volumeChangeMatch = volumeChangeExp.Match(Convert.ToString(activeExp.Match(row)));
                volumeChange = Convert.ToInt32(Convert.ToString(volumeChangeMatch));
                //Получение даты действия
                string rowDateTemp = Convert.ToString(rowMatchDate);
                DateTime rowDate = Convert.ToDateTime(rowDateTemp);
                rowDate = rowDate.ToUniversalTime();                //Перевод даты без часового пояса (уже в строке)

                bool act = row.Contains("top"); //Наливает или сливает

                if (rowDate >= dateTimeStart & rowDate <= dateTimeEnd)
                {
                    if (act == true)
                    {

                        volumeTemp = volumeNow + volumeChange;
                        //Начальное значение на промежутке
                        if (flagStart == 0)
                        {
                            volumeStart = volumeNow;
                            flagStart++;
                        }

                        if (volumeTemp <= volumeMax)
                        {
                            volumeNow = volumeTemp;
                            tryTop++;
                            volumeSumTop = volumeSumTop + volumeChange;
                        }
                        else
                        {
                            tryTop++;
                            tryTopError++;
                            volumeSumTopError = volumeSumTopError + volumeChange;
                        }

                    }
                    else
                    {
                        volumeTemp = volumeNow - volumeChange;

                        //Начальное значение на промежутке
                        if (flagStart == 0)
                        {
                            volumeStart = volumeNow;
                            flagStart++;
                        }

                        if (volumeTemp >= 0)
                        {
                            volumeNow = volumeTemp;
                            tryScoop++;
                            volumeSumScoop = volumeSumScoop + volumeChange;
                        }
                        else
                        {
                            tryScoop++;
                            tryScoopError++;
                            volumeSumScoopError = volumeSumScoopError + volumeChange;

                        }
                    }
                }
                else
                {
                    
                    if (act == true)
                    {
                        volumeTemp = volumeNow + volumeChange;

                        if (volumeTemp <= volumeMax)
                        {
                            //Конечное значение на промежутке
                            if (flagStart == 1 & dateTimeEnd < rowDate & flagFinish == 0)
                            {
                                volumeFinish = volumeNow;
                                flagFinish++;
                                //Конец промежутка, дальше нет смысла обрабатывать
                                break;
                            }
                            volumeNow = volumeTemp;
                        }
                    }
                    else
                    {
                        volumeTemp = volumeNow - volumeChange;
                        if (volumeTemp >= 0)
                        {        
                            //Конечное значение на промежутке
                            if (flagStart == 1 & dateTimeEnd < rowDate & flagFinish == 0)
                            {
                                volumeFinish = volumeNow;
                                flagFinish++;
                                //Конец промежутка, дальше нет смысла обрабатывать
                                break;
                            }
                            volumeNow = volumeTemp;
                        }
                    }
                }
            }

            if (flagStart == 1 & flagFinish == 0) volumeFinish = volumeNow;

            if (flagStart == 0 & flagFinish == 0)
            {
                Console.WriteLine("Не было действий за заданный промежуток времени");
                csvCreate = false;
            }
            else
            {
                if (tryTopError != 0) percentTopError = Convert.ToDouble(tryTopError) * 100 / Convert.ToDouble(tryTop);
                else percentTopError = 0;
                if (tryScoopError != 0) percentScoopError = Convert.ToDouble(tryScoopError) * 100 / Convert.ToDouble(tryScoop);
                else percentScoopError = 0;

                //Создаем файл csv
                CreateCSV(path, tryTop, percentTopError, volumeSumTop, volumeSumTopError, tryScoop, percentScoopError, volumeSumScoop, volumeSumScoopError, volumeStart, volumeFinish);
                csvCreate = true;
            }
        }

        static void CreateCSV(string path, int tryTop, double percentTopError, int volumeSumTop, int volumeSumTopError, int tryScoop, double percentScoopError, int volumeSumScoop, int volumeSumScoopError, int volumeStart, int volumeFinish)
        {
            if (!File.Exists(path + "log.csv"))
            {
                using (FileStream csvStream = new FileStream(path + "log.csv", FileMode.Create))
                using (StreamWriter scvWrite = new StreamWriter(csvStream, Encoding.Default))
                {
                    scvWrite.WriteLine("Количество попыток налива; процент ошибок налива; Сумма налитого; Сумма не налитого; Количество попыток слить; Процент ошибок слива; Сумма слитого; Сумма не слитого; Значение объема на старте; Значение объема на финише");
                    scvWrite.Close();
                }
            }

            string row = $"{tryTop}; {percentTopError:F2}; {volumeSumTop}; {volumeSumTopError}; {tryScoop}; {percentScoopError:F2}; {volumeSumScoop}; {volumeSumScoopError}; {volumeStart}; {volumeFinish}";
            File.AppendAllText(path + "log.csv", row + Environment.NewLine);

        }
    }
}





