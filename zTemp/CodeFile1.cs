[Index(Columns = "Name", Name = "Company_Index_Name")]  
[Table]    
public class Company
     {
        private int _id;
       [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
           get
           {
              return _id;
          }
           set
            {
               if (_id != value)
                {
                NotifyPropertyChanging(()=>Id);
                  _id = value;
                    NotifyPropertyChanged(()=>Id);
               }
            }
        }
         private string _name;
       [Column(DbType = "NVARCHAR(300)")]
       public string Name
        {
            get
           {
               return _name;
           }
            set
            {
             if (_name != value)
               {
                    NotifyPropertyChanging(()=>Name);
                    _name = value;
                    NotifyPropertyChanged(()=>Name);
                }
            }
        }
        private EntitySet<CompanyAddress> _addresses = new EntitySet<CompanyAddress>();
        [Association(Storage = "_addresses", OtherKey = "_companyId", ThisKey = "Id")]
        public EntitySet<CompanyAddress> Addresses
        {
           get { return _addresses; }
            set
            {
               _addresses.Assign(value);
           }
        }
}
[Table]
public class CompanyAddress
{
private int _id;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
       public int Id 
{
       get
            {
                return _id;
           }
            set
            {
                if (_id != value)
                {
                  NotifyPropertyChanging(()=>Id);
                    _id = value;
                    NotifyPropertyChanged(()=>Id);

                }
            }
       }
  [Column]
       private int _companyId;
        public int CompanyId
        {
         get { return _companyId; }