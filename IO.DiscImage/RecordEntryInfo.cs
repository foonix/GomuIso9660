using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Structure contenant les informations sur une entr�e fichier/r�pertoire.
    /// </summary>
    public struct RecordEntryInfo
    {
        /// <summary>
        /// Emplacement de l'entr�e.
        /// </summary>
        public uint Extent;

        /// <summary>
        /// Taille du fichier.
        /// </summary>
        public uint Size;

        /// <summary>
        /// Derni�re date et heure d'�criture du fichier.
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
        /// L'entr�e est un r�pertoire si �gale Vrai.
        /// </summary>
        public bool Directory;

        /// <summary>
        /// L'entr�e est cach� si �gale Vrai.
        /// </summary>
        public bool Hidden;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>RecordEntryInfo</b>.
        /// </summary>
        /// <param name="extent">Emplacement de l'entr�e.</param>
        /// <param name="size">Taille du fichier.</param>
        /// <param name="date">Derni�re date et heure d'�criture du fichier.</param>
        /// <param name="name">Nom du fichier.</param>
        /// <param name="fullPath">Chemin complet du fichier.</param>
        /// <param name="hidden">L'entr�e est cach� si �gale Vrai.</param>
        /// <param name="directory">L'entr�e est un r�pertoire si �gale Vrai.</param>
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
        /// Retourne la date et l'heure de la derni�re modification de l'entr�e.
        /// </summary>
        /// <returns>Structure <see cref="System.DateTime"/> � laquelle sont assign�es la date et l'heure
        /// de la derni�re modification de l'entr�e.</returns>
        public DateTime GetDate()
        {
            DateTime dt = new DateTime(Date[0] + 1900, Date[1], Date[2], Date[3], Date[4], Date[5], DateTimeKind.Utc);

            return dt;
        }
    }
}
