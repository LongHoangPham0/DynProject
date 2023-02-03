using BlueMoon.Business;
using BlueMoon.MVC.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BlueMoon.MVC
{
    public interface IDataItemConverter
    {
        object CastValue(int type, string propertyName, string attemptedValue);
        void Validate(string type, DataItem data);
        void Validate(string propName, string attemptedValue, object[] rules);
    }
    class SpecialBinder : DefaultModelBinder
    {

        public SpecialBinder()
        {
        }
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext.HttpContext.Request.ContentType == "application/json")
            {
                var obj = BaseBindModel(controllerContext, bindingContext);
                if (bindingContext.ModelName == "type")
                {
                    controllerContext.HttpContext.Items["Request-Type"] = obj;
                }
                else if(obj is DataItem)
                {
                    Controller.DataItemConverter.Validate(controllerContext.HttpContext.Items["Request-Type"] as string, (DataItem)obj);
                }
                /*
                else
                {
                    var val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                    
                    if (val !=null && val.AttemptedValue.IsEncrypted())
                    {
                        obj = Convert.ChangeType(val.AttemptedValue.Decrypt(), bindingContext.ModelType);
                    }
                }
                */
                return obj;
            }
                
            bindingContext.ModelMetadata.ConvertEmptyStringToNull = false;
            if (typeof(BaseEntity).IsAssignableFrom(bindingContext.ModelType))
            {
                var model =  BaseBindModel(controllerContext, bindingContext);
                Type type = model.GetType();
                var props = type.GetProperties();
                foreach(var p in props)
                {
                    var inputInfo = p.GetCustomAttributes(typeof(InputAttribute), true);
                    if (inputInfo.Length > 0)
                    {
                        var att = inputInfo[0] as InputAttribute;
                        if (!string.IsNullOrEmpty(att.Validation))
                        {
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            object[] rules = serializer.Deserialize<object[]>(att.Validation);
                            var val = p.GetValue(model);
                            if (val != null) Controller.DataItemConverter.Validate(p.Name, val.ToString(), rules);
                        }
                        
                    }
                }
                return model;
            }
            else if (typeof(DataItem).IsAssignableFrom(bindingContext.ModelType))
            {
                HttpRequestBase request = controllerContext.HttpContext.Request;
                DataItem ret = (DataItem)Activator.CreateInstance(bindingContext.ModelType);
                string prefix = bindingContext.ModelName + ".";
                string propertyName = null;
                string attemptedValue = null;
                NameValueCollection requestData = new NameValueCollection();
                requestData.Add(request.QueryString);
                requestData.Add(request.Form);
                //requestData.Add(request.Files);
                for (int i = 0; i < requestData.Count; i++)
                {
                    string key = requestData.AllKeys[i];
                    if (key.StartsWith(prefix))
                    {
                        propertyName = key.Replace(prefix, "");
                        attemptedValue = bindingContext.ValueProvider.GetValue(key).AttemptedValue;
                        if (attemptedValue.IsEncrypted()) attemptedValue = attemptedValue.Decrypt();
                        ret[propertyName] = Controller.DataItemConverter == null ? attemptedValue : Controller.DataItemConverter.CastValue(int.Parse(requestData[bindingContext.ModelName + ".Type"] ?? "0"), propertyName, attemptedValue);
                    }
                }
                //file
                
                for (int i = 0; i < request.Files.Count; i++)
                {
                    string uploadFolder = Controller.UploadFolder;
                    Directory.CreateDirectory(uploadFolder);
                    string key = request.Files.AllKeys[i];
                    if (key.StartsWith(prefix))
                    {
                        propertyName = key.Replace(prefix, "");
                        if (string.IsNullOrEmpty(request.Files[i].FileName))
                        {
                            ret[propertyName] = null;
                        }
                        else
                        {
                            FileInfo fileInfo = new FileInfo(request.Files[i].FileName);
                            //save file
                            Guid fileId = Guid.NewGuid();
                            request.Files[i].SaveAs(uploadFolder + fileId + ".dat");
                            new FileDataInfo(fileId, fileInfo.Name).Save();
                            ret[propertyName] = fileId;
                        }
                    }
                }
                return ret;
            }
            else if (typeof(Delegate).IsAssignableFrom(bindingContext.ModelType))
            {
                var val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (val != null && val.RawValue is Delegate)
                {
                    return BaseBindModel(controllerContext, bindingContext);
                }
                return null;
            }
            else return BaseBindModel(controllerContext, bindingContext);
        }
        object BaseBindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (val != null && val.AttemptedValue.IsEncrypted())
            {
                return Convert.ChangeType(val.AttemptedValue.Decrypt(), bindingContext.ModelType);
            }
            return base.BindModel(controllerContext, bindingContext);
        }


    }
}
