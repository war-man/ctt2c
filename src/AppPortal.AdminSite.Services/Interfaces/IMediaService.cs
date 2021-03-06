﻿using AppPortal.AdminSite.Services.Models.News;
using AppPortal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppPortal.AdminSite.Services.Interfaces
{
    public interface IMediaService
    {
        IList<Media> GetMedia(string type , int is_publish);
        Media AddOrEdit(Media model);
        void delete(int id);
        Config GetConfig(string type);
        Config AddOrEditConfig(string type, string url);

        Vanban AddOrEditVanban(Vanban model);
        IList<Vanban> GetVanban(string type, string searchValue = "", int number = 0);
        void deleteVanban(int id);

        IList<Logs> GetLogs();
    }
}
