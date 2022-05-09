using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using diff = GomuLibrary.IO.DiscImage;
using System.Runtime.InteropServices;
using System.Threading;

namespace GomuLibrary.IO
{
    /// <summary>
    /// Classe de base pour lire les fichiers images ISO9660.
    /// </summary>
    public abstract class Iso9660Reader : IDisposable
    {
        /// <summary>
        /// Taille d'un bloc du système de fichiers CDFS ISO9660.
        /// </summary>
        public const int ISO_SECTOR_SIZE = 2048;

        /// <summary>
        /// Dictionnaire de la table des répertoires.
        /// </summary>
        protected Dictionary<string, diff.PathTableRecordPub> _tableRecords;

        /// <summary>
        /// Dictionnaire des entrées de fichiers.
        /// </summary>
        protected Dictionary<string, diff.RecordEntryInfo> _basicFilesInfo;

        private bool disposed;
        private string _discFile;
        private BinaryReader _baseBinaryReader;
        private FileStream _baseFileStream;
        private diff.VolumeDescriptor primVolDesc;
        private int _dataBeginSector = 0;
        private int _sectorSize = 2048;
        private Object thisLock = new Object();             //Objet de vérouillage.       
        private AutoResetEvent _autoEvent;

        //GetFileSystemEntries.
        private List<string> _fsEntries;

        /// <summary>
        /// Evénement pour les informations d'extraction de fichiers.
        /// </summary>
        public event EventHandler<Iso9660FileExtractEventArgs> Reading;

        /// <summary>
        /// Evénement pour l'arrêt forcé par l'utilisateur d'une opération d'extraction en cours.
        /// </summary>
        public virtual event EventHandler<Iso9660FileExtractEventArgs> Aborted;

        /// <summary>
        /// Obtient ou définit la taille d'un secteur de l'image disque.
        /// Valeur par défaut à 2048 (ISO 9660).
        /// </summary>
        protected int SectorSize
        {
            get { return _sectorSize; }
            set { _sectorSize = value; }
        }

        /// <summary>
        /// Obtient ou définit la position de départ des données utilisateurs dans les secteurs de l'image disque.
        /// Valeur par défaut à 0 (ISO 9660).
        /// </summary>
        protected int DataBeginSector
        {
            get { return _dataBeginSector; }
            set { _dataBeginSector = value; }
        }

        /// <summary>
        /// Obtient le chemin complet de l'image disque ouvert.
        /// </summary>
        protected string DiscFilename
        {
            get { return _discFile; }
        }

        /// <summary>
        /// Avertit le thread d'extraction en attente qu'un événement s'est produit.
        /// </summary>
        /// <remarks>A utilisé lors d'une demande d'annulation d'extraction complète de l'image ISO9660.
        /// (Mécanisme déjà implémenté en mode fichier).</remarks>
        protected AutoResetEvent AutoThreadEvent
        {
            get { return _autoEvent; }
            set { _autoEvent = value; }
        }

        /// <summary>
        /// Expose un <see cref="System.IO.Stream"/> autour du fichier de l'image ISO9660, 
        /// pour fournir un accès au flux de l'image.
        /// </summary>
        protected FileStream BaseFileStream
        {
            get { return _baseFileStream; }
            set { _baseFileStream = value; }
        }

        /// <summary>
        /// Lit les types de données primitifs comme des valeurs binaires sur le fichier ISO9660.
        /// </summary>
        protected BinaryReader BaseBinaryReader
        {
            get { return _baseBinaryReader; }
            set { _baseBinaryReader = value; }
        }

        /// <summary>
        /// Lit le Descripteur de Volume.
        /// </summary>
        /// <param name="discPath">Chemin du fichier image.</param>
        /// <param name="rootDirLocation">Retourne l'emplacement du répertoire racine du Volume.</param>
        /// <param name="rootDirRecordLength">Retourne la taille de la structure du répertoire racine (34 octets).</param>
        /// <returns>Une valeur <see cref="GomuLibrary.IO.DiscImage.VolumeInfo"/> représentant les infos du volume ISO9660.</returns>
        protected virtual diff.VolumeInfo ReadVolumeDescriptor(string discPath,
            ref uint rootDirLocation, ref uint rootDirRecordLength)
        {
            this._discFile = discPath;

            //Si fichier inexistant.
            if (!File.Exists(discPath))
                throw new IOException(string.Format("{0} file not found", this._discFile));

            //Taille de la structure du Descripteur de Volume (2048 octets).
            int istructsize = Marshal.SizeOf(typeof(diff.VolumeDescriptor));
            GCHandle gch = new GCHandle();

            //Tableaux qui vont contenir la structure VolumeDesciptor.
            byte[] bufTmp = new byte[_sectorSize];
            byte[] buf = new byte[_sectorSize];

            try
            {
                //Ouvre un flux sur le fichier ISO en lecture seulement.
                _baseFileStream = new FileStream(discPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _baseBinaryReader = new BinaryReader(_baseFileStream);

                //Déplacement vers la structure Primary Volume Descriptor.
                _baseBinaryReader.BaseStream.Seek((16 * _sectorSize) + _dataBeginSector, SeekOrigin.Begin);

                //Recherche le dernier Level du Descripteur de Volume.
                //Et lecture de celui-ci dans buffer.
                while (bufTmp[0] != (int)diff.VolumeType.VolumeDescriptorSetTerminator)
                {
                    _baseBinaryReader.Read(bufTmp, 0, (int)_sectorSize);

                    if (bufTmp[0] != 255)
                        Buffer.BlockCopy(bufTmp, 0, buf, 0, _sectorSize);
                }

                //Alloue un handle dans l'espace non managé pour effectuer le transfert vers
                //la structure diff.VolumeDescriptor.
                gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
                //Obtient son handle.
                IntPtr ptrBuf = gch.AddrOfPinnedObject();

                //Lecture du VolumeDescriptor.
                primVolDesc = (diff.VolumeDescriptor)Marshal.PtrToStructure(
                    ptrBuf, typeof(diff.VolumeDescriptor));

                //Libère le handle alloué pour le transfert de la structure.
                gch.Free();

                rootDirLocation = primVolDesc.RootDirectoryRecord.ExtentLocation;
                rootDirRecordLength = primVolDesc.RootDirectoryRecord.DataLength;
            }
            catch (IOException IOEx)
            {
                throw new IOException(IOEx.Message, IOEx);
            }
            catch (Exception)
            {
                throw new Exception("An error occured during reading the volume descriptor of the disc image file");
            }
            finally
            {
                if (gch != null && gch.IsAllocated)
                    gch.Free();

                CloseDiscImageFile();
            }

            return (diff.VolumeInfo)primVolDesc;
        }

        /// <summary>
        /// Lit la table des répertoires.
        /// </summary>
        /// <returns>Tableau <b>String"</b> de la table des répertoires.</returns>
        protected virtual string[] GetTable()
        {
            List<string> lTable = new List<string>();
            Dictionary<string, diff.PathTableRecordPub> lTableRec = new Dictionary<string, diff.PathTableRecordPub>();

            int iSizeof = Marshal.SizeOf(typeof(diff.PathTableRecord));
            int iOffset = 0;

            //Lecture des secteurs de la table des répertoires.
            this.BaseFileStream = new FileStream(this._discFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.BaseBinaryReader = new BinaryReader(this.BaseFileStream);

            //Ne garde que les données utilisateurs des secteurs de la table
            //Si la taille d'un secteur n'est pas de 2048 octets.
            byte[] bufTable = new byte[0];
            uint uiLPathTable = primVolDesc.TypeLPathTable;
            uint uiTableSize = this.primVolDesc.PathTableSize;

            if (this.SectorSize == ISO_SECTOR_SIZE)
            {
                bufTable = this.ReadSectors(uiLPathTable, uiTableSize);
            }
            else
            {
                bufTable = this.ReadSectors(uiLPathTable, Convert.ToUInt32(((uiTableSize / ISO_SECTOR_SIZE) + 1) * this.SectorSize));
                bufTable = this.CleanUserDataBuffer(bufTable, uiTableSize);
            }

            try
            {
                //Itération sur la table des chemins
                while (iOffset > -1)
                {
                    byte[] bufRec = new byte[iSizeof];

                    Buffer.BlockCopy(bufTable, iOffset, bufRec, 0, bufRec.Length);

                    //Alloue un handle dans l'espace non managé pour effectuer le transfert vers
                    //la  structure diff.PrimaryVolumeDescriptor.
                    GCHandle gch = GCHandle.Alloc(bufRec, GCHandleType.Pinned);
                    IntPtr ptrBuf = gch.AddrOfPinnedObject();

                    //Lecture du DirectoryRecord.
                    diff.PathTableRecord r = (diff.PathTableRecord)Marshal.PtrToStructure(
                        ptrBuf, typeof(diff.PathTableRecord));

                    gch.Free();

                    diff.PathTableRecordPub r2 = (diff.PathTableRecordPub)r;

                    //Récupère son nom
                    byte[] bufName = new byte[r.Length];
                    Buffer.BlockCopy(bufTable, iOffset + 8, bufName, 0, bufName.Length);

                    if (primVolDesc.Type == 1)
                        r2.Name = ASCIIEncoding.Default.GetString(bufName);
                    else if (primVolDesc.Type == 2)
                    {
                        if ((r2.Number % 2) == 0)
                            r2.Name = ASCIIEncoding.BigEndianUnicode.GetString(bufName);
                        else
                            r2.Name = ASCIIEncoding.Default.GetString(bufName);
                    }

                    //Si nom valide
                    if (!string.IsNullOrEmpty(r2.Name))
                    {
                        //Puis ajoute à liste des chemins
                        if (!r2.Name.Equals("\0"))
                        {
                            string szFullPath = Path.Combine(lTable[r.ParentNumber - 1], r2.Name);

                            lTable.Add(szFullPath);
                            lTableRec.Add(szFullPath, r2);
                        }
                        else
                        {
                            lTable.Add(@"\");
                            lTableRec.Add(@"\", r2);
                        }
                    }

                    //Quand la lecture de la table est fini on sort.
                    if (iOffset >= uiTableSize)
                        break;

                    iOffset += r.Length + 8;

                    if ((r2.Number % 2) > 0)
                        iOffset++;

                    if (r2.Number == 0)       //Si pas de chemin passe au suivant.
                        iOffset++;

                    if (r2.ParentNumber == 0)     //Si chemins trouvé = racine on passe au suivant.
                        iOffset++;
                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }

            _tableRecords = lTableRec;

            return lTable.ToArray();
        }

        /// <summary>
        /// Retourne les noms de tous les fichiers et sous-répertoires dans un répertoire spécifié
        /// </summary>
        /// <param name="path">Répertoire pour lequel les noms des fichiers et des sous-répertoires sont retournés.</param>
        /// <param name="lba">Adresse du secteur de l'entrée <paramref name="path"/>.</param>
        /// <param name="length">Taille de l'entrée <paramref name="path"/>.</param>
        /// <param name="recursive">Spécifie s'il faut rechercher le répertoire en cours, ou le répertoire en cours et tous les sous-répertoires.</param>
        /// <param name="first"><b>True</b> pour premier appel, <b>False</b> pour appel récursif.</param>
        /// <returns>Tableau String contenant les noms des entrées du système de fichiers dans le répertoire spécifié.</returns>
        protected virtual List<string> GetFileSystemEntries(string path, uint lba, uint length, bool recursive, bool first)
        {
            int iOffset = 0;
            uint uiExtent = 0, uiSize = 0;
            bool bFlgDir = false;
            bool bFlgHidden = false;
            int iNameLength = 0;
            string szName = "";
            byte[] date = new byte[] { };
            List<DiscImage.RecordEntryInfo> lstDir =
                new List<DiscImage.RecordEntryInfo>();

            //RAZ de la liste des entrées + 
            //définit le répertoire de recherche.
            if (first)
            {
                _fsEntries.Clear();
                _basicFilesInfo.Clear();
            }

            try
            {
                //Longueur totale des entrées contenu dans path.
                uint uiDataLength = GetDataLengthDirectoryEntry(path, ref uiExtent);

                //Ne garde que les données utilisateurs des secteurs de la table
                //Si la taille d'un secteur n'est pas de 2048 octets.
                byte[] bufData = new byte[0];
                if (_sectorSize == ISO_SECTOR_SIZE)
                {
                    bufData = this.ReadSectors(uiExtent, uiDataLength);
                }
                else
                {
                    bufData = this.ReadSectors(uiExtent, Convert.ToUInt32((uiDataLength / ISO_SECTOR_SIZE) * _sectorSize));
                    bufData = this.CleanUserDataBuffer(bufData, uiDataLength);
                }

                while (iOffset > -1)
                {
                    int iRecordLength = bufData[iOffset];      //Taille de la structure.

                    //Si contient des données.
                    if (iRecordLength > 0)
                    {
                        byte[] bufRecord = new byte[4];

                        //Extent - Position de départ du fichier.
                        Buffer.BlockCopy(bufData, iOffset + diff.DirectoryRecord.OFFSET_EXTENT_LOC, bufRecord, 0, 4);
                        uiExtent = BitConverter.ToUInt32(bufRecord, 0);

                        bufRecord = new byte[4];

                        //Size - Taille du fichier
                        Buffer.BlockCopy(bufData, iOffset + diff.DirectoryRecord.OFFSET_DATA_LENGTH, bufRecord, 0, 4);
                        uiSize = BitConverter.ToUInt32(bufRecord, 0);

                        bufRecord = new byte[7];

                        //Récupère la date
                        Buffer.BlockCopy(bufData, iOffset + diff.DirectoryRecord.OFFSET_REC_DATETIME, bufRecord, 0, 7);
                        date = bufRecord;

                        //Flag répertoire + attribut caché.
                        int iFlg = bufData[iOffset + diff.DirectoryRecord.OFFSET_FILE_FLAGS];
                        bFlgDir = Convert.ToBoolean(iFlg & (int)diff.FileFlags.Directory);
                        bFlgHidden = Convert.ToBoolean(iFlg & (int)diff.FileFlags.Hidden);

                        //Longeur du chemin.
                        iNameLength = bufData[iOffset + diff.DirectoryRecord.OFFSET_FILEID_LEN];

                        //Récupère le nom du chemin du fichier.
                        if (primVolDesc.Type == 1)
                            szName = ASCIIEncoding.Default.GetString(bufData, iOffset + diff.DirectoryRecord.OFFSET_FILEID_NAME, iNameLength);
                        else if (primVolDesc.Type == 2)
                        {
                            if ((iNameLength % 2) == 0)
                                szName = ASCIIEncoding.BigEndianUnicode.GetString(bufData, iOffset + diff.DirectoryRecord.OFFSET_FILEID_NAME, iNameLength);
                            else
                                szName = ASCIIEncoding.Default.GetString(bufData, iOffset + diff.DirectoryRecord.OFFSET_FILEID_NAME, iNameLength);
                        }

                        //Si nom n'est pas vide.
                        //Et caractère valide.
                        if (!string.IsNullOrEmpty(szName) && (ASCIIEncoding.Default.GetBytes(szName.ToCharArray(), 0, 1)[0] > 1))
                        {
                            string szFullPath = Path.Combine(path, szName);
                            szFullPath = szFullPath.EndsWith(@";1") ?
                                szFullPath.Remove(szFullPath.Length - 2) : szFullPath;

                            _fsEntries.Add(szFullPath);
                            _basicFilesInfo.Add(szFullPath, new diff.RecordEntryInfo(uiExtent, uiSize, date,
                                szName, szFullPath, bFlgHidden, bFlgDir));

                            if (recursive && bFlgDir)
                                lstDir.Add(new diff.RecordEntryInfo(uiExtent, uiSize, date, szName,
                                    szFullPath, bFlgHidden, bFlgDir));
                        }
                    }
                    else
                        //Passe au chemin suivant.
                        iRecordLength = 1;

                    iOffset += iRecordLength;     //Avance dans le bloc de la table fichiers pour le suivant.

                    //Si fin de la table lu on sort de l'itération.
                    if (iOffset >= uiDataLength)
                        iOffset = -1;
                }

                bufData = null;

                if (recursive)
                {
                    //Itération sur chaque chemins de répertoires
                    //Et lecture des structures des fichiers présents dans ces chemins.
                    for (int j = 0; j < lstDir.Count; j++)
                    {
                        DiscImage.RecordEntryInfo brfiDir = lstDir[j];

                        //if (!brfiDir.Name.EndsWith(@"\"))
                        //    brfiDir.Name += @"\";

                        string sz = Path.Combine(path, brfiDir.Name);

                        GetFileSystemEntries(sz, brfiDir.Extent, brfiDir.Size, recursive, false);
                    }
                }

                return _fsEntries;
            }
            catch (Exception)
            {
                return _fsEntries;
            }
        }

        /// <summary>
        /// Lit dans le flux d'octets un nombre spécifique d'octets dépendant de la taille
        /// du système de fichier de l'image disque.
        /// </summary>
        /// <param name="startOffset">Position dans le l'iso à partir duquel la lecture se fait.</param>
        /// <param name="dataLength">Nombre d'octets à lire.</param>
        /// <returns></returns>
        protected internal virtual byte[] ReadSectors(uint startOffset, uint dataLength)
        {
            this._baseBinaryReader.BaseStream.Seek(startOffset *  _sectorSize, SeekOrigin.Begin);

            byte[] buf = new byte[dataLength];
            this._baseBinaryReader.Read(buf, 0, (int)dataLength);

            return buf;
        }

        /// <summary>
        /// Lit et extrait un fichier contenu dans l'image disque.
        /// </summary>
        /// <param name="recordFileInfo">Information sur l'enregistrement du fichier dans l'image disque.</param>
        /// <param name="outputPath">Chemin de sortie.</param>
        /// <param name="mode">Mode de lecture du fichier image disque.</param>
        /// <param name="format">Format de l'image disque source.</param>
        /// <param name="recordIndex">Index du fichier dans la table des fichiers de l'image disque source.</param>
        protected void ReadFile(diff.RecordEntryInfo recordFileInfo,
            string outputPath, DiscImageReadMode mode, diff.ImageFileFormat? format, int? recordIndex)
        {
            lock (thisLock)
            {
                //Fichier de sortie.
                string szoutput = mode == DiscImageReadMode.EXTRACT_FILE ?
                    Path.Combine(outputPath, Path.GetFileName(recordFileInfo.Name)) :
                    Path.Combine(outputPath, recordFileInfo.FullPath.Substring(1));

                //Info de copie.
                uint uiStart = recordFileInfo.Extent * (uint)_sectorSize;
                uint uiSize = recordFileInfo.Size;
                uint uiDelta = uiSize;

                //En mode fichier on ouvre et ferme le fichier image disque.
                //En mode disque on ouvre le fichier image complet et le referme à la fin de la lecture de tout les fichiers.
                if (mode == DiscImageReadMode.EXTRACT_FILE)
                {
                    this._baseFileStream = new FileStream(this._discFile, FileMode.Open, FileAccess.Read, FileShare.Read, _sectorSize * 16);
                    this._baseBinaryReader = new BinaryReader(this._baseFileStream);
                }

                //Ouvre un flux en écriture sur le fichier de sortie.
                FileStream fsDst = new FileStream(szoutput, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, ISO_SECTOR_SIZE * 16);
                BinaryWriter bwDst = new BinaryWriter(fsDst);

                //Déplace le curseur au 1ier offset du fichier dans l'image.
                this._baseBinaryReader.BaseStream.Seek(uiStart, SeekOrigin.Begin);
                byte[] buf = new byte[0];

                //Lecture/écriture...
                do
                {
                    //Si l'état de l'objet d'évènement pour thread signalé on sort de la boucle.
                    //Si mode fichier oui, si mode disque cela n'est pas possible.
                    if (mode == DiscImageReadMode.EXTRACT_FILE && this._autoEvent.WaitOne(0, false))
                    {
                        if (this.Aborted != null)
                            this.Aborted(null, new Iso9660FileExtractEventArgs(recordFileInfo.FullPath, 0, 0, 0, szoutput, 0));

                        break;
                    }

                    uiDelta = uiSize - (uint)bwDst.BaseStream.Position;
                    if (uiDelta > 0)
                    {
                        //Ici, copie des données x32.
                        if (uiDelta > (ISO_SECTOR_SIZE * 16))
                        {
                            buf = new byte[_sectorSize * 16];
                            this._baseBinaryReader.Read(buf, 0, buf.Length);

                            if (_sectorSize == ISO_SECTOR_SIZE)
                                bwDst.Write(buf);
                            else
                                bwDst.Write(this.CleanUserDataBuffer(buf, ISO_SECTOR_SIZE * 16));
                        }
                        //Si données restante à écrire < 32ko c'est par ici...
                        else
                        {
                            if (_sectorSize == ISO_SECTOR_SIZE)
                            {
                                buf = new byte[uiDelta];
                                this._baseBinaryReader.Read(buf, 0, buf.Length);

                                bwDst.Write(buf);
                            }
                            else
                            {
                                buf = new byte[((uiDelta / ISO_SECTOR_SIZE) + 1) * _sectorSize];
                                this._baseBinaryReader.Read(buf, 0, buf.Length);

                                bwDst.Write(this.CleanUserDataBuffer(buf, uiDelta));
                            }
                        }

                        //Pousse les données du flux dans le fichier de sortie.
                        bwDst.Flush();

                        //Info progression.
                        if (this.Reading != null)
                        {
                            Iso9660FileExtractEventArgs dieEa = new Iso9660FileExtractEventArgs(recordFileInfo.FullPath,
                                uiStart, uiSize, uiSize - uiDelta, szoutput, recordIndex.Value + 1);
                            this.Reading(null, dieEa);
                        }
                    }
                } while (uiDelta > 0);

                //Ferme le fichier de sortie.
                bwDst.Close();
                fsDst.Close();
                fsDst.Dispose();

                //Inscrit la date d'origine sur le fichier extrait.
                File.SetLastWriteTimeUtc(szoutput, recordFileInfo.GetDate());
                //Attribut caché ?
                if (recordFileInfo.Hidden)
                    File.SetAttributes(szoutput, FileAttributes.Normal | FileAttributes.Hidden);
            }
        }

        /// <summary>
        /// Ferme le fichier ouvert et les flux sous-jacents.
        /// </summary>
        protected void CloseDiscImageFile()
        {
            if (_baseBinaryReader != null)
                _baseBinaryReader.Close();

            if (_baseFileStream != null)
            {
                _baseFileStream.Close();
                _baseFileStream.Dispose();
            }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe DiscImageReader.
        /// </summary>
        public Iso9660Reader()
        {
            _discFile = string.Empty;
            _fsEntries = new List<string>();
            _basicFilesInfo = new Dictionary<string, diff.RecordEntryInfo>();
            _tableRecords = new Dictionary<string, diff.PathTableRecordPub>();
            _autoEvent = new AutoResetEvent(false);
        }

        /// <summary>
        /// Libère les ressources utilisées par <b>Iso9660Reader</b>.
        /// </summary>
        ///<remarks>Appelez la méthode <b>Dispose</b> une fois que vous avez terminé d'utiliser <b>Iso9660Reader</b>.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libère les ressources utilisées par <b>Iso9660Reader</b>.
        /// </summary>
        /// <param name="disposing"><b>True</b> pour libérer également les ressources managées.</param>
        ///<remarks>Appelez la méthode <b>Dispose</b> une fois que vous avez terminé d'utiliser <b>Iso9660Reader</b>.</remarks>
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //Ressources managées.
                    CloseDiscImageFile();
                    _autoEvent.Close();       
                }

                //Ressources non managées à libérer ici.
            }
            disposed = true;
        }

        /// <summary>
        /// Obtient la longueur en octets des chemins des entrées (fichiers/répertoires) enfants d'un répertoire spécifié.
        /// </summary>
        /// <param name="path">Répertoire pour lequel les noms des fichiers et des sous-répertoires font parties.</param>
        /// <param name="lba">Adresse du secteur de l'entrée <paramref name="path"/>.</param>
        /// <returns>Un entier UInt32 de la longueur des entrées enfants.</returns>
        private uint GetDataLengthDirectoryEntry(string path, ref uint lba)
        {
            string szParent = System.IO.Path.GetDirectoryName(path);
            diff.PathTableRecordPub recParent = (path == @"\") ? _tableRecords[@"\"] : _tableRecords[szParent];
            diff.PathTableRecordPub recPath = _tableRecords[path];
            lba = recPath.ExtentLocation;

            int iSize = Marshal.SizeOf(typeof(diff.DirectoryRecord));
            diff.DirectoryRecord dirRec = new diff.DirectoryRecord();
            byte[] buf = new byte[iSize];

            this._baseBinaryReader.BaseStream.Seek(recParent.ExtentLocation * _sectorSize, SeekOrigin.Begin);
            this._baseBinaryReader.BaseStream.Seek(_dataBeginSector, SeekOrigin.Current);

            while (true)
            {
                this._baseBinaryReader.Read(buf, 0, buf.Length);

                GCHandle gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
                IntPtr pBuf = gch.AddrOfPinnedObject();

                dirRec = (diff.DirectoryRecord)Marshal.PtrToStructure(pBuf, typeof(diff.DirectoryRecord));

                gch.Free();

                if (dirRec.ExtentLocation > 0)
                {
                    if (dirRec.ExtentLocation == recPath.ExtentLocation)
                        return dirRec.DataLength;
                    else
                        this._baseBinaryReader.BaseStream.Seek(dirRec.Length - Marshal.SizeOf(dirRec), SeekOrigin.Current);
                }
                else
                {
                    this._baseBinaryReader.BaseStream.Seek(recPath.ExtentLocation * _sectorSize, SeekOrigin.Begin);
                    this._baseBinaryReader.BaseStream.Seek(_dataBeginSector, SeekOrigin.Current);

                    buf = new byte[iSize];
                    this._baseBinaryReader.Read(buf, 0, buf.Length);

                    GCHandle gch2 = GCHandle.Alloc(buf, GCHandleType.Pinned);
                    IntPtr pBuf2 = gch2.AddrOfPinnedObject();

                    dirRec = (diff.DirectoryRecord)Marshal.PtrToStructure(pBuf2, typeof(diff.DirectoryRecord));

                    gch2.Free();

                    return dirRec.DataLength;
                }
            }
        }

        /// <summary>
        /// Nettoie le buffer contenant les données des secteurs pour ne garder 
        /// que leurs données utilisateurs.
        /// </summary>
        /// <param name="source">Buffer d'origine.</param>
        /// <param name="dataLength">Taille des données utilisateurs.</param>
        /// <returns>Un tableau de Byte contenant uniquement les données utilisateurs des secteurs.</returns>
        private byte[] CleanUserDataBuffer(byte[] source, uint dataLength)
        {
            byte[] bufDst = new byte[dataLength];

            int icount = 0;
            for (int i = 0; i < dataLength; i += ISO_SECTOR_SIZE)
            {
                if ((dataLength - i) >= ISO_SECTOR_SIZE)
                {
                    Buffer.BlockCopy(source, (icount * _sectorSize) + _dataBeginSector, bufDst,
                        icount * ISO_SECTOR_SIZE, ISO_SECTOR_SIZE);

                    icount++;
                }
                else
                {
                    Buffer.BlockCopy(source, (_sectorSize * icount) + _dataBeginSector, bufDst,
                        icount * ISO_SECTOR_SIZE, (int)(dataLength - i));
                }
            }

            return bufDst;
        }
    }
}