using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;
using iText.Barcodes;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Chemin du dossier à surveiller
        string watchedFolderPath = @"Z:\teste";

        // Créer une instance de FileSystemWatcher pour surveiller le dossier spécifié
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = watchedFolderPath;

        // Activer les notifications pour les changements de fichier
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

        // Définir l'événement pour surveiller la création de nouveaux fichiers
        watcher.Created += OnFileCreated;

        // Démarrer la surveillance
        watcher.EnableRaisingEvents = true;

        Console.WriteLine("Le programme est en attente de nouveaux fichiers dans le dossier...");

        // Attendre une touche pour terminer
        Console.ReadKey();
    }
    private static string ExtractNumberOfPackages(PdfDocument pdfDocument)
    {
        int numberOfPages = pdfDocument.GetNumberOfPages();
        string numberOfPackages = "";

        for (int i = 1; i <= numberOfPages; i++)
        {
            PdfPage page = pdfDocument.GetPage(i);
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            string currentPageText = PdfTextExtractor.GetTextFromPage(page, strategy);

            int index = currentPageText.IndexOf("Nb Colis ");
            if (index != -1)
            {
                // Trouver la valeur suivante après "Nb Colis"
                int startIndex = index + "Nb Colis ".Length;
                int endIndex = startIndex;
                while (endIndex < currentPageText.Length && char.IsDigit(currentPageText[endIndex]))
                {
                    endIndex++;
                }

                if (startIndex < endIndex)
                {
                    numberOfPackages = currentPageText.Substring(startIndex, endIndex - startIndex).Trim();
                    break;
                }
            }
        }
        return numberOfPackages;
    }
    private static void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        try
        {
            if (Path.GetExtension(e.FullPath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Nouveau fichier PDF détecté : {e.Name}");

                // Attendre quelques secondes pour s'assurer que le fichier est complètement écrit
                System.Threading.Thread.Sleep(2000);

                // Utilisation de iTextProcessor
                iTextProcessor.ProcessFile(e.FullPath);

                // Utilisation de AsposeProcessor
                AsposeProcessor.ProcessFile(e.FullPath);

                Console.WriteLine($"Les opérations ont été effectuées dans le fichier {e.Name}.");

                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(e.FullPath)))
                {
                    string numberOfCopies = ExtractNumberOfPackages(pdfDocument);

                    if (!string.IsNullOrEmpty(numberOfCopies) && int.TryParse(numberOfCopies, out int copies))
                    {
                        Console.WriteLine($"Nombre de colis trouvé : {copies}");
                       
                        // Imprimer le fichier PDF avec le nombre de copies récupéré
                        for (int i = 0; i < copies; i++)
                        {
                            PrintPdfWithAcrobat(e.FullPath);
                        }

                    }
                    else
                    {
                        Console.WriteLine("Nombre de colis non trouvé ou invalide. Impression par défaut à une copie.");
                        PrintPdfWithAcrobat(e.FullPath, 1); // Impression par défaut à une copie
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la manipulation du fichier {e.Name} : {ex.Message}");
        }
    }
    private static void PrintPdfWithAcrobat(string filePath, int numberOfCopies = 1)
    {
        try
        {
            // Chemin de l'exécutable d'Acrobat Reader
            string acrobatPath = @"C:\Program Files\Adobe\Acrobat DC\Acrobat\Acrobat.exe";

            if (File.Exists(acrobatPath))
            {
                // Arguments pour imprimer le fichier PDF
                string arguments = $"/p /h \"{filePath}\"";

                System.Diagnostics.Process.Start(acrobatPath, arguments);
            }
            else
            {
                Console.WriteLine("Acrobat Reader n'a pas été trouvé.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'impression du fichier avec Acrobat Reader : {ex.Message}");
        }
    }
}
