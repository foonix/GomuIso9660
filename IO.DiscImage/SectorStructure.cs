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
        /// Emplacement des donn�es utilisateurs dans le secteur.
        /// </summary>
        /// <remarks>La taille des donn�es utilisateurs sont �gales � 2048ko dans un secteur. Donc pour un volume ISO9660 celles-ci se lisent � l'emplacement z�ro du secteur.</remarks>
        public int userDataOffset;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>SectorStructure</b>.
        /// </summary>
        /// <param name="length">Taille du secteur.</param>
        /// <param name="dataOffset">Emplacement des donn�es utilisateurs.</param>
        public SectorStructure(int length, int dataOffset)
        {
            sectorLength = length;
            userDataOffset = dataOffset;
        }
    }
}
