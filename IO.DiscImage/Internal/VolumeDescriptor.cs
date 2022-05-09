using System;
using System.Runtime.InteropServices;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Represent the volume descriptor structure of an ISO9660 file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2048)]
    internal struct VolumeDescriptor
    {
        /// <summary>
        /// Volume Descriptor Type (Level).
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public byte Type;

        /// <summary>
        /// Standard Identifier must be 'CD001'.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public char[] ID;

        /// <summary>
        /// Volume Descriptor Version.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public byte Version;

        [MarshalAs(UnmanagedType.I1)]
        public byte Unused1;

        /// <summary>
        /// System Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] SystemID;

        /// <summary>
        /// Volume Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] VolumeID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Unused2;

        /// <summary>
        /// Volume Space Size.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint VolumeSpaceSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Unused3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] Unused4;

        /// <summary>
        /// Volume Set Size.
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short VolumeSetSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Unused5;

        /// <summary>
        /// Volume Sequence Number.
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short VolumeSequenceNumber;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Unused6;

        /// <summary>
        /// Logical Block Size.
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public short LogicalBlockSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Unused7;

        /// <summary>
        /// Path Table Size.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint PathTableSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Unused8;

        /// <summary>
        /// Location of Type L Path Table.
        /// </summary>
        /// <remarks>Logical Block Number of first Block allocated to the Type L Path Table,
        /// Type L meaning multiple byte numerical values are recorded with least significant byte first.
        /// This value is also recorded with least significant byte first.</remarks>
        [MarshalAs(UnmanagedType.U4)]
        public uint TypeLPathTable;

        /// <summary>
        /// Location of Optional Type L Path Table.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint OptTypeLPathTable;

        /// <summary>
        /// Location of Type M Path Table.
        /// </summary>
        /// <remarks>Logical Block Number of first Block allocated to the Type M Path Table,
        /// Type M meaning multiple byte numerical values are recorded with least significant byte first.
        /// This value is also recorded with least significant byte first.</remarks> 
        [MarshalAs(UnmanagedType.U4)]
        public uint TypeMPathTable;

        /// <summary>
        /// Location of Optional Type M Path Table.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint OptTypeMPathTable;

        /// <summary>
        /// Directory record for Root Directory.
        /// </summary>
        [MarshalAs(UnmanagedType.Struct, SizeConst = 34)]
        public DirectoryRecord RootDirectoryRecord;

        /// <summary>
        /// Volume Set Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public char[] VolumeSetID;

        /// <summary>
        /// Publisher Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public char[] PublisherID;

        /// <summary>
        /// Data Preparer Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public char[] PreparerID;

        /// <summary>
        /// Application Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public char[] ApplicationID;

        /// <summary>
        /// Copyright File Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
        public char[] CopyrightFileID;

        /// <summary>
        /// Abstract File Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
        public char[] AbstractFileID;

        /// <summary>
        /// Bibliographic File Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
        public char[] BibliographicFileID;

        /// <summary>
        /// Volume Creation Date and Time.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] CreationDate;

        /// <summary>
        /// Volume Modification Date and Time.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] ModificationDate;

        /// <summary>
        /// Volume Expiration Date and Time.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] ExpirationDate;

        /// <summary>
        /// Volume Effective Date and Time.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] EffectiveDate;

        /// <summary>
        /// File Structure Version.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public byte FileStructureVersion;

        /// <summary>
        /// Retourne la date et l'heure de création du volume.
        /// </summary>
        /// <returns>Structure <see cref="System.DateTime"/> à laquelle sont assignées la date et l'heure
        /// de création du volume.</returns>
        public DateTime GetCreationDate()
        {
            return this.BytesToDateTime(CreationDate);
        }

        /// <summary>
        /// Retourne la date et l'heure de la dernière modification du volume.
        /// </summary>
        /// <returns>Structure <see cref="System.DateTime"/> à laquelle sont assignées la date et l'heure
        /// de la dernière modification du volume.</returns>
        public DateTime GetModificationDate()
        {
            return this.BytesToDateTime(ModificationDate);
        }

        /// <summary>
        /// Retourne la date et l'heure à partir duquel le volume est considéré comme obsolète.
        /// </summary>
        /// <returns>Structure <see cref="System.DateTime"/> à laquelle sont assignées la date et l'heure
        /// du volume considéré comme obsolète.</returns>
        public DateTime GetExpirationDate()
        {
            return this.BytesToDateTime(ExpirationDate);
        }

        /// <summary>
        /// Retourne la date et l'heure à partir duquel le volume peut-être utilisé.
        /// </summary>
        /// <returns>Structure <see cref="System.DateTime"/> à laquelle sont assignées la date et l'heure
        /// du volume peut être utilisé.</returns>
        public DateTime GetEffectiveDate()
        {
            return this.BytesToDateTime(EffectiveDate);
        }

        /// <summary>
        /// Convertit un tableau de <see cref="System.Byte"/> en une structure <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="date">Tableau de <see cref="System.Byte"/> à convertir.</param>
        /// <returns>Structure <see cref="System.DateTime"/> à laquelle sont assignées la date et l'heure
        /// représenté par le tableau de <see cref="System.Byte"/>.</returns>
        private DateTime BytesToDateTime(byte[] date)
        {
            try
            {
                if (date != null && date.Length == 17)
                {
                    string szDateTime = System.Text.ASCIIEncoding.Default.GetString(date);

                    string szYear = szDateTime.Substring(0, 4);
                    string szMonth = szDateTime.Substring(4, 2);
                    string szDay = szDateTime.Substring(6, 2);
                    string szHour = szDateTime.Substring(8, 2);
                    string szMin = szDateTime.Substring(10, 2);
                    string szSec = szDateTime.Substring(12, 2);

                    return new DateTime(Convert.ToInt32(szYear), Convert.ToInt32(szMonth), Convert.ToInt32(szDay),
                        Convert.ToInt32(szHour), Convert.ToInt32(szMin), Convert.ToInt32(szSec));
                }
                else
                    return DateTime.MinValue;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Conversion explicite de la structure <see cref="VolumeDescriptor"/> vers la structure <see cref="VolumeInfo"/>.
        /// </summary>
        /// <param name="pvd">Structure <see cref="VolumeDescriptor"/>.</param>
        /// <returns>Une structure <see cref="VolumeInfo"/>.</returns>
        public static explicit operator VolumeInfo(VolumeDescriptor pvd)
        {
            char[] cTrim = new char[] { ' ' }; //, '\0' };

            string szID = new string(pvd.ID);
            string szSysID = new string(pvd.SystemID).Trim(cTrim).Replace("\0", string.Empty);
            string szVolID = new string(pvd.VolumeID).Trim(cTrim).Replace("\0", string.Empty);
            string szPubID = new string(pvd.PublisherID).Trim(cTrim).Replace("\0", string.Empty);
            string szPrepID = new string(pvd.PreparerID).Trim(cTrim).Replace("\0", string.Empty);
            string szAppID = new string(pvd.ApplicationID).Trim(cTrim).Replace("\0", string.Empty);
            string szCopyID = new string(pvd.CopyrightFileID).Trim(cTrim).Replace("\0", string.Empty);

            VolumeInfo dvi = new VolumeInfo((VolumeType)pvd.Type,
                szID, pvd.Version, szSysID, szVolID, pvd.VolumeSpaceSize,
                pvd.VolumeSequenceNumber, pvd.LogicalBlockSize, szPubID,
                szPrepID, szAppID, szCopyID, pvd.GetCreationDate(), pvd.GetModificationDate(),
                pvd.GetExpirationDate());

            return dvi;
        }
    }
}
