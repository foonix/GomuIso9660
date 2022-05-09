using System;
using System.IO;

namespace GomuLibrary.IO.DiscImage.Type
{
    /// <summary>
    /// Classe représentant les images disques au format .bin.
    /// </summary>
    public class BinType : IDiscImage
    {
        SectorStructure _sector;
        string _imagePath;

        /// <summary>
        /// Nom de l'image disque .bin.
        /// </summary>
        public string ImagePath
        {
            get { return _imagePath; }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>BinType</b>.
        /// </summary>
        public BinType(string imagePath)
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
            return 0;
        }

        /// <summary>
        /// Obtient des informations sur les secteurs de l'image disque.
        /// </summary>
        /// <returns>Structure <see cref="SectorStructure"/> qui représente des informations sur les secteurs.</returns>
        public SectorStructure GetSectorStucture()
        {
            _sector = new SectorStructure();
            FileStream fs = null;
            BinaryReader br = null;

            try
            {
                fs = new FileStream(_imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                br = new BinaryReader(fs);

                byte[] buf = new byte[16];
                br.Read(buf, 0, buf.Length);

                if (BytesHelper.CompareBytesArray(BytesHelper.CopyRange(buf, 0, Iso9660Conv.SYNC_HEADER.Length),
                    Iso9660Conv.SYNC_HEADER))
                {
                    switch (buf[15])
                    {
                        case 1:
                            _sector.sectorLength = 2352;
                            _sector.userDataOffset = 16;
                            break;
                        case 2:
                            _sector.sectorLength = 2352;
                            _sector.userDataOffset = 24;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    _sector.sectorLength = 2336;
                    _sector.userDataOffset = 8;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (br != null)
                    br.Close();

                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            return _sector;
        }
    }
}
