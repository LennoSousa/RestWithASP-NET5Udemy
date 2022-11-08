using Microsoft.AspNetCore.Http;
using RestWithASPNETUdemy.Data.VO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RestWithASPNETUdemy.Business.Implementations
{
    public class FileBusinessImplementation : IFileBusiness
    {
        private readonly string _basePath;
        private readonly IHttpContextAccessor _context;

        public FileBusinessImplementation(IHttpContextAccessor context)
        {
            _context = context;
            _basePath = Directory.GetCurrentDirectory() + "\\UploadDir\\";
        }

        public byte[] GetFile(string filename)
        {
            var filePath = _basePath + filename;
            return File.ReadAllBytes(filePath);
        }

        public async Task<FileDetailVO> SaveFileToDisk(IFormFile file)
        {
            //criando o objeto de retorno, com as informações preenchidas.
            FileDetailVO fileDetail = new FileDetailVO();

            //em seguida, descobrimos qual é a extensão do arquivo que está sendo carregado.
            var fileType = Path.GetExtension(file.FileName);

            //recuperamos a url, baseado no host da aplicação. sendo local, servidor, nuvem...
            var baseUrl = _context.HttpContext.Request.Host;

            //verifica se o arquivo a ser salvo, tem algumas dessas extencões suportadas, caso não tenha, é necessário incluir a nova extensão.
            if (fileType.ToLower() == ".pdf" || 
                fileType.ToLower() == ".jpg" || 
                fileType.ToLower() == ".png" || 
                fileType.ToLower() == ".jpeg")
            {
                //recupera o nome do arquivo
                var docName = Path.GetFileName(file.FileName);

                //caso o arquivo não seja nulo e maior do que 0, pode prosseguir com o salvamento.
                if (file != null && file.Length > 0)
                {

                    var destination = Path.Combine(_basePath, "", docName);
                    fileDetail.DocumentName = docName;
                    fileDetail.DocType = fileType;
                    fileDetail.DocUrl = Path.Combine(baseUrl + "/api/file/v1/" + fileDetail.DocumentName);

                    // abrindo o file stream com o disco, com o sistema de arquivos da máquina, no modo de criação/gravação.
                    using var stream = new FileStream(destination, FileMode.Create);
                    //realizando gravação no disco.
                    await file.CopyToAsync(stream);

                    // o stream pode ser salvo em um banco de dados, fica a dica para implementar essa solução tbm.
                }
            }
            
            return fileDetail;
        }
        public async Task<List<FileDetailVO>> SaveFilesToDisk(IList<IFormFile> files)
        {
            List<FileDetailVO> list = new List<FileDetailVO>();
            foreach (var file in files)
            {
                list.Add(await SaveFileToDisk(file));
            }
            return list;
        }

        
    }
}
