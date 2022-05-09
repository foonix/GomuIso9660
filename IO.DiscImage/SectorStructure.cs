using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Stocke les informations sur un secteur.
    /// </summary>
    public struct SectorStructure
    {
        /// <summary>
        /// Taille du secteur.
        /// </summary>
        /// <remarks>La taille pour un secteur d'un volume ISO9660 est 2048ko.</remarks>
        public int sectorLength;

        /// <summary>
        /// Emplacement des données utilisateurs dans le secteur.
        /// </summary>
        /// <remarks>La taille des données utilisateurs sont égales à 2048ko dans un secteur. Donc pour un volume ISO9660 celles-ci se lisent à l'emplacement zéro du secteur.</remarks>
        public int userDataOffset;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>SectorStructure</b>.
        /// </summary>
        /// <param name="length">Taille du secteur.</param>
        /// <param name="dataOffset">Emplacement des données utilisateurs.</param>
        public SectorStructure(int length, int dataOffset)
        {
            sectorLength = length;
            userDataOffset = dataOffset;
        }
    }
}
