using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// D�finit des m�thodes pour r�cup�rer la structure d'une image disque.
    /// </summary>
    public interface IDiscImage
    {
        /// <summary>
        /// Nom d'une image disque.
        /// </summary>
        string ImagePath { get;}

        /// <summary>
        /// Obtient le nombre d'octets pr�c�dents le premier secteur utilisable.
        /// </summary>
        /// <returns>Nombre d'octets � passer.</returns>
        long FirstDumpSector();

        /// <summary>
        /// Obtient des informations sur les secteurs de l'image disque.
        /// </summary>
        /// <returns>Structure <see cref="SectorStructure"/> qui repr�sente des informations sur les secteurs.</returns>
        SectorStructure GetSectorStucture();
    }
}
