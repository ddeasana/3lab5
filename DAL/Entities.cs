using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;



namespace DAL
{
    [JsonDerivedType(typeof(Entity), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(Student), typeDiscriminator: "student")]
    [JsonDerivedType(typeof(McWorker), typeDiscriminator: "McWorker")]
    [JsonDerivedType(typeof(Manager), typeDiscriminator: "Manager")]
    [Serializable]
    [XmlInclude(typeof(Student))]
    [XmlInclude(typeof(McWorker))]
    [XmlInclude(typeof(Manager))]
    public class Entity :  IPLayChess
    {
        [XmlElement]
        public string LastName { get; set; }
        public Entity() { }
        [JsonConstructor]
        public Entity(string LastNameInput)
        {
            LastName = LastNameInput;
        }
        public virtual string[] Methods { get { return new string[] { "PlayChess" }; } }
        public string PlayChess()
        {
            return LastName + " plays chess";
        }
        public override string ToString() => LastName;
        protected Entity(SerializationInfo info, StreamingContext context)
        {
            LastName = info.GetString("LastName");
           
        }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LastName", LastName);
            
        }
    }
    public class StudentEventArgs : EventArgs
    {
        public string EventMessage { get; }
        public StudentEventArgs(string eventMessage) { EventMessage = eventMessage; }
    }

    [Serializable]
    public class Student : Entity, IStudy
    {
        private string studentID;
        private int? gpa;
        private int? course;
        private string? country;
        private string? numberOfTheScoreBook;
        public delegate void StudentExpulsionHandler(object sender, StudentEventArgs e);
        public event StudentExpulsionHandler StudentEvent;
        public int? Course { get => course; set => course = value; }
        public string StudentID { get => studentID; set => studentID = value; }
        public int? GPA { get => gpa; set => gpa = value; }

        public string? Country { get => country; set => country = value; }
        public string? NumberOfTheScoreBook { get => numberOfTheScoreBook; set => numberOfTheScoreBook = value; }
        public Student() { }
        protected Student(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            StudentID = info.GetString("StudentID");
            Course = info.GetInt32("Course");
            GPA = info.GetInt32("GPA");
            Country = info.GetString("Country");
            NumberOfTheScoreBook = info.GetString("NumberOfTheScoreBook");
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StudentID", StudentID);
            info.AddValue("Course", Course);
            info.AddValue("GPA", GPA);
            info.AddValue("Country", Country);
            info.AddValue("NumberOfTheScoreBook", NumberOfTheScoreBook);
        }
        public Student(string LastNameInput, string StudentIDInput) : base(LastNameInput)
        {
            studentID = StudentIDInput;
        }
        [JsonConstructor]
        public Student(int? Course, string StudentID, int? GPA, string? Country, string? NumberOfTheScoreBook, string LastName) : base(LastName)
        {
            (studentID, gpa, course, country, numberOfTheScoreBook) = (StudentID, GPA, Course, Country, NumberOfTheScoreBook);
        }
        public Student(string LastName, string StudentID, int? Course, int? GPA, string? Country, string? NumberOfTheScoreBook) :
            this(Course, StudentID, GPA, Country, NumberOfTheScoreBook, LastName)
        { }
        private void OnStudentEvent(string message)
        {
            StudentEvent?.Invoke(this, new StudentEventArgs(message));
        }
        public void CheckGpa() 
        {
            if (gpa < 2) 
            {
                OnStudentEvent("Student is expulsed!"); 
            }
            else{ OnStudentEvent("Student is not expulsed!"); }
        }
        public string Study()
        {
            course = course == 6 ? 1 : course + 1;
            return LastName + " is now studing in " + course + " course";
        }

        public override string[] Methods { get { return base.Methods.Union(new string[] { "Study" }).ToArray(); } }
        public override string ToString() =>
            "Student - " + LastName +
            ", StudentID: " + studentID +
            ", Course: " + Course +
            ", GPA: " + GPA +
            ", Country: " + Country +
            ", NumberOfTheScoreBook: " + NumberOfTheScoreBook;
    }
    [Serializable]
    public class McWorker : Entity, ICook
    {
        public McWorker() { }
        public McWorker(string LastName) : base(LastName) { }
        public string Cook()
        {
            return LastName + " prepared your order!";
        }
        public override string[] Methods { get { return base.Methods.Union(new string[] { "Cook" }).ToArray(); } }
        public override string ToString() => "McWorker - " + LastName;
        protected McWorker(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
    [Serializable]
    public class Manager : Entity, IManage
    {
        public Manager() { }
        public Manager(string LastName) : base(LastName) { }
        public string Manage()
        {
            return LastName + " manages something!";
        }
        public override string[] Methods { get { return base.Methods.Union(new string[] { "Manage" }).ToArray(); } }
        public override string ToString() => "Manager - " + LastName;
        protected Manager(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
