using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Collector
{
    class Program
    {
        public struct MyData
        {
            public string BuildName;
            public string MaterialValue;
            public string SupportMaterial;
            public string BuildTime;
            public string Modeler;
            public string MaterialType;
            public string SliceHeight;
        }
        static void Main(string[] args)
        {
            const string RevDate = "19.06.2019"; // Дата редакции

            string RootFolder = ""; // Корневая папка для обработки
            string OutputFile = @"output.csv"; // Имя выходного файла
            string OutputPath; // Путь выходного файла
            StreamWriter OutputStream = null; // Поток выходного файла
            MyData Data; // Собранные даты

            Debug.WriteLine("Collector started.");

            Assembly Ass = typeof(Program).Assembly;
            AssemblyName AssName = Ass.GetName();
            string Version = AssName.Version.ToString();
            string Name = AssName.Name;
            Debug.WriteLine($"Name: {Name}");
            Debug.WriteLine($"Version: {Version}");

            // Разбор параметров
            Debug.WriteLine("Параметры:");
            foreach (string arg in args)
            {
                Debug.WriteLine(arg);
            }
            var qty = args.Length; // Количество аргументов
            if (qty > 0)
            {
                var arg1 = args[0].Trim();
                switch (arg1)
                {
                    case "-help":
                    case "-h":
                        // Вывод подсказки
                        Console.WriteLine("Запуск:");
                        Console.WriteLine("\t Collector -help  или Collector -h для получения помощи.");
                        Console.WriteLine("\t Collector -info  или Collector -i для получения информации о программе.");
                        Console.WriteLine("\t Collector [<корневая папка>] [-out <выходной файл>]");
                        break;

                    case "-info":
                    case "-i":
                        // Вывод информации о программе
                        Console.WriteLine($"{Name} Ver.{Version} от {RevDate}");
                        Console.WriteLine("(c) E+E.SU (www.epe.su)");
                        break;

                    case "-out":
                    case "-o":
                        // Задан выходной файл
                        // Корневая папка не задана (берём текущую), задан выходной файл
                        RootFolder = Directory.GetCurrentDirectory();
                        if (qty > 1)
                        {
                            OutputFile = args[1];
                            if (OutputFile.IndexOf('.') == -1)
                            {
                                OutputFile += ".csv";
                            }
                        }
                        else
                        {
                            Console.WriteLine("ОШИБКА: Не задан выходной файл!");
                            Environment.Exit(1);
                        }
                        break;

                    default:
                        RootFolder = arg1;
                        if (qty > 1)
                        {
                            var arg2 = args[1];
                            switch (arg2)
                            {
                                case "-out":
                                case "-o":
                                    // Задан выходной файл
                                    if (qty > 2)
                                    {
                                        OutputFile = args[2];
                                        if (OutputFile.IndexOf('.') == -1)
                                        {
                                            OutputFile += ".csv";
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("ОШИБКА: Не задан выходной файл!");
                                        Environment.Exit(1);
                                    }
                                    break;

                                default:
                                    Console.WriteLine("ОШИБКА: Не правильный формат команды!");
                                    Environment.Exit(2);
                                    break;
                            }
                        }
                        break;
                }
            }

            // Разбор аргументов закончен
            Debug.WriteLine($"Корневой каталог: {RootFolder}");
            Debug.WriteLine($"Выходной файл: {OutputFile}");

            // Проверка существвания каталога
            if (!Directory.Exists(RootFolder))
            {
                Console.WriteLine("ОШИБКА: Корневой каталог не существует или задан не правильно!");
                Environment.Exit(3);
            }
            // Создание выходного файла
            try
            {
                if (OutputFile.IndexOf(Path.DirectorySeparatorChar) == -1)
                {
                    if (RootFolder[RootFolder.Length - 1] == Path.DirectorySeparatorChar)
                    {
                        OutputPath = RootFolder + OutputFile;
                    }
                    else
                    {
                        OutputPath = RootFolder + Path.DirectorySeparatorChar + OutputFile;
                    }
                }
                else
                {
                    OutputPath = OutputFile;
                }
                Debug.WriteLine($"Путь выходного файла: {OutputPath}");

                bool FileExist = File.Exists(OutputPath);
                try
                {
                    OutputStream = File.AppendText(OutputPath);
                    if (!FileExist)
                    { // Вывести шапку таблицы
                        OutputStream.WriteLine("Build name,Model material value,Support material,Est.build time,Printer (Modeler), Material type, Slice height");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Environment.Exit(4);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(5);
            }

            // Получить список подкаталогов
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(RootFolder));
            // Перебираем подкаталоги
            foreach (var dir in dirs)
            {
                Debug.Write($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)} - ");
                Debug.WriteLine($"{dir}");
                var txtFiles = Directory.EnumerateFiles(dir, "log.txt");

                foreach (string currentFile in txtFiles)
                {
                    Debug.Write($"{currentFile.Substring(currentFile.LastIndexOf(Path.DirectorySeparatorChar) + 1)} - ");
                    Debug.WriteLine($"{currentFile}");
                    Data = new MyData();
                    Data.BuildName = dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    GetData(currentFile, ref Data);
                    try
                    {
                        OutputStream.WriteLine($"{Data.BuildName},{Data.MaterialValue},{Data.SupportMaterial},"
                            + $"{Data.BuildTime},{Data.Modeler},{Data.MaterialType},{Data.SliceHeight}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Environment.Exit(6);
                    }

                }
            }

            if (OutputStream != null)
            {
                OutputStream.Close();
                OutputStream.Dispose();
            }

            Debug.WriteLine("Collector ends its work.");
        }
        static void GetData(string FilePath, ref MyData Data)
        {
            int Pos;
            string Pic;
            string strValue;
            decimal decValue;
            int Hours;
            int Minutes;

            foreach (string line in File.ReadLines(FilePath))
            {
                if (line.Contains("Modeler:"))
                {
                    Debug.WriteLine(line);
                    Pic = "\t";
                    Pos = line.LastIndexOf(Pic) + Pic.Length;
                    Data.Modeler = line.Substring(Pos,line.IndexOf("}")-Pos).Trim();
                    Debug.WriteLine($"[{Data.Modeler}]");
                }
                else if (line.Contains("Est. build time:"))
                {
                    Debug.WriteLine(line);
                    Pic = "\t";
                    Pos = line.LastIndexOf(Pic) + Pic.Length;
                    strValue = line.Substring(Pos, line.IndexOf("}") - Pos).Trim().Trim();
                    int.TryParse(strValue.Substring(0, strValue.IndexOf("hr")).Trim(), out Hours);
                    Pos = strValue.IndexOf("hr") + 3;
                    int.TryParse(strValue.Substring(Pos, strValue.IndexOf("min") - Pos - 1).Trim(), out Minutes);
                    Data.BuildTime = string.Format("{0,2:D2}:{1,2:D2}", Hours, Minutes);
                    Debug.WriteLine($"[{Data.BuildTime}]");
                }
                else if (line.Contains("Model material:"))
                {
                    Debug.WriteLine(line);
                    Pic = "Model material:";
                    Pos = line.LastIndexOf(Pic) + Pic.Length;
                    strValue = line.Substring(Pos, line.IndexOf("cm") - Pos).Trim();
                    decimal.TryParse(strValue, out decValue);
                    decValue = Math.Round(decValue + 0.005m, 2);
                    Data.MaterialValue = decValue.ToString();
                    Debug.WriteLine($"[{Data.MaterialValue}]");

                    Pic = "\t";
                    Pos = line.LastIndexOf(Pic) + Pic.Length;
                    Data.MaterialType = line.Substring(Pos, line.IndexOf("}") - Pos).Trim();
                    Debug.WriteLine($"[{Data.MaterialType}]");
                }
                else if (line.Contains("Support material:"))
                {
                    Debug.WriteLine(line);
                    Pic = "Support material:";
                    Pos = line.LastIndexOf(Pic) + Pic.Length;
                    strValue = line.Substring(Pos, line.IndexOf("cm") - Pos).Trim();
                    decimal.TryParse(strValue, out decValue);
                    decValue = Math.Round(decValue + 0.005m, 2);
                    Data.SupportMaterial = decValue.ToString();
                    Debug.WriteLine($"[{Data.SupportMaterial}]");
                }
                else if (line.Contains("Slice height:"))
                {
                    Debug.WriteLine(line);
                    Pic = "\t";
                    Pos = line.LastIndexOf(Pic) + Pic.Length;
                    Data.SliceHeight = line.Substring(Pos, line.IndexOf("}") - Pos).Trim();
                    Debug.WriteLine($"[{Data.SliceHeight}]");
                }
            }
        }
    }
}
