using BlobServices.Entity;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlobServices.Services
{
    public class BlobService : IBlobService
    {
        private readonly CloudBlobClient _blobClient;
        private CloudBlobContainer _activeContainer;

        /// <summary>
        /// Recebe a string de conexão da conta de armazenamento do azure e inicia um cliente blob.
        /// </summary>
        /// <param name="connectionString"></param>
        public BlobService(string connectionString)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

            _blobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// Recupera a lista de arquivos como objeto BlobFile do container blob ativo
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BlobFile> ListFiles()
        {
            var blobList = _activeContainer.ListBlobs(useFlatBlobListing: true);

            List<BlobFile> result = new List<BlobFile>();
            foreach (var blob in blobList)
            {
                result.Add(new BlobFile(blob.Uri));
            }

            return result;
        }

        /// <summary>
        /// Retorna uma string listando todos os arquivos do container blob ativo.
        /// </summary>
        /// <returns></returns>
        public string ListFilesString()
        {
            var blobList = _activeContainer.ListBlobs(useFlatBlobListing: true);

            string resultStr = "";
            foreach (var blob in blobList)
            {
                var blobFileName = blob.Uri.LocalPath;
                var partialStr = "File: ";
                partialStr += blobFileName + " ";
                partialStr += "at " + blob.Uri + "\n";

                resultStr += partialStr;
            }

            return resultStr;
        }

        /// <summary>
        /// Recupera o arquivo em um byte array atráves do seu caminho.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] RetrieveFile(string path)
        {
            var blobBlock = _activeContainer.GetBlockBlobReference(path);
            if (!blobBlock.Exists())
                return Enumerable.Empty<byte>().ToArray();

            byte[] file = null;
            blobBlock.DownloadToByteArray(file, 0);

            return file;
        }

        /// <summary>
        /// Ativa um blob container com o nome passado no parâmetro container.
        /// </summary>
        /// <param name="container"></param>
        public void SelectBlobContainer(string container)
        {
            _activeContainer = _blobClient.GetContainerReference(container);
            _activeContainer.CreateIfNotExists(BlobContainerPublicAccessType.Blob, null, null);
        }

        /// <summary>
        /// Faz upload de um arquivo esalva ele com o nome especificado em fileName.
        /// </summary>
        /// <param name="file">Byte Array do arquivo.</param>
        /// <param name="fileName">Nome do arquivo para recuperar depois.</param>
        public void UploadFile(byte[] file, string fileName)
        {
            var blobBlock = _activeContainer.GetBlockBlobReference(fileName);
            blobBlock.UploadFromByteArray(file, 0, file.Count());
        }
    }
}
