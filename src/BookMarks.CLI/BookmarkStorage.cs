using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nett;

namespace BookMarks.CLI
{
    class Bookmark
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    class BookmarkStorage
    {
        private const string Key = "bookmarks";
        private readonly string _storeLocation;

        public BookmarkStorage(string storeLocation)
        {
            _storeLocation = storeLocation;
        }

        private void InitConfigFile()
        {
            var filePath = Path.Combine(this._storeLocation, "bookmarks.toml");
            var table = Toml.Create();
            table.Add(Key, Array.Empty<Bookmark>());
            File.WriteAllText(filePath, table.ToString(), Encoding.UTF8);
        }

        public Dictionary<string, Bookmark> ReadBookmarks()
        {
            var filePath = Path.Combine(this._storeLocation, "bookmarks.toml");
            if (File.Exists(filePath) == false)
            {
                InitConfigFile();
            }

            var table = Toml.ReadFile(filePath);
            if (table.TryGetValue(Key, out var obj) == false)
            {
                return new Dictionary<string, Bookmark>(); 
            }
            obj.

            var list = obj.Get<List<Bookmark>>();
            return list.ToDictionary(x => x.Name, x => x);
        }

        public void Save(Dictionary<string, Bookmark> bookmarks)
        {
            var list = bookmarks.Select(x => x.Value).OrderBy(x => x.Name);
            var filePath = Path.Combine(this._storeLocation, "bookmarks.toml");
            var table = Toml.Create();
            table.Add(Key, list);
            File.WriteAllText(filePath, table.ToString(), Encoding.UTF8);
        }

        public void Set(Bookmark bookmark)
        {
            var bookmarks = ReadBookmarks();
            if (bookmarks.ContainsKey(bookmark.Name))
            {
                throw new InvalidOperationException("Name has been taken");
            }

            bookmarks.Add(bookmark.Name, bookmark);
            Save(bookmarks);
        }

        public void Delete(string name)
        {
            var bookmarks = ReadBookmarks();
            bookmarks.Remove(name);
            Save(bookmarks);
        }

        public Bookmark Get(string name)
        {
            var bookmarks = ReadBookmarks();
            var exists = bookmarks.TryGetValue(name, out var bookmark);
            if (exists == false)
            {
                throw new InvalidOperationException($"\"{name}\" was not found");
            }

            return bookmark;
        }
    }
}