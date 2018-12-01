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
    public class NewsService : INewsService
    {
        private readonly IRepository<News, int> _news;
        private readonly IAsyncRepository<News, int> _newsAsync;
        private readonly IRepository<NewsCategory, int> _newsCat;
        private readonly IRepository<NewsRelated, int> _newsRelated;
        private readonly IRepository<Category, int> _category;
        private readonly IAppLogger<NewsService> _appLogger;
        private readonly IRepository<ReportNews , int> _rptNews;
        private readonly IRepository<Notifications , int> _notifi;
        private readonly IRepository<NewsLog , int> _newLog;
        private readonly IRepository<HomeNews , int> _homeNews;

        public NewsService(
            IRepository<News, int> news,
            IAsyncRepository<News, int> newsAsync,
            IRepository<NewsCategory, int> newsCat,
            IRepository<NewsRelated, int> newsRelated,
            IRepository<ReportNews, int> ReportNews,
            IRepository<Category, int> category,
            IRepository<Notifications, int> notifi,
            IRepository<NewsLog, int> newLog,
            IRepository<HomeNews, int> homeNews,
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
            _newLog = newLog;
            _homeNews = homeNews;
        }

        //Quy trình mới
        public void ChuyenLenLanhDao(string id ,string note)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var item = _news.GetById(Int32.Parse(id));
                if (item != null)
                {
                    item.Note = note;
                    item.IsStatus = IsStatus.baocaoldtc;
                    _news.Update(item);
                }
            }
        }



        public IList<Notifications> GetNotifications(string username)
        {
            return _notifi.Table.Where(x => x.UserName == username).ToList();
        }

        public void UpdateNote(string id , string note)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var item = _news.GetById(Int32.Parse(id));
                if(item != null)
                {
                    item.Note = note;
                    _news.Update(item);
                }
            }
        }

        public void AddOrUpdateNotification(Notifications model)
        {
            if(model.Id != 0)
            {
                // la update
                var item = _notifi.GetById(model.Id);
                if (item == null) item = new Notifications();
                item.isRead = model.isRead;
                item.UserName = model.UserName;
                item.OnCreated = DateTime.Now;
                _notifi.Update(item);
            }
            else
            {

                var item = new Notifications();
                item.Notification = model.Notification;
                item.NewsId = model.NewsId;
                item.isRead = model.isRead;
                item.UserName = model.UserName;
                item.OnCreated = DateTime.Now;
                _notifi.Update(item);
                // la add
            }
        }

        public IList<ListItemNewsCategory> ReportNewsCategory()
        {
            var dataCat = _category.Table.ToList();
            var dataReturn = new List<ListItemNewsCategory>();
            foreach (var item in dataCat)
            {
                var obj = new ListItemNewsCategory();
                var countItem = _news.Table.Where(x => x.CategoryId == item.Id).Count();
                obj.category = item.Name.ToString();
                obj.count = countItem;
                dataReturn.Add(obj);
            }
            return dataReturn;
        }

        public IList<ListItemNewsMapYear> ReportNewsYear()
        {
            var dataYear = _news.Table.Select(s => new {
                year = s.OnCreated != null ? s.OnCreated.Value.Year : 0
            }).Distinct().OrderBy(i => i.year).ToList();

            var dataReturn = new List<ListItemNewsMapYear>();
            try
            {
                dataYear = dataYear.Where(x => x.year != 0).ToList();
                if (dataYear.Count > 0)
                {
                    for (var i = dataYear.First().year; i <= dataYear.Last().year; i++)
                    {
                        var obj = new ListItemNewsMapYear();
                        obj.year = i.ToString();
                        obj.count = _news.Table.Where(x => x.OnCreated.HasValue && x.OnCreated.Value.Year == i).Count();
                        dataReturn.Add(obj);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
            
            return dataReturn;
        }
        
        public IQueryable<ReportNews> GetTablesRpt()
        {
            return _rptNews.Table;
        }

        public void AddOrUpdateNewReport(ReportNewsView model)
        {
            if (model.Id > 0)
            {
                var item = _rptNews.GetById(model.Id);
                if (item == null) item = new ReportNews();
                item.NewsId = model.NewsId;
                item.data = model.data;
                item.UserName = model.UserName;
                item.OnCreated = DateTime.Now;
                _rptNews.Update(item);
            }
            else
            {
                var item = new ReportNews();
                item.NewsId = model.NewsId;
                item.data = model.data;
                item.UserName = model.UserName;
                item.OnCreated = DateTime.Now;
                _rptNews.Add(item);
            }
        }

        public ReportNews GetBaocaos(int id)
        {
            var query = GetTablesRpt().Where(x => x.NewsId == id).FirstOrDefault();
            return query;
        }

        public void AddNewRelated(NewsRelatedModel model)
        {
            if (model.Id > 0)
            {
                var item = _newsRelated.GetById(model.Id);
                item = model.ModelToEntity(item);
                _newsRelated.Update(item);
            }
            else 
                _newsRelated.Add(model.ModelToEntity());
        }

        public News AddOrUpdateModel(NewsModel model)
        {
            News entity = null;
            if (model.Id > 0) entity = _news.GetById(model.Id);
            entity = model.ModelToEntity(entity);
            if (model.Id > 0)
            {

                entity.IsStatus = model.IsStatus != null ? (IsStatus)model.IsStatus : IsStatus.tiepnhan;
                entity.OnUpdated = DateTime.Now;
                _news.Update(entity);               
            }
            else
            {
                var itemCat = _category.GetById(model.CategoryId.Value) ?? null;
                entity.IsStatus = model.IsStatus != null ? (IsStatus)model.IsStatus : IsStatus.tiepnhan;
                entity.OnCreated = DateTime.Now;
                entity.CategoryId = model.CategoryId;
                var modelAdd = _news.Add(entity);

                //push notifi
                var notifi = new Notifications();
                notifi.isRead = false;
                notifi.UserName = model.UserName;
                notifi.Notification = model.Name;
                notifi.NewsId = modelAdd.Id;
                notifi.OnCreated = DateTime.Now;
                _notifi.Add(notifi);
            }
            return entity;
        }

        public void AddOrUpdateHome(NewsModel model)
        {
            HomeNews entity = null;
            if (model.Id > 0) entity = _homeNews.GetById(model.Id);
            entity = model.ModelToEntityHome(entity);
            if (model.Id > 0)
            {

                entity.IsStatus = model.IsStatus != null ? (IsStatus)model.IsStatus : IsStatus.tiepnhan;
                entity.OnUpdated = DateTime.Now;
                _homeNews.Update(entity);
            }
            else
            {
                var itemCat = _category.GetById(model.CategoryId.Value) ?? null;
                entity.IsStatus = model.IsStatus != null ? (IsStatus)model.IsStatus : IsStatus.tiepnhan;
                entity.OnCreated = DateTime.Now;
                entity.CategoryId = model.CategoryId;
                var modelAdd = _homeNews.Add(entity);
            }
        }

        public void AddOrUpdateHomeNews(HomeNews model)
        {
            HomeNews entity = null;
            if (model.Id > 0) entity = _homeNews.GetById(model.Id);
            entity = model.ModelToEntityHomeNews(entity);
            if (model.Id > 0)
            {

                entity.IsStatus = (IsStatus)model.IsStatus;
                entity.OnUpdated = DateTime.Now;
                _homeNews.Update(entity);
            }
            else
            {
                var itemCat = _category.GetById(model.CategoryId.Value) ?? null;
                entity.IsStatus = (IsStatus)model.IsStatus;
                entity.OnCreated = DateTime.Now;
                entity.CategoryId = model.CategoryId;
                var modelAdd = _homeNews.Add(entity);
            }
        }

        public void AddOrUpdate(NewsModel model)
        {
            News entity = null;
            if (model.Id > 0) entity = _news.GetById(model.Id);
            entity = model.ModelToEntity(entity);
            if (model.Id > 0)
            {
                //entity.IsStatus = model.IsStatus != null ? (IsStatus)model.IsStatus : IsStatus.tiepnhan;
                if (!(entity.CategoryId > 0))
                {
                    entity.CategoryId = 12;
                }
                entity.OnUpdated = DateTime.Now;
                _news.Update(entity);               
            }
            else
            {
                var itemCat = _category.GetById(model.CategoryId.Value) ?? null;
                entity.IsStatus = model.IsStatus != null ? (IsStatus)model.IsStatus : IsStatus.tiepnhan;
                entity.OnCreated = DateTime.Now;
                entity.OnUpdated = DateTime.Now;
                entity.NewsCategories = new List<NewsCategory>
                {
                    new NewsCategory
                    {
                        CategoryId = entity.CategoryId,
                        NewsId = entity.Id,
                        News = entity,
                        Categories = itemCat
                    }
                };
                var modelAdd = _news.Add(entity);

                //push notifi
                var notifi = new Notifications();
                notifi.isRead = false;
                notifi.UserName = model.UserName;
                notifi.Notification = model.Name;
                notifi.NewsId = modelAdd.Id;
                notifi.OnCreated = DateTime.Now;
                _notifi.Add(notifi);
            }
        }

        public void Delete(int id)
        {
            var entity = _news.GetById(id);
            if(entity != null)
            {
                entity.OnDeleted = DateTime.Now;
                entity.IsShow = false;
                entity.IsStatus = IsStatus.deleted;
                _news.Update(entity);
            }
        }

        public void DeleteHome(int id)
        {
            var entity = _homeNews.GetById(id);
            if (entity != null)
            {
                entity.OnDeleted = DateTime.Now;
                entity.IsShow = false;
                entity.IsStatus = IsStatus.deleted;
                _homeNews.Update(entity);
            }
        }

        public void Hoactac(int id)
        {
            var entity = _homeNews.GetById(id);
            if (entity != null)
            {
                entity.OnDeleted = null;
                entity.IsShow = true;
                entity.IsStatus = IsStatus.publish;
                _homeNews.Update(entity);
            }
        }

        public int DeleteAll(params string[] ids)
        {
            int count = 0;
            try
            {
                if (ids == null || ids.Count() == 0) return count;
                ids.ToList().ForEach(item =>
                {
                    if(int.TryParse(item, out int Id))
                    {
                        count += UpdateNews(Id, false, IsStatus.deleted, true);
                    }
                });
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning(ex.ToString());
                return count;
            }
            return count;
        }

        public int Drafts(params string[] ids)
        {
            int count = 0;
            try
            {               
                if (ids == null || ids.Count() == 0) return count;
                ids.ToList().ForEach(item =>
                {
                    if (int.TryParse(item, out int Id))
                    {
                        count += UpdateNews(Id, false, IsStatus.draft, false);
                    }
                });
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning(ex.ToString());
                return count;
            }
            return count;
        }

        public ItemsNewWithCategory GetItemNewsForTopOfCategory()
        {
            var datas = new List<News>();
            for (int i = 1; i <= 5; i++)
            {
                var data = GetTables()
                             .Where(t => t.CategoryId == i && t.IsShow == true && (t.IsStatus == IsStatus.publish || t.IsStatus == IsStatus.approved))
                             .OrderByDescending(t => t.OnCreated).Take(1).ToList();
                datas.AddRange(data);
            }

            return new ItemsNewWithCategory(1, null, datas.MapLstEntityToListModel());
        }

        public ItemsNewWithCategory GetItemNewsWithCategory(int? catId, int? count)
        {
            if (!count.HasValue) count = 4;
            var catName = _category.GetById(catId.Value)?.Name;
            var datas = GetTables()
                .Where(t => t.CategoryId == catId && t.IsShow == true && (t.IsStatus == IsStatus.publish || t.IsStatus == IsStatus.approved))
                .OrderByDescending(t => t.OnCreated).Take(count.Value);
            return new ItemsNewWithCategory(catId.Value, catName, datas.ToList().MapLstEntityToListModel());
        }

        public NewsModel GetNewsById(int id)
        {
            return GetTables().SingleOrDefault(x => x.Id == id).EntityToModel();
        }

        public HomeNews GetHomeNewsById(int id)
        {
            return _homeNews.Table.SingleOrDefault(x => x.Id == id);
        }

        public IList<HomeNews> GetHomeNewsByCate(int? id = 0 , int? number = 0)
        {
            var homeNews = _homeNews.Table.Where(x => x.IsStatus != IsStatus.deleted && !x.OnDeleted.HasValue);
            homeNews = homeNews.Where(z => z.IsStatus == IsStatus.publish);
            if (id != 0)
            {
                homeNews = homeNews.Where(x => x.CategoryId == id);
            }
            if(number != 0)
            {
                homeNews = homeNews.Take((int)number);
            }
            homeNews = homeNews.OrderByDescending(x => x.OnCreated);
            var homeNewsLst = homeNews.ToList();

            foreach (var iem in homeNewsLst)
            {
                if (!string.IsNullOrEmpty(iem.Image))
                {
                    iem.Image = "http://cdn.eportal.today" + iem.Image;
                }
            }
            return homeNewsLst;
        }

        public void UpdateStatus(string id, IsStatus status)
        {
            var data = _news.GetById(Int32.Parse(id));
            if(data != null)
            {
                data.IsStatus = status;
                _news.Update(data);
            }
        }

        public IList<NewsModel> GetNewsPaging(out int rows, int? take = 15, int? skip = 0, int? catId = -1)
        {
            var query = GetTables().Where(x => x.IsShow == true && x.IsStatus == IsStatus.publish && !x.OnDeleted.HasValue);
            if (query == null)
            {
                rows = 0;
                return new List<NewsModel>() { };
            }
            if (catId.HasValue && catId.Value > -1) query = query.Where(x => x.CategoryId == catId);
            rows = query.Count();
            query = query.OrderByDescending(x => x.OnCreated);
            if (skip > rows)
            {
                take = rows % take.Value;
                skip = 0;
                query = query.Skip(rows - take.Value).Take(take.Value);
            }
            else query = query.Skip(skip.Value).Take(take.Value);
            return query.Select(x => x.EntityToModel()).ToList();
        }

        public IList<News> GetLstNewsAno(string name, string email, string sdt, string publish_date)
        {
            var query = _news.Table;
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.UserFullName == name);
            }

            if (!string.IsNullOrEmpty(sdt))
            {
                query = query.Where(x => x.UserPhone == sdt);
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(x => x.UserEmail == email);
            }

            //if (!string.IsNullOrEmpty(publish_date))
            //{
            //    query = query.Where(x => x.OnPublished == publish_date);
            //}
            return query.ToList();
        }

        public IList<ListItemNewsModel> GetLstNewsPaging(out int rows, int? skip = 0, int? take = 15, string keyword = "",
            int? categoryId = -1, int? status = -1, int? type = -1 , string username = "", string GroupId = "")
        {
            //var query = GetTables().Where(x => x.IsType == IsType.noType && x.IsType != IsType.topic);
            var query = GetTables();
            if (status == -1)
            {
                //query = query.Where(x => !x.OnDeleted.HasValue && !x.IsShow);
            }

            if (!string.IsNullOrEmpty(username))
            {
                if (GroupId == "ldtcmt" || GroupId == "ttdl")
                {
                    query = query.Where(z =>
                        _newLog.Table.Where(i => i.NewsId == z.Id).Select(x => x.GroupNameTo).Contains(GroupId)
                    );
                }
                else
                {
                    query = query.Where(z =>
                        _newLog.Table.Where(i => i.NewsId == z.Id).Select(x => x.UserName).Contains(username)
                    );
                }
                
            }

            if (query == null)
            {
                rows = 0;
                return new List<ListItemNewsModel>() { };
            }
            if (!string.IsNullOrEmpty(keyword) && keyword != "")
                query = query.Where(x => x.Name.Contains(keyword) || x.Sename.Contains(keyword));

            if (categoryId > 0) query = query.Where(x => x.CategoryId == categoryId);
            if (status >= 0) query = query.Where(x => x.IsStatus == (IsStatus)status);
            if (type > 0) query = query.Where(x => x.IsType == (IsType)type);
            rows = query.Count();

            if (skip > rows)
            {
                take = rows % take.Value;
                skip = 0;
                query = query.OrderByDescending(x => x.Id).Skip(rows - take.Value).Take(take.Value);
            }
            else query = query.OrderByDescending(x => x.Id).Skip(skip.Value).Take(take.Value);

            query = query.OrderByDescending(x => x.OnUpdated);
            var dataRetun =  query.Select(x => new ListItemNewsModel
            {
                Id = x.Id,
                Name = x.Name ?? null,
                Image = x.Image ?? null,
                Sename = x.Sename ?? null,
                Abstract = x.Abstract ?? null,
                Content = x.Content ?? null,
                IsShow = x.IsShow,
                OnCreated = x.OnCreated,
                OnDeleted = x.OnDeleted,
                OnUpdated = x.OnUpdated,
                OnPublished = x.OnPublished,
                status = x.IsStatus,
                Note = x.Note,
                fileUpload = x.fileUpload,
                IsType = x.IsType,
                

            }).ToList();
            var dataNews = new List<ListItemNewsModel>();
            foreach(var itemdata in dataRetun)
            {
                if (GroupId == "ldtcmt" || GroupId == "ttdl")
                {
                    var data2 = _newLog.Table.Where(x => x.NewsId == itemdata.Id)
                        .Where(i => i.GroupNameTo == username).FirstOrDefault();
                    itemdata.Note = data2.Note;
                }
                else
                {
                    var data2 = _newLog.Table.Where(x => x.NewsId == itemdata.Id)
                        .Where(i => i.UserName == username).FirstOrDefault();
                    itemdata.Note = data2.Note;
                }
                dataNews.Add(itemdata);
            }
            return dataNews;
        }

        public IList<ListItemNewsModel> GetLstHomeNewsPaging(out int rows, int? skip = 0, int? take = 15, string keyword = "",
            int? categoryId = -1, int? status = -1, int? type = -1, string username = "", string GroupId = "")
        {
            //var query = GetTables().Where(x => x.IsType == IsType.noType && x.IsType != IsType.topic);
            var query = _homeNews.Table;
            if (status == -1)
            {
                //query = query.Where(x => !x.OnDeleted.HasValue && !x.IsShow);
            }

            if (query == null)
            {
                rows = 0;
                return new List<ListItemNewsModel>() { };
            }
            if (!string.IsNullOrEmpty(keyword) && keyword != "")
                query = query.Where(x => x.Name.Contains(keyword) || x.Sename.Contains(keyword));

            if (categoryId > 0) query = query.Where(x => x.CategoryId == categoryId);
            if (status >= 0) query = query.Where(x => x.IsStatus == (IsStatus)status);
            if (type > 0) query = query.Where(x => x.IsType == (IsType)type);
            if(status != 4)
            {
                query = query.Where(x => x.IsStatus != IsStatus.deleted && !x.OnDeleted.HasValue);
            }
            
            rows = query.Count();

            if (skip > rows)
            {
                take = rows % take.Value;
                skip = 0;
                query = query.OrderByDescending(x => x.Id).Skip(rows - take.Value).Take(take.Value);
            }
            else query = query.OrderByDescending(x => x.Id).Skip(skip.Value).Take(take.Value);

            var dataRetun = query.Select(x => new ListItemNewsModel
            {
                Id = x.Id,
                Name = x.Name ?? null,
                Image = x.Image ?? null,
                Sename = x.Sename ?? null,
                Abstract = x.Abstract ?? null,
                Content = x.Content ?? null,
                IsShow = x.IsShow,
                OnCreated = x.OnCreated,
                OnDeleted = x.OnDeleted,
                OnUpdated = x.OnUpdated,
                OnPublished = x.OnPublished,
                status = x.IsStatus,
                Note = x.Note,
                fileUpload = x.fileUpload,
                IsType = x.IsType
            }).ToList();
            return dataRetun;
        }

        public IList<ListItemNewsMap> ReportNews()
        {
            //var query = GetTables().Where(x => x.IsType == IsType.noType && x.IsType != IsType.topic);
            var query = GetTables().Where(x => !string.IsNullOrEmpty(x.Lat) && !string.IsNullOrEmpty(x.Lng)).Take(100).ToList();
            
            return query.Select(x => new ListItemNewsMap
            {
                Name = x.Name ?? null,
                AddressString = x.AddressString,
                Lat = x.Lat,
                Lng = x.Lng
            }).ToList();
        }

        public IList<ListItemNewsModel> GetLstTopicPaging(out int rows, int? skip = 0, int? take = 15, string keyword = "", int? categoryId = -1, int? status = -1, int? type = -1)
        {
            var query = GetTables().Where(x => x.IsType == IsType.topic);
            if(status == -1)
            {
                query = query.Where(x => !x.OnDeleted.HasValue && !x.IsShow);
            }
            if (query == null)
            {
                rows = 0;
                return new List<ListItemNewsModel>() { };
            }
            if (!string.IsNullOrEmpty(keyword) && keyword != "")
                query = query.Where(x => x.Name.Contains(keyword) || x.Sename.Contains(keyword));

            if (categoryId > 0) query = query.Where(x => x.CategoryId == categoryId);
            if (status > 0) query = query.Where(x => x.IsStatus == (IsStatus)status);
            if (type > 0) query = query.Where(x => x.IsType == (IsType)type);
            rows = query.Count();

            if (skip > rows)
            {
                take = rows % take.Value;
                skip = 0;
                query = query.OrderByDescending(x => x.Id).Skip(rows - take.Value).Take(take.Value);
            }
            else query = query.OrderByDescending(x => x.Id).Skip(skip.Value).Take(take.Value);

            return query.Select(x => new ListItemNewsModel
            {
                Id = x.Id,
                Name = x.Name ?? null,
                Image = x.Image ?? null,
                Sename = x.Sename ?? null,
                Abstract = x.Abstract ?? null,
                IsShow = x.IsShow,
                OnCreated = x.OnCreated,
                OnDeleted = x.OnDeleted,
                OnUpdated = x.OnUpdated,
                OnPublished = x.OnPublished,
                status = x.IsStatus
            }).ToList();
        }

        public IQueryable<News> GetTables()
        {
            return _news.Table;
        }

        public int Process(params string[] ids)
        {
            int count = 0;
            try
            {               
                if (ids == null || ids.Count() == 0) return count;
                ids.ToList().ForEach(item =>
                {
                    if (int.TryParse(item, out int Id))
                    {
                        count += UpdateNews(Id, false, IsStatus.approved, false);
                    }
                });                
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning(ex.ToString());
                return count;
            }
            return count;
        }

        public int Publishs(string[] ids , string username = "")
        {
            int count = 0;
            try
            {
                if (ids == null || ids.Count() == 0) return count;
                ids.ToList().ForEach(item =>
                {
                    if (int.TryParse(item, out int Id))
                    {
                        count += UpdateNews(Id, true, IsStatus.publish, false , username);
                        if(string.IsNullOrEmpty(username))
                        {
                            //push notifi
                            var notifi = _notifi.Table.Where(x => x.NewsId == Id).FirstOrDefault();
                            if(notifi != null)
                            {
                                var notifiAdd = new Notifications();
                                notifiAdd.isRead = false;
                                notifiAdd.UserName = "ldtcmt";
                                notifiAdd.Notification = notifi.Notification;
                                notifiAdd.NewsId = notifi.NewsId;
                                notifiAdd.OnCreated = DateTime.Now;
                                _notifi.Add(notifiAdd);
                            }
                        }
                        else
                        {
                            //push notifi
                            var notifi = _notifi.Table.Where(x => x.NewsId == Id).FirstOrDefault();
                            if (notifi != null)
                            {
                                var notifiAdd = new Notifications();
                                notifiAdd.isRead = false;
                                notifiAdd.UserName = username;
                                notifiAdd.Notification = notifi.Notification;
                                notifiAdd.NewsId = notifi.NewsId;
                                notifiAdd.OnCreated = DateTime.Now;
                                _notifi.Add(notifiAdd);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning(ex.ToString());
                return count;
            }
            return count;
        }

        public int Baocaos(string[] ids, string username = "")
        {
            int count = 0;
            try
            {
                if (ids == null || ids.Count() == 0) return count;
                ids.ToList().ForEach(item =>
                {
                    if (int.TryParse(item, out int Id))
                    {
                        count += UpdateNews(Id, true, IsStatus.baocao, false, username);
                    }
                });
            }
            catch (Exception ex)
            {
                _appLogger.LogWarning(ex.ToString());
                return count;
            }
            return count;
        }



        private int UpdateNews(int Id, bool isShow, IsStatus status, bool isDelete = false , string username = "")
        {
            int count = 0;
            var entity = _news.GetById(Id);
            if(status == IsStatus.publish)
            {
                if (string.IsNullOrEmpty(username))
                {
                    entity.UserName = entity.UserName + ",ldtcmt";
                }
                else
                {
                    entity.UserName = entity.UserName + "," + username;
                    status = IsStatus.phancong;
                    //push notifi
                    var notifi = new Notifications();
                    notifi.isRead = false;
                    notifi.UserName = username;
                    notifi.Notification = "Bạn được lãnh đạo phân công " + entity.Name;
                    notifi.NewsId = entity.Id;
                    notifi.OnCreated = DateTime.Now;
                    _notifi.Add(notifi);
                }
                
            }

            if (entity != null)
            {
                entity.IsShow = isShow;
                entity.IsStatus = status;
                if (isDelete)
                {
                    entity.OnDeleted = DateTime.Now;
                    entity.OnPublished = null;
                    entity.OnUpdated = null;
                }
                else
                {
                    entity.OnUpdated = DateTime.Now;
                    entity.OnDeleted = null;
                }
                if (status == IsStatus.publish)
                {
                    entity.OnPublished = DateTime.Now;
                    entity.OnDeleted = null;
                }
                
                _news.Update(entity);
                count += 1;
            }
            return count;
        }
    }
}
