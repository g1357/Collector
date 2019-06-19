using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Collector
{
    class Program
    {
        static void Main(string[] args)
        {
            string RootFolder = ""; // Корневая папка для обработки
            string OutputFile = @"output.csv"; // Имя выходного файла
            string OutputPath; // Путь выходного файла

            Debug.WriteLine("Collector v1.02 has started.");
            // Разбор параметров
            Debug.WriteLine("Параметры:");
            foreach(string arg in args)
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
                        Console.WriteLine("Collector v.1.02 от 19.06.2019");
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
                    if (RootFolder[RootFolder.Length-1] == Path.DirectorySeparatorChar)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(4);
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
                }

            }

            Debug.WriteLine("Collector ends its work.");
        }
    }
}
