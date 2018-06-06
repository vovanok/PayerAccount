using System;
using System.IO;

namespace PayerAccount.Utils
{
    public class ReceiptDocumentWorker
    {
        private string templatePath;
        private string contentValue;

        public ReceiptDocumentWorker(string templatePath)
        {
            this.templatePath = templatePath;

            if (!File.Exists(templatePath))
                throw new Exception($"Template is not exist.");

            contentValue = File.ReadAllText(templatePath);
        }

        public void PutToPlaceholder(int placeholderNumber, string value)
        {
            contentValue = contentValue.Replace($"{{{placeholderNumber}}}", value);
        }

        public void Save(string filename)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(contentValue))
                return;

            if (File.Exists(filename))
                File.Delete(filename);

            File.WriteAllText(filename, contentValue);
        }
    }
}
