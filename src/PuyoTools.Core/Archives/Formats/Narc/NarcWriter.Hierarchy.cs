using PuyoTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuyoTools.Archives.Formats.Narc
{
    public partial class NarcWriter
    {
        private class Hierarchy
        {
            private readonly List<DirectoryEntry> _directories = new();
            private readonly List<FileEntry> _files = new();

            private int _fileEntryDataLength = 0;

            private static readonly char[] s_directorySeperators =
            {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar,
            };

            public Hierarchy(IEnumerable<ArchiveWriterEntry> entries)
            {
                DirectoryEntry rootDirectory = new()
                {
                    Name = string.Empty,
                };

                // Loop through the archive entries and create the appropiate directory & file entries.
                foreach (ArchiveWriterEntry entry in entries)
                {
                    DirectoryEntry currentDirectory = rootDirectory;

                    string[] paths = entry.FullName.Split(s_directorySeperators, StringSplitOptions.RemoveEmptyEntries);
                    int pathIndex = 0;
                    for (; pathIndex < paths.Length - 1; pathIndex++)
                    {
                        // Check to see if the current directory has already been created.
                        // If not, create it and add it to its parent's list of child directories.
                        if (!currentDirectory.Directories.TryGetValue(paths[pathIndex], out DirectoryEntry directory))
                        {
                            directory = new DirectoryEntry
                            {
                                Parent = currentDirectory,
                                Name = paths[pathIndex],
                            };
                            currentDirectory.Entries.Add(directory);
                            currentDirectory.Directories.Add(directory.Name, directory);
                        }

                        currentDirectory = directory;
                    }

                    // Create the file entry and add it to the curent directory.
                    FileEntry fileEntry = new()
                    {
                        Entry = entry,
                        Name = paths[pathIndex],
                        Length = (int)entry.Length,
                    };
                    currentDirectory.Entries.Add(fileEntry);
                }

                _directories.Add(rootDirectory);

                ushort directoryIndex = 1; // The root directory index is always 0.
                ushort fileIndex = 0;
                int position = 0;
                for (int i = 0; i < _directories.Count; i++) // We'll be modifying directories during the loop, so we can't use a foreach loop here.
                {
                    foreach (Entry entry in _directories[i].Entries)
                    {
                        if (entry is DirectoryEntry directoryEntry)
                        {
                            directoryEntry.Index = directoryIndex;
                            directoryEntry.FirstFileIndex = fileIndex;

                            _directories.Add(directoryEntry);

                            directoryIndex++;
                        }
                        else if (entry is FileEntry fileEntry)
                        {
                            fileEntry.Index = fileIndex;
                            fileEntry.Offset = position;

                            position += MathHelper.RoundUp(fileEntry.Length, 4); // Offsets must be a multiple of 4

                            _files.Add(fileEntry);

                            fileIndex++;
                        }
                    }
                }
                _fileEntryDataLength = position;
            }

            public List<DirectoryEntry> Directories => _directories;

            public List<FileEntry> Files => _files;

            public int FileEntryDataLength => _fileEntryDataLength;
        }
    }
}
