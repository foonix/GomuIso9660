using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Représente un enregistrement dans la table des répertoires d'une image ISO9660.
    /// </summary>
    public struct PathTableRecordPub
    {
        /// <summary>
        /// Taille de DirectoryID.
        /// </summary>
        public byte Number;

        /// <summary>
        /// Emplacement de l'enregistrement.
        /// </summary>
        public byte Extent;

        /// <summary>
        /// Emplacement de l'extent.
        /// </summary>
        public uint ExtentLocation;

        /// <summary>
        /// Index du répertoire parent.
        /// </summary>
        public ushort ParentNumber;

        /// <summary>
        /// Nom du répertoire.
        /// </summary>
        public string Name;
    }
}
