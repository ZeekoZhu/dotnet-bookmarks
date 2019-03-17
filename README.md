# dotnet-bookmarks

## Installation

```shell
git clone https://github.com/ZeekoZhu/dotnet-bookmarks.git
cd dotnet-bookmarks
dotnet publish
cd bin
dotnet tool install -g --add-source ./ BookMarks.CLI
# try it out
bookmarks add -p $(pwd) -n hello
```

## Configuration

It store bookmarks in `$HOME/.config/dotnet-bookmarks/bookmarks.toml`.

