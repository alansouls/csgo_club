using BlobServices.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobServices.Services
{
    public interface IBlobService
    {
        /// <summary>
        /// Ativa um blob container com o nome passado no parâmetro container.
        /// </summary>
        /// <param name="container"></param>
        void SelectBlobContainer(string container);

        /// <summary>
        /// Faz upload de um arquivo esalva ele com o nome especificado em fileName.
        /// </summary>
        /// <param name="file">Byte Array do arquivo.</param>
        /// <param name="fileName">Nome do arquivo para recuperar depois.</param>
        string UploadFile(byte[] file, string fileName);

        /// <summary>
        /// Recupera o arquivo em um byte array atráves do seu caminho.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        byte[] RetrieveFile(string path);

        /// <summary>
        /// Retorna uma string listando todos os arquivos do container blob ativo.
        /// </summary>
        /// <returns></returns>
        string ListFilesString();

        /// <summary>
        /// Recupera a lista de arquivos como objeto BlobFile do container blob ativo
        /// </summary>
        /// <returns></returns>
        IEnumerable<BlobFile> ListFiles();
    }
}
