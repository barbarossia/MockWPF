using MockWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF
{
    public static class ViewViewModelMappings
    {
        /// <summary>
        /// Dictionary that contains the mappings of viewmodels and views.
        /// </summary>
        private readonly static IDictionary<Type, Type> Mappings;

        /// <summary>
        /// Default constructor
        /// </summary>
        static ViewViewModelMappings()
        {
            Mappings = new Dictionary<Type, Type>
            {
                {typeof (ImportAssemblyViewModel), typeof (ImportAssemblyView)},
                {typeof(SelectImportAssemblyViewModel),typeof( SelectImportAssemblyView)},
                {typeof(CDSPackagesManagerViewModel),typeof( CDSPackagesManagerView)},
            };
        }

        /// <summary>
        /// Searches in the mappings dictionary and returns the corresponding view of a viewmodel.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        public static Type GetViewTypeFromViewModelType(Type viewModelType)
        {
            return Mappings[viewModelType];
        }

    }
}
