using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddIn
{
    [Serializable]
    [DebuggerDisplay("Name = '{Name}'")]
    public sealed class ActivityAssemblyItem : NotificationObject
    {
        private const string DefaultTags = "";
        private const string DefaultDescription = "";

        ObservableCollection<ActivityItem> activityItems;
        string authorityGroup;
        CachingStatus cachingStatus;
        string category;
        string createdBy;
        DateTime creationDateTime;
        string description;
        string displayName;
        string friendlyName;
        bool isReadOnly;
        bool isReviewed;
        string location;
        bool? notSafeForTypeLoad;
        ObservableCollection<AssemblyName> referencedAssemblies;
        string releaseNotes;
        string status;
        string tags;
        DateTime updateDateTime;
        string updatedBy;
        bool userSelected;
        private bool userWantsToUpload = false;
        /// <summary>
        /// The assembly. Maybe null. Sometime we just use the AssemblyName.
        /// </summary>
        [NonSerialized]
        private Assembly assembly;

        /// <summary>
        /// The AssemblyName of the Assembly. Name/FullName/Version all derive from AssemblyName.
        /// </summary>
        private AssemblyName assemblyName = new AssemblyName();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAssemblyItem"/> class.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        public ActivityAssemblyItem(Assembly assembly)
            : this(assembly.GetName())
        {
            Assembly = assembly;
            Location = Assembly.Location;
            ActivityItem.GetActivityItemsFromAssembly(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAssemblyItem"/> class.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        public ActivityAssemblyItem(AssemblyName assemblyName)
            : this()
        {
            AssemblyName = assemblyName;
            FriendlyName = assemblyName.Name;
            DisplayName = assemblyName.Name;
            Version = assemblyName.Version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAssemblyItem"/> class.
        /// </summary>
        /// 
        public ActivityAssemblyItem()
        {
            ActivityItems = new ObservableCollection<ActivityItem>();
            ReferencedAssemblies = new ObservableCollection<AssemblyName>();
            Location = string.Empty;
            Tags = DefaultTags;
            Description = DefaultDescription;
        }


        /// <summary>
        /// Gets or sets ActivityItems.
        /// ActivityItems represents the activity types in this assembly. Activity type was wrapped in ActivityItem class.
        /// </summary>
        public ObservableCollection<ActivityItem> ActivityItems
        {
            get { return activityItems; }
            set
            {
                activityItems = value;
                RaisePropertyChanged(() => ActivityItems);
            }
        }

        /// <summary>
        /// Gets or sets AuthorityGroup.
        /// Assembly only show to user whose group is/great than assembly's authority group.
        /// </summary>
        public string AuthorityGroup
        {
            get { return authorityGroup; }
            set
            {
                authorityGroup = value;
                RaisePropertyChanged(() => AuthorityGroup);
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether the assembly has been reviewed.
        /// </summary>
        public bool IsReviewed
        {
            get { return isReviewed; }
            set
            {
                isReviewed = value;
                RaisePropertyChanged(() => IsReviewed);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the assembly is safe for type load.
        /// </summary>
        public bool? NotSafeForTypeLoad
        {
            get { return notSafeForTypeLoad; }
            set
            {
                notSafeForTypeLoad = value;
                RaisePropertyChanged(() => NotSafeForTypeLoad);
            }
        }

        /// <summary>
        /// Gets or sets the caching status of the assembly.
        /// </summary>
        public CachingStatus CachingStatus
        {
            get { return cachingStatus; }
            set { cachingStatus = value; RaisePropertyChanged(() => CachingStatus); }
        }

        /// <summary>
        /// Gets or sets Category. Object will be grouped into toolbox category by this property.
        /// </summary>
        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                RaisePropertyChanged(() => Category);
            }
        }

        /// <summary>
        /// Gets or sets CreateDateTime.
        /// </summary>
        public DateTime CreationDateTime
        {
            get { return creationDateTime; }
            set
            {
                creationDateTime = value;
                RaisePropertyChanged(() => CreationDateTime);
            }
        }

        /// <summary>
        /// Gets or sets the name of the user that created the assembly.
        /// </summary>
        public string CreatedBy
        {
            get { return createdBy; }
            set
            {
                createdBy = value;
                RaisePropertyChanged(() => CreatedBy);
            }
        }

        /// <summary>
        /// Gets or sets the description of the assembly.
        /// </summary>
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        /// <summary>
        /// Gets or sets the display name of the assembly.
        /// </summary>
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                RaisePropertyChanged(() => DisplayName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                RaisePropertyChanged(() => IsReadOnly);
            }
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
                RaisePropertyChanged(() => Status);
            }
        }

        /// <summary>
        /// Gets or sets Tags for searching.
        /// </summary>
        public string Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                RaisePropertyChanged(() => Tags);
            }
        }

        public bool UserWantsToUpload
        {
            get { return userWantsToUpload; }
            set
            {
                userWantsToUpload = value;
                RaisePropertyChanged(() => UserWantsToUpload);
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
                RaisePropertyChanged(() => UpdatedBy);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the user selected this assembly. If true, it will be shown in the toolbox.
        /// </summary>
        public bool UserSelected
        {
            get { return userSelected; }
            set
            {
                if (userSelected != value)
                {
                    userSelected = value;
                    RaisePropertyChanged(() => UserSelected);
                }
            }
        }

        /// <summary>
        /// Gets or set the full path of the assembly
        /// </summary>
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                RaisePropertyChanged(() => Location);
            }
        }

        public string FullName
        {
            get
            {
                string result = null == assembly
                                    ? String.Empty
                                    : assembly.FullName;

                return result;
            }
        }

        /// <summary>
        /// Friendly name of the assembly.
        /// </summary>
        public string FriendlyName
        {
            get { return friendlyName; }
            set
            {
                friendlyName = value;
                RaisePropertyChanged(() => FriendlyName);
            }
        }

        /// <summary>
        /// Release notes of the assembly.
        /// </summary>
        public string ReleaseNotes
        {
            get { return releaseNotes; }
            set
            {
                releaseNotes = value;
                RaisePropertyChanged(() => ReleaseNotes);
            }
        }

        /// <summary>
        /// Gets or sets Name.
        /// For ActivityItem and WorkflowItem, name is its Activity/Workflow class name.
        /// For AssemblyItem, name is its Assembly's name (Assembly.GetName().Name)
        /// </summary>
        public string Name
        {
            get { return assemblyName.Name; }
            set
            {
                assemblyName.Name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        /// <summary>
        /// Gets or sets the version of the assembly.
        /// </summary>
        public Version Version
        {
            get { return assemblyName.Version; }
            set
            {
                assemblyName.Version = value;
                RaisePropertyChanged(() => Version);
            }
        }

        /// <summary>
        /// Gets or sets Assembly. Maybe null. Sometime we just use the AssemblyName. Set this property will influent AssemblyName property.
        /// </summary>
        public Assembly Assembly
        {
            get
            {
                return assembly;
            }

            set
            {
                assembly = value;

                if (value != null)
                {
                    // Don't necessarily want to automatically overwrite AssemblyName or 
                    // Location because Assembly may come from a temp location like the Output folder.
                    // However, we set it if it hasn't been set already, for back-compat.
                    if (AssemblyName == null)
                    {
                        AssemblyName = Assembly.GetName();
                    }

                    if (string.IsNullOrEmpty(Location))
                    {
                        Location = value.Location;
                    }
                }

                RaisePropertyChanged(() => Assembly);
            }
        }

        /// <summary>
        /// Gets or sets AssemblyName. The AssemblyName of the Assembly. Set this property will influent Name/FullName/Version
        /// </summary>
        public AssemblyName AssemblyName
        {
            get
            {
                return assemblyName;
            }

            set
            {
                if (value != null)
                {
                    assemblyName = TreatUnsignedAsUnversioned(value);
                }

                RaisePropertyChanged(() => AssemblyName);
            }
        }

        /// <summary>
        /// Gets the list of references of the current assembly object..
        /// </summary>
        public ObservableCollection<AssemblyName> ReferencedAssemblies
        {
            get { return referencedAssemblies; }
            set
            {
                referencedAssemblies = value;
                RaisePropertyChanged(() => ReferencedAssemblies);
            }
        }

        /// <summary>
        /// Sets ReferencedAssemblies, but doesn't change version since the DB never stores signatures.
        /// Occurs during ComputeReferencedAssemblies.
        /// </summary>
        /// <param name="assemblyNames"></param>
        public void SetDatabaseReferences(IEnumerable<AssemblyName> assemblyNames)
        {
            if (assemblyNames == null)
            {
                throw new ArgumentNullException("assemblyNames");
            }

            if (assemblyNames.Any(name => name == null))
            {
                throw new ArgumentNullException("assemblyNames");
            }

            ReferencedAssemblies = new ObservableCollection<AssemblyName>(assemblyNames);
        }

        /// <summary>
        /// Makes a copy of the assembly name without including the code base.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssemblyName TreatUnsignedAsUnversioned(AssemblyName name)
        {
            // copy (w/out CodeBase)
            var copy = new AssemblyName() { Name = name.Name, Version = name.Version, CultureInfo = name.CultureInfo };
            copy.SetPublicKeyToken(name.GetPublicKeyToken());
            copy.SetPublicKey(name.GetPublicKey());
            return TreatUnsignedAsUnversionedNoCopy(copy);
        }

        /// <summary>
        /// Performance optimization for Match() to use
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssemblyName TreatUnsignedAsUnversionedNoCopy(AssemblyName name)
        {
            name.Version = GetVersionIfSigned(name);
            return name;
        }

        /// <summary>
        /// Gets the version of one assembly name only if its signed.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Version GetVersionIfSigned(AssemblyName name)
        {
            if (name.GetPublicKeyToken().IfNotNull(token => token.Length) > 0)
            {
                return name.Version;
            }
            else
            {
                // unsigned is treated as not versioned
                return null;
            }
        }


        /// <summary>
        /// Does this ActivityAssemblyItem represent this assembly?
        /// </summary>
        /// <param name="assembly">Name of assembly to use in the operation</param>
        /// <returns></returns>
        public bool Matches(Assembly assembly)
        {
            return Matches(AssemblyName, TreatUnsignedAsUnversionedNoCopy(assembly.GetName()));
        }

        public bool Matches(ActivityAssemblyItem item)
        {
            return this.Name == item.Name
                && this.Version == item.Version;
        }

        /// <summary>
        /// Does this ActivityAssemblyItem represent this assembly name?
        /// </summary>
        /// <param name="name">Name of assembly to use in the operation</param>
        /// <returns></returns>
        public bool Matches(AssemblyName name)
        {
            // can't do NoCopy in this case because we don't know where assemblyName came from
            return Matches(AssemblyName, TreatUnsignedAsUnversioned(name));
        }

        /// <summary>
        /// Does this ActivityAssemblyItem represent this assembly name?
        /// </summary>
        /// <param name="name">Name of assembly to use in the operation</param>
        /// <returns></returns>
        public bool Matches(ActivityItem otherObject)
        {
            // Return less than if the inbound assembly is null, this is unexpected but possible
            if (null == otherObject)
                return false;

            // Check for a Name match, not using CompareTo 
            bool isNameMatch = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(otherObject.Name) && Name.Equals(otherObject.Name, StringComparison.InvariantCultureIgnoreCase);

            // If the Name matches then return the CompareTo for the Versions
            if (isNameMatch)
            {
                var otherVersion = new Version();
                try
                {
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
                return this.Version.Equals(otherVersion);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Helper method, compares based on Name/Version only
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        static bool Matches(AssemblyName lhs, AssemblyName rhs)
        {
            // DB doesn't even store culture or public key token so we can't consider it for structural equality
            // because e.g. the ActivityAssemblyItem doing the matching might not even have its real executable yet,
            // so it would have no way to get public key token or culture
            return lhs.Name == rhs.Name && lhs.Version == rhs.Version;
        }


    }
}
