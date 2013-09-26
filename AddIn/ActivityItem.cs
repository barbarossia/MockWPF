using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddIn
{
    [Serializable]
    public class ActivityItem : ViewModelBase, IComparable<ActivityItem>
    {

        private const string DefaultTags = "";
        private const string DefaultStatus = "";

        private bool hasCodeBehind;       // The has code behind.
        private bool isUserFavorite;      // The is user favorite.
        private ActivityAssemblyItem parentAssemblyItem; // The parent assembly.
        private string xamlCode;          // The xaml code.
        private bool isUserInteraction;   // is user interaction.
        private bool isSwitch;            // is a switch.
        private string developerNote;     // The devloper note of activity.
        private string activityType;      // The type of activity.
        private bool isReviewed = false;  // has this item been reviewed?
        private CachingStatus cachingStatus;     // The caching status.
        private string category;          // The category. Used to group object into toolbox category.
        private DateTime createDateTime;  // The create date time.
        private string createdBy;         // The created by.
        private string description;       // The description of object.
        private string displayName;       // The display name of object.
        private string fullName;          // The full name of object.
        private bool isReadOnly;          // If the object is read only.
        private string name;              // The name of object
        private string oldName = null;    // Tracking the old name for the re-processing of the XamlCode that is required
        private string status;            // The status of object.
        private string tags;              // The tags of object for search.
        private DateTime updateDateTime;  // The update date time.
        private string updatedBy;         // The updated by.
        private bool userSelected = true; // If user selected this object. If true, it will be shown in toolbox. Default value is True.
        private string version;           // The version.
        private bool isEditing = false;   // Is this item currently being edited? Used in the Import Wizard to track the current item.
        private bool isEdited = false;    // Has the item been edited? This is distinct from IsDirty. It indicates this item is dirty and focus has moved to some other item. Used in the Import Wizard.
        private string oldVersion;        // The version will be translate to service update lock


        /// <summary>
        ///  Is this item currently being edited? Used in the Import Wizard to track the current item.
        /// </summary>
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                IsEdited = !value;
                RaisePropertyChanged(() => IsEditing);
            }
        }

        /// <summary>
        /// Has the item been edited? This is distinct from IsDirty. It indicates this item is dirty and focus has moved to some other item. Used in the Import Wizard.
        /// </summary>
        public bool IsEdited
        {
            get { return isEdited; }
            set
            {
                isEdited = value;
                RaisePropertyChanged(() => IsEdited);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether a Activity type has code behind.
        /// For example, the class inherent CodeActivity has its code behind.
        /// If a activity type has code behind, means we cannot show its XAML.
        /// </summary>
        public bool HasCodeBehind
        {
            get { return hasCodeBehind; }
            set
            {
                hasCodeBehind = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => HasCodeBehind);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user select this activity type as favorite.
        /// </summary>
        public bool IsUserFavorite
        {
            get { return isUserFavorite; }
            set
            {
                isUserFavorite = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => IsUserFavorite);
            }
        }

        /// <summary>
        /// Gets or sets the AssemblyItem contains this ActivityItem.
        /// </summary>
        public ActivityAssemblyItem ParentAssemblyItem
        {
            get { return parentAssemblyItem; }
            set
            {
                parentAssemblyItem = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => ParentAssemblyItem);
            }
        }

        /// <summary>
        /// Gets the AssemblyName of the AssemblyItem contains this ActivityItem.
        /// </summary>
        public AssemblyName ParentAssemblyName
        {
            get { return parentAssemblyItem.IfNotNull(parent => parent.AssemblyName); }
        }

        /// <summary>
        /// Gets or sets XamlCode.
        /// </summary>
        public virtual string XamlCode
        {
            get { return xamlCode; }
            set
            {
                xamlCode = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => XamlCode);
            }
        }

        /// <summary>
        /// Gets or sets a value of avtivity type.
        /// </summary>
        public string ActivityType
        {
            get { return activityType; }
            set
            {
                activityType = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => ActivityType);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsUserInteraction.
        /// </summary>
        public bool IsUserInteraction
        {
            get { return isUserInteraction; }
            set
            {
                isUserInteraction = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => IsUserInteraction);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsSwitch.
        /// </summary>
        public bool IsSwitch
        {
            get { return isSwitch; }
            set
            {
                isSwitch = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => IsSwitch);
            }
        }


        /// <summary>
        /// Gets or sets a value of developer's note.
        /// </summary>
        public string DeveloperNote
        {
            get { return developerNote; }
            set
            {
                developerNote = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => DeveloperNote);
            }
        }

        /// <summary>
        /// Get all activity types from a assembly and wrap them into a list of ActivityItem
        /// </summary>
        /// <param name="activityAssemblyItem">
        /// Host assembly
        /// </param>
        /// <returns>
        /// A list of ActivtyItem
        /// </returns>
        public static void GetActivityItemsFromAssembly(ActivityAssemblyItem activityAssemblyItem)
        {
            if (activityAssemblyItem == null)
            {
                throw new ArgumentNullException("activityAssemblyItem");
            }

            if (activityAssemblyItem.Assembly == null)
            {
                throw new ArgumentNullException("activityAssemblyItem");
            }

            var assembly = activityAssemblyItem.Assembly;
            List<Type> activityTypes = XamlHelper.GetAllActivityTypesInAssembly(assembly, activityAssemblyItem); // get activities and/or set NotSafeForTypeLoad
            var activityItems = new List<ActivityItem>();

            if (activityTypes != null)
            {
                foreach (var type in activityTypes)
                {
                    // Filter abstract type
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    var item = new ActivityItem
                    {
                        Name = type.Name,
                        DisplayName = type.Name,
                        FullName = type.FullName,
                        Version = GetActivityVersion(type),
                        Status = DefaultStatus,
                        Tags = DefaultTags,
                        ParentAssemblyItem = activityAssemblyItem
                    };
                    activityItems.Add(item);
                    item.HasCodeBehind = type.IsSubclassOf(typeof(CodeActivity));
                }
            }

            activityAssemblyItem.ActivityItems = new ObservableCollection<ActivityItem>(activityItems);

            if (activityAssemblyItem.ActivityItems.Count == 0)
            {
                activityAssemblyItem.IsReviewed = true;
            }
            else
            {
                activityAssemblyItem.UserSelected = true;
            }
        }

        /// <summary>
        /// Get the version from Activity type's metadata
        /// </summary>
        /// <param name="type">
        /// The Activity type.
        /// </param>
        /// <returns>
        /// The version of Activity type.
        /// </returns>
        private static string GetActivityVersion(Type type)
        {
            return type.Assembly.GetName().Version.ToString();
        }



        /// <summary>
        /// Gets or sets CachingStatus.
        /// </summary>
        public CachingStatus CachingStatus
        {
            get { return cachingStatus; }
            set
            {
                cachingStatus = value;
                IsDirty = true;
                IsEditing = true;
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
            get { return category; }
            set
            {
                category = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => Category);
            }
        }

        /// <summary>
        /// Gets or sets CreateDateTime.
        /// </summary>
        public DateTime CreateDateTime
        {
            get { return createDateTime; }
            set
            {
                createDateTime = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => CreateDateTime);
            }
        }

        /// <summary>
        /// Gets or sets CreatedBy.
        /// </summary>
        public string CreatedBy
        {
            get { return createdBy; }
            set
            {
                createdBy = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => CreatedBy);
            }
        }

        /// <summary>
        /// Gets or sets Description of object.
        /// </summary>
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                IsDirty = true;
                IsEditing = true;
                RaisePropertyChanged(() => Description);
            }
        }

        /// <summary>
        /// Gets or sets DisplayName.
        /// </summary>
        [Required]
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                IsDirty = true;
                IsEditing = true;
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
            get { return fullName; }
            set
            {
                fullName = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => FullName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether object is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => IsReadOnly);
            }
        }

        /// <summary>
        /// Gets or sets Name.
        /// For ActivityItem and WorkflowItem, name is its Activity/Workflow class name.
        /// For AssemblyItem, name is its Assembly's name (Assembly.GetName().Name)
        /// </summary>
        [Required]
        public string Name
        {
            get { return name; }
            set
            {
                oldName = name;
                name = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => Name);
                OnSetName();
            }
        }

        protected virtual void OnSetName()
        {
        }

        /// <summary>
        /// Gets or sets Status of object.
        /// </summary>
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => Status);
            }
        }

        /// <summary>
        /// Gets or sets Tags for searching.
        /// </summary>
        public virtual string Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                IsDirty = true;
                IsEditing = true;
                RaisePropertyChanged(() => Tags);
            }
        }

        /// <summary>
        /// Gets or sets UpdateDateTime.
        /// </summary>
        public DateTime UpdateDateTime
        {
            get { return updateDateTime; }
            set
            {
                updateDateTime = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => UpdateDateTime);
            }
        }

        /// <summary>
        /// Gets or sets UpdatedBy.
        /// </summary>
        public string UpdatedBy
        {
            get { return updatedBy; }
            set
            {
                updatedBy = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => UpdatedBy);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user selected it. If true, it will be shown in toolbox.
        /// </summary>
        public bool UserSelected
        {
            get { return userSelected; }
            set
            {
                userSelected = value;
                IsDirty = true;
                IsEditing = true;
                Validate();
                RaisePropertyChanged(() => UserSelected);
            }
        }

        public bool IsReviewed
        {
            get { return isReviewed; }
            set
            {
                isReviewed = value;
                Validate();
                RaisePropertyChanged(() => IsReviewed);
            }
        }
        /// <summary>
        /// Gets or sets version of object.
        /// </summary>
        public virtual string Version
        {
            get { return version; }
            set
            {
                Version newVersion = null;

                if (string.IsNullOrEmpty(value) || value == "None" || System.Version.TryParse(value, out newVersion))
                {
                    version = value;
                    IsDirty = true;
                    IsEditing = true;
                    Validate();
                    RaisePropertyChanged(() => Version);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(string.Format("\"{0}\" is not a valid version number", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets version of object.
        /// </summary>
        public string OldVersion
        {
            get { return oldVersion ?? version; }
            set
            {
                Version newVersion = null;

                if (string.IsNullOrEmpty(value) || value == "None" || System.Version.TryParse(value, out newVersion))
                {
                    oldVersion = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(string.Format("\"{0}\" is not a valid version number", value));
                }
            }
        }
        /// <summary>
        /// IComparer T implementation
        /// </summary>
        /// <param name="otherObject">The BusinessObject to compare</param>
        /// <returns>Returns 0 if the Name and Verson are the same, returns -1 if the Version is less than this and 1 if the Version is greater than this</returns>
        public int CompareTo(ActivityItem otherObject)
        {
            // Return less than if the inbound assembly is null, this is unexpected but possible
            if (null == otherObject)
                return -1;

            // Create default versions 
            var thisVersion = new Version();
            var otherVersion = new Version();

            try
            {
                thisVersion = new System.Version(Version);
                otherVersion = new System.Version(otherObject.Version);
            }
            catch (ArgumentException)
            {
                // Nothing to do for first chance exception we already have a version to start with for both
            }
            catch (FormatException)
            {
                // Nothing to do for second chance exception we already have a version to start with for both
            }
            catch (OverflowException)
            {
                // Nothing to do for tertiary exception we already have a version to start with for both
            }

            // Check for a Name match, not using CompareTo 
            bool isNameMatch = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(otherObject.Name) && Name.Equals(otherObject.Name, StringComparison.InvariantCultureIgnoreCase);

            // If the Name matches then return the CompareTo for the Versions
            if (isNameMatch)
            {
                return thisVersion.CompareTo(otherVersion);
            }
            else
            {
                // Return the Compare on the name only
                return string.Compare(Name, otherObject.Name, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public bool Matchs(AssemblyName otherObject)
        {
            // Return less than if the inbound assembly is null, this is unexpected but possible
            if (null == otherObject)
                return false;

            // Check for a Name match, not using CompareTo 
            bool isNameMatch = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(otherObject.Name) && Name.Equals(otherObject.Name, StringComparison.InvariantCultureIgnoreCase);

            // If the Name matches then return the CompareTo for the Versions
            if (isNameMatch)
            {
                var thisVersion = new Version();
                try
                {
                    thisVersion = new System.Version(Version);
                }
                catch (ArgumentException)
                {
                    // Nothing to do for first chance exception we already have a version to start with for both
                }
                catch (FormatException)
                {
                    // Nothing to do for second chance exception we already have a version to start with for both
                }
                catch (OverflowException)
                {
                    // Nothing to do for tertiary exception we already have a version to start with for both
                }
                return thisVersion.Equals(otherObject.Version);
            }
            else
            {
                return false;
            }

        }

        public bool IsLoadedXamlFromServer { get; set; }

        public List<ActivityAssemblyItem> References { get; set; }
    }
}
