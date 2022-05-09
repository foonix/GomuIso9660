using System;
using System.IO;

namespace GomuLibrary.IO.DiscImage.Type
{
    /// <summary>
    /// Classe représentant les images disques au format Alcohol.
    /// </summary>
    public class MdfType : IDiscImage
    {
        byte[] SYNC_HEADER_MDF = { 0x80, 0xc0, 0x80, 0x80, 0x80, 0x80, 0x80, 0xc0, 0x80, 0x80, 0x80, 0x80 };

        SectorStructure _sector;
        string _imagePath;

        /// <summary>
        /// Nom de l'image disque .mdf.
        /// </summary>
        public string ImagePath
        {
            get { return _imagePath; }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>MdfType</b>.
        /// </summary>
        public MdfType(string imagePath)
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

                byte[] buf = new byte[12];
                br.Read(buf, 0, buf.Length);

                if (BytesHelper.CompareBytesArray(buf, Iso9660Conv.SYNC_HEADER))
                {
                    br.BaseStream.Seek(2352, SeekOrigin.Begin);
                    br.Read(buf, 0, buf.Length);

                    if (BytesHelper.CompareBytesArray(buf, SYNC_HEADER_MDF))
                    {
                        _sector.sectorLength = 2448;
                        _sector.userDataOffset = 16;
                    }
                    else
                    {
                        _sector.sectorLength = 2352;
                        _sector.userDataOffset = 16;
                    }
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
