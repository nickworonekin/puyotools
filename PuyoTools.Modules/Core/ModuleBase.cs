using System;
using System.Collections.Generic;
using System.IO;

namespace PuyoTools.Modules
{
    public interface IModule { }

    public abstract class ModuleBase : IModule
    {
        /// <summary>
        /// Name of the format.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Returns if data can be written to this format.
        /// </summary>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// The source path of the file.
        /// </summary>
        public string SourcePath = String.Empty;

        /// <summary>
        /// The destination path of the file.
        /// </summary>
        public string DestinationPath = String.Empty;

        /// <summary>
        /// Returns the ModuleSettingsControl associated with this format.
        /// </summary>
        /// <returns></returns>
        public virtual ModuleSettingsControl GetModuleSettingsControl()
        {
            return null;
        }

        #region Is Methods
        /// <summary>
        /// Determines if the data is in the specified format.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="fname">Name of the file.</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public abstract bool Is(Stream source, int length, string fname);

        /// <summary>
        /// Determines if the data is in the specified format.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public bool Is(string path)
        {
            using (FileStream source = File.OpenRead(path))
            {
                return Is(source, (int)source.Length, Path.GetFileName(path));
            }
        }

        /// <summary>
        /// Determines if the data is in the specified format.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="fname">Name of the file.</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public bool Is(Stream source, string fname)
        {
            return Is(source, (int)(source.Length - source.Position), fname);
        }

        /// <summary>
        /// Determines if the data is in the specified format.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="fname">Name of the file.</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public bool Is(byte[] source, string fname)
        {
            return Is(source, 0, source.Length, fname);
        }

        /// <summary>
        /// Determines if the data is in the specified format.
        /// </summary>
        /// <param name="source">Byte array containing the data.</param>
        /// <param name="sourceIndex">Index in the array to start checking at.</param>
        /// <param name="length">Length of the data in the array.</param>
        /// <param name="fname">Name of the file.</param>
        /// <returns>True if the data is in the specified format, false otherwise.</returns>
        public bool Is(byte[] source, int sourceIndex, int length, string fname)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                buffer.Write(source, sourceIndex, length);
                buffer.Position = 0;

                return Is(buffer, length, fname);
            }
        }
        #endregion
    }
}