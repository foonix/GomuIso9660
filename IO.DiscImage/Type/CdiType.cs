using System;
using System.IO;

namespace GomuLibrary.IO.DiscImage.Type
{
    /// <summary>
    /// Classe représentant les images disques au format DiscJuggler.
    /// </summary>
    public class CdiType : IDiscImage
    {
         SectorStructure _sector;
        string _imagePath;

        /// <summary>
        /// Nom de l'image disque .cdi.
        /// </summary>
        public string ImagePath
        {
            get { return _imagePath; }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>CdiType</b>.
        /// </summary>
        public CdiType(string imagePath)
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
            return 150 * _sector.sectorLength;
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

                if (br.BaseStream.Length > 358400)  //taille minimun type image normal (=secteur 2048).
                {
                    byte[] buf = new byte[16];
                    br.Read(buf, 0, buf.Length);

                    if (BytesHelper.CompareBytesArray(BytesHelper.CopyRange(buf, 0, Iso9660Conv.SYNC_HEADER.Length),
                        Iso9660Conv.SYNC_HEADER))
                    {
                        br.BaseStream.Seek(2352, SeekOrigin.Begin);
                        br.Read(buf, 0, buf.Length);

                        //Secteur raw ?.
                        if (BytesHelper.CompareBytesArray(BytesHelper.CopyRange(buf, 0, Iso9660Conv.SYNC_HEADER.Length),
                            Iso9660Conv.SYNC_HEADER))
                        {
                            _sector.sectorLength = 2352;
                            _sector.userDataOffset = 16;
                        }
                        //Autre structure ?.
                        else
                        {
                            br.BaseStream.Seek(2368, SeekOrigin.Begin);
                            br.Read(buf, 0, buf.Length);

                            //Secteur type PQ ?.
                            if (BytesHelper.CompareBytesArray(BytesHelper.CopyRange(buf, 0, Iso9660Conv.SYNC_HEADER.Length),
                                Iso9660Conv.SYNC_HEADER))
                            {
                                _sector.sectorLength = 2368;
                                _sector.userDataOffset = 16;
                            }
                            //Secteur type cd+g ?.
                            else
                            {
                                _sector.sectorLength = 2448;
                                _sector.userDataOffset = 16;
                            }
                        }
                    }
                    else        //secteur normal ?.
                    {
                        _sector.sectorLength = 2048;
                        _sector.userDataOffset = 0;
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
