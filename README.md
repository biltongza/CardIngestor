# CardIngestor

## About

This is a tool I am writing to make my life easier to ingest media from my camera on my Mac. I have a Canon EOS 1200D and 77D, which work just fine with Canon's EOS Utility tools, but having to plug them in via USB on a 2021 Mac requires the use of a USB C adapter, which seems kinda pointless given there is a full size SD card slot on them.

So far, I have only used it on MacOS 12.0.1, but it is built in a very modular way to be able to adapt to any platform you can run .NET on.

The architecture also allows for customization of the ingestion process, but so far I have only written a simple clone of what EOS Utility does as that fits my personal needs.

The program runs as a background service, monitoring for any removable drives that get attached to the system, and then checking if the drive has a config file that tells the program what to ingest, how to do it, and where to put the media.

## Usage

`dotnet run` is all you need. Proper installation support is planned.

## Configuration

Configuration is done via a `.ingest.json` file on the root of your removable media. The file describes what files to search for, where to put them, and how to get them there.

### Example

An example config file follows:

```
{
    "SearchPattern": "*.cr2",
    "Destinations": [
        {
            "MachineName": "Logans-MacBook-Pro",
            "IngestionStrategy": "FileCreationDateFolderIngestionStrategy",
            "StrategyParams": {
                "Destination": "/users/logan/Pictures/"
            }
        }
    ]
}
```

### Structure

- `SearchPattern`: the search pattern to use to look for files on the media to ingest. Internally it uses .NET's `Directory.GetFiles()` documented [here](https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=net-6.0#System_IO_Directory_GetFiles_System_String_System_String_).
- `Destinations`: An array of destination objects. This is intended to handle using different configuration for different machines, in case you use your media on multiple machines.
  - `MachineName`: an identifier that is used to filter which destination applies to the currently running machine.
  - `IngestionStrategy`: which ingestion strategy to use. Currently there is only a single one: `FileCreationDateFolderIngestionStrategy`.
  - `StrategyParams`: a dictionary of parameters passed to the strategy to customise the process. In this example, it is used to specify the root destination folder for ingested media.

## Ingestion Strategies

An ingestion strategy determines the destination for all of the source media that is identified. As an example, they can be used to group media based on specific attributes.

At present, there is a single strategy provided: `FileCreationDateFolderIngestionStrategy`. It groups source files based on their file creation date, and puts the files in subfolders accordingly.

## Roadmap

- [ ] Windows support
- [ ] Installers
- [ ] Some sort of UI to configure ingestion
- [ ] UI to indicate ingestion progress
