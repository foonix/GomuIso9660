using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Définit des méthodes pour récupérer la structure d'une image disque.
    /// </summary>
    public interface IDiscImage
    {
        /// <summary>
        /// Nom d'une image disque.
        /// </summary>
        string ImagePath { get;}

        /// <summary>
        /// Obtient le nombre d'octets précèdents le premier secteur utilisable.
        /// </summary>
        /// <returns>Nombre d'octets à passer.</returns>
        long FirstDumpSector();

        /// <summary>
        /// Obtient des informations sur les secteurs de l'image disque.
        /// </summary>
        /// <returns>Structure <see cref="SectorStructure"/> qui représente des informations sur les secteurs.</returns>
        SectorStructure GetSectorStucture();
    }
}
