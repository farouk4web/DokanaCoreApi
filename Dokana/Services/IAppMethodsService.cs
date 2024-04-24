namespace Dokana.Services
{
    public interface IAppMethodsService
    {
        string UploadPicture(IFormFile picture, string folderName, string fileName);

        bool RemovePicture(string src);
    }
}