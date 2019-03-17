using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;

namespace BookMarks.CLI
{
    [Verb("add", HelpText = "Add a new bookmark")]
    class AddOptions
    {
        [Option('n', "name", Required = true)]
        public string Name { get; set; }

        [Option('p', "path", Required = true)]
        public string Path { get; set; }
    }

    [Verb("rm", HelpText = "Remove a bookmark")]
    class RemoveOptions
    {
        [Option('n', "name", Required = true)]
        public string Name { get; set; }
    }

    [Verb("list", HelpText = "List all bookmarks")]
    class ListOptions
    {
    }

    [Verb("get", HelpText = "Get the bookmark path by name")]
    class GetOptions
    {
        [Option('n', "name", Required = true)]
        public string Name { get; set; }
    }

    class Program
    {
        static void ParseArgs(string[] args)
        {
            var configPath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config", "dotnet-bookmarks");
            var configDir = Directory.CreateDirectory(configPath);
            var storage = new BookmarkStorage(configDir.FullName);
            Parser.Default.ParseArguments<AddOptions, RemoveOptions, ListOptions, GetOptions>(args)
                .WithParsed<AddOptions>(opt =>
                {
                    storage.Set(new Bookmark {Name = opt.Name, Path = opt.Path});
                })
                .WithParsed<RemoveOptions>(opt =>
                {
                    storage.Delete(opt.Name);
                })
                .WithParsed<ListOptions>(opt =>
                {
                    var list = storage.ReadBookmarks().Select(x => x.Value).OrderBy(x => x.Name).ToList();
                    var longestName = list.OrderBy(x => x.Name.Length).FirstOrDefault()?.Name.Length ?? 0;
                    var format = $"{{0,{-longestName}}} {{1}}";
                    if (list.Count == 0)
                    {
                        Console.WriteLine("No bookmarks");
                        return;
                    }
                    Console.WriteLine(format, "Name", "Path");
                    foreach (var bookmark in list)
                    {
                        Console.WriteLine(format, bookmark.Name, bookmark.Path);
                    }
                })
                .WithParsed<GetOptions>(opt =>
                {
                    var bookmark = storage.Get(opt.Name);
                    Console.WriteLine(bookmark.Path);
                })
                .WithNotParsed(errors =>
                {
                    foreach (var error in errors)
                    {
                        Console.Error.WriteLine(error.ToString());
                    }
                });
        }
        static int Main(string[] args)
        {
            try
            {
                ParseArgs(args);
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return 1;
            }
        }
    }
}