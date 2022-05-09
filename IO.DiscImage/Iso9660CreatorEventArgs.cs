using System;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Classe fournissant les données pour l'événement <see cref="Iso9660Creator.Progression"/>.
    /// </summary>
    public class Iso9660CreatorEventArgs : EventArgs
    {
        long _bytesWritted;
        long _discLength;

        /// <summary>
        /// Taille finale du Volume en cours de création.
        /// </summary>
        public long DiscLength
        {
            get { return _discLength; }
        }

        /// <summary>
        /// Octets écrits depuis le début de la création.
        /// </summary>
        public long BytesWritted
        {
            get { return _bytesWritted; }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>Iso9660CreatorEventArgs</b>.
        /// </summary>
        public Iso9660CreatorEventArgs()
            : base()
        {
        }

        /// <summary>
        /// /// Initialise une nouvelle instance de la classe <b>Iso9660CreatorEventArgs</b>.
        /// </summary>
        /// <param name="writted">Octets écrits.</param>
        /// <param name="discLength">Taille du Volume.</param>
        public Iso9660CreatorEventArgs(long writted, long discLength)
            : base()
        {
            _bytesWritted = writted;
            _discLength = discLength;
        }
    }
}
