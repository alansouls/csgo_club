using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlobServices.Entity
{
    /// <summary>
    /// Estrutura para guardar informações de um arquivo salvo no Blob.
    /// </summary>
    public class BlobFile
    {
        /// <summary>
        /// Nome completo do arquivo: pasta/arquivo
        /// </summary>
        public string FullName { get; }
        /// <summary>
        /// Nome da pasta do arquivo, ou caminho de pastas: pasta ou pasta1/pasta2
        /// </summary>
        public string Folder { get; set; }
        /// <summary>
        /// Nome apenas do arquivo: arquivo.bin
        /// </summary>
        public string Name { get;}
        /// <summary>
        /// Uri para recuperação do arquivo
        /// </summary>
        public Uri Uri { get;}

        public BlobFile(Uri blobUri)
        {
            Name = ExtractName(blobUri);
            Uri = blobUri;
            Folder = ExtractFolder(blobUri);
            FullName = Folder + "/" + Name;
        }

        private string ExtractName(Uri uri)
        {
            var name = uri.LocalPath.Split("/").Last();
            return name;
        }

        private string ExtractFolder(Uri uri)
        {
            var paths = uri.LocalPath.Split("/");
            return string.Join("/", paths.Take(paths.Count() - 1).ToList());
        }
    }
}
