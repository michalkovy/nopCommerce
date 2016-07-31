using System;
using System.IO;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Plugin.Feed.Zbozi.Models;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using Nop.Web.Framework.Controllers;
using Nop.Core.Domain.Stores;
using System.Collections.Generic;
using Nop.Services.Stores;

namespace Nop.Plugin.Feed.Zbozi.Controllers
{
    [AdminAuthorize]
    public class FeedZboziController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly ZboziSettings _ZboziSettings;
        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;

        public FeedZboziController(ICurrencyService currencyService,
            ILocalizationService localizationService, IPluginFinder pluginFinder, 
            ILogger logger, IWebHelper webHelper,
            ZboziSettings ZboziSettings, ISettingService settingService, IScheduleTaskService scheduleTaskService, IStoreService storeService, IStoreContext storeContext)
        {
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._logger = logger;
            this._webHelper = webHelper;
            this._ZboziSettings = ZboziSettings;
            this._settingService = settingService;
            this._scheduleTaskService = scheduleTaskService;
            this._storeService = storeService;
            this._storeContext = storeContext;
        }

        [NonAction]
        private ScheduleTask FindScheduledTask()
        {
            return _scheduleTaskService.GetTaskByType("Nop.Plugin.Feed.Zbozi.StaticFileGenerationTask, Nop.Plugin.Feed.Zbozi");
        }

        public ActionResult Configure()
        {
            var model = new FeedZboziModel();
            model.ProductPictureSize = _ZboziSettings.ProductPictureSize;
            model.CurrencyId = _ZboziSettings.CurrencyId;
            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                    {
                         Text = c.Name,
                         Value = c.Id.ToString()
                    });
            }

            //task
            ScheduleTask task = FindScheduledTask();
            if (task != null)
            {
                model.GenerateStaticFileEachMinutes = task.Seconds / 60;
                model.TaskEnabled = task.Enabled;
            }

            return View("~/Plugins/Feed.Zbozi/Views/FeedZbozi/Configure.cshtml", model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Configure(FeedZboziModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            string saveResult = "";
            //save settings
            _ZboziSettings.ProductPictureSize = model.ProductPictureSize;
            _ZboziSettings.CurrencyId = model.CurrencyId;
            _settingService.SaveSetting(_ZboziSettings);

            // Update the task
            var task = FindScheduledTask();
            if (task != null)
            {
                task.Enabled = model.TaskEnabled;
                task.Seconds = model.GenerateStaticFileEachMinutes * 60;
                _scheduleTaskService.UpdateTask(task);
                saveResult = _localizationService.GetResource("Plugins.Feed.Zbozi.TaskRestart");
            }

            //redisplay the form
            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            return View("~/Plugins/Feed.Zbozi/Views/FeedZbozi/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("generate")]
        public ActionResult GenerateFeed(FeedZboziModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }


            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("PromotionFeed.Zbozi");
                if (pluginDescriptor == null)
                    throw new Exception("Cannot load the plugin");

                //plugin
                var plugin = pluginDescriptor.Instance() as ZboziService;
                if (plugin == null)
                    throw new Exception("Cannot load the plugin");
                string result = "";
                var store = _storeContext.CurrentStore;
                //string fileName = string.Format("Zbozi_{0}_{1}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                string fileName = string.Format("Zbozi_{0}.xml", Nop.Services.Seo.SeoExtensions.GetSeName(store.Name));
                string filePath = string.Format("{0}content\\files\\exportimport\\{1}", Request.PhysicalApplicationPath, fileName);
                plugin.GenerateFeed(filePath, store);

                //string fileSeznamPath = string.Format("{0}SeznamXML.aspx", Request.PhysicalApplicationPath);
                //System.IO.File.Copy(filePath, fileSeznamPath, true);

                string clickhereStr = string.Format("<a href=\"{0}content/files/exportimport/{1}\" target=\"_blank\">{2}</a> ", _webHelper.GetStoreLocation(false), fileName, _localizationService.GetResource("Plugins.Feed.Zbozi.ClickHere"));
                result += string.Format(_localizationService.GetResource("Plugins.Feed.Zbozi.SuccessResult"), clickhereStr);
                
                model.GenerateFeedResult = result;
            }
            catch (Exception exc)
            {
                model.GenerateFeedResult = exc.Message;
                _logger.Error(exc.Message, exc);
            }


            foreach (var c in _currencyService.GetAllCurrencies(false))
            {
                model.AvailableCurrencies.Add(new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }

            //task
            ScheduleTask task = FindScheduledTask();
            if (task != null)
            {
                model.GenerateStaticFileEachMinutes = task.Seconds / 60;
                model.TaskEnabled = task.Enabled;
            }

            return View("~/Plugins/Feed.Zbozi/Views/FeedZbozi/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("regenerateUrls")]
        public ActionResult RegenerateUrls(FeedZboziModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }


            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("PromotionFeed.Zbozi");
                if (pluginDescriptor == null)
                    throw new Exception("Cannot load the plugin");

                //plugin
                var plugin = pluginDescriptor.Instance() as ZboziService;
                if (plugin == null)
                    throw new Exception("Cannot load the plugin");
                string result = "";

                plugin.RegenerateUrls();

                model.GenerateFeedResult = result;
            }
            catch (Exception exc)
            {
                model.GenerateFeedResult = exc.Message;
                _logger.Error(exc.Message, exc);
            }

            return View("~/Plugins/Feed.Zbozi/Views/FeedZbozi/Configure.cshtml", model);
        }
    }
}
