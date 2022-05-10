using System;
using System.Threading;
using System.IO;
using Microsoft.Win32.SafeHandles;
using GomuLibrary.Win32API;

namespace GomuLibrary.IO.DiscImage
{
    /// <summary>
    /// Classe permettant la cr�ation d'une image ISO9660 d'un disque CD/DVD.
    /// </summary>
    public class Iso9660Creator : IDisposable
    {
        /// <summary>
        /// Taille par d�faut de la m�moire tampon.
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 0x20000;

        /// <summary>
        /// Taille max. de la m�moire tampon.
        /// </summary>
        public const int BUFFER_SIZE_MAX = 0x100000;

        const uint FAT32_DISK_LIMIT = uint.MaxValue;

        private bool disposed;
        Thread _thWorks;
        AutoResetEvent _autoEvent;
        long _discLength;
        int _bufSize;
        string _discPath;
        string _outputPath;
        private Object thisLock = new Object();             //Objet de v�rouillage.   

        /// <summary>
        /// Ev�nement pour les informations sur l'avancement de la cr�ation de l'image ISO9660.
        /// </summary>
        public event EventHandler<Iso9660CreatorEventArgs> Progression;

        /// <summary>
        /// Ev�nement pour l'arr�t forc� par l'utilisateur de la cr�ation de l'image ISO9660.
        /// </summary>
        public event EventHandler<Iso9660CreatorEventArgs> Aborted;

        /// <summary>
        /// Ev�nement pour la fin d'une extraction.
        /// </summary>
        public event EventHandler Terminate;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>Iso9660Creator</b>.
        /// </summary>
        public Iso9660Creator()
            : base()
        {
            _autoEvent = new AutoResetEvent(false);
        }

        /// <summary>
        /// Cr�er une image ISO9660 d'un disque CD/DVD.
        /// </summary>
        /// <param name="discPath">Lettre du lecteur CD/DVD.</param>
        /// <param name="output">Chemin complet du fichier iso de sortie.</param>
        /// <param name="newThread"><b>True</b> pour lancer l'extraction dans un nouveau <see cref="System.Threading.Thread"/> (recommand� dans une application WindowsForm pour ne pas figer votre application).
        /// <b>False</b> pour rester dans le <see cref="System.Threading.Thread"/> principal (pour application Console).</param>
        /// <exception cref="ArgumentNullException"><paramref name="discPath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="IOException">Une erreur d'E/S s'est produite. Soit:
        /// <list type="bullet">
        /// <item><description><paramref name="discPath"/> n'est pas un lecteur de type CD/DVD.</description></item>
        /// <item><description>Le lecteur source <paramref name="discPath"/> n'est pas pr�t.</description></item>
        /// <item><description>Aucun media CD/DVD n'est pr�sent dans le lecteur <paramref name="discPath"/>.</description></item>
        /// <item><description>Le fichier iso de sortie existe pour <paramref name="overwrite"/> � <b>True</b>.</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est en FAT32 et la taille du CD/DVD est sup�rieur � la taille max. d'une partition FAT32 (<see cref="uint.MaxValue"/>).</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est insuffisant.</description></item>
        /// </list>
        /// </exception>
        public void CreateImage(string discPath, string output, bool newThread)
        {
            this.CreateImage(discPath, output, newThread, DEFAULT_BUFFER_SIZE, true);
        }

        /// <summary>
        /// Cr�er une image ISO9660 d'un disque CD/DVD.
        /// </summary>
        /// <param name="discPath">Lettre du lecteur CD/DVD.</param>
        /// <param name="output">Chemin complet du fichier iso de sortie.</param>
        /// <param name="newThread"><b>True</b> pour lancer l'extraction dans un nouveau <see cref="System.Threading.Thread"/> (recommand� dans une application WindowsForm pour ne pas figer votre application).
        /// <b>False</b> pour rester dans le <see cref="System.Threading.Thread"/> principal (pour application Console).</param>
        /// <param name="overwrite"><b>True</b> pour �craser le fichier iso de sortie <paramref name="output"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="discPath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="IOException">Une erreur d'E/S s'est produite. Soit:
        /// <list type="bullet">
        /// <item><description><paramref name="discPath"/> n'est pas un lecteur de type CD/DVD.</description></item>
        /// <item><description>Le lecteur source <paramref name="discPath"/> n'est pas pr�t.</description></item>
        /// <item><description>Aucun media CD/DVD n'est pr�sent dans le lecteur <paramref name="discPath"/>.</description></item>
        /// <item><description>Le fichier iso de sortie existe pour <paramref name="overwrite"/> � <b>True</b>.</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est en FAT32 et la taille du CD/DVD est sup�rieur � la taille max. d'une partition FAT32 (<see cref="uint.MaxValue"/>).</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est insuffisant.</description></item>
        /// </list>
        /// </exception>
        public void CreateImage(string discPath, string output, bool newThread, bool overwrite)
        {
            this.CreateImage(discPath, output, newThread, DEFAULT_BUFFER_SIZE, overwrite);
        }

        /// <summary>
        /// Cr�er une image ISO9660 d'un disque CD/DVD.
        /// </summary>
        /// <param name="discPath">Lettre du lecteur CD/DVD.</param>
        /// <param name="output">Chemin complet du fichier iso de sortie.</param>
        /// <param name="bufferSize">Valeur <see cref="Int32"/> positive sup�rieure � 0 indiquant la taille de la m�moire tampon.</param>
        /// <exception cref="ArgumentNullException"><paramref name="discPath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize"/> est n�gatif ou sup�rieur � <see cref="BUFFER_SIZE_MAX"/>.</exception>
        /// <exception cref="IOException">Une erreur d'E/S s'est produite. Soit:
        /// <list type="bullet">
        /// <item><description><paramref name="discPath"/> n'est pas un lecteur de type CD/DVD.</description></item>
        /// <item><description>Le lecteur source <paramref name="discPath"/> n'est pas pr�t.</description></item>
        /// <item><description>Aucun media CD/DVD n'est pr�sent dans le lecteur <paramref name="discPath"/>.</description></item>
        /// <item><description>Le fichier iso de sortie existe pour <paramref name="overwrite"/> � <b>True</b>.</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est en FAT32 et la taille du CD/DVD est sup�rieur � la taille max. d'une partition FAT32 (<see cref="uint.MaxValue"/>).</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est insuffisant.</description></item>
        /// </list>
        /// </exception>
        /// <example>
        /// L'exemple de code suivant cr�� un fichier image ISO9660 � partir d'une source de type CD/DVD.
        /// La taille de la m�moire tampon pour la copie est d�finie � 32Ko.
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
        ///             using (Iso9660Creator iso = new Iso9660Creator())
        ///             {
        ///                 iso.Terminate += new EventHandler(delegate(object sender, EventArgs e)
        ///                 {
        ///                     Console.WriteLine(@"Extraction complete !.");
        ///                     Console.Read();
        ///                 });
        /// 
        ///                 //Passe � la cr�ation de l'image iso (C:\MyIso.iso) du cd/dvd (lecteur E:\).
        ///                 iso.CreateImage(@"E:\", @"C:\MyIso.iso", 32768);
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public void CreateImage(string discPath, string output, int bufferSize)
        {
            this.CreateImage(discPath, output, false, bufferSize, true);
        }

        /// <summary>
        /// Cr�er une image ISO9660 d'un disque CD/DVD.
        /// </summary>
        /// <param name="discPath">Lettre du lecteur CD/DVD.</param>
        /// <param name="output">Chemin complet du fichier iso de sortie.</param>
        /// <param name="bufferSize">Valeur <see cref="Int32"/> positive sup�rieure � 0 indiquant la taille de la m�moire tampon.</param>
        /// <param name="overwrite"><b>True</b> pour �craser le fichier iso de sortie <paramref name="output"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="discPath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize"/> est n�gatif ou sup�rieur � <see cref="BUFFER_SIZE_MAX"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="discPath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="IOException">Une erreur d'E/S s'est produite. Soit:
        /// <list type="bullet">
        /// <item><description><paramref name="discPath"/> n'est pas un lecteur de type CD/DVD.</description></item>
        /// <item><description>Le lecteur source <paramref name="discPath"/> n'est pas pr�t.</description></item>
        /// <item><description>Aucun media CD/DVD n'est pr�sent dans le lecteur <paramref name="discPath"/>.</description></item>
        /// <item><description>Le fichier iso de sortie existe pour <paramref name="overwrite"/> � <b>True</b>.</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est en FAT32 et la taille du CD/DVD est sup�rieur � la taille max. d'une partition FAT32 (<see cref="uint.MaxValue"/>).</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est insuffisant.</description></item>
        /// </list>
        /// </exception>
        public void CreateImage(string discPath, string output, int bufferSize, bool overwrite)
        {
            this.CreateImage(discPath, output, false, bufferSize, overwrite);
        }

        /// <summary>
        /// Cr�er une image ISO9660 d'un disque CD/DVD.
        /// </summary>
        /// <param name="discPath">Lettre du lecteur CD/DVD.</param>
        /// <param name="output">Chemin complet du fichier iso de sortie.</param>
        /// <exception cref="ArgumentNullException"><paramref name="discPath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="IOException">Une erreur d'E/S s'est produite. Soit:
        /// <list type="bullet">
        /// <item><description><paramref name="discPath"/> n'est pas un lecteur de type CD/DVD.</description></item>
        /// <item><description>Le lecteur source <paramref name="discPath"/> n'est pas pr�t.</description></item>
        /// <item><description>Aucun media CD/DVD n'est pr�sent dans le lecteur <paramref name="discPath"/>.</description></item>
        /// <item><description>Le fichier iso de sortie existe pour <paramref name="overwrite"/> � <b>True</b>.</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est en FAT32 et la taille du CD/DVD est sup�rieur � la taille max. d'une partition FAT32 (<see cref="uint.MaxValue"/>).</description></item>
        /// <item><description>L'espace disque du disque de destination <paramref name="output"/> est insuffisant.</description></item>
        /// </list>
        /// </exception>
        /// <example>
        /// L'exemple de code suivant cr�� un fichier image ISO9660 � partir d'une source de type CD/DVD.
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
        ///             using (Iso9660Creator iso = new Iso9660Creator())
        ///             {
        ///                 iso.Terminate += new EventHandler(delegate(object sender, EventArgs e)
        ///                 {
        ///                     Console.WriteLine(@"Extraction complete !.");
        ///                     Console.Read();
        ///                 });
        /// 
        ///                 //Passe � la cr�ation de l'image iso (C:\MyIso.iso) du cd/dvd (lecteur E:\) .
        ///                 iso.CreateImage(@"E:\", @"C:\MyIso.iso");
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public void CreateImage(string discPath, string output)
        {
            this.CreateImage(discPath, output, false, DEFAULT_BUFFER_SIZE, true);
        }

        /// <summary>
        /// Cr�er une image ISO9660 d'un disque CD/DVD.
        /// </summary>
        /// <param name="discPath">Lettre du lecteur CD/DVD.</param>
        /// <param name="output">Chemin complet du fichier iso de sortie.</param>
        /// <param name="newThread"><b>True</b> pour lancer l'extraction dans un nouveau <see cref="System.Threading.Thread"/> (recommand� dans une application WindowsForm pour ne pas figer votre application).
        /// <b>False</b> pour rester dans le <see cref="System.Threading.Thread"/> principal (pour application Console).</param>
        /// <param name="bufferSize">Valeur <see cref="Int32"/> positive sup�rieure � 0 indiquant la taille de la m�moire tampon.</param>
        /// <param name="overwrite"><b>True</b> pour �craser le fichier iso de sortie <paramref name="output"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="discPath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize"/> est n�gatif ou sup�rieur � <see cref="BUFFER_SIZE_MAX"/>.</exception>
        /// <exception cref="IOException">Une erreur d'E/S s'est produite. Soit:
        /// <list type="bullet">
        /// <item>
        /// <description><paramref name="discPath"/> n'est pas un lecteur de type CD/DVD.</description>
        /// </item>
        /// <item>
        /// <description>Le lecteur source <paramref name="discPath"/> n'est pas pr�t.</description>
        /// </item>
        /// <item>
        /// <description>Aucun media CD/DVD n'est pr�sent dans le lecteur <paramref name="discPath"/>.</description>
        /// </item>
        /// <item>
        /// <description>Le fichier iso de sortie existe pour <paramref name="overwrite"/> � <b>True</b>.</description>
        /// </item>
        /// <item>
        /// <description>L'espace disque du disque de destination <paramref name="output"/> est en FAT32 et la taille du CD/DVD est sup�rieur � la taille max. d'une partition FAT32 (<see cref="uint.MaxValue"/>).</description>
        /// </item>
        /// <item>
        /// <description>L'espace disque du disque de destination <paramref name="output"/> est insuffisant.</description>
        /// </item>
        /// </list>
        /// </exception>
        public void CreateImage(string discPath, string output, bool newThread, int bufferSize, bool overwrite)
        {
            _discPath = discPath;
            _outputPath = output;
            _bufSize = bufferSize;

            DriveInfo diCD = new DriveInfo(_discPath);
            _discLength = diCD.TotalSize;

            if (_discPath == string.Empty)
                throw new ArgumentNullException("discPath", @"The cd/dvd drive path must not be null.");

            if (_outputPath == string.Empty)
                throw new ArgumentNullException("output", @"The output file path must not be null.");

            if (diCD.DriveType != DriveType.CDRom)
                throw new IOException(@"The cd/dvd drive specified is not a CDRom type.");

            if (!diCD.IsReady)
                throw new IOException(@"The cd/dvd drive specified is empty or is not ready.");

            if (!overwrite)
            {
                if (File.Exists(_outputPath))
                    throw new IOException(@"The output file already exists.");
            }

            DriveInfo diOutput = new DriveInfo(Path.GetPathRoot(output));
            if (diOutput.DriveFormat.Equals("FAT32") && (diCD.TotalSize > FAT32_DISK_LIMIT))
                throw new IOException(@"Not enough space on FAT32 destination drive.");

            if (diOutput.TotalSize < _discLength)
                throw new IOException(@"Not enough space on the destination drive.");

            if (_bufSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize", @"The buffer size is negative");

            if (bufferSize > BUFFER_SIZE_MAX)
                throw new ArgumentOutOfRangeException(@"Buffer is too large.");

            //Execution dans un thread.
            if (newThread)
            {
                if (_thWorks != null)
                {
                    if (_thWorks.ThreadState == ThreadState.Running)
                        return;
                }

                _thWorks = new Thread(new ThreadStart(ThWorks));
                _thWorks.Name = "ISO9660CreatorWorks";

                _thWorks.Start();
            }
            else
            {
                ThWorks();
            }
        }

        /// <summary>
        /// Met fin � l'op�ration.
        /// </summary>
        public void Abort()
        {
            _autoEvent.Set();
        }

        /// <summary>
        /// M�thode charg�e de cr�er l'image iso du CD/DVD.
        /// </summary>
        void ThWorks()
        {
            //Verrou.
            lock (thisLock)
            {
                //RAZ �tat non signal�.
                _autoEvent.Reset();
                //Ouvre le lecteur en lecture.
                SafeFileHandle sfhDev = CreateFileDevice(_discPath);
                //Objet FileStream pour lecture et �criture.
                FileStream fsSrc = null;
                FileStream fsDst = null;

                try
                {
                    //V�rif si handle ok du lecteur
                    if (sfhDev.IsClosed)
                        throw new IOException("The device handle is closed.");

                    if (sfhDev.IsInvalid)
                        throw new IOException("The device handle is invalid.");

                    //Nouvelle instance FileStream en lecture sur le handle (lecteur) � encapsuler.
                    fsSrc = new FileStream(sfhDev, FileAccess.Read, _bufSize);
                    //Nouvelle instance FileStream en �criture pour le fichier iso de sortie.
                    fsDst = new FileStream(_outputPath, FileMode.Create, FileAccess.Write, FileShare.None, _bufSize);
                    //Buffer.
                    byte[] buf = new byte[_bufSize];

                    do
                    {
                        //V�rifie l'�tat si signal� alors annulation...
                        //Sinon on continue la copie.
                        if (_autoEvent.WaitOne(0, false))
                        {
                            //Si signal stop !!!!!.
                            if (Aborted != null)
                            {
                                Iso9660CreatorEventArgs dieCe = new Iso9660CreatorEventArgs(fsDst.Position, _discLength);
                                this.Aborted(null, dieCe);

                                break;
                            }
                        }
                        else
                        {
                            //Lecture dans la source.
                            int iBytesRead = fsSrc.Read(buf, 0, buf.Length);
                            //Puis �criture.
                            fsDst.Write(buf, 0, iBytesRead);

                            //Info progression.
                            if (Progression != null)
                            {
                                Iso9660CreatorEventArgs dieCe = new Iso9660CreatorEventArgs(fsDst.Position, _discLength);
                                Progression(new object(), dieCe);
                            }
                        }
                    } while (_discLength != fsDst.Position);

                    //Vide le buffer.
                    Array.Clear(buf, 0, buf.Length);

                    //Info termin�e.
                    if (Terminate != null)
                        Terminate(null, EventArgs.Empty);
                }
                catch (Exception)
                {
                }
                finally
                {
                    //Ferme les flux ouverts et lib�re les ressources utilis�es.
                    if (fsSrc != null)
                    {
                        fsSrc.Close();
                        fsSrc.Dispose();
                    }

                    if (fsDst != null)
                    {
                        fsDst.Close();
                        fsDst.Dispose();
                    }

                    //Lib�re le handle du lecteur et les ressources utilis�es.
                    sfhDev.Close();
                    sfhDev.Dispose();
                }
            }
        }

        /// <summary>
        /// Fournit un handle de fichier pour le lecteur CD/DVD.
        /// </summary>
        /// <param name="path">Chemin du lecteur.</param>
        /// <returns>Un objet <see cref="SafeFileHandle"/> encapsulant le handle du lecteur <paramref name="path"/>.</returns>
        SafeFileHandle CreateFileDevice(string path)
        {
            const uint GENERIC_READ = 0x80000000;
            const uint FILE_SHARE_READ = 0x1;
            const uint OPEN_EXISTING = 0x3;
            const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

            string szdev = path.EndsWith(@"\") ? path.Substring(0, path.Length - 1) : path;

            IntPtr hdev = SafeNativeMethods.CreateFile(@"\\.\" + szdev, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero,
                OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            return new SafeFileHandle(hdev, true);
        }

        /// <summary>
        /// Lib�re les ressources utilis�es par <b>Iso9660Reader</b>.
        /// </summary>
        ///<remarks>Appelez la m�thode <b>Dispose</b> une fois que vous avez termin� d'utiliser <b>Iso9660Creator</b>.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Lib�re les ressources utilis�es par <b>Iso9660Reader</b>.
        /// </summary>
        /// <param name="disposing"><b>True</b> pour lib�rer �galement les ressources manag�es.</param>
        ///<remarks>Appelez la m�thode <b>Dispose</b> une fois que vous avez termin� d'utiliser <b>Iso9660Creator</b>.</remarks>
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //Ressources manag�es.
                    _autoEvent.Close();
                }

                //Ressources non manag�es � lib�rer ici.
            }
            disposed = true;
        }
    }
}
