using System;

namespace GomuLibrary.IO
{
    /// <summary>
    /// Classe représentant les infos de base d'une entrée dans un Volume.
    /// </summary>
    public class EntryInfo
    {
        private string _path = string.Empty;
        private bool _isDirectory = false;
        private bool _isHidden = false;
        private DateTime _lastWrite;

        /// <summary>
        /// Obtient la dernière date d'accès en écriture.
        /// </summary>
        public DateTime LastWriteAccess
        {
            get { return _lastWrite; }
        }

        /// <summary>
        /// Précise si l'entrée à l'attribut "Cachée".
        /// </summary>
        public bool IsHidden
        {
            get { return _isHidden; }
        }

        /// <summary>
        /// Précise si l'entrée est un répertoire ou un fichier.
        /// </summary>
        public bool IsDirectory
        {
            get { return _isDirectory; }
        }

        /// <summary>
        /// Obtient le chemin complet de l'entrée.
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// Conversion explicite d'une structure <see cref="DiscImage.RecordEntryInfo"/> 
        /// vers la structure <see cref="EntryInfo"/>.
        /// </summary>
        /// <param name="record">Structure <see cref="DiscImage.RecordEntryInfo"/>.</param>
        /// <returns>Une structure <see cref="EntryInfo"/>.</returns>
        public static explicit operator EntryInfo(DiscImage.RecordEntryInfo record)
        {
            EntryInfo ei = new EntryInfo();

            ei._isDirectory = record.Directory;
            ei._isHidden = record.Hidden;
            ei._path = record.FullPath;
            ei._lastWrite = record.GetDate();

            return ei;
        }
    }
}
