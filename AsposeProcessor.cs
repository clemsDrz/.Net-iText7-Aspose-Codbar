using System;
using System.IO;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using System.Collections.Generic;

public static class AsposeProcessor
{
    public static void ProcessFile(string filePath)
    {
        int maxRetries = 3;
        int retries = 0;
        
        while (retries < maxRetries)
        {
            try
            {
                Console.WriteLine($"Nouveau fichier PDF détecté : {Path.GetFileName(filePath)}");

                // Charger le fichier PDF
                using (Document pdf = new Document(filePath))
                {
                    // Effectuer les remplacements spécifiques
                    ReplaceTextInPdf(pdf, "FRPAR", "Fret Maritime");
                    ReplaceTextInPdf(pdf, "ORLY", "Entree a Quai");

                    // Enregistrer le fichier PDF mis à jour
                    pdf.Save(filePath);
                    Console.WriteLine($"Les remplacements ont été effectués dans le fichier {Path.GetFileName(filePath)}.");
                    break; // Sortir de la boucle si tout s'est bien passé
                }
            }
            catch (IOException ex)
            {
                retries++;
                if (retries < maxRetries)
                {
                    Console.WriteLine($"Erreur lors de la manipulation du fichier {Path.GetFileName(filePath)}. Tentative {retries + 1} sur {maxRetries}. Attente avant la nouvelle tentative...");
                    System.Threading.Thread.Sleep(5000); // Attendre 5 secondes avant de réessayer
                }
                else
                {
                    Console.WriteLine($"Échec après {maxRetries} tentatives pour manipuler le fichier {Path.GetFileName(filePath)} : {ex.Message}");
                }
            }
        }
    }

    private static void ReplaceTextInPdf(Document pdf, string searchText, string replaceText)
    {
        // Instancier l'objet TextFragmentAbsorber
        TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber(searchText);

        // Parcourir chaque page du document
        foreach (Page pdfPage in pdf.Pages)
        {
            // Rechercher le texte
            pdfPage.Accept(textFragmentAbsorber);
        }

        // Vérifier si le texte a été trouvé
        if (textFragmentAbsorber.TextFragments.Count > 0)
        {
            // Itérer à travers TextFragment individuel
            foreach (TextFragment tf in textFragmentAbsorber.TextFragments)
            {
                // Mettre à jour le texte avec le remplacement
                tf.Text = replaceText;

                // Augmenter la taille du texte
                tf.TextState.FontSize = 40; // Mettre la taille de la police à 18 points
            }
        }
    }
    
}


