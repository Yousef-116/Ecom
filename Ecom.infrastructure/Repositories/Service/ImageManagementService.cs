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
    public Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
    {
        throw new NotImplementedException();
    }

    public Task DeleteImageAsync(string src)
    {
        throw new NotImplementedException();
    }
}
