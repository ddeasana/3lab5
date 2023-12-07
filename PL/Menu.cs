using BLL;
using DAL;
using System;
using System.ComponentModel.Design;
using System.Reflection;


namespace PL
{
    public class Menu
    {
        EntityService list = new();
        public void ShowMain()
        {
            Console.Clear();
            Console.WriteLine(
               "1 - view all;\n" +
               "2 - view the database of students;\n" +
               "3 - view the database of mcworkers;\n" +
               "4 - view the database of managers;\n" +
               "5 - add students to the database;\n" +
               "6 - add mcworkers to the database;\n" +
               "7 - add managers to the database;\n" +
               "8 - search\n" +
               "9 - db settings (name and format)\n" +
               "0 - EXIT");
        }
        public void Search()
        {
            List<Tuple<int, Student>> searchList = list.Search();
            Console.Clear();
            Console.WriteLine("Search: " + searchList.Count + " items ");
            int currentIndex = 0;
            while (currentIndex < searchList.Count)
            {
                Console.WriteLine(1 + searchList[currentIndex].Item1 + ". " + searchList[currentIndex].Item2);
                currentIndex++;
            }
            Console.WriteLine("0 - menu");

            bool done = false;
            do
            {
                var input = Console.ReadLine();
                switch (input)
                {
                    case "0": ShowMain(); return;
                    default:
                        ShowMain(); return;
                }
            }
            while (!done);
        }
        public void ShowPeople(string? typeOfEntity)
        {
            Console.Clear();
            Console.WriteLine(list.Length() + " entities:");
            for (int index = 0; index < list.Length(); index++)
            {
                if (typeOfEntity == null || list[index].GetType().Name == typeOfEntity)
                {
                    Console.WriteLine(1 + index + ". " + list[index]);
                }
            }
            Console.WriteLine("0 - back to menu, entity number - view entity");
            bool done = false;
            do
            {
                var i = Console.ReadLine();
                switch (i)
                {
                    case "0": ShowMain(); return;
                    default:
                        try
                        {
                            int parsed = Convert.ToInt32(i);
                            if (parsed < 1 || parsed > list.Length()) throw new WrongInputException();
                            ViewEntity(parsed - 1);
                            done = true;
                            return;
                        }
                        catch (Exception) { Console.WriteLine("Invalid input!"); }
                        break;
                }

            } while (!done);
        }
        public void ViewEntity(int index)
        {
            Console.Clear();
            Console.WriteLine("Entity: " + (index + 1) + " of " + list.Length() + " items");
            Entity entity = list[index];
            Console.WriteLine(1 + index + ". " + entity);
            Console.WriteLine("This entity can:");
            for (int i = 0; i < entity.Methods.Length; i++)
            {
                Console.WriteLine(entity.Methods[i] + " ");
            }
            Console.WriteLine("0 - go back to menu; 1 - edit; 2 - delete; do something;");
            if (list[index].GetType().Name == "Student") { Console.WriteLine("3 - check for expulsion;"); }
            bool done = false;
            do
            {
                var input = Console.ReadLine();
                if (input != null && input.StartsWith("do "))
                {
                    String MethodName = input.Split(' ')[1]; ;
                    Type type = entity.GetType();
                    MethodInfo? theMethod = type.GetMethod(MethodName);
                    if (theMethod == null)
                    {
                        Console.WriteLine("Error: no such function");
                    }
                    else
                    {
                        if (entity.GetType().Name == "McWorker")
                        {
                            bool flag = true;
                            Console.WriteLine("Choose ingredients for burger: tomato, cucumber, ketchup, beef, chicken, onion, cheese, lettuce; 0 - done");
                            List<String> ingredientslist = new();
                            do
                            {
                                String console = Console.ReadLine();
                                
                                switch (console) 
                                {
                                    case "0":
                                        Console.WriteLine(theMethod.Invoke(entity, new string[] { }));
                                        Console.WriteLine("burger with: ");
                                        foreach (String i in ingredientslist) { Console.WriteLine(i.ToString()); }
                                        flag = false;
                                        Console.WriteLine("0 - go back to menu; 1 - edit; 2 - delete; do something;");
                                        break;
                                    default:
                                        if (console != null && ((console.Contains("tomato")) || (console.Contains("cucumber")) || (console.Contains("ketchup")) || (console.Contains("beef")) || (console.Contains("chicken")) || (console.Contains("onion")) || (console.Contains("cheese")) || (console.Contains("lettuce"))))
                                        {
                                            ingredientslist.Add(console);
                                        }
                                        break;
                                }
                                
                            } while (flag);
                            
                        }
                        else
                        {
                            Console.WriteLine(theMethod.Invoke(entity, new string[] { }));
                        }
                        list.Save();
                    }
                }
                else
                {
                    switch (input)
                    {
                        case "0": ShowMain(); return;
                        case "1": AddOrEdit(entity, index, null); return;
                        case "2":
                            try
                            {
                                list.Delete(index);
                                ShowMain(); return;
                            }
                            catch (Exception) { Console.WriteLine("Failed to delete!"); }
                            break;
                        case "3":
                            if (list[index].GetType().Name == "Student") 
                            {
                                Student student = new Student();
                                student = (Student)list[index];
                                student.StudentEvent += StudentExpulsionHandler;
                                student.CheckGpa();
                                break;
                            };
                            break; 
                       default: Console.WriteLine("Invalid input!"); break;
                    }
                }
            }
            while (!done);
        }
        public static void StudentExpulsionHandler(object sender, StudentEventArgs msg)
        {
            Console.WriteLine(msg.EventMessage);
        }
        public void AddOrEdit(Entity? entity, int? index, string? type)
        {
            Console.Clear();
            bool isEditing = entity != null;
            Console.WriteLine(isEditing ? "Editing " + entity.GetType().Name : "Adding " + type);
            if (isEditing)
            {
                Console.WriteLine(entity.ToString());
                type = entity.GetType().Name;
            }
            Entity? newEntity = null;
            switch (type)
            {
                case "Student":

                    Student? oldStudent = isEditing ? (Student)entity : null;
                    newEntity = new Student(
                        AskName(isEditing ? oldStudent.LastName : null),
                        AskID(isEditing ? oldStudent.StudentID : null),
                        AskCourse(isEditing ? oldStudent.Course : null),
                        AskGPA(isEditing ? oldStudent.GPA : null),
                        AskCountry(isEditing ? oldStudent.Country : null),
                        AskNumberOfTheScoreBook(isEditing ? oldStudent.NumberOfTheScoreBook : null)
                    );
                    
                    ShowMain();
                    break;
                case "McWorker":
                    McWorker? oldTailor = isEditing ? (McWorker)entity : null;
                    newEntity = new McWorker(
                        AskName(isEditing ? oldTailor.LastName : null)
                    );
                    ShowMain();
                    break;
                case "Manager":
                    Manager? oldSinger = isEditing ? (Manager)entity : null;
                    newEntity = new Manager(
                        AskName(isEditing ? oldSinger.LastName : null)
                    );
                    ShowMain();
                    break;
                default:
                    Console.WriteLine("I don`t know how to create this entity(\nNew key to go back");
                    Console.ReadKey();
                    break;
            }
            if (newEntity != null)
            {
                if (isEditing)
                {
                    list.Update(newEntity, (int)index);
                }
                else
                {
                    list.Insert(newEntity);
                }
            }
            return;
        }
        public string? AskName(string? input)
        {
            bool done = false;
            string name = "";
            do
            {
                Console.WriteLine("Enter name: " + (input != null ? "(" + input + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "" && input != null) { return input; }
                try
                {
                    EntityService.ValidateName(stringFromConsole);
                    name = stringFromConsole;
                }
                catch (Exception) { Console.WriteLine("Wrong name!"); continue; }
                done = true;
            }
            while (!done);
            return name;
        }
        public string? AskID(string? input)
        {
            bool done = false;
            string id = "";
            do
            {
                Console.WriteLine("Enter id (like KB00000000): " + (input != null ? "(" + input + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "" && input != null) { return input; }
                try
                {
                    EntityService.ValidateID(stringFromConsole);
                    id = stringFromConsole;
                }
                catch (Exception) { Console.WriteLine("Wrong id!"); continue; }
                done = true;
            }
            while (!done);
            return id;
        }
        public int? AskCourse(int? input)
        {
            bool done = false;
            int course = 0;
            do
            {
                Console.WriteLine("Enter course: " + (input != null ? "(" + input + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "")
                {
                    if (input != null) { return input; }
                    return null;
                }
                try
                {
                    int parsed = Convert.ToInt32(stringFromConsole);
                    EntityService.ValidateCourse(parsed);
                    course = parsed;
                }
                catch (Exception) { Console.WriteLine("Wrong course!"); continue; }
                done = true;
            }
            while (!done);
            return course;
        }
        public int? AskGPA(int? input)
        {
            bool done = false;
            int gpa = 0;
            do
            {
                Console.WriteLine("Enter GPA: " + (input != null ? "(" + input + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "")
                {
                    if (input != null) { return input; }
                    return null;
                }
                try
                {
                    int parsed = Convert.ToInt32(stringFromConsole);
                    EntityService.ValidateMark(parsed);
                    gpa = parsed;
                }
                catch (Exception) { Console.WriteLine("Wrong gpa!"); continue; }
                done = true;
            }
            while (!done);
            return gpa;
        }
        public string? AskCountry(string? input)
        {
            bool done = false;
            string? country = null;
            do
            {
                Console.WriteLine("Enter Country: " + (input != null ? "(" + input + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "")
                {
                    if (input != null) { return input; }
                    return null;
                }
                try
                {
                    EntityService.ValidateCountry(stringFromConsole);
                    country = stringFromConsole;
                }
                catch (Exception) { Console.WriteLine("Wrong Country!"); continue; }
                done = true;
            }
            while (!done);
            return country;
        }
        public string? AskNumberOfTheScoreBook(string? input)
        {
            bool done = false;
            string? numberOfTheScoreBook = null;
            do
            {
                Console.WriteLine("Enter Number of the score book: " + (input != null ? "(" + input + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "")
                {
                    if (input != null) { return input; }
                    return null;
                }
                try
                {
                    EntityService.ValidateNumberOfTheScoreBook(stringFromConsole);
                    numberOfTheScoreBook = stringFromConsole;
                }
                catch (Exception) { Console.WriteLine("Wrong Number of the score book!"); continue; }
                done = true;
            }
            while (!done);
            return numberOfTheScoreBook;
        }
        public void ShowDBSettings()
        {
            string CurrentDBName = $"{list.DBName}.{list.DBType}";
            Console.Clear();
            Console.WriteLine("Current DB: " + CurrentDBName);
            Console.WriteLine("Available db types: " + string.Join(",", list.AvailableDBTypes));
            Console.WriteLine("Enter new db name: ");
            Console.WriteLine("0 - menu");
            bool done = false;
            do
            {
                var input = Console.ReadLine();
                if (input == null || input == "") { continue; }
                switch (input)
                {
                    case "0": ShowMain(); return;
                    default:
                        try
                        {
                            list.SetProvider(input);
                            ShowMain();
                            return;
                        }
                        catch (Exception) { Console.WriteLine("Wrong input"); continue; }
                }
            }
            while (!done);
        }
        public void MainMenu() 
        {
            bool flag = false;
            ShowMain();
            do
            {
                Console.WriteLine("Enter command:");
                string? i = Console.ReadLine();
                if (!int.TryParse(i, out int result)) { Console.WriteLine("Invalid input!"); }
                else
                {
                    int command = int.Parse(i);
                    switch (command)
                    {
                        case 1:
                            ShowPeople(null);
                            break;
                        case 2:
                            ShowPeople("Student");
                            break;
                        case 3:
                            ShowPeople("McWorker");
                            break;
                        case 4:
                            ShowPeople("Manager");
                            break;
                        case 5:
                            AddOrEdit(null, null, "Student");
                            break;
                        case 6:
                            AddOrEdit(null, null, "McWorker");
                            break;
                        case 7:
                            AddOrEdit(null, null, "Manager");
                            break;
                        case 8:
                            Search();
                            break;
                        case 9:
                            ShowDBSettings();
                            break;
                        case 0:
                            flag = true;
                            break;
                        default:
                            Console.WriteLine("Invalid number!");
                            break;
                    }
                }
            } while (!flag);
        }
    }
}
