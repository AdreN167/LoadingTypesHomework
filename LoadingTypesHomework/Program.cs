using Castle.Components.DictionaryAdapter;
using LoadingTypesHomework.Data;
using LoadingTypesHomework.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace LoadingTypesHomework
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Придумайте логин или введите существующий: ");

            var rootDirectoryName = Console.ReadLine();

            var rootDirectory = new Directory
            {
                Path = rootDirectoryName,
                Name = rootDirectoryName
            };

            using (var context = new CloudStorageContext())
            {
                if (context.Directories.Where(directory => directory.Name == rootDirectoryName).ToList().Count == 0)
                {
                    context.AddRange(rootDirectory);
                    context.SaveChanges();
                }

                var isEnd = false;
                var currentDirectoryPath = rootDirectoryName;
                var currentDirectory = context.Directories.First(directory => directory.Path == currentDirectoryPath);

                while (!isEnd)
                {
                    Console.Clear();

                    ShowEntitiesFrom(currentDirectory);

                    ShowMenu("-----------------------------------", "Создать папку", "Удалить папку", "Создать файл", "Удалить файл", "Открыть папку", "Назад", "Закрыть");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                    {
                        ThrowError("Некорректный ввод!");
                    }

                    switch (choice)
                    {
                        case 1:
                            if (currentDirectory.Directories == null)
                                currentDirectory.Directories = new List<Directory>();

                            Console.Write("Введите название папки: ");

                            var createDirectoryName = Console.ReadLine();

                            if (currentDirectory.Directories.Where(directory => directory.Name == createDirectoryName).ToList().Count != 0)
                            {
                                ThrowError("Папка с таким именем уже существует!");
                                break;
                            }

                            var newDirectory = new Directory
                            {
                                Name = createDirectoryName,
                                Path = $@"{currentDirectory.Path}\{createDirectoryName}"
                            };

                            currentDirectory.Directories.Add(newDirectory);

                            context.Add(newDirectory);
                            context.Update(currentDirectory);

                            break;

                        case 2:
                            Console.Write("Введите название папки: ");

                            var deleteDirectoryName = Console.ReadLine();

                            if (currentDirectory.Directories.Where(directory => directory.Name == deleteDirectoryName).ToList().Count == 0)
                            {
                                ThrowError("Такой папки не существует существует!");
                                break;
                            }

                            var deleteFiles = context.Files.Where(file => file.Path.StartsWith($@"{currentDirectory.Path}\{deleteDirectoryName}")).ToList();
                            var deleteDirectories = context.Directories.Where(directory => directory.Path.StartsWith($@"{currentDirectory.Path}\{deleteDirectoryName}")).ToList();

                            context.RemoveRange(deleteFiles);
                            context.RemoveRange(deleteDirectories);
                            context.Update(currentDirectory);

                            break;

                        case 3:
                            if (currentDirectory.Files == null)
                                currentDirectory.Files = new List<File>();

                            Console.Write("Введите название файлы с расширением (файл.расширение): ");

                            var createFileName = Console.ReadLine();

                            if (currentDirectory.Files.Where(file => file.Name == createFileName).ToList().Count != 0)
                            {
                                ThrowError("Файл с таким именем уже существует!");
                                break;
                            }

                            var newFile = new File
                            {
                                Name = createFileName,
                                Path = $@"{currentDirectory.Path}\{createFileName}"
                            };

                            currentDirectory.Files.Add(newFile);

                            context.Add(newFile);
                            context.Update(currentDirectory);

                            break;

                        case 4:
                            Console.Write("Введите название файлы с расширением (файл.расширение): ");

                            var deleteFileName = Console.ReadLine();

                            if (currentDirectory.Files.Where(file => file.Name == deleteFileName).ToList().Count == 0)
                            {
                                ThrowError("Такого файла не существует!");
                                break;
                            }

                            var deleteFile = currentDirectory.Files.First(file => file.Name == deleteFileName);

                            currentDirectory.Files.Remove(deleteFile);

                            context.Remove(deleteFile);
                            context.Update(currentDirectory);

                            break;

                        case 5:
                            Console.Write("Введите имя папки: ");
                            var openDirectoryName = Console.ReadLine();

                            if (currentDirectory.Directories.Where(directory => directory.Name == openDirectoryName).ToList().Count == 0)
                            {
                                ThrowError("Такой папки не существует!");
                                break;
                            }

                            currentDirectoryPath = @$"{currentDirectoryPath}\{openDirectoryName}";

                            break;

                        case 6:
                            if (currentDirectoryPath == rootDirectoryName)
                                break;

                            var splitedPath = currentDirectory.Path.Split(@"\");
                            var newPath = "";

                            for (int i = 0; i < splitedPath.Length - 1; i++) 
                            {
                                newPath = $@"{newPath}{splitedPath[i]}";

                                if (i != splitedPath.Length - 2)
                                    newPath = $@"{newPath}\";
                            }

                            currentDirectoryPath = newPath;
                            break;

                        case 7:
                            isEnd = true;
                            break;

                        default:
                            ThrowError("Такого пункта нет!");
                            break;

                    }

                    context.SaveChanges();
                    currentDirectory = context.Directories.First(directory => directory.Path == currentDirectoryPath);
                }
            }
        }

        private static void ShowEntitiesFrom(Directory currentDirectory)
        {
            Console.WriteLine($"Текущая папка: {currentDirectory.Name}");
            Console.WriteLine($"Путь к этой папке: {currentDirectory.Path}");

            Console.WriteLine("\nВнутренние папки: ");
            if (currentDirectory.Directories != null)
            {
                foreach (var directory in currentDirectory.Directories)
                {
                    Console.WriteLine($"\t{directory.Name}");
                }
            }

            Console.WriteLine("\nВнутренние файлы: ");
            if (currentDirectory.Files != null)
            {
                foreach (var file in currentDirectory.Files)
                {
                    Console.WriteLine($"\t{file.Name}");
                }
            }
        }

        private static void ShowMenu(string head, params string[] arguments)
        {
            Console.WriteLine(head);
            for (int i = 0; i < arguments.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {arguments[i]}");
            }
            Console.Write("Ваш выбор: ");
        }

        private static void ThrowError(string message)
        {
            Console.WriteLine($"\n{message}");
            Console.ReadLine();
        }
    }
}
