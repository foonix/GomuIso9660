using System;

namespace GomuLibrary.IO
{
    /// <summary>
    /// Classe repr�sentant les infos de base d'une entr�e dans un Volume.
    /// </summary>
    public class EntryInfo
    {
        private string _path = string.Empty;
        private bool _isDirectory = false;
        private bool _isHidden = false;
        private DateTime _lastWrite;

        /// <summary>
        /// Obtient la derni�re date d'acc�s en �criture.
        /// </summary>
        public DateTime LastWriteAccess
        {
            get { return _lastWrite; }
        }

        /// <summary>
        /// Pr�cise si l'entr�e � l'attribut "Cach�e".
        /// </summary>
        public bool IsHidden
        {
            get { return _isHidden; }
        }

        /// <summary>
        /// Pr�cise si l'entr�e est un r�pertoire ou un fichier.
        /// </summary>
        public bool IsDirectory
        {
            get { return _isDirectory; }
        }

        /// <summary>
        /// Obtient le chemin complet de l'entr�e.
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
