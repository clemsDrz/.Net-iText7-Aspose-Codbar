using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Barcodes;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

public static class iTextProcessor
{
    public static void ProcessFile(string filePath)
    {
        try
        {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(filePath)))
            {
                string keyword = "No :";
                string extractedText = ExtractTextAfterKeyword(pdfDocument, keyword);

                Console.WriteLine($"Texte extrait : '{extractedText}'");  // Affiche le texte extrait

                pdfDocument.Close();

                string barcodeText = CleanBarcodeText(extractedText);

                Console.WriteLine($"Suite de caractères pour le code-barres : '{barcodeText}'");  // Affiche la suite de caractères pour le code-barres

                if (IsValidBarcodeText(barcodeText))
                {
                    GenerateBarcodeInPdf(filePath, barcodeText);
                    Console.WriteLine("Le code-barres a été ajouté au fichier PDF avec succès !");
                }
                else
                {
                    Console.WriteLine("Le texte du code-barres n'est pas valide. Veuillez vérifier et réessayer.");
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Erreur lors de la lecture du fichier PDF : {ex.Message}");
        }
    }

    private static string ExtractTextAfterKeyword(PdfDocument pdfDocument, string keyword)
    {
        int numberOfPages = pdfDocument.GetNumberOfPages();
        string extractedText = "";

        for (int i = 1; i <= numberOfPages; i++)
        {
            PdfPage page = pdfDocument.GetPage(i);
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
            string currentPageText = PdfTextExtractor.GetTextFromPage(page, strategy);

            int keywordIndex = currentPageText.IndexOf(keyword);
            if (keywordIndex != -1 && keywordIndex + keyword.Length + 11 <= currentPageText.Length)
            {
                extractedText = currentPageText.Substring(keywordIndex + keyword.Length, 11).Trim();
                break;
            }
        }
        return extractedText;
    }

    private static string CleanBarcodeText(string text)
    {
        string cleanedText = text.Replace("No :", "").Trim();
        return new string(cleanedText.Where(c => char.IsLetterOrDigit(c)).ToArray());
    }
    
    private static bool IsValidBarcodeText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        string allowedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return text.All(c => allowedCharacters.Contains(c));
    }

    private static void GenerateBarcodeInPdf(string pdfFilePath, string barcodeText)
    {
        using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfFilePath), new PdfWriter(pdfFilePath + ".temp")))
        {
            Document document = new Document(pdfDocument);

            Barcode128 barcode = new Barcode128(document.GetPdfDocument());
            barcode.SetCode(barcodeText);
            PdfFormXObject barcodeObject = barcode.CreateFormXObject(null, null, pdfDocument);
            Image barcodeImage = new Image(barcodeObject);

            barcodeImage.Scale(5f, 5f);

            float pageWidth = pdfDocument.GetPage(1).GetPageSize().GetWidth();
            float barcodeWidth = barcodeImage.GetImageScaledWidth();

            float x = (pageWidth - barcodeWidth) / 2;
            float y = 50;
            barcodeImage.SetFixedPosition(x, y);

            document.Add(barcodeImage);

            document.Close();
        }

        File.Delete(pdfFilePath);
        File.Move(pdfFilePath + ".temp", pdfFilePath);
    }
    
}
