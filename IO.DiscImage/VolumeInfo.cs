using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Represente la structure du Descripteur de Volume de l'image ISO9660.
    /// </summary>
    public class VolumeInfo
    {
        private VolumeType _type;
        private string _standardId;
        private byte _version;
        private string _systemId;
        private string _volumeId;
        private long _volumeSpaceSize;
        private int _volumeSequenceNumber;
        private short _logicalBlockSize;
        private string _publisherId;
        private string _preparerId;
        private string _applicationId;
        private string _copyrightFileId;
        private DateTime _creationDate;
        private DateTime _modificationDate;
        private DateTime _expirationDate;

        /// <summary>
        /// Type du volume.
        /// </summary>
        public VolumeType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Identificateur ISO - doit être égale à 'CD001'.
        /// </summary>
        public string StandardId
        {
            get { return _standardId; }
        }

        /// <summary>
        /// Version du Descripteur de Volume.
        /// </summary>
        public byte Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Identificateur système - ('WIN32' supporté par un OS 32bits).
        /// </summary>
        public string SystemId
        {
            get { return _systemId; }
        }

        /// <summary>
        /// Nom du volume.
        /// </summary>
        public string VolumeId
        {
            get { return _volumeId; }
        }

        /// <summary>
        /// Nombre de blocs (secteurs) occupés par le Volume.
        /// </summary>
        public long VolumeSpaceSize
        {
            get { return _volumeSpaceSize; }
            set { _volumeSpaceSize = value; }
        }

        /// <summary>
        /// Numéro de séquence du Volume.
        /// </summary>
        public int VolumeSequenceNumber
        {
            get { return _volumeSequenceNumber; }
        }

        /// <summary>
        /// Taille d'un bloc (secteur) en octet du système de fichier utilisé par le Volume.
        /// </summary>
        public short LogicalBlockSize
        {
            get { return _logicalBlockSize; }
        }

        /// <summary>
        /// Nom du fournisseur des données du Volume.
        /// </summary>
        public string PublisherId
        {
            get { return _publisherId; }
        }

        /// <summary>
        /// Nom du fournisseur du Volume.
        /// </summary>
        public string PreparerId
        {
            get { return _preparerId; }
        }

        /// <summary>
        /// Nom du programme qui créé l'image ISO9660.
        /// </summary>
        public string ApplicationId
        {
            get { return _applicationId; }
        }

        /// <summary>
        /// Identificateur du fichier qui possède la notice du Copyright du Volume.
        /// </summary>
        public string CopyrightFileID
        {
            get { return _copyrightFileId; }
        }

        /// <summary>
        /// Date et heure de création du Volume.
        /// </summary>
        public DateTime CreationDate
        {
            get { return _creationDate; }
        }

        /// <summary>
        /// Date et heure de modification du Volume.
        /// </summary>
        public DateTime ModificationDate
        {
            get { return _modificationDate; }
        }

        /// <summary>
        /// Date et heure les données du Volume sont considérées comme obsolète.
        /// </summary>
        public DateTime ExpirationDate
        {
            get { return _expirationDate; }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe VolumeInfo. 
        /// </summary>
        /// <param name="t">Type du volume.</param>
        /// <param name="stdID">Identificateur ISO.</param>
        /// <param name="ver">Version du Descripteur de Volume.</param>
        /// <param name="sysID">Identificateur système.</param>
        /// <param name="volID">Nom du volume.</param>
        /// <param name="volSpaceSize">Nombre de blocs occupés.</param>
        /// <param name="volSeqNum">Numéro de séquence du Volume.</param>
        /// <param name="logBlckSize">Taille d'un bloc du système de fichier.</param>
        /// <param name="pubID">Nom du fournisseur des données du Volume.</param>
        /// <param name="prepID">Nom du fournisseur du Volume.</param>
        /// <param name="appID">Nom du programme qui créé l'image ISO9660.</param>
        /// <param name="cpyFileIF">Identificateur du fichier qui possède la notice du Copyright du Volume.</param>
        /// <param name="create">Date et heure de création du Volume.</param>
        /// <param name="modif">Date et heure de modification du Volume.</param>
        /// <param name="expir">Date et heure les données du Volume sont considérées comme obsolète.</param>
        internal VolumeInfo(VolumeType t, string stdID, byte ver, string sysID, string volID,
            long volSpaceSize, int volSeqNum, short logBlckSize, string pubID,
            string prepID, string appID, string cpyFileIF, DateTime create, DateTime modif, DateTime expir)
        {
            _type = t;
            _standardId = stdID;
            _version = ver;
            _systemId = sysID;
            _volumeId = volID;
            _volumeSpaceSize = volSpaceSize;
            _volumeSequenceNumber = volSeqNum;
            _logicalBlockSize = logBlckSize;
            _publisherId = pubID;
            _preparerId = prepID;
            _applicationId = appID;
            _copyrightFileId = cpyFileIF;
            _creationDate = create;
            _modificationDate = modif;
            _expirationDate = expir;
        }
    }
}
