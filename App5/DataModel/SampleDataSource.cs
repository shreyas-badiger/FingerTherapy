using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace App5.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : App5.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public SampleDataSource()
        {
            
            /*GROUP 1*/
            var group1 = new SampleDataGroup("Group-1",
                    "GROUP 1",
                    " ",
                    "Assets/MAINPAGE/1.png",
                    " ");
            String ITEM_CONTENT = String.Format(" POSITION : Lotus position \n \n DURATION : 48 mins per day \n \n PERIOD : As per convenience \n \n EFFECT : Imagination power, Concentration, Mental peace\n\n");
            group1.Items.Add(new SampleDataItem("Group-1-Item-1",
                    "JNANA MUDRA",
                    " ",
                    "Assets/GROUP1/JNANA.jpg",
                    "Concentration power",
                    ITEM_CONTENT,
                    group1));
            ITEM_CONTENT = String.Format(" POSITION : Lotus position \n \n DURATION : 20 mins per day \n \n PERIOD : 21 Days \n \n EFFECT : Immunity power, Ease of Blood Circulation, Mental peace \n\n");
            group1.Items.Add(new SampleDataItem("Group-1-Item-2",
                    "PRANA MUDRA",
                    " ",
                    "Assets/GROUP1/PRANA.jpg",
                    "Immunity power",
                    ITEM_CONTENT,
                    group1));
            ITEM_CONTENT = String.Format(" POSITION : Diamond position \n \n DURATION : 30 mins per day \n \n PERIOD : 21 Days \n \n EFFECT : Muscle pain, Back pain\n\n");
            group1.Items.Add(new SampleDataItem("Group-1-Item-3",
                    "VAYU MUDRA",
                    " ",
                    "Assets/GROUP1/VAYU.jpg",
                    "Muscle pain",
                    ITEM_CONTENT,
                    group1));
            ITEM_CONTENT = String.Format(" POSITION : Diamond position \n \n DURATION : 45 mins per day(15 mins set) \n \n PERIOD : 21 Days \n \n EFFECT : Headache, Acidity, Migrane\n\n");
            group1.Items.Add(new SampleDataItem("Group-1-Item-4",
                    "APAANA MUDRA",
                    " ",
                    "Assets/GROUP1/APAANA.jpg",
                    "Headache",
                    ITEM_CONTENT,
                    group1));
            ITEM_CONTENT = String.Format(" POSITION : Any \n \n DURATION : 20 mins per day \n \n PERIOD : 21 Days \n \n EFFECT : Throat related disorders\n\n");
            group1.Items.Add(new SampleDataItem("Group-1-Item-5",
                    "UDANA MUDRA",
                    " ",
                    "Assets/GROUP1/UDANA.jpg",
                    "Throat related disorders",
                    ITEM_CONTENT,
                    group1));
            this.AllGroups.Add(group1);

            /*GROUP 2*/ 
            var group2 = new SampleDataGroup("Group-2",
                    "GROUP 2",
                    " ",
                    "Assets/MAINPAGE/2.png",
                    " ");
            ITEM_CONTENT = String.Format(" PROCEDURE : Join your hands as shown above. Alternate closing fingers of left and right hand. \n \n POSITION : Semi Lotus position \n \n DURATION : 2 mins per day(25 to 30 times) \n \n PERIOD : Always \n \n EFFECT : Sleep disorders, Reduced Mental stress\n\n");
            group2.Items.Add(new SampleDataItem("Group-2-Item-1",
                    "PARIVARTHANA MUDRA",
                    " ",
                    "Assets/GROUP2/PARIVARTHANA.jpg",
                    "Sleep disorders",
                    ITEM_CONTENT,
                    group2));
            ITEM_CONTENT = String.Format(" POSITION : Lotus position \n \n DURATION : 30 mins per day \n \n PERIOD : 15 Days(Do stop once the disease is cured) \n \n EFFECT : Common cold, increases appetite, Stomach related disorders\n\n");
            group2.Items.Add(new SampleDataItem("Group-2-Item-2",
                    "SOORYA MUDRA",
                    " ",
                    "Assets/GROUP2/SOORYA.jpg",
                    "Common cold",
                    ITEM_CONTENT,
                    group2));
            ITEM_CONTENT = String.Format(" POSITION : Diamond position \n \n DURATION : 30 to 48 mins per day \n \n PERIOD : 30 Days(Do stop once the disease is cured) \n \n EFFECT : Improper digestion, Activness,Imunity power\n\n");
            group2.Items.Add(new SampleDataItem("Group-2-Item-3",
                    "PRUTHVI MUDRA",
                    " ",
                    "Assets/GROUP2/PRITHVI.jpg",
                    "Improper digestion",
                    ITEM_CONTENT,
                    group2));
            ITEM_CONTENT = String.Format(" POSITION : Any \n \n DURATION : 10 to 48 mins per day \n \n PERIOD : 21 Days(Do stop once the disease is cured) \n \n EFFECT : Ear related disorders, Neck pain\n\n");
            group2.Items.Add(new SampleDataItem("Group-2-Item-4",
                    "SHOONYA MUDRA",
                    " ",
                    "Assets/GROUP2/SHOONYA.jpg",
                    "Hearing Problem",
                    ITEM_CONTENT,
                    group2));
            ITEM_CONTENT = String.Format(" POSITION : Diamond position \n \n DURATION : 5 to 15 mins per day \n \n PERIOD : 21 Days \n \n EFFECT : Controlled Blood Pressure, Chest Pain, Breathing problems\n\n");
            group2.Items.Add(new SampleDataItem("Group-2-Item-5",
                    "SANJEEVINI MUDRA",
                    " ",
                    "Assets/GROUP2/SANJEEVINI.jpg",
                    "Blood Pressure",
                    ITEM_CONTENT,
                    group2));
            this.AllGroups.Add(group2);

            /*GROUP 3*/
            var group3 = new SampleDataGroup("Group-3",
                    "GROUP 3",
                    " ",
                    "Assets/MAINPAGE/3.png",
                    " ");
            ITEM_CONTENT = String.Format(" POSITION : Diamond position \n \n DURATION : 48 mins per day(3 sets of 16 mins each) \n \n PERIOD : 21 Days (Do stop once the disease is cured) \n \n EFFECT : Tooth related disorder, Synus problem\n\n");
            group3.Items.Add(new SampleDataItem("Group-3-Item-1",
                    "AAKASHA MUDRA",
                    " ",
                    "Assets/GROUP3/AKASHA.jpg",
                    "Tooth related disorder",
                    ITEM_CONTENT,
                    group3));
            ITEM_CONTENT = String.Format(" POSITION : Semi lotus position \n \n DURATION :15 mins(winter)/48 mins(other time) per day \n \n PERIOD : 21 Days \n \n EFFECT : Excessive sweating, whit patches on skin, wrinkles\n\n");
            group3.Items.Add(new SampleDataItem("Group-3-Item-2",
                    "JALA MUDRA",
                    " ",
                    "Assets/GROUP3/JALA.jpg",
                    "Excessive sweating",
                    ITEM_CONTENT,
                    group3));
            ITEM_CONTENT = String.Format(" PROCEDURE : Close your fingers as stated in image. Place it on nose, such that Middle finger meets the nose tip \n \n POSITION : Any \n \n DURATION : Whenever you are anger \n \n PERIOD :Till you learn to control temper (Do stop once the disease is cured) \n \n EFFECT : Greater ability to control Anger\n\n");
            group3.Items.Add(new SampleDataItem("Group-3-Item-3",
                    "SHANTA MUDRA",
                    " ",
                    "Assets/GROUP3/SHANTA.jpg",
                    "Short Temperdness",
                    ITEM_CONTENT,
                    group3));
            ITEM_CONTENT = String.Format(" POSITION : Diamond position or Semi lotus position \n \n DURATION : 5 to 40 mins per day \n \n PERIOD : 21 Days (Do stop once the disease is cured) \n \n EFFECT : Voice related disorders, Dust allergy\n\n");
            group3.Items.Add(new SampleDataItem("Group-3-Item-4",
                    "SHANKHA MUDRA",
                    " ",
                    "Assets/GROUP3/SHANKA.jpg",
                    "Voice tone",
                    ITEM_CONTENT,
                    group3));
            ITEM_CONTENT = String.Format(" POSITION : Any \n \n DURATION : 10 to 20 mins per day \n \n PERIOD : 21 Days (Do stop once the disease is cured) \n \n EFFECT : Kidney stone, Urinal problems, other Kidney related disorders\n\n");
            group3.Items.Add(new SampleDataItem("Group-3-Item-5",
                    "BANDOOKU MUDRA",
                    " ",
                    "Assets/GROUP3/BANDOOKU.jpg",
                    "Kidney related disorders",
                    ITEM_CONTENT,
                    group3));
            this.AllGroups.Add(group3);


            /*GROUP 4*/
            var group4 = new SampleDataGroup("Group-4",
                    "GROUP 4",
                    " ",
                    "Assets/MAINPAGE/4.png",
                    " ");
            ITEM_CONTENT = String.Format(" ");
            group4.Items.Add(new SampleDataItem("Group-4-Item-1",
                    "LOTUS POSITION",
                    "Padma asana",
                    "Assets/GROUP4/PADMASANA.jpg",
                    " ",
                    ITEM_CONTENT,
                    group4));
            group4.Items.Add(new SampleDataItem("Group-4-Item-2",
                    "SEMI LOTUS POSITION",
                    "Ardha padma asana",
                    "Assets/GROUP4/ARDHAPADMASANA.jpg",
                    " ",
                    ITEM_CONTENT,
                    group4));
            group4.Items.Add(new SampleDataItem("Group-4-Item-3",
                    "DIAMOND POSITION",
                    "Vajra asana",
                     "Assets/GROUP4/VAJRASANA.jpg",
                    " ",
                    ITEM_CONTENT,
                    group4));
            this.AllGroups.Add(group4);

           
        }
    }
}
