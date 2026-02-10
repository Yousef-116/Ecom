using System;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Ecom.infrastructure.Repositories.Service;

public class ImageManagementService : IImageManagementService
{
    private readonly IFileProvider fileProvider;
    public ImageManagementService(IFileProvider fileProvider)
    {
        this.fileProvider = fileProvider;
    }
    public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
    {
        var savedImageSrc = new List<string>();

        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesPath = Path.Combine(rootPath, "Images", src);

        // 🔥 Ensure directories exist
        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
        }

        foreach (var image in files)
        {
            if (image.Length <= 0) continue;

            var imageName = Path.GetFileName(image.FileName); // safety
            var imagePath = Path.Combine(imagesPath, imageName);
            var imageSrc = $"/Images/{src}/{imageName}";

            using var stream = new FileStream(imagePath, FileMode.Create);
            await image.CopyToAsync(stream);

            savedImageSrc.Add(imageSrc);
        }

        return savedImageSrc;
    }


    public void DeleteImageAsync(string src)
    {
        var info = fileProvider.GetFileInfo(src);
        var root = info.PhysicalPath;
        File.Delete(root);
    }
}
