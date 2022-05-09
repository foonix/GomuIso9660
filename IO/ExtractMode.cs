using System;

namespace GomuLibrary.IO
{
    /// <summary>
    /// Méthode d'extraction du contenu d'une image ISO9660.
    /// </summary>
    public enum DiscImageReadMode : int
    {
        /// <summary>
        /// Extraction d'un fichier.
        /// </summary>
        EXTRACT_FILE = 1,

        /// <summary>
        /// Extraction complète.
        /// </summary>
        EXTRACT_FULL_IMAGE = 2
    }
}
