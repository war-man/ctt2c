﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppPortal.AdminSite.Services.Extensions;
using AppPortal.AdminSite.Services.Interfaces;
using AppPortal.AdminSite.Services.Models;
using AppPortal.AdminSite.Services.Models.News;
using AppPortal.Core.Entities;
using AppPortal.Core.Interfaces;

namespace AppPortal.AdminSite.Services.Administrator
{
    public class NewsLogService : INewsLog
    {
        private readonly IRepository<News, int> _news;
        private readonly IAsyncRepository<News, int> _newsAsync;
        private readonly IRepository<NewsCategory, int> _newsCat;
        private readonly IRepository<NewsRelated, int> _newsRelated;
        private readonly IRepository<Category, int> _category;
        private readonly IAppLogger<NewsService> _appLogger;
        private readonly IRepository<ReportNews, int> _rptNews;
        private readonly IRepository<Notifications, int> _notifi;
        private readonly IRepository<NewsLog, int> _newslog;
        public NewsLogService(
            IRepository<News, int> news,
            IAsyncRepository<News, int> newsAsync,
            IRepository<NewsCategory, int> newsCat,
            IRepository<NewsRelated, int> newsRelated,
            IRepository<ReportNews, int> ReportNews,
            IRepository<Category, int> category,
            IRepository<Notifications, int> notifi,
            IRepository<NewsLog, int> newslog,
            IAppLogger<NewsService> appLogger)
        {
            _news = news;
            _newsAsync = newsAsync;
            _newsCat = newsCat;
            _newsRelated = newsRelated;
            _category = category;
            _appLogger = appLogger;
            _rptNews = ReportNews;
            _notifi = notifi;
            _newslog = newslog;
        }


        public void AddOrUpdate(NewsLog model)
        {
            NewsLog entity = null;
            if (model.Id > 0) entity = _newslog.GetById(model.Id);
            if (model.Id > 0)
            {

                entity.OnCreated = DateTime.Now;
                _newslog.Update(entity);
            }
            else
            {
                entity = model;
                _newslog.Add(entity);
            }
        }

        public void Delete(int id)
        {
            var entity = _news.GetById(id);
            if (entity != null)
            {
                entity.OnDeleted = DateTime.Now;
                entity.IsShow = false;
                entity.IsStatus = IsStatus.deleted;
                _news.Update(entity);
            }
        }

        
        public NewsLog GetNewsLogByNewsIdUser(int NewsId , string username)
        {
            return _newslog.Table.Where(x => x.NewsId == NewsId).Where(z => z.UserName == username).FirstOrDefault();
        }

        public IList<NewsLog> GetNewsLogByNewsIdGroupNameFrom(int NewsId, string GroupNameFrom , int type)
        {
            var compare = IsTypeStatus.is_phancong;
            if(type == 4)
            {
                compare = IsTypeStatus.is_chuyencongvan;
            }
            return _newslog.Table.Where(x => x.NewsId == NewsId)
                .Where(i => i.TypeStatus == compare)
                .ToList();
        }

        public IList<NewsLog> GetReport(int NewsId)
        {
            return _newslog.Table.Where(x => x.NewsId == NewsId).ToList();
        }

        public IList<NewsLog> GetNewsLogByNewsIdNameFrom(int NewsId, string UserName)
        {
            return _newslog.Table.Where(x => x.NewsId == NewsId)
                .Where(z => z.UserName == UserName)
                .ToList();
        }

        public NewsLog AddOrUpdateReport(int id, string data)
        {
            var item = _newslog.Table.Where(x => x.Id == id).FirstOrDefault();
            if(item != null)
            {
                item.Data = data;
                _newslog.Update(item);
                var news = _news.Table.Where(x => x.Id == item.NewsId).FirstOrDefault();
                if(news != null)
                {
                    news.IsStatus = IsStatus.baocao;
                    _news.Update(news);
                }
            }
            return item;
        }
    }
}
