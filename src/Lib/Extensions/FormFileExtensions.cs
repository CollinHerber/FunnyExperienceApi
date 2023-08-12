using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FunnyExperience.Server.Lib.Extensions;

public static class FormFileExtensions
{
    public const int ImageMinimumBytes = 64;
    
    public static bool IsImageOrVideo(this IFormFile postedFile)
    {
        //-------------------------------------------
        //  Check the image / pdf / video mime types
        //-------------------------------------------
        if (postedFile.ContentType.ToLower() != "image/jpg" &&
            postedFile.ContentType.ToLower() != "image/jpeg" &&
            postedFile.ContentType.ToLower() != "image/pjpeg" &&
            postedFile.ContentType.ToLower() != "image/gif" &&
            postedFile.ContentType.ToLower() != "image/x-png" &&
            postedFile.ContentType.ToLower() != "image/png" &&
            postedFile.ContentType.ToLower() != "image/jfif" &&
            postedFile.ContentType.ToLower() != "image/svg+xml" &&
            postedFile.ContentType.ToLower() != "application/pdf" &&
            postedFile.ContentType.ToLower() != "video/mp4" &&
            postedFile.ContentType.ToLower() != "video/mov" &&
            postedFile.ContentType.ToLower() != "video/wmv" &&
            postedFile.ContentType.ToLower() != "video/flv" &&
            postedFile.ContentType.ToLower() != "video/avi" &&
            postedFile.ContentType.ToLower() != "video/avchd" &&
            postedFile.ContentType.ToLower() != "video/webm" &&
            postedFile.ContentType.ToLower() != "video/mkv")
        {
            return false;
        }
    
        //-------------------------------------------
        //  Check the image / pdf / video extension
        //-------------------------------------------
        if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".jfif"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".svg"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".pdf"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".mp4"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".mov"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".wmv"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".flv"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".avi"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".avchd"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".webm"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".mkv")
        {
            return false;
        }
    
        //-------------------------------------------
        //  Attempt to read the file and check the first bytes
        //-------------------------------------------
        try
        {
            if (!postedFile.OpenReadStream().CanRead)
            {
                return false;
            }
            //------------------------------------------
            //check whether the image size exceeding the limit or not
            //------------------------------------------ 
            if (postedFile.Length < ImageMinimumBytes)
            {
                return false;
            }
    
            var buffer = new byte[ImageMinimumBytes];
            postedFile.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
            var content = System.Text.Encoding.UTF8.GetString(buffer);
            if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    
        return true;
    }

    public static bool IsBelowGivenSize(this IFormFile postedFile, int mb)
    {
        var fileSize = postedFile.Length;
        return !((fileSize / 1048576.0) > mb);
    }

}