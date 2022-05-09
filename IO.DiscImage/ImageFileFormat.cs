using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Décrit le format d'une image ISO9660.
    /// </summary>
    public enum ImageFileFormat
    {
        /// <summary>
        /// Image inconnue.
        /// </summary>
        UNKNOWN = -1,

        /// <summary>
        /// Fichier image ISO 9660.
        /// </summary>
        ISO = 0,

        /// <summary>
        /// Fichier binaire.
        /// </summary>
        BIN_Mode1 = 1,

        /// <summary>
        /// Fichier binaire.
        /// </summary>
        BIN_Mode2_Form1 = 2,

        /// <summary>
        /// Fichier binaire.
        /// </summary>
        BIN_Mode2_Form2 = 3,

        /// <summary>
        /// Format de fichier Media Descriptor utilisé par Alcohol.
        /// </summary>
        MDF = 4,

        /// <summary>
        /// Format de fichier image de Clone CD.
        /// </summary>
        CCD_Mode1 = 5,

        /// <summary>
        /// Format de fichier image de Clone CD.
        /// </summary>
        CCD_Mode2 = 6,

        ///// <summary>
        ///// Format de fichier de DiscJuggler.
        ///// </summary>
        //CDI = 7,

        ///// <summary>
        ///// Format de fichier de DiscJuggler.
        ///// </summary>
        //CDI_Raw = 8,

        ///// <summary>
        ///// Format de fichier de DiscJuggler.
        ///// </summary>
        //CDI_Pq = 9,

        ///// <summary>
        ///// Format de fichier image Nero version 1 - Antérieur à la version 5.5 de Nero.
        ///// </summary>
        ////NRGv1 = 4,

        ///// <summary>
        ///// Format de fichier image Nero version 2 - Version 5.5 de Nero et supérieur.
        ///// </summary>
        ////NRGv2 = 5,

        ///// <summary>
        ///// Fichier de copie binaire d'un Cd.
        ///// </summary>
        ////IMG = 6,

        ///// <summary>
        ///// Format de fichier BlindWrite 5
        ///// </summary>
        ////B5I = 6,

        ///// <summary>
        ///// Format de fichier BlindWrite 6
        ///// </summary>
        //B6I = 7
    }
}
