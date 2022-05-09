using System;
using System.IO;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Classe fournissant une méthode de convertion des images disques de divers formats vers le format ISO9660.
    /// </summary>
    public class Iso9660Conv
    {
        //Taille d'un bloc du système de fichiers CDFS ISO9660.
        const int ISO_SECTOR_SIZE = 2048;
        //Nombre de secteur à traiter par lecture/écriture.
        const int BUFFER_LENGTH_FACTOR = 64;    //x128ko.
        //Taille max. d'un lecteur FAT32.
        const uint FAT32_DISK_LIMIT = uint.MaxValue;

        SectorStructure _sectorStruct;
        long _startFirstSector;
        private IDiscImage _discImage;

        FileStream _fsSrc = null;
        FileStream _fsDst = null;
        BinaryReader _brSrc = null;
        BinaryWriter _wrDst = null;

        /// <summary>
        /// Bloc d'en-tête qui définissant touts fichiers images non iso.
        /// </summary>
        internal static byte[] SYNC_HEADER = { 0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0 };

        /// <summary>
        /// Evénement pour les informations sur l'avancement de la convertion.
        /// </summary>
        public event EventHandler<Iso9660CreatorEventArgs> Progression;

        /// <summary>
        /// Evénement pour la fin d'une extraction.
        /// </summary>
        public event EventHandler Terminate;

        /// <summary>
        /// Obtient/Définit un objet implémentant <see cref="IDiscImage"/> représentant une image disque.
        /// </summary>
        public IDiscImage DiscImage
        {
            get { return _discImage; }
            set 
            {
                _discImage = value;
                SetDiscImage();
            }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>Iso9660Conv</b>.
        /// </summary>
        public Iso9660Conv()
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>Iso9660Conv</b>.
        /// </summary>
        /// <param name="converter">Tout objet qui implémente <see cref="IDiscImage"/> qui représente une image disque à convertir.</param>
        /// <example>
        /// L'exemple de code suivant convertit un fichier image Alcohol (.mdf) vers le format iso.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///            Iso9660Conv isoConv = new Iso9660Conv();
        ///
        ///            try
        ///            {
        ///                isoConv = new Iso9660Conv(new GomuLibrary.IO.DiscImage.Type.NrgType(@"C:\MyImage.nrg"));
        /// 
        ///                isoConv.Terminate += new EventHandler(delegate(object sender, EventArgs e)
        ///                {
        ///                    Console.WriteLine(@"Conversion complete !.");
        ///                    Console.Read();
        ///                });
        ///
        ///                isoConv.Convert(@"C:\MyConvertedImage.iso");
        ///            }
        ///            finally
        ///            {
        ///                if (isoConv != null)
        ///                    isoConv.Close();
        ///            }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public Iso9660Conv(IDiscImage converter)
        {
            _discImage = converter;
            SetDiscImage();
        }

        /// <summary>
        /// Ferme les fichiers ouverts et libère leurs ressources utilisées.
        /// </summary>
        public void Close()
        {
            if (_brSrc != null)
                _brSrc.Close();

            if (_wrDst != null)
                _wrDst.Close();

            if (_fsSrc != null)
            {
                _fsSrc.Close();
                _fsSrc.Dispose();
            }

            if (_fsDst != null)
            {
                _fsDst.Close();
                _fsDst.Dispose();
            }
        }

        /// <summary>
        /// Convertit l'image vers le format iso.
        /// </summary>
        /// <param name="discImage">Tout objet qui implémente <see cref="IDiscImage"/> qui représente une image disque à convertir.</param>
        /// <param name="isoDestination">Fichier iso de destination.</param>
        /// <example>
        /// L'exemple de code suivant convertit un fichier image .bin vers le format iso.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///            Iso9660Conv isoConv = new Iso9660Conv();
        ///
        ///            try
        ///            {
        ///                GomuLibrary.IO.DiscImage.Type.BinType binImage = new GomuLibrary.IO.DiscImage.Type.BinType(@"C:\MyImage.bin");
        ///
        ///                isoConv.Terminate += new EventHandler(delegate(object sender, EventArgs e)
        ///                {
        ///                    Console.WriteLine(@"Conversion complete !.");
        ///                    Console.Read();
        ///                });
        ///
        ///                isoConv.Convert(binImage, @"C:\MyConvertedImage.iso");
        ///            }
        ///            finally
        ///            {
        ///                if (isoConv != null)
        ///                    isoConv.Close();
        ///            }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">Soit:
        /// <list type="bullet">
        /// <item><description>Aucun objet implémentant l'interface <see cref="IDiscImage"/> n'a été définit.</description></item>
        /// <item><description><paramref name="discImage"/> est nul.</description></item>
        /// <item><description><paramref name="isoDestination"/> est nul.</description></item>
        /// </list>
        /// </exception>
        /// <exception cref="IOException">Une erreur d'E/S s'est produite. Soit:
        /// <list type="bullet">
        /// <item><description>Le fichier image source définit par l'interface <see cref="IDiscImage"/> n'existe pas.</description></item>
        /// <item><description>Le format du fichier image source définit par l'interface <see cref="IDiscImage"/> est inconnu.</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est en FAT32 et la taille du fichier convertie est supérieur à la taille max. d'une partition FAT32 (<see cref="uint.MaxValue"/>).</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est insuffisant.</description></item>
        /// </list>
        /// </exception>
        public void Convert(IDiscImage discImage, string isoDestination)
        {
            if (discImage == null)
                throw new ArgumentNullException("discImage", "The IDiscImage interface is null.");

            _discImage = discImage;
            SetDiscImage();

            this.Convert(isoDestination);
        }

        /// <summary>
        /// Convertit l'image vers le format iso.
        /// </summary>
        /// <param name="isoDestination">Fichier iso de destination.</param>
        /// <example>
        /// L'exemple de code suivant convertit un fichier image Alcohol (.mdf) vers le format iso.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///            Iso9660Conv isoConv = new Iso9660Conv();
        ///
        ///            try
        ///            {
        ///                GomuLibrary.IO.DiscImage.Type.MdfType mdfImage = new GomuLibrary.IO.DiscImage.Type.MdfType(@"C:\MyImage.mdf");
        ///                isoConv.DiscImage = mdfImage;
        /// 
        ///                isoConv.Terminate += new EventHandler(delegate(object sender, EventArgs e)
        ///                {
        ///                    Console.WriteLine(@"Conversion complete !.");
        ///                    Console.Read();
        ///                });
        ///
        ///                isoConv.Convert(@"C:\MyConvertedImage.iso");
        ///            }
        ///            finally
        ///            {
        ///                if (isoConv != null)
        ///                    isoConv.Close();
        ///            }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">Soit:
        /// <list type="bullet">
        /// <item><description>Aucun objet implémentant l'interface <see cref="IDiscImage"/> n'a été définit.</description></item>
        /// <item><description><paramref name="isoDestination"/> est nul.</description></item>
        /// </list>
        /// </exception>
        /// <exception cref="IOException">Une erreur d'E/S s'est produite. Soit:
        /// <list type="bullet">
        /// <item><description>Le fichier image source définit par l'interface <see cref="IDiscImage"/> n'existe pas.</description></item>
        /// <item><description>Le format du fichier image source définit par l'interface <see cref="IDiscImage"/> est inconnu.</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est en FAT32 et la taille du fichier convertie est supérieur à la taille max. d'une partition FAT32 (<see cref="uint.MaxValue"/>).</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est insuffisant.</description></item>
        /// </list>
        /// </exception>
        public void Convert(string isoDestination)
        {
            if (_discImage == null)
                throw new ArgumentNullException("The IDiscImage interface is null. You can specified an instance from DiscImage propertie.");

            if (isoDestination == string.Empty)
                throw new ArgumentNullException("output", @"The output file path must not be null.");

            if (_sectorStruct.sectorLength == 0)
                throw new IOException("The source image file format is unknown.");

            if (!File.Exists(_discImage.ImagePath))
                throw new FileNotFoundException("The souce image file does not exists.");

            //Vérif IO.
            DriveInfo diOutput = new DriveInfo(Path.GetPathRoot(isoDestination));
            FileInfo fiInput = new FileInfo(_discImage.ImagePath);
            long lIsoSize = ((fiInput.Length / _sectorStruct.sectorLength) * ISO_SECTOR_SIZE) - _startFirstSector;

            if (diOutput.DriveFormat.Equals("FAT32") && (lIsoSize > FAT32_DISK_LIMIT))
                throw new IOException(@"Not enough space on FAT32 destination drive.");

            if (diOutput.TotalSize < lIsoSize)
                throw new IOException(@"Not enough space on the destination drive.");

            try
            {
                //Ouverture du fichier image disque source en lecture.
                _fsSrc = new FileStream(_discImage.ImagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _brSrc = new BinaryReader(_fsSrc);

                //Ouvre un flux en écriture sur le fichier iso de sortie.
                _fsDst = new FileStream(isoDestination, FileMode.Create, FileAccess.Write, FileShare.None);
                _wrDst = new BinaryWriter(_fsDst);

                //Déplace le curseur au 1ier secteur utilisable de la source.
                _brSrc.BaseStream.Seek(_startFirstSector, SeekOrigin.Begin);
                
                //Buffer.
                byte[] buf = new byte[_sectorStruct.sectorLength * BUFFER_LENGTH_FACTOR];
                //Recoit le nombre d'octets lu.
                int iRead = 0;

                //Convertion...
                do
                {
                    iRead = _brSrc.Read(buf, 0, buf.Length);
                    if (iRead == _sectorStruct.sectorLength * BUFFER_LENGTH_FACTOR)
                    {
                        _wrDst.Write(CleanUserDataBuffer(buf, ISO_SECTOR_SIZE * BUFFER_LENGTH_FACTOR));
                    }
                    else if (iRead > 0)
                    {
                        uint uiRead2 = global::System.Convert.ToUInt32(
                            (iRead / _sectorStruct.sectorLength) * ISO_SECTOR_SIZE);

                        _wrDst.Write(CleanUserDataBuffer(buf, uiRead2));
                    }

                    if (Progression != null)
                        Progression(null, new Iso9660CreatorEventArgs(_wrDst.BaseStream.Position, _brSrc.BaseStream.Length));

                } while ((iRead > 0) && (_wrDst.BaseStream.Position != lIsoSize));

                if (Terminate != null)
                    Terminate(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// Nettoie le buffer contenant les données des secteurs pour ne garder 
        /// que leurs données utilisateurs.
        /// </summary>
        /// <param name="source">Buffer d'origine.</param>
        /// <param name="dataSize">Taille des données utilisateurs.</param>
        /// <returns>Un tableau de Byte contenant uniquement les données utilisateurs des secteurs.</returns>
        byte[] CleanUserDataBuffer(byte[] source, uint dataSize)
        {
            byte[] bufDst = new byte[dataSize];

            int icount = 0;
            for (int i = 0; i < dataSize; i += ISO_SECTOR_SIZE)
            {
                if ((dataSize - i) >= ISO_SECTOR_SIZE)
                {
                    Buffer.BlockCopy(source, (icount * _sectorStruct.sectorLength) + _sectorStruct.userDataOffset, bufDst,
                        icount * ISO_SECTOR_SIZE, ISO_SECTOR_SIZE);

                    icount++;
                }
                else
                {
                    Buffer.BlockCopy(source, (_sectorStruct.sectorLength * icount) + _sectorStruct.userDataOffset, bufDst,
                        icount * ISO_SECTOR_SIZE, (int)(dataSize - i));
                }
            }

            return bufDst;
        }

        /// <summary>
        /// Définit une instance d'interface <see cref="IDiscImage"/>.
        /// </summary>
        void SetDiscImage()
        {
            _sectorStruct = _discImage.GetSectorStucture();
            _startFirstSector = _discImage.FirstDumpSector();
        }
    }
}
