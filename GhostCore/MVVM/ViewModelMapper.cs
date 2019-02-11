using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace GhostCore.MVVM
{
    public static class ViewModelMapper
    {
        private static Dictionary<object, INotifyPropertyChanged> _modelViewModelMapping = new Dictionary<object, INotifyPropertyChanged>();

        public static void Register(object model, INotifyPropertyChanged vm)
        {
            if (_modelViewModelMapping.ContainsKey(model))
            {
                Debug.Log("WARNING: Unspecified behaviour! What do we do if a model is already regitered?");
                _modelViewModelMapping[model] = vm;
                return;
            }

            _modelViewModelMapping.Add(model, vm);
        }

        public static INotifyPropertyChanged GetViewModel(object model)
        {
            if (model == null)
                return null;

            if (!_modelViewModelMapping.ContainsKey(model))
                return null;

            return _modelViewModelMapping[model];
        }

        public static object[] GetModels(INotifyPropertyChanged viewModel)
        {
            var q = from x in _modelViewModelMapping.Values
                    where x == viewModel
                    select x;

            return q.ToArray();
        }

        public static void Clear()
        {
            _modelViewModelMapping = new Dictionary<object, INotifyPropertyChanged>();
        }
    }
}
