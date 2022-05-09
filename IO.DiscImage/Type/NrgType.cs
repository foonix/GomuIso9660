using System;

namespace GomuLibrary.IO.DiscImage.Type
{
    /// <summary>
    /// Classe représentant les images disques au format Nero v5 et supérieure.
    /// </summary>
    public class NrgType : IDiscImage
    {
        SectorStructure _sector;
        string _imagePath;

        /// <summary>
        /// Nom de l'image disque .nrg.
        /// </summary>
        public string ImagePath
        {
            get { return _imagePath; }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>NrgType</b>.
        /// </summary>
        public NrgType(string imagePath)
        {
            _imagePath = imagePath;
            _sector = new SectorStructure();
        }

        /// <summary>
        /// Obtient le nombre d'octets précèdents le premier secteur utilisable.
        /// </summary>
        /// <returns>Nombre d'octets à passer.</returns>
        public long FirstDumpSector()
        {
            return 307200;  //150 secteurs x 2048.
        }

        /// <summary>
        /// Obtient des informations sur les secteurs de l'image disque.
        /// </summary>
        /// <returns>Structure <see cref="SectorStructure"/> qui représente des informations sur les secteurs.</returns>
        public SectorStructure GetSectorStucture()
        {
            _sector = new SectorStructure();

            try
            {
                if (new System.IO.FileInfo(_imagePath).Length > 358400 /*350ko*/)
                    _sector = new SectorStructure(2048, 0);
            }
            catch (Exception)
            {
            }
            finally
            {
            }

            return _sector;
        }
    }
}
