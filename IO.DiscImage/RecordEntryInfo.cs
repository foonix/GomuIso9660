using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Structure contenant les informations sur une entrée fichier/répertoire.
    /// </summary>
    public struct RecordEntryInfo
    {
        /// <summary>
        /// Emplacement de l'entrée.
        /// </summary>
        public uint Extent;

        /// <summary>
        /// Taille du fichier.
        /// </summary>
        public uint Size;

        /// <summary>
        /// Dernière date et heure d'écriture du fichier.
        /// </summary>
        public byte[] Date;

        /// <summary>
        /// Nom du fichier.
        /// </summary>
        public string Name;

        /// <summary>
        /// Chemin complet du fichier.
        /// </summary>
        public string FullPath;

        /// <summary>
        /// L'entrée est un répertoire si égale Vrai.
        /// </summary>
        public bool Directory;

        /// <summary>
        /// L'entrée est caché si égale Vrai.
        /// </summary>
        public bool Hidden;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>RecordEntryInfo</b>.
        /// </summary>
        /// <param name="extent">Emplacement de l'entrée.</param>
        /// <param name="size">Taille du fichier.</param>
        /// <param name="date">Dernière date et heure d'écriture du fichier.</param>
        /// <param name="name">Nom du fichier.</param>
        /// <param name="fullPath">Chemin complet du fichier.</param>
        /// <param name="hidden">L'entrée est caché si égale Vrai.</param>
        /// <param name="directory">L'entrée est un répertoire si égale Vrai.</param>
        public RecordEntryInfo(uint extent, uint size, byte[] date, string name, string fullPath,
            bool hidden, bool directory)
        {
            Extent = extent;
            Size = size;
            Date = date;
            Name = name;
            FullPath = fullPath;
            Hidden = hidden;
            Directory = directory;
        }

        /// <summary>
        /// Retourne la date et l'heure de la dernière modification de l'entrée.
        /// </summary>
        /// <returns>Structure <see cref="System.DateTime"/> à laquelle sont assignées la date et l'heure
        /// de la dernière modification de l'entrée.</returns>
        public DateTime GetDate()
        {
            DateTime dt = new DateTime(Date[0] + 1900, Date[1], Date[2], Date[3], Date[4], Date[5], DateTimeKind.Utc);

            return dt;
        }
    }
}
