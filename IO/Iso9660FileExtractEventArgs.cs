using System;

namespace GomuLibrary.IO
{
    /// <summary>
    /// Classe fournissant les données pour l'événement <see cref="Iso9660Reader.Reading"/>.
    /// </summary>
    public class Iso9660FileExtractEventArgs : EventArgs
    {
        uint _StartOffset;
        uint _Length;
        string _File;
        string _Output;
        uint _Position;
        int _CurrentFileIndex;

        /// <summary>
        /// Index du fichier dans la liste.
        /// </summary>
        public int CurrentFileIndex
        {
            get { return _CurrentFileIndex; }
        }

        /// <summary>
        /// Emplacement du fichier de sortie.
        /// </summary>
        public string Output
        {
            get { return _Output; }
        }

        /// <summary>
        /// Position du curseur dans le fichier.
        /// </summary>
        public uint Position
        {
            get { return _Position; }
        }

        /// <summary>
        /// Fichier source.
        /// </summary>
        public string File
        {
            get { return _File; }
        }

        /// <summary>
        /// Taille du fichier.
        /// </summary>
        public uint Length
        {
            get { return _Length; }
        }

        /// <summary>
        /// Emplacement du fichier dans l'ISO.
        /// </summary>
        public uint StartOffset
        {
            get { return _StartOffset; }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>DiscImageExtractEventArgs</b>.
        /// </summary>
        public Iso9660FileExtractEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>DiscImageExtractEventArgs</b>.
        /// </summary>
        /// <param name="file">Fichier source.</param>
        /// <param name="start">Emplacement du fichier dans l'ISO.</param>
        /// <param name="length">Taille du fichier.</param>
        /// <param name="position">Position du curseur dans le fichier.</param>
        /// <param name="output">Emplacement du fichier de sortie.</param>
        /// <param name="index">Index du fichier dans la liste.</param>
        public Iso9660FileExtractEventArgs(string file, uint start, uint length, uint position, string output, int index)
            : base()
        {
            _File = file;
            _StartOffset = start;
            _Length = length;
            _Position = position;
            _Output = output;
            _CurrentFileIndex = index;
        }
    }
}
