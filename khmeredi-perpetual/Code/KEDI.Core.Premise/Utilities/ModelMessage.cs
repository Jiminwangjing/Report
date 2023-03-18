using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
//KEDI.Core.Models.Validation
namespace KEDI.Core.Models.Validation
{
    public enum ModelAction { Reject = -1, Alert = 0, Approve = 1 };
    public enum ModelRedirect { Current };
    public class ModelMessage
    {
        public ModelMessage(){}
        public ModelMessage(ModelStateDictionary ModelState)
        {
            BindModelState(ModelState);
        }

        public ModelAction Action { get; set; } = ModelAction.Reject;
        public bool IsRejected { get { return Action == ModelAction.Reject; } }
        public bool IsAlerted { get { return Action == ModelAction.Alert; } }
        public bool IsApproved { get { return Action == ModelAction.Approve; } }
        public int Count { get { return Data.Count; } }
        public string Redirect { get; set; }

        public Dictionary<string, string> Data = new Dictionary<string, string>();
        public Dictionary<string, object> Items = new Dictionary<string, object>();
        public ModelStateDictionary ModelState
        {
            set { BindModelState(value); }
        }

        private void BindModelState(ModelStateDictionary modelState)
        {
            foreach (var key in modelState.Keys)
            {
                foreach (var error in modelState[key].Errors)
                {
                    if (!Data.ContainsKey(key))
                    {
                        string _key = key;
                        if (key.Contains("."))
                        {
                            string[] keys = key.Split(".");
                            _key = keys[keys.Length - 1];
                        }
                        Data.TryAdd(_key, error.ErrorMessage);
                    }
                }
            }
        }

        public object this[string key] => Items[key];
        public ModelMessage AddItem<T>(T item, string key = "")
        {
            string _key = key;
            if (string.IsNullOrWhiteSpace(key)) { _key = item.GetType().Name; }           
            Items.TryAdd(_key, item);
            return this;
        }

        public ModelMessage Bind(ModelStateDictionary ModelState)
        {
            BindModelState(ModelState);
            return this;
        }

        public ModelMessage Reject()
        {
            Action = ModelAction.Reject;
            return this;
        }

        public ModelMessage Alert()
        {
            Action = ModelAction.Alert;
            return this;
        }

        public ModelMessage Approve()
        {
            Action = ModelAction.Approve;
            return this;
        }

        public void Add(string key, string value)
        {
            try
            {
                if (!Data.ContainsKey(key))
                {
                    Data.Add(key, value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Remove(string key)
        {
            Data.Remove(key);
        }

        public async void SetTimeout(Action<ModelMessage> action, int timeout)
        {
            if (timeout > 0)
            {
                await Task.Delay(timeout);
            }
            else
            {
                await Task.Yield();
            }

            action(this);
        }
    }
}
