using System;
using System.Collections.Generic;
using System.IO;

namespace Collector
{
    class Program
    {
        static void Main(string[] args)
        {
            string RootFolder = ""; // Корневая папка для обработки
            string OutputFile = @"output.csv"; // Имя выходного файла

            Console.WriteLine("Collector v1.02 has started.");
            // Разбор параметров
            Console.WriteLine("Параметры:");
            foreach(string arg in args)
            {
                Console.WriteLine(arg);
            }
            var qty = args.Length; // Количество аргументов
            if (qty > 0)
            {
                var arg1 = args[0].Trim();
                if (arg1 == "-help" && arg1 == "-h")
                { // Запрос выводв подсказки
                    Console.WriteLine("Запуск: Collector -help  или Collector -h для получения помощи.");
                    Console.WriteLine("Запуск: Collector [<корневая папка>] [-out <выходной файл>]");
                }
                else if (arg1 == "-out")
                { // Корневая папка не задана (берём текущую), задан выходной файл
                    RootFolder = Directory.GetCurrentDirectory();
                    if (qty > 1)
                    {
                        OutputFile = args[1];
                        if ( OutputFile.IndexOf('.') == -1)
                        {
                            OutputFile += ".csv";
                        }
                    }
                    else
                    {
                        Console.WriteLine("ОШИБКА: Не задан выходной файл!");
                        Environment.Exit(1);
                    }
                }
                RootFolder = arg1;

                if (qty > 1)
                {
                    var arg2 = args[1];
                    if (arg2 == "-out")
                    { // Задан выходной файл
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
                    }
                }
            }

            // Разбор аргументов закончен
            Console.WriteLine($"Корневой каталог: {RootFolder}"); 
            Console.WriteLine($"Выходной файл: {OutputFile}");

            // Проверка существвания каталога
            if (!Directory.Exists(RootFolder))
            {
                Console.WriteLine("ОШИБКА: Корневой каталог не существует или задан не правильно!");
                Environment.Exit(2);
            }
            // Создание выходного файла
            try
            {
                if (OutputFile.IndexOf(Path.DirectorySeparatorChar) == -1)
                {
                    //OutputPath = RootFolder
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(3);
            }
            // Получить список подкаталогов
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(RootFolder));
            // Перебираем подкаталоги
            foreach (var dir in dirs)
            {
                Console.WriteLine($"{dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                var txtFiles = Directory.EnumerateFiles(dir, "log.txt");

                foreach (string currentFile in txtFiles)
                {
                    Console.WriteLine($"{currentFile.Substring(currentFile.LastIndexOf(Path.DirectorySeparatorChar) + 1)}");
                }

            }

            Console.WriteLine("Collector ends its work.");
        }
    }
}
