using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Repr�sente un enregistrement dans la table des r�pertoires d'une image ISO9660.
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
        /// Index du r�pertoire parent.
        /// </summary>
        public ushort ParentNumber;

        /// <summary>
        /// Nom du r�pertoire.
        /// </summary>
        public string Name;
    }
}
