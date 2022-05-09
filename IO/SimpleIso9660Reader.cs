using System;
using System.Collections.Generic;
//using System.Text;
using System.IO;
using diff = GomuLibrary.IO.DiscImage;
using System.Threading;

namespace GomuLibrary.IO
{
    /// <summary>
    /// Classe fournissant un moyen simple de lecture des fichiers images ISO9660 et dérivés (BIN/IMG/NRG).
    /// Cette classe ne peut pas être héritée.
    /// </summary>
    public sealed class SimpleIso9660Reader : Iso9660Reader, IDisposable
    {
        Thread _thExtraction;
        uint _rootDirExtLoc;
        uint _rootDirLength;

        /// <summary>
        /// Evénement pour la fin d'une extraction.
        /// </summary>
        public event EventHandler Terminate;

        /// <summary>
        /// Evénement pour l'arrêt forcé par l'utilisateur d'une opération d'extraction en cours.
        /// </summary>
        public override event EventHandler<Iso9660FileExtractEventArgs> Aborted;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <b>SimpleIso9660Reader</b>.
        /// </summary>
        public SimpleIso9660Reader()
            : base()
        {
        }

        /// <summary>
        /// Ouvre une image ISO9660 et lit le Descripteur de Volume.
        /// </summary>
        /// <param name="discImageFile">Chemin du fichier image disque.</param>
        /// <param name="type">Format de l'image.</param>
        /// <example>
        /// L'exemple de code suivant ouvre un fichier ISO9660 et lit le contenu du Descripteur de Volume.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO;
        /// using GomuLibrary.IO.DiscImage;
        ///
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             using (SimpleIso9660Reader myIso = new SimpleIso9660Reader())
        ///             {
        ///                 //Lit le Descripteur de Volume.
        ///                 VolumeInfo volInfo = myIso.Open(@"C:\MonIso.iso", ImageFileFormat.ISO);
        /// 
        ///                 //Affiche ici ces infos...
        ///                 if (volInfo != null)
        ///                 {
        ///                     System.Reflection.PropertyInfo[] propInfos = typeof(VolumeInfo).GetProperties(
        ///                         System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance |
        ///                         System.Reflection.BindingFlags.Public);
        /// 
        ///                     foreach (System.Reflection.PropertyInfo pi in propInfos)
        ///                     {
        ///                         Console.WriteLine(string.Format("{0}: {1}", pi.Name, pi.GetValue(volInfo, null)));
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>Un objet <see cref="diff.VolumeInfo"/> fournissant les informations 
        /// du Descripteur de Volume.</returns>
        public diff.VolumeInfo Open(string discImageFile, diff.ImageFileFormat type)
        {
            //Ferme les flux ouverts sur l'image.
            Close();

            switch (type)
            {
                //Défaut ISO 9660.
                case diff.ImageFileFormat.ISO:
                    base.DataBeginSector = 0;
                    base.SectorSize = 2048;
                    break;
                //Binaire Mode1.
                case diff.ImageFileFormat.BIN_Mode1:
                    base.DataBeginSector = 16;
                    base.SectorSize = 2352;
                    break;
                //Binaire Mode2 form1.
                case diff.ImageFileFormat.BIN_Mode2_Form1:
                    base.DataBeginSector = 24;
                    base.SectorSize = 2352;
                    break;
                //Binaire Mode2 form2.
                case diff.ImageFileFormat.BIN_Mode2_Form2:
                    base.DataBeginSector = 8;
                    base.SectorSize = 2336;
                    break;
                //MDF.
                case diff.ImageFileFormat.MDF:
                    base.DataBeginSector = 16;
                    base.SectorSize = 2352;
                    break;
                //CloneCD Mode1.
                case diff.ImageFileFormat.CCD_Mode1:
                    base.DataBeginSector = 16;
                    base.SectorSize = 2352;
                    break;
                //CloneCD Mode2.
                case diff.ImageFileFormat.CCD_Mode2:
                    base.DataBeginSector = 24;
                    base.SectorSize = 2352;
                    break;
                ////DiscJungler.
                //case diff.ImageFileFormat.CDI:
                //    base.DataBeginSector = 12;
                //    base.SectorSize = 2048;
                //    break;
                ////DiscJungler Raw.
                //case diff.ImageFileFormat.CDI_Raw:
                //    base.DataBeginSector = 16;
                //    base.SectorSize = 2352;
                //    break;
                ////DiscJungler mode PQ.
                //case diff.ImageFileFormat.CDI_Pq:
                //    base.DataBeginSector = 16;
                //    base.SectorSize = 2368;
                //    break;
                default:
                    break;
            }
            
            //Lit de Descripteur de Volume et le retourne.
            return base.ReadVolumeDescriptor(discImageFile, ref _rootDirExtLoc, ref _rootDirLength);
        }

        /// <summary>
        /// Ouvre une image ISO9660 et lit le Descripteur de Volume.
        /// </summary>
        /// <param name="discImageFile">Chemin du fichier image disque.</param>
        /// <example>Exemple disponible <seealso cref="SimpleIso9660Reader.Open(string,diff.ImageFileFormat)"/></example>
        /// <returns>Un objet <see cref="diff.VolumeInfo"/> fournissant les informations 
        /// du Descripteur de Volume.</returns>
        public diff.VolumeInfo Open(string discImageFile)
        {
            return Open(discImageFile, diff.ImageFileFormat.ISO);
        }

        /// <summary>
        /// Ferme le fichier.
        /// </summary>
        public void Close()
        {
            base.CloseDiscImageFile();
        }

        /// <summary>
        /// Extrait de l'image disque le fichier spécifié.
        /// </summary>
        /// <param name="filePath">Chemin complet du fichier à extraire.</param>
        /// <param name="output">Chemin de sortie.</param>
        /// <example>
        /// L'exemple de code suivant montre comment extraire un fichier contenu dans une image iso.
        /// Avant d'extraire un fichier il est obligatoire d'appeler une des méthodes chargée de récupérer le contenu des entrées ou des fichiers d'un répertoire parent au fichier que vous souhaitez extraire.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             using (SimpleIso9660Reader myIso = new SimpleIso9660Reader())
        ///             {
        ///                 //Lit le Descripteur de Volume.
        ///                 VolumeInfo volInfo = myIso.Open(@"C:\MonIso.iso", ImageFileFormat.ISO);
        /// 
        ///                 if (volInfo != null)
        ///                 {
        ///                     //Voir les méthodes disponible (au choix) ci-dessous pour extraire dans cet exemple
        ///                     //un fichier appelé "doc01.txt" dans le répertoire "\Docs\Txt" de l'iso.
        ///                     
        ///                     //1 - myIso.GetFileSystemEntries();
        ///                     //2 - myIso.GetFileSystemEntries(@"\Docs\");
        ///                     //3 - myIso.GetFileSystemEntries(@"\Docs\Txt", System.IO.SearchOption.TopDirectoryOnly);                    
        ///                     //4 - myIso.GetFiles();
        ///                     //5 - myIso.GetFiles(@"\Docs\Txt");
        ///                     //6 - myIso.GetFiles(@"\Docs\", System.IO.SearchOption.AllDirectories);
        /// 
        ///                     //Extraction du fichier.
        ///                     myIso.ExtractFile(@"\Docs\Txt\doc01.txt", @"C:\Extraction");
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="FileNotFoundException">Le fichier demandé n'a pas été trouvé dans l'iso.</exception>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> ou <paramref name="output"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        public void ExtractFile(string filePath, string output)
        {
            this.ExtractFile(filePath, output, false);
        }

        /// <summary>
        /// Extrait de l'image disque le fichier spécifié.
        /// </summary>
        /// <param name="filePath">Chemin complet du fichier à extraire.</param>
        /// <param name="output">Chemin de sortie.</param>
        /// <param name="newThread"><b>True</b> pour lancer l'extraction dans un nouveau <see cref="System.Threading.Thread"/> (recommandé dans une application WindowsForm pour ne pas figer votre application).
        /// <b>False</b> pour rester dans le <see cref="System.Threading.Thread"/> principal (pour application Console).</param>
        /// <exception cref="FileNotFoundException">Le fichier demandé n'a pas été trouvé dans l'iso.</exception>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> ou <paramref name="output"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> ou <paramref name="output"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        public void ExtractFile(string filePath, string output, bool newThread)
        {
            try
            {
                object[] param = new object[] { DiscImageReadMode.EXTRACT_FILE, 0, filePath, output };

                if (newThread)
                {
                    //Recherche de l'entrée du fichier.
                    diff.RecordEntryInfo recInfo = base._basicFilesInfo[filePath.StartsWith(@"\") ?
                        filePath : string.Concat(@"\", filePath)];

                    if (!string.IsNullOrEmpty(recInfo.Name))
                    {
                        //Définit l'état non signalé du thread.
                        base.AutoThreadEvent.Reset();
                        //Nouveau thread pour éxecuter l'extraction du fichier.
                        Thread th = new Thread(new ParameterizedThreadStart(ThExtractFile));
                        th.Name = "ExtractFileMethod";
                        //Params. passés à la méthodes (mode d'extraction + index + fichier sortie)
                        th.Start(param);
                    }
                }
                else
                {
                    ThExtractFile(param);
                }
            }
            catch (ArgumentNullException argNulEx)
            {
                throw new ArgumentNullException(argNulEx.ParamName, argNulEx.Message);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException(argEx.Message, argEx.ParamName);
            }
            catch (KeyNotFoundException)
            {
                throw new FileNotFoundException(string.Format(@"The file ""{0}"" does not exists !.", filePath));
            }
            catch (Exception)
            {
                throw new Exception(@"An internal error occured during the extraction process.");
            }

        }

        /// <summary>
        /// Extrait tout les fichiers contenu dans l'image.
        /// </summary>
        /// <param name="output">Répertoire de sortie.</param>
        /// <example>
        /// L'exemple de code suivant montre comment extraire l'ensemble des fichiers contenu dans une image iso.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             using (SimpleIso9660Reader myIso = new SimpleIso9660Reader())
        ///             {
        ///                 //Lit le Descripteur de Volume.
        ///                 VolumeInfo volInfo = myIso.Open(@"C:\MonIso.iso", ImageFileFormat.ISO);
        /// 
        ///                 if (volInfo != null)
        ///                 {
        ///                     //Extraction complète du volume.
        ///                     myIso.ExtractFullImage(@"C:\RepSortie");
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="IOException">L'espace disque est inssufisant.</exception>
        /// <exception cref="ArgumentException"><paramref name="output"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="output"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        public void ExtractFullImage(string output)
        {
            this.ExtractFullImage(output, false);
        }

        /// <summary>
        /// Extrait tout les fichiers contenu dans l'image.
        /// </summary>
        /// <param name="output">Répertoire de sortie.</param>
        /// <param name="newThread"><b>True</b> pour lancer l'extraction dans un nouveau <see cref="System.Threading.Thread"/> (recommandé dans une application WindowsForm pour ne pas figer votre application).
        /// <b>False</b> pour rester dans le <see cref="System.Threading.Thread"/> principal (pour application Console).</param>
        /// <exception cref="IOException">L'espace disque est inssufisant.</exception>
        /// <exception cref="ArgumentException"><paramref name="output"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="output"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        public void ExtractFullImage(string output, bool newThread)
        {
            //Param. à passer à ThExtractDiscImage.
            object[] param = new object[] { DiscImageReadMode.EXTRACT_FULL_IMAGE, output };

            try
            {
                if (newThread)
                {
                    //Si une opération d'extraction complète est en cours alors on sort.
                    if (_thExtraction != null)
                    {
                        if (_thExtraction.ThreadState == ThreadState.Running)
                            return;
                    }

                    //Définit l'état non signalé de l'event.
                    base.AutoThreadEvent.Reset();

                    //Si le répertoire de sortie est inexistant on le crée.
                    if (!Directory.Exists(output))
                        Directory.CreateDirectory(output);

                    //Vérifie que l'on dispose d'assez d'espace disque pour extraire l'image.
                    if (new DriveInfo(Path.GetPathRoot(output)).AvailableFreeSpace > new FileInfo(base.DiscFilename).Length)
                    {
                        //Nouveau thread pour éxecuter l'extraction de l'ensemble des fichiers.
                        _thExtraction = new Thread(new ParameterizedThreadStart(ThExtractDiscImage));
                        _thExtraction.Name = "ExtractDiscImageMethod";
                        //Params. passés à la méthodes (mode d'extraction + chemin de sortie)
                        _thExtraction.Start(param);
                    }
                    else
                    {
                        throw new IOException(@"Not enough disk space for extracting the full disc image.");
                    }
                }
                else
                {
                    ThExtractDiscImage(param);
                }
            }
            catch (ArgumentNullException argNulEx)
            {
                throw new ArgumentNullException(argNulEx.ParamName, argNulEx.Message);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException(argEx.Message, argEx.ParamName);
            }
            catch (Exception)
            {
                throw new Exception(@"An internal error occured during the extraction process.");
            }
        }

        /// <summary>
        /// Met fin à l'opération d'extraction.
        /// </summary>
        public void Abort()
        {
            base.AutoThreadEvent.Set();
        }

        /// <summary>
        /// Retourne la table des répertoires de l'iso.
        /// </summary>
        /// <example>
        /// L'exemple de code suivant montre comment récupérer la table des répertoires de l'image.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             using (SimpleIso9660Reader myIso = new SimpleIso9660Reader())
        ///             {
        ///                 //Lit le Descripteur de Volume.
        ///                 VolumeInfo volInfo = myIso.Open(@"C:\MonIso.iso", ImageFileFormat.ISO);
        /// 
        ///                 if (volInfo != null)
        ///                 {
        ///                     //Récupère la liste de tout les répertoires et fichiers présent dans l'image.
        ///                     string[] pathsTable = myIso.GetPathsTable();
        ///                     foreach (string p in pathsTable)
        ///                     {
        ///                         Console.WriteLine(p);
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>Tableau <b>String</b> contenant la liste des répertoires de l'image.</returns>
        public string[] GetPathsTable()
        {
            return this.GetTable();
        }

        /// <summary>
        /// Retourne les noms de tous les fichiers et sous-répertoires de l'image disque. 
        /// </summary>
        /// <example>
        /// L'exemple de code suivant montre comment récupérer la liste de tout les répertoires et fichiers présent dans l'image.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             using (SimpleIso9660Reader myIso = new SimpleIso9660Reader())
        ///             {
        ///                 //Lit le Descripteur de Volume.
        ///                 VolumeInfo volInfo = myIso.Open(@"C:\MonIso.iso", ImageFileFormat.ISO);
        /// 
        ///                 if (volInfo != null)
        ///                 {
        ///                     //Récupère la liste de tout les répertoires et fichiers présent dans l'image.
        ///                     string[] entries = myIso.GetFileSystemEntries();
        ///                     foreach (string e in entries)
        ///                     {
        ///                         Console.WriteLine(e);
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des entrées du système de fichiers dans le répertoire spécifié.</returns>
        public string[] GetFileSystemEntries()
        {
            return this.GetFileSystemEntries(@"\", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Retourne les noms de tous les fichiers et sous-répertoires du répertoire spécifié.
        /// </summary>
        /// <param name="path">Répertoire pour lequel les noms des fichiers et des sous-répertoires sont retournés.</param>
        /// <exception cref="DirectoryNotFoundException">Le chemin d'accès spécifié <paramref name="path"/> dans l'image n'a pas été trouvé.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des entrées du système de fichiers dans le répertoire spécifié.</returns>
        public string[] GetFileSystemEntries(string path)
        {
            return this.GetFileSystemEntries(path, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Retourne les noms de tous les fichiers et sous-répertoires du répertoire spécifié.
        /// </summary>
        /// <param name="path">Répertoire pour lequel les noms des fichiers et des sous-répertoires sont retournés.</param>
        /// <param name="recursive">Une des valeurs SearchOption qui spécifie si l'opération de recherche
        /// doit inclure tous les sous-répertoires ou uniquement le répertoire actif.</param>
        /// <exception cref="DirectoryNotFoundException">Le chemin d'accès spécifié <paramref name="path"/> dans l'image n'a pas été trouvé.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>. Ou <paramref name="recursive"/> ne contient pas un modèle valide.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> ou <paramref name="recursive"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des entrées du système de fichiers dans le répertoire spécifié.</returns>
        public string[] GetFileSystemEntries(string path, SearchOption recursive)
        {
            try
            {
                //Lecture de la table des répertoires.
                this.GetTable();

                //Recherche de l'extent du répertoire.
                diff.PathTableRecordPub rec = _tableRecords[path.StartsWith(@"\") ?
                    path : string.Concat(@"\", path)];

                base.BaseFileStream = new FileStream(base.DiscFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                base.BaseBinaryReader = new BinaryReader(base.BaseFileStream);

                if (!string.IsNullOrEmpty(rec.Name))
                {
                    List<string> szEntries = base.GetFileSystemEntries(path, rec.ExtentLocation,
                        _rootDirLength, recursive == SearchOption.AllDirectories ? true : false, true);

                    return szEntries.ToArray();
                }
                else
                    return null;
            }
            catch (ArgumentNullException argExNull)
            {
                throw new ArgumentNullException(argExNull.ParamName, argExNull.Message);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException(argEx.Message, argEx.ParamName);
            }
            catch (KeyNotFoundException)
            {
                throw new DirectoryNotFoundException(string.Format(@"The entry ""{0}"" does not exists in the paths table", path));
            }
            catch (Exception ex)
            {
                throw new Exception(@"Method failed, an internal error occured.", ex);
            }
        }

        /// <summary>
        /// Retourne les noms de tous les répertoires et sous-répertoires de l'image disque.
        /// </summary>
        /// <example>
        /// L'exemple de code suivant montre comment récupérer la liste de tout les répertoires présent dans l'image.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             using (SimpleIso9660Reader myIso = new SimpleIso9660Reader())
        ///             {
        ///                 //Lit le Descripteur de Volume.
        ///                 VolumeInfo volInfo = myIso.Open(@"C:\MonIso.iso", ImageFileFormat.ISO);
        /// 
        ///                 if (volInfo != null)
        ///                 {
        ///                     //Récupère la liste de tout les répertoires présent dans l'image.
        ///                     string[] directories = myIso.GetDirectories();
        ///                     foreach (string d in directories)
        ///                     {
        ///                         Console.WriteLine(d);
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des répertoires et sous-répertoires.</returns>
        public string[] GetDirectories()
        {
            return FilterEntries(@"\", true, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Obtient les noms des sous-répertoires dans le répertoire spécifié.
        /// </summary>
        /// <param name="path">Obtient les noms des sous-répertoires dans le répertoire spécifié.</param>
        /// <exception cref="DirectoryNotFoundException">Le chemin d'accès spécifié <paramref name="path"/> dans l'image n'a pas été trouvé.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des sous-répertoires dans path.</returns>
        public string[] GetDirectories(string path)
        {
            return FilterEntries(path, true, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Obtient les noms des sous-répertoires dans le répertoire spécifié.
        /// </summary>
        /// <param name="path">Chemin d'accès pour lequel un tableau de noms de sous-répertoires est retourné.</param>
        /// <param name="recursive">Une des valeurs SearchOption qui spécifie si l'opération de recherche
        /// doit inclure tous les sous-répertoires ou uniquement le répertoire actif.</param>
        /// <exception cref="DirectoryNotFoundException">Le chemin d'accès spécifié <paramref name="path"/> dans l'image n'a pas été trouvé.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>. Ou <paramref name="recursive"/> ne contient pas un modèle valide.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> ou <paramref name="recursive"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des sous-répertoires dans path.</returns>
        public string[] GetDirectories(string path, SearchOption recursive)
        {
            return FilterEntries(path, true, recursive);
        }

        /// <summary>
        /// Retourne les noms de tous les fichiers de l'image disque.
        /// </summary>
        /// <example>
        /// L'exemple de code suivant montre comment récupérer la liste de tout les fichiers présent dans l'image.
        /// <code>
        /// using System;
        /// using GomuLibrary.IO;
        /// using GomuLibrary.IO.DiscImage;
        /// 
        /// namespace Demo
        /// {
        ///     class Program
        ///     {
        ///         static void Main(string[] args)
        ///         {
        ///             using (SimpleIso9660Reader myIso = new SimpleIso9660Reader())
        ///             {
        ///                 //Lit le Descripteur de Volume.
        ///                 VolumeInfo volInfo = myIso.Open(@"C:\MonIso.iso", ImageFileFormat.ISO);
        /// 
        ///                 if (volInfo != null)
        ///                 {
        ///                     //Récupère la liste de tout les fichiers présent dans l'image.
        ///                     string[] files = myIso.GetFiles();
        ///                     foreach (string f in files)
        ///                     {
        ///                         Console.WriteLine(f);
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des fichiers.</returns>
        public string[] GetFiles()
        {
            return FilterEntries(@"\", false, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Obtient les noms des sous-répertoires dans le répertoire spécifié.
        /// </summary>
        /// <param name="path">Obtient les noms des fichiers dans le répertoire spécifié.</param>
        /// <exception cref="DirectoryNotFoundException">Le chemin d'accès spécifié <paramref name="path"/> dans l'image n'a pas été trouvé.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des fichiers dans path.</returns>
        public string[] GetFiles(string path)
        {
            return FilterEntries(path, false, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Obtient les noms des fichiers dans le répertoire spécifié.
        /// </summary>
        /// <param name="path">Chemin d'accès pour lequel un tableau de noms des fichiers est retourné.</param>
        /// <param name="recursive">Une des valeurs SearchOption qui spécifie si l'opération de recherche
        /// doit inclure tous les sous-répertoires ou uniquement le répertoire actif.</param>
        /// <exception cref="DirectoryNotFoundException">Le chemin d'accès spécifié <paramref name="path"/> dans l'image n'a pas été trouvé.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> est une chaîne de longueur nulle, ne contient que des espaces blancs ou contient un ou plusieurs caractères non valides comme défini par <b>InvalidPathChars</b>. Ou <paramref name="recursive"/> ne contient pas un modèle valide.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> ou <paramref name="recursive"/> est nul.</exception>
        /// <exception cref="Exception">Error interne à la classe de base <see cref="Iso9660Reader"/>.</exception>
        /// <returns>Tableau <b>String</b> contenant les noms des fichiers dans path.</returns>
        public string[] GetFiles(string path, SearchOption recursive)
        {
            return FilterEntries(path, false, recursive);
        }

        /// <summary>
        /// Obtient les noms des entrées dans le répertoire spécifié pour un filtre appliqué.
        /// </summary>
        /// <param name="path">Chemin d'accès pour lequel un tableau de noms est retourné.</param>
        /// <param name="directory">Filtre de sortie répertoire ou fichier.</param>
        /// <param name="recursive">Une des valeurs SearchOption qui spécifie si l'opération de recherche
        /// doit inclure tous les sous-répertoires ou uniquement le répertoire actif.</param>
        /// <returns>Tableau <b>String</b> contenant les noms entrées filtrées dans path.</returns>
        string[] FilterEntries(string path, bool directory, SearchOption recursive)
        {
            if ((path == @"\") && (recursive == SearchOption.AllDirectories) && directory)
                return this.GetTable();
            else
            {
                this.GetFileSystemEntries(path, recursive);

                List<string> lEntries = new List<string>();
                if (base._basicFilesInfo.Count > 0)
                {
                    Dictionary<string, diff.RecordEntryInfo>.ValueCollection dicVal = base._basicFilesInfo.Values;

                    List<diff.RecordEntryInfo> lrecs = new List<diff.RecordEntryInfo>(dicVal);
                    lrecs.FindAll(delegate(diff.RecordEntryInfo rec)
                    {
                        if (directory)
                        {
                            if (rec.Directory)
                            {
                                lEntries.Add(rec.FullPath);
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            if (rec.Directory)
                                return false;
                            else
                            {
                                lEntries.Add(rec.FullPath);
                                return true;
                            }
                        }
                    });
                }

                return lEntries.ToArray();
            }
        }

        /// <summary>
        /// Méthode chargée d'extraire un fichier particulier de l'iso.
        /// </summary>
        /// <param name="param">Paramètre sur le comportemment de l'extraction.</param>
        void ThExtractFile(object param)
        {
            //Récupération des params. passés à la méthode.
            object[] oParams = (object[])param;
            DiscImageReadMode em = (DiscImageReadMode)oParams[0];
            int idx = Convert.ToInt32(oParams[1]);
            string szFilepath = oParams[2].ToString();
            string szOutput = oParams[3].ToString();

            //Lit le fichier demandé de l'iso et l'extrait.
            base.ReadFile(base._basicFilesInfo[szFilepath], szOutput, em,
                diff.ImageFileFormat.ISO, idx);

            //Si on est en mode extraction fichier simple
            //On ferme le fichier ISO et libère les ressources utilisées.
            if (em == DiscImageReadMode.EXTRACT_FILE)
            {
                Close();

                //Levé de l'évément Terminate indiquant la fin de l'extraction de l'image ISO.
                if (this.Terminate != null)
                    this.Terminate(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Méthode chargée d'extraire l'ISO entièrement.
        /// </summary>
        /// <param name="param">Paramètre sur le comportemment de l'extraction.</param>
        void ThExtractDiscImage(object param)
        {
            try
            {
                //Récupération des params. passés à la méthode.
                object[] oParams = (object[])param;
                DiscImageReadMode em = (DiscImageReadMode)oParams[0];
                string szoutput = oParams[1].ToString();
                uint icount = 0;

                //Ouvre un flux sur le fichier ISO en lecture seulement.
                base.BaseFileStream = new FileStream(base.DiscFilename, FileMode.Open, FileAccess.Read, FileShare.Read, base.SectorSize * 16);
                base.BaseBinaryReader = new BinaryReader(base.BaseFileStream);

                //Récupère en interne la liste des répertoires, sous-répertoires et fichiers de l'image.
                this.GetFileSystemEntries();

                lock (base._basicFilesInfo)
                {
                    Dictionary<string, diff.RecordEntryInfo>.ValueCollection vcolEntries = base._basicFilesInfo.Values;
                    foreach (diff.RecordEntryInfo entry in vcolEntries)
                    {
                        //Thread bloqué pour écoute d'un état signalé pour le Thread.
                        if (base.AutoThreadEvent.WaitOne(0, false))
                        {
                            //Si signal stop l'extraction.
                            if (this.Aborted != null)
                            {
                                Iso9660FileExtractEventArgs dieEa =
                                    new Iso9660FileExtractEventArgs(entry.FullPath, 0, 0, 0, szoutput, 0);
                                this.Aborted(null, dieEa);

                                break;
                            }
                        }
                        else
                        {
                            //Si entrée = répertoire.
                            if (entry.Directory)
                            {
                                string szFullpathDir = Path.Combine(szoutput, entry.FullPath.StartsWith(@"\") ?
                                    entry.FullPath.Substring(1) : entry.FullPath);
                                //Création de celui-ci si inexistant.
                                if (!Directory.Exists(szFullpathDir))
                                    Directory.CreateDirectory(szFullpathDir);
                            }
                            //Si entrée = fichier.
                            else
                            {
                                //Lecture du fichier et extraction.
                                this.ThExtractFile(new object[] { em, icount, entry.FullPath, szoutput });
                                icount++;
                            }
                        }
                    }

                    //Tâche terminée...
                    if (this.Terminate != null)
                        this.Terminate(null, EventArgs.Empty);
                }
            }
            catch (IOException ioEx)
            {
                throw new IOException(ioEx.Message, ioEx.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception(@"Method failed, an internal error occured.", ex);
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Lit la table des répertoires.
        /// </summary>
        /// <returns>Tableau <b>String"</b> de la table des répertoires.</returns>
        protected override string[] GetTable()
        {
            Close();

            string[] ret = base.GetTable();

            Array.Sort(ret);
            return ret;
        }
    }
}