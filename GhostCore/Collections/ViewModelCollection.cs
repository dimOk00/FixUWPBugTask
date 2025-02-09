﻿using GhostCore.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace GhostCore.Collections
{
    public class ViewModelCollection<T, K> : ObservableCollection<T> where T : ViewModelBase<K>
    {
        private IList<K> _modelCollection;

        public ViewModelCollection(IList<K> modelCollection)
        {
            _modelCollection = modelCollection;
        }

        public ViewModelCollection(IList<K> modelCollection, IEnumerable<T> col) : base(col)
        {
            _modelCollection = modelCollection;
        }

        public ViewModelCollection(IList<K> modelCollection, List<T> list) : base(list)
        {
            _modelCollection = modelCollection;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T x in e.NewItems)
                    {
                        _modelCollection.Add(x.Model);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (T x in e.OldItems)
                    {
                        _modelCollection.Remove(x.Model);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }
}
