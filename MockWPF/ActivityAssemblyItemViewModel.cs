using AddIn;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MockWPF
{
    public class ActivityAssemblyItemViewModel : ViewModelBase
    {
        private readonly ActivityAssemblyItem assemblyItem;
        private readonly ReadOnlyObservableCollection<ActivityItem> activityItems;
        private readonly ReadOnlyObservableCollection<AssemblyName> referencedAssemblies;
        public DelegateCommand ReviewAssemblyCommand;

        private bool isResolved = false; // Backing for the IsResolved property, which determines if the Location was missing at the start but is now filled in.


        public ActivityAssemblyItem Source
        {
            get { return assemblyItem; }
        }

        public ActivityAssemblyItemViewModel(ActivityAssemblyItem item)
        {
            assemblyItem = item;
            activityItems =
                new ReadOnlyObservableCollection<ActivityItem>(new ObservableCollection<ActivityItem>(item.ActivityItems));
            referencedAssemblies = new ReadOnlyObservableCollection<AssemblyName>(new ObservableCollection<AssemblyName>(item.ReferencedAssemblies));
            ReviewAssemblyCommand = new DelegateCommand(ReviewAssemblyCommandExecute, CanReviewAssemblyCommandExecute);
        }

        /// <summary>
        /// Gets or sets ActivityItems.
        /// ActivityItems represents the activity types in this assembly. Activity type was wrapped in ActivityItem class.
        /// </summary>
        public ReadOnlyObservableCollection<ActivityItem> ActivityItems
        {
            get { return activityItems; }
        }

        /// <summary>
        /// Gets or sets AuthorityGroup.
        /// Assembly only show to user whose group is/is greater than assembly's authority group.
        /// </summary>
        public string AuthorityGroup
        {
            get
            {
                return assemblyItem.AuthorityGroup;
            }

            set
            {
                assemblyItem.AuthorityGroup = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => AuthorityGroup);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the assembly has been reviewed.
        /// </summary>
        public bool IsReviewed
        {
            get
            {
                return assemblyItem.IsReviewed;
            }

            set
            {
                assemblyItem.IsReviewed = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => IsReviewed);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the assembly is safe for type load.
        /// </summary>
        public bool? NotSafeForTypeLoad
        {
            get
            {
                return assemblyItem.NotSafeForTypeLoad;
            }
            set
            {
                assemblyItem.NotSafeForTypeLoad = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => NotSafeForTypeLoad);
            }
        }

        /// <summary>
        /// Gets or sets the caching status of the assembly.
        /// </summary>
        public CachingStatus CachingStatus
        {
            get
            {
                return assemblyItem.CachingStatus;
            }
            set
            {
                assemblyItem.CachingStatus = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => CachingStatus);
            }
        }


        /// <summary>
        /// Gets or sets Category. Object will be grouped into toolbox category by this property.
        /// </summary>
        [Required]
        public string Category
        {
            get
            {
                return assemblyItem.Category;
            }

            set
            {
                assemblyItem.Category = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => Category);
            }
        }

        /// <summary>
        /// Gets or sets CreateDateTime.
        /// </summary>
        public DateTime CreationDateTime
        {
            get
            {
                return assemblyItem.CreationDateTime;
            }

            set
            {
                assemblyItem.CreationDateTime = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => CreationDateTime);
            }
        }

        /// <summary>
        /// Gets or sets the name of the user that created the assembly.
        /// </summary>
        public string CreatedBy
        {
            get
            {
                return assemblyItem.CreatedBy;
            }

            set
            {
                assemblyItem.CreatedBy = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => CreatedBy);
            }
        }

        /// <summary>
        /// Gets or sets the description of the assembly.
        /// </summary>
        public string Description
        {
            get
            {
                return assemblyItem.Description;
            }

            set
            {
                assemblyItem.Description = value;
                IsDirty = true;
                RaisePropertyChanged(() => Description);
            }
        }

        /// <summary>
        /// Gets or sets the display name of the assembly.
        /// </summary>
        [Required]
        public string DisplayName
        {
            get
            {
                return assemblyItem.DisplayName;
            }

            set
            {
                assemblyItem.DisplayName = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => DisplayName);
            }
        }

        /// <summary>
        /// Gets or sets FullName.
        /// For ActivtyItem, full name is the FullName of its Activity type.
        /// For AssemblyName, full name is the FullName of its Assembly (or AssemblyName.FullName)
        /// For WorkflowItem, full name is its class name with namespace.
        /// </summary>
        public string FullName
        {
            get
            {
                return assemblyItem.AssemblyName.FullName;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return assemblyItem.IsReadOnly;
            }

            set
            {
                assemblyItem.IsReadOnly = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => IsReadOnly);
            }
        }

        /// <summary>
        /// Gets or sets Name.
        /// For ActivityItem and WorkflowItem, name is its Activity/Workflow class name.
        /// For AssemblyItem, name is its Assembly's name (Assembly.GetName().Name)
        /// </summary>
        public string Name
        {
            get
            {
                return assemblyItem.Name;
            }

            set
            {
                assemblyItem.Name = value;
                IsDirty = true;
                Validate();
                NotifyChangedAssemblyNameDerivedProperties();
            }
        }

        /// <summary>
        /// Gets or sets Status of object.
        /// </summary>
        public string Status
        {
            get
            {
                return assemblyItem.Status;
            }

            set
            {
                assemblyItem.Status = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => Status);
            }
        }

        /// <summary>
        /// Gets or sets Tags for searching.
        /// </summary>
        public string Tags
        {
            get
            {
                return assemblyItem.Tags;
            }

            set
            {
                assemblyItem.Tags = value;
                IsDirty = true;

                foreach (var item in ActivityItems)
                {
                    if (string.IsNullOrEmpty(item.Tags))
                        item.Tags = value;
                }
                RaisePropertyChanged(() => Tags);
            }
        }

        /// <summary>
        /// Gets or sets UpdateDateTime.
        /// </summary>
        public DateTime UpdateDateTime
        {
            get
            {
                return assemblyItem.UpdateDateTime;
            }

            set
            {
                assemblyItem.UpdateDateTime = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => UpdateDateTime);
            }
        }

        /// <summary>
        /// Gets or sets UpdatedBy.
        /// </summary>
        public string UpdatedBy
        {
            get
            {
                return assemblyItem.UpdatedBy;
            }

            set
            {
                assemblyItem.UpdatedBy = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => UpdatedBy);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the user selected this assembly. If true, it will be shown in the toolbox.
        /// </summary>
        public bool UserSelected
        {
            get
            {
                return assemblyItem.UserSelected;
            }

            set
            {
                assemblyItem.UserSelected = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => UserSelected);
            }
        }

        /// <summary>
        /// Notes added by the user for this specific assembly
        /// </summary>
        public string ReleaseNotes
        {
            get
            {
                return assemblyItem.ReleaseNotes;
            }
            set
            {
                assemblyItem.ReleaseNotes = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => ReleaseNotes);
            }
        }

        /// <summary>
        /// Gets or sets the version of the assembly.
        /// </summary>
        public string Version
        {
            get
            {
                return assemblyItem.Version != null ? assemblyItem.Version.ToString() : string.Empty;
            }
            set
            {
                Version assemblyVersion;
                if (System.Version.TryParse(value, out assemblyVersion))
                {
                    assemblyItem.Version = assemblyVersion;
                    IsDirty = true;
                    Validate();
                }
                else
                {
                    throw new ArgumentOutOfRangeException(string.Format("\"{0}\" is not a valid version number", value));
                }
                NotifyChangedAssemblyNameDerivedProperties();
            }
        }

        /// <summary>
        /// Collection of the referenced assemblies in this assembly
        /// </summary>
        public ReadOnlyObservableCollection<AssemblyName> ReferencedAssemblies
        {
            get { return referencedAssemblies; }
        }

        /// <summary>
        /// Gets or sets AssemblyName. The AssemblyName of the Assembly. Set this property will influent Name/FullName/Version
        /// </summary>
        public AssemblyName AssemblyName
        {
            get
            {
                return assemblyItem.AssemblyName;
            }

            set
            {
                if (value != null)
                {
                    assemblyItem.AssemblyName = value;
                    IsDirty = true;
                    Validate();
                }

                NotifyChangedAssemblyNameDerivedProperties();
            }
        }

        /// <summary>
        /// Gets or set the full path of the assembly
        /// </summary>
        public string Location
        {
            get
            {
                return assemblyItem.Location;
            }
            set
            {
                assemblyItem.Location = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => Location);
            }
        }

        /// <summary>
        ///  Was the location blank, and has been then filled in?
        /// </summary>
        public bool IsResolved
        {
            get { return isResolved; }
            set
            {
                isResolved = value;
                RaisePropertyChanged(() => IsResolved);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UserWantsToUpload.
        /// </summary>
        public bool UserWantsToUpload
        {
            get
            {
                return assemblyItem.UserWantsToUpload;
            }

            set
            {
                assemblyItem.UserWantsToUpload = value;
                IsDirty = true;
                Validate();
                RaisePropertyChanged(() => UserWantsToUpload);
            }
        }

        /// <summary>
        /// Refresh the name and version information.
        /// </summary>
        private void NotifyChangedAssemblyNameDerivedProperties()
        {
            RaisePropertyChanged(() => AssemblyName);
            RaisePropertyChanged(() => Name);
            RaisePropertyChanged(() => FullName);
            RaisePropertyChanged(() => Version);
        }

        private void ReviewAssemblyCommandExecute()
        {
            assemblyItem.IsReviewed = true;
        }

        private bool CanReviewAssemblyCommandExecute()
        {
            return (!string.IsNullOrEmpty(assemblyItem.Location));
        }

        /// <summary>
        /// This is for debugging purposes -- makes it easier to see the items in a list (etc).
        /// </summary>
        /// <returns>A string representation of this assembly item.</returns>
        public override string ToString()
        {
            return string.Format("{0} version {1}, {2}",
                                 Name,
                                 Version,
                                 Location == String.Empty
                                   ? "<no location specified>"
                                   : Location);
        }

    }
}
