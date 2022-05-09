using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Représente un enregistrement dans la table des répertoires d'une image disque ISO 9660.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PathTableRecord
    {
        /// <summary>
        /// Length of Directory Identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public byte Length;

        /// <summary>
        /// Extended Attribute Record Length.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public byte ExtAttrRecLength;

        /// <summary>
        /// Location of Extent.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint ExtentLocation;

        /// <summary>
        /// Parent Directory Number.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public ushort ParentNumber;

        /// <summary>
        /// Conversion explicite de la structure <see cref="PathTableRecord"/> vers la structure <see cref="PathTableRecordPub"/>.
        /// </summary>
        /// <param name="record">Structure <see cref="PathTableRecord"/>.</param>
        /// <returns>Une structure <see cref="PathTableRecordPub"/>.</returns>
        public static explicit operator PathTableRecordPub(PathTableRecord record)
        {
            PathTableRecordPub rec = new PathTableRecordPub();

            rec.Number = record.Length;
            rec.Extent = record.ExtAttrRecLength;
            rec.ExtentLocation = record.ExtentLocation;
            rec.ParentNumber = record.ParentNumber;

            return rec;
        }
    }
}
