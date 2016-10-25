using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Configuration;

namespace Insignia.Painel.Helpers.AmazonS3
{
    public class AmazonUpload
    {
        /// <summary>
        /// Efetua o upload de arquivos na Amazon S3
        /// </summary>
        /// <param name="CaminhoArquivo"></param>
        /// <param name="BucketNome"></param>
        /// <param name="SubDiretorio"></param>
        /// <param name="NomeDoArquivo"></param>
        /// <returns>Retorna true caso tenha conseguido efetuar o upload, caso contrário retorna false</returns>
        public bool EnviaArquivoS3(string CaminhoArquivo, string BucketNome, string SubDiretorio, string NomeDoArquivo)
        {
            bool resp = false;

            //Objeto cliente que cria acesso com as chaves e define a região como São Paulo para maior otimização de upload
            IAmazonS3 cliente = new AmazonS3Client(ConfigurationManager.AppSettings["AWSAccessKey"], ConfigurationManager.AppSettings["AWSSecretKey"], RegionEndpoint.SAEast1);

            //Cria o objeto de transferência com o objeto cliente
            TransferUtility util = new TransferUtility(cliente);

            //Cria o objeto de upload
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

            if (SubDiretorio == "" || SubDiretorio == null)
            {
                //Se não tiver sub diretorio recebe o nome do bucket
                request.BucketName = BucketNome;
            }
            else
            {   //Nome do bucket com o diretorio
                request.BucketName = BucketNome + @"/" + SubDiretorio;
            }

            //Nome do arquivo no S3
            request.Key = NomeDoArquivo;

            //Nome local do arquivo
            request.FilePath = CaminhoArquivo;

            //Adiciona o arquivo já como publico para ter permissão de visualização pela url
            request.CannedACL = S3CannedACL.PublicRead;

            //Efetua o upload do arquivo
            util.Upload(request);

            resp = true;

            return resp;
        }

        /// <summary>
        /// Busca a Url de um arquivo em um bucket do S3 na amazon
        /// </summary>
        /// <param name="BucketNome"></param>
        /// <param name="CaminhoDoArquivo"></param>
        /// <param name="NomeArquivo"></param>
        /// <returns>Retorna a url contendo o endereço par ao arquivo</returns>
        public string BuscaUrlArquivo(string BucketNome, string CaminhoDoArquivo, string NomeArquivo)
        {
            string Url = string.Empty;

            //Objeto cliente que cria acesso com as chaves e define a região como São Paulo para maior otimização de upload
            IAmazonS3 cliente = new AmazonS3Client(ConfigurationManager.AppSettings["AWSAccessKey"], ConfigurationManager.AppSettings["AWSSecretKey"], RegionEndpoint.SAEast1);

            //Objeto de requestUrl
            GetPreSignedUrlRequest requestUrl = new GetPreSignedUrlRequest();

            //Nome do bucket no S3
            requestUrl.BucketName = BucketNome;
            //Diretorio e nome do arquivo a ser econtrado
            requestUrl.Key = CaminhoDoArquivo + "/" + NomeArquivo;

            //Data de expiração do link
            requestUrl.Expires = DateTime.Now.AddHours(1);

            //Url que leva ao arquivo
            Url = cliente.GetPreSignedURL(requestUrl);

            return Url;
        }
    }
}