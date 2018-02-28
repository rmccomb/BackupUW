using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using System.Collections.Specialized;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using System.Collections;

namespace BackupUW
{
    public class ThingWithName
    {
        public ThingWithName(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }

    public interface IEmployee
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        Manager DirectManager { get; set; }
        bool IsManager { get; }
        string Title { get; }
        string Name { get; }
        bool Visibility { get; set; }
        string NullStringProperty { get; set; }
        Windows.UI.Xaml.Media.ImageSource NullImageSource { get; set; }
        void Poke(object sender, RoutedEventArgs e);
        string FullName();
    }

    

    
    public sealed class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        public CollectionChange CollectionChange { get; set; }
        public uint Index { get; set; }
    }


    //Implements both IObservableVector and INotifyCollectionChanged and as its used from both C++ and C# apps
    public sealed class EmployeeCollection : IObservableVector<IEmployee>, IList<IEmployee>, INotifyCollectionChanged, IList
    {
        private ObservableCollection<IEmployee> data;
        public event VectorChangedEventHandler<IEmployee> VectorChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public EmployeeCollection()
        {
            data = new ObservableCollection<IEmployee>();
            data.CollectionChanged += Data_CollectionChanged;
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (VectorChanged != null)
            {
                VectorChangedEventArgs args = new VectorChangedEventArgs();
                args.CollectionChange = CollectionChange.Reset;
                VectorChanged(this, args);
            }

            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                CollectionChanged(this, e);
            }
        }

        #region IObservableVector
        //IObservableVector derives from IList<T> so most of the interface members are strongly typed

        public IEmployee this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        public int Count { get { return data.Count; } }
        public bool IsReadOnly { get { return false; } }

        public void Add(IEmployee item)
        {
            data.Add(item);
        }

        public void Clear()
        {
            data.Clear();
        }
        public bool Contains(IEmployee item)
        {
            return data.Contains(item);
        }

        public void CopyTo(IEmployee[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }
        public IEnumerator<IEmployee> GetEnumerator() { return data.GetEnumerator(); }

        public int IndexOf(IEmployee item) { return data.IndexOf(item); }

        public void Insert(int index, IEmployee item)
        {
            data.Insert(index, item);
        }

        public bool Remove(IEmployee item)
        {
            return data.Remove(item);
        }

        public void RemoveAt(int index)
        {
            data.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
        #endregion

        #region INotifyCollectionChanged
        //INotifyCollection changed inherits from IList (non generic) so most of its members are not strongly typed

        object IList.this[int index]
        {
            get { return data[index]; }
            set { data[index] = (Employee)value; }
        }

        int ICollection.Count { get { return data.Count; } }
        bool IList.IsFixedSize { get { return false; } }
        bool ICollection.IsSynchronized { get { return false; } }
        object ICollection.SyncRoot { get { throw new NotImplementedException(); } }

        int IList.Add(object item)
        {
            data.Add((IEmployee)item);
            return data.Count;
        }

        bool IList.IsReadOnly { get { return false; } }

        bool IList.Contains(object item) { return data.Contains(item); }

        int IList.IndexOf(object item) { return data.IndexOf((IEmployee)item); }

        void IList.Insert(int index, object item)
        {
            data.Insert(index, (IEmployee)item);
        }

        void IList.Remove(object item)
        {
            data.Remove((IEmployee)item);
        }

        void IList.Clear()
        {
            data.Clear();
        }

        void IList.RemoveAt(int index)
        {
            data.RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

       
        #endregion
    }

    internal class MapChangedEventArgs<K> : IMapChangedEventArgs<K>
    {
        public CollectionChange CollectionChange { get; set; }
        public K Key { get; set; }
    }

    public sealed class EmployeeDictionary : IDictionary<string, IEmployee>, IObservableMap<string, IEmployee>
    {
        private Dictionary<string, IEmployee> data = new Dictionary<string, IEmployee>();
        #region IObservable MAP
        public event MapChangedEventHandler<string, IEmployee> MapChanged;

        private void FireMapChanged(CollectionChange change, string key)
        {
            if (MapChanged != null)
            {
                MapChanged(this, new MapChangedEventArgs<string>() { CollectionChange = change, Key = key });
            }
        }

        #endregion

        #region Generic Dictionary
        public IEmployee this[string key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
                FireMapChanged(CollectionChange.ItemChanged, key);
            }
        }

        public int Count { get { return data.Count; } }

        public bool IsReadOnly { get { return false; } }

        public ICollection<string> Keys { get { return data.Keys; } }

        public ICollection<IEmployee> Values { get { return data.Values; } }

        public void Add(KeyValuePair<string, IEmployee> item)
        {
            data.Add(item.Key, item.Value);
            FireMapChanged(CollectionChange.ItemInserted, item.Key);
        }

        public void Add(string key, IEmployee value)
        {
            data.Add(key, value);
            FireMapChanged(CollectionChange.ItemInserted, key);
        }

        public void Clear()
        {
            data.Clear();
            FireMapChanged(CollectionChange.Reset, default(string));
        }

        public bool Contains(KeyValuePair<string, IEmployee> item) { return data.Contains(item); }

        public bool ContainsKey(string key) { return data.ContainsKey(key); }

        public void CopyTo(KeyValuePair<string, IEmployee>[] array, int arrayIndex)
        {
            var keys = data.Keys.GetEnumerator();
            var values = data.Values.GetEnumerator();
            for (int i = 0; i < data.Count; i++)
            {
                KeyValuePair<string, IEmployee> pair = new KeyValuePair<string, IEmployee>(keys.Current, values.Current);
                array[arrayIndex + i] = pair;
                keys.MoveNext();
                values.MoveNext();
            }
        }

        public IEnumerator<KeyValuePair<string, IEmployee>> GetEnumerator() { return data.GetEnumerator(); }

        public bool Remove(KeyValuePair<string, IEmployee> item) { return Remove(item.Key); }

        public bool Remove(string key)
        {
            bool removed = data.Remove(key);
            if (removed) { FireMapChanged(CollectionChange.ItemRemoved, key); }
            return removed;
        }

        public bool TryGetValue(string key, out IEmployee value)
        {
            return data.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)data).GetEnumerator(); }
        #endregion

    }

    public sealed class Manager : IEmployee, INotifyPropertyChanged
    {
        private Employee _employee;
        private EmployeeCollection _reports;
        private EmployeeDictionary _reportsDictionary;

        public Manager()
        {
            _employee = new Employee();
            _reports = new EmployeeCollection();
            _reportsDictionary = new EmployeeDictionary();
        }

        public IEnumerable<IEmployee> ReportsEnum { get { return _reports; } }
        public IList<IEmployee> ReportsList { get { return _reports; } }
        public EmployeeCollection ReportsOC { get { return _reports; } }
        public EmployeeDictionary ReportsDict { get { return _reportsDictionary; } }

        public string NullStringProperty { get; set; }
        public Windows.UI.Xaml.Media.ImageSource NullImageSource
        {
            get { return null; }
            set { }
        }

        public bool Visibility
        {
            get { return _employee.Visibility; }
            set
            {
                if (_employee.Visibility != value)
                {
                    _employee.Visibility = value;
                    NotifyPropertyChanged("Visibility");
                }
            }
        }
        public string FirstName
        {
            get { return _employee.FirstName; }
            set
            {
                if (value != _employee.FirstName)
                {
                    _employee.FirstName = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string LastName
        {
            get { return _employee.LastName; }
            set
            {
                if (value != _employee.LastName)
                {
                    _employee.LastName = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public Manager DirectManager
        {
            get { return _employee.DirectManager; }
            set
            {
                if (value != _employee.DirectManager)
                {
                    _employee.DirectManager = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void Poke(object sender, RoutedEventArgs e)
        {
            MessageDialog dlg = new MessageDialog("Employees of " + Name + " have been poked.");
            var t = dlg.ShowAsync();
        }

        public bool IsManager { get { return true; } }

        public String Title
        {
            get
            {
                return "Manager";
            }
        }

        public string Name { get { return FirstName + " " + LastName; } }

        public string FullName() { return FirstName + " " + LastName; }

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    internal class Employee : IEmployee
    {
        private string _FirstName;
        private string _LastName;
        private Manager _Manager;

        internal Employee()
        {
            Visibility = true;
        }

        public void Poke(object sender, RoutedEventArgs e)
        {
            MessageDialog dlg = new MessageDialog(Name + " has been poked.");
            var t = dlg.ShowAsync();
        }

        public bool Visibility { get; set; }

        public string NullStringProperty
        {
            get { return null; }
            set { }
        }
        public Windows.UI.Xaml.Media.ImageSource NullImageSource
        {
            get { return null; }
            set { }
        }

        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                if (value != _FirstName)
                {
                    _FirstName = value;
                }
            }
        }

        public string LastName
        {
            get { return _LastName; }
            set
            {
                if (value != _LastName)
                {
                    _LastName = value;
                }
            }
        }

        public Manager DirectManager
        {
            get { return _Manager; }
            set
            {
                if (value != _Manager)
                {
                    _Manager = value;
                }
            }
        }

        public bool IsManager { get { return false; } }

        public String Title { get { return "Developer"; } }

        public string Name { get { return FirstName + " " + LastName; } }

        public string FullName() { return FirstName + " " + LastName; }
    }

    public sealed class DataModel : DependencyObject, INotifyPropertyChanged
    {
        public DataModel()
        {
            InitializeValues();
        }

        public string StringPropNoINPC { get; set; }
        public string StringPropAsInteger { get; set; }
        public int IntPropNoINPC { get; set; }
        public IEmployee EmployeePropNoINPC { get; set; }

        private string _StringPropWithINPC;
        public string StringPropWithINPC
        {
            get { return _StringPropWithINPC; }
            set
            {
                if (value != _StringPropWithINPC)
                {
                    _StringPropWithINPC = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _IntPropWithINPC;
        public int IntPropWithINPC
        {
            get { return _IntPropWithINPC; }
            set
            {
                if (value != _IntPropWithINPC)
                {
                    _IntPropWithINPC = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Color _ColorPropWithINPC;
        public Color ColorPropWithINPC
        {
            get { return _ColorPropWithINPC; }
            set
            {
                if (value != _ColorPropWithINPC)
                {
                    _ColorPropWithINPC = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private IEmployee _EmployeePropWithINPC;
        public IEmployee EmployeePropWithINPC
        {
            get { return _EmployeePropWithINPC; }
            set
            {
                if (value != _EmployeePropWithINPC)
                {
                    _EmployeePropWithINPC = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private EmployeeCollection _Employees;
        public EmployeeCollection Employees
        {
            get { return _Employees; }
            set
            {
                if (value != _Employees)
                {
                    _Employees = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Manager _ManagerProp;
        public Manager ManagerProp
        {
            get { return _ManagerProp; }
            set
            {
                if (value != _ManagerProp)
                {
                    _ManagerProp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _BoolPropWithINPC;
        public bool BoolPropWithINPC
        {
            get { return _BoolPropWithINPC; }
            set
            {
                if (value != _BoolPropWithINPC)
                {
                    _BoolPropWithINPC = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int IntPropertyDP
        {
            get { return (int)GetValue(IntPropertyDPProperty); }
            set { SetValue(IntPropertyDPProperty, value); }
        }
        #region IntPropertyDP DP
        private const string IntPropertyDPName = "IntPropertyDP";
        private static readonly DependencyProperty _IntPropertyDPProperty =
            DependencyProperty.Register(IntPropertyDPName, typeof(int), typeof(DataModel), new PropertyMetadata(0));
        public static DependencyProperty IntPropertyDPProperty { get { return _IntPropertyDPProperty; } }
        #endregion

        public string StringPropertyDP
        {
            get { return (string)GetValue(StringPropertyDPProperty); }
            set { SetValue(StringPropertyDPProperty, value); }
        }
        #region StringPropertyDP DP
        private const string StringPropertyDPName = "StringPropertyDP";
        private static readonly DependencyProperty _StringPropertyDPProperty =
            DependencyProperty.Register(StringPropertyDPName, typeof(string), typeof(DataModel), new PropertyMetadata(""));
        public static DependencyProperty StringPropertyDPProperty { get { return _StringPropertyDPProperty; } }
        #endregion

        public IEmployee EmployeePropertyDP
        {
            get { return (IEmployee)GetValue(EmployeePropertyDPProperty); }
            set { SetValue(EmployeePropertyDPProperty, value); }
        }
        #region EmployeePropertyDP DP
        private const string EmployeePropertyDPName = "EmployeePropertyDP";
        private static readonly DependencyProperty _EmployeePropertyDPProperty =
            DependencyProperty.Register(EmployeePropertyDPName, typeof(IEmployee), typeof(DataModel), new PropertyMetadata(0));
        public static DependencyProperty EmployeePropertyDPProperty { get { return _EmployeePropertyDPProperty; } }

        #endregion


        public void InitializeValues()
        {
            ManagerProp = CreateManagerEmployeeTree();
            Employees = ManagerProp.ReportsOC;

            StringPropNoINPC = "String Property without INPC";
            StringPropWithINPC = "String Property with INPC";
            ColorPropWithINPC = Colors.Red;
            IntPropNoINPC = 42;
            IntPropWithINPC = 2;
            IntPropertyDP = 42;
            StringPropertyDP = "String Property as a DP";
            StringPropAsInteger = "65";
            EmployeePropNoINPC = Employees[0];
            EmployeePropWithINPC = Employees[1];

        }
        public void UpdateValues()
        {
            this.BoolPropWithINPC = !this.BoolPropWithINPC;
            this.IntPropNoINPC = IntPropNoINPC + 10 % 100;
            this.IntPropWithINPC = IntPropWithINPC + 1 % 10;
            this.IntPropertyDP = IntPropertyDP + 10 % 100;

            string name = ", " + lnames[(new Random()).Next(lnames.Length)];
            this.StringPropNoINPC += name;
            this.StringPropWithINPC += name;
            this.StringPropertyDP += name;

            EmployeePropWithINPC = Employees[IntPropWithINPC % Employees.Count];
            EmployeePropertyDP = Employees[(IntPropWithINPC + 1) % Employees.Count];
            // Swap Manager Reports [0], [2]
            var temp = ManagerProp.ReportsOC[0];
            ManagerProp.ReportsOC[0] = ManagerProp.ReportsOC[2];
            ManagerProp.ReportsOC[2] = temp;
        }


        #region Employee Tree Creation
        private static string[] fnames = { "Alice", "Brian", "Chris", "David", "Emily", "Freda", "Garry", "Harriet", "Isobel", "John", "Keith", "Lisa", "Mark", "Nathan", "Olivia", "Penelope", "Quentin", "Robert", "Simon", "Tom", "Veronica", "Will", "Xavier", "Yvette", "Zach" };
        private static string[] lnames = { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker" };

        public static Manager CreateManagerEmployeeTree()
        {
            Random rnd = new Random(4524); // Passing in a seed guarantees the same values each time
            Manager CEO = new Manager() { FirstName = fnames[rnd.Next(fnames.Length)], LastName = lnames[rnd.Next(lnames.Length)] };
            CreateReports(5, CEO, rnd);
            return CEO;
        }

        private static void CreateReports(int depth, Manager m, Random rnd)
        {
            int count = rnd.Next(10) + 3;
            for (int i = 0; i < count; i++)
            {
                int j = rnd.Next(10);
                IEmployee e = (j > 2 && depth > 1) ? (IEmployee)new Manager() : (IEmployee)new Employee();
                e.LastName = lnames[rnd.Next(lnames.Length)];
                e.FirstName = fnames[rnd.Next(fnames.Length)];
                e.DirectManager = m;
                m.ReportsList.Add(e);
                m.ReportsDict.Add(e.FirstName + i, e);
                if (e.IsManager) CreateReports(depth - 1, (Manager)e, rnd);
            }
        }

        #endregion
        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
