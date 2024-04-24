namespace Dokana.Services
{
    public class AppMethodsService : IAppMethodsService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public AppMethodsService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public string UploadPicture(IFormFile picture, string folderName, string fileName)
        {
            try
            {
                //  Upload To Server method
                FileInfo info = new(picture.FileName);
                var src = $"/Uploads/{folderName}/{fileName}{info.Extension}";

                var path = Path.Combine(_env.WebRootPath + src);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    picture.CopyTo(stream);
                }

                return src;
            }

            catch (Exception)
            {
                var placeholderImage = "product.png";
                if (folderName == "Users")
                    placeholderImage = "user.png";

                return $"/Uploads/{folderName}/{placeholderImage}";
            }

            #region Cloudinary Service Not Tested Cause الناقة داست علي الباقة
            //try
            //{
            //var cloudName = _configuration.GetValue<string>("SecretKeys:CloudinaryCloudName");
            //var apiKey = _configuration.GetValue<string>("SecretKeys:CloudinaryApiKey");
            //var apiSecret = _configuration.GetValue<string>("SecretKeys:CloudinaryApiSecret");

            //var account = new Account
            //{
            //    Cloud = cloudName,
            //    ApiKey = apiKey,
            //    ApiSecret = apiSecret
            //};

            //Cloudinary _cloudinary = new(account);

            //var result = new ImageUploadResult();

            //using (var stream = picture.OpenReadStream())
            //{
            //    var pictureInUpload = new ImageUploadParams()
            //    {
            //        File = new FileDescription(picture.FileName, stream),
            //        PublicId = directory
            //    };
            //    result = _cloudinary.Upload(pictureInUpload);
            //}

            //return result.SecureUrl.AbsoluteUri;

            //}
            //catch (Exception)
            //{
            //    return "/uploads/products/p.png";
            //}
            #endregion
        }

        public bool RemovePicture(string src)
        {
            // remove picture 
            var path = Path.Combine(_env.WebRootPath + src);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }
    }
}
