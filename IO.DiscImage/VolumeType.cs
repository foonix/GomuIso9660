using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Type du volume.
    /// </summary>
    public enum VolumeType : int
    {
        /// <summary>
        /// Secteur de démarrage.
        /// </summary>
        BootRecord = 0,

        /// <summary>
        /// VolumeDescriptor Level 1.
        /// </summary>
        PrimaryVolumeDescriptor = 1,

        /// <summary>
        /// VolumeDescriptor Level 2.
        /// </summary>
        SupplementaryVolumeDescriptor = 2,

        /// <summary>
        /// VolumeVolumeDescriptor Level 3.
        /// </summary>
        VolumePartitionDescriptor = 3,

        /// <summary>
        /// Fin de la structure du Descripteur de volume.
        /// </summary>
        VolumeDescriptorSetTerminator = 255
    }
}
